using System.Collections.Generic;
using Daihenka.AssetPipeline.Filters;
using Daihenka.AssetPipeline.Import;
using NUnit.Framework;
using UnityEngine;

namespace Daihenka.AssetPipeline.Tests
{
    class AssetFilterTests
    {
        AssetImportProfile profile;
        AssetFilter filter;

        [SetUp]
        public void Setup()
        {
            profile = ScriptableObject.CreateInstance<AssetImportProfile>();
            profile.enabled = true;
            profile.path = new NamingConventionRule("Test Pattern", "Assets/Test/{varName}/");

            filter = ScriptableObject.CreateInstance<AssetFilter>();
            filter.enabled = true;
            filter.parent = profile;
            filter.file = new NamingConventionRule("Test Pattern", "{varFile}_001");
            filter.assetType = ImportAssetType.Textures;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(filter, true);
            Object.DestroyImmediate(profile, true);
        }

        [Test]
        public void ShouldMatchValidAssetPath()
        {
            var assetPath = "Assets/Test/SomeAsset/SomeAsset_001.png";
            var isMatch = filter.IsMatch(assetPath);
            Assert.IsTrue(isMatch);
        }

        [Test]
        public void ShouldNotMatchInvalidAssetPath()
        {
            var assetPath = "Assets/Test/SomeAsset/SomeAsset_002.png";
            var isMatch = filter.IsMatch(assetPath);
            Assert.IsFalse(isMatch);
        }

        [Test]
        public void ShouldNotMatchValidAssetPathWhenDisabled()
        {
            filter.enabled = false;
            var assetPath = "Assets/Test/SomeAsset/SomeAsset_001.png";
            var isMatch = filter.IsMatch(assetPath);
            Assert.IsFalse(isMatch);
        }

        [Test]
        public void ShouldNotMatchAssetPathForInvalidFileExtension()
        {
            var assetPath = "Assets/Test/SomeAsset/SomeAsset_001.jpeg";
            var isMatch = filter.IsMatch(assetPath);
            Assert.IsFalse(isMatch);
        }

        [Test]
        public void ShouldNotMatchAssetPathForDifferentAssetType()
        {
            filter.assetType = ImportAssetType.Animations;
            var assetPath = "Assets/Test/SomeAsset/SomeAsset_001.png";
            var isMatch = filter.IsMatch(assetPath);
            Assert.IsFalse(isMatch);
        }

        [Test]
        public void ShouldMatchAssetPathForOtherAssetType()
        {
            filter.assetType = ImportAssetType.Other;
            filter.otherAssetExtensions = new List<string> {".json"};
            var assetPath = "Assets/Test/SomeAsset/SomeAsset_001.json";
            var isMatch = filter.IsMatch(assetPath);
            Assert.IsTrue(isMatch);
        }

        [Test]
        public void ShouldNotMatchAssetPathForExclusion()
        {
            filter.fileExclusions.Add(new PathFilter { ignoreCase = true, matchType = StringMatchType.EndsWith, pattern = "_001"});
            var assetPath = "Assets/Test/SomeAsset/SomeAsset_001.png";
            var isMatch = filter.IsMatch(assetPath);
            Assert.IsFalse(isMatch);
        }

        [Test]
        public void ShouldOnlyMatchAssetPathsWithSubPath()
        {
            filter.subPaths = new List<string> {"VFX"};
            var assetPath = "Assets/Test/SomeAsset/SomeAsset_001.png";
            var isMatch = filter.IsMatch(assetPath);
            Assert.IsFalse(isMatch);

            assetPath = "Assets/Test/SomeAsset/SomeAssetVFX_001.png";
            isMatch = filter.IsMatch(assetPath);
            Assert.IsFalse(isMatch);

            assetPath = "Assets/Test/SomeAsset/VFX/SomeAsset_001.png";
            isMatch = filter.IsMatch(assetPath);
            Assert.IsTrue(isMatch);
        }
    }
}