using System.Linq;
using Daihenka.AssetPipeline.Import;
using UnityEditor;
using UnityEditor.Presets;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

namespace Daihenka.AssetPipeline.Processors
{
    [AssetProcessorDescription(typeof(Preset), ImportAssetTypeFlag.Textures | ImportAssetTypeFlag.Models | ImportAssetTypeFlag.Audio | ImportAssetTypeFlag.SpriteAtlases | ImportAssetTypeFlag.Fonts | ImportAssetTypeFlag.Videos)]
    public class ApplyPreset : AssetProcessor
    {
        [SerializeField] Preset preset;
        public override int Priority => int.MaxValue;

        protected override Object[] PrepareEmbeddedObjects(ImportAssetType assetType)
        {
            preset = CreatePresetForType(assetType);
            preset.name = $"Preset_{assetType}";
            return new[] {preset};
        }

        public override void OnPostprocess(Object asset, string assetPath)
        {
            ApplyPresetToSpriteAtlas(asset, assetPath);
        }

        public override bool ShouldImport(string assetPath)
        {
            return IsForceApply(assetPath) || !ImportProfileUserData.HasProcessor(assetPath, this);
        }

        public override void OnPreprocessAsset(string assetPath, AssetImporter importer)
        {
            if (preset == null || !preset.CanBeAppliedTo(importer) || !ShouldImport(importer))
            {
                return;
            }

            var isTextureImporter = importer as TextureImporter != null;
            if (isTextureImporter)
            {
                var obj = new SerializedObject(importer);

                var widthProp = obj.FindProperty("m_Output.sourceTextureInformation.width");
                var heightProp = obj.FindProperty("m_Output.sourceTextureInformation.height");

                var prevW = widthProp.intValue;
                var prevH = heightProp.intValue;

                preset.ApplyTo(importer);

                obj.Update();
                widthProp.intValue = prevW;
                heightProp.intValue = prevH;

                obj.ApplyModifiedProperties();
            }
            else
            {
                preset.ApplyTo(importer);
            }

            ImportProfileUserData.AddOrUpdateProcessor(assetPath, this);
            Debug.Log($"[{GetName()}] Preset applied for <b>{assetPath}</b>");
        }

        static Preset CreatePresetForType(ImportAssetType importAssetType)
        {
            if (importAssetType == ImportAssetType.SpriteAtlases)
            {
                return new Preset(new SpriteAtlas());
            }


            var dummyAsset = AssetDatabaseUtility.FindAssetPaths($"__importer{importAssetType.ToString().ToLowerInvariant()}dummy__").FirstOrDefault();
            var defaultImporter = AssetImporter.GetAtPath(dummyAsset);
            return new Preset(defaultImporter);
        }

        void ApplyPresetToSpriteAtlas(Object asset, string assetPath)
        {
            if (asset.GetType() != typeof(SpriteAtlas) || preset == null || !preset.CanBeAppliedTo(asset) || !ShouldImport(assetPath))
            {
                return;
            }

            var atlas = (SpriteAtlas) asset;
            var isVariant = atlas.isVariant;
            var so = new SerializedObject(atlas);
            var includeInBuild = so.FindProperty("m_EditorData.bindAsDefault").boolValue;
            var masterAtlas = (SpriteAtlas) so.FindProperty("m_MasterAtlas").objectReferenceValue;
            var variantScale = so.FindProperty("m_EditorData.variantMultiplier").floatValue;
            var packables = atlas.GetPackables();
            preset.ApplyTo(atlas);
            atlas.SetIsVariant(isVariant);
            atlas.SetIncludeInBuild(includeInBuild);
            if (isVariant)
            {
                atlas.SetMasterAtlas(masterAtlas);
                atlas.SetVariantScale(variantScale);
            }

            atlas.Add(packables);
            EditorUtility.SetDirty(atlas);
            AssetDatabase.SaveAssets();
            Debug.Log($"[{GetName()}] Preset applied for <b>{assetPath}</b>");
            ImportProfileUserData.AddOrUpdateProcessor(assetPath, this);
        }
    }
}