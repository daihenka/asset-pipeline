using Daihenka.AssetPipeline.Filters;
using Daihenka.AssetPipeline.Import;
using NUnit.Framework;
using UnityEngine;

namespace Daihenka.AssetPipeline.Tests
{
    class ImportProfileTests
    {
        AssetImportProfile profile;

        [SetUp]
        public void Setup()
        {
            profile = ScriptableObject.CreateInstance<AssetImportProfile>();
            profile.enabled = true;
            profile.path = new NamingConventionRule("Test Pattern", "Assets/Test/{varName}/");
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(profile, true);
        }

        [Test]
        public void ShouldMatchValidAssetPath()
        {
            var assetPath = "Assets/Test/SomeAsset/SomeAsset.png";

            var isMatch = profile.IsMatch(assetPath);
            Assert.IsTrue(isMatch);

            profile.path.pattern = "Assets/Test/{varName}";
            isMatch = profile.IsMatch(assetPath);
            Assert.IsTrue(isMatch);
        }

        [Test]
        public void ShouldNotMatchInvalidAssetPath()
        {
            var assetPath = "Assets/Test2/SomeAsset/SomeAsset.png";

            var isMatch = profile.IsMatch(assetPath);
            Assert.IsFalse(isMatch);

            profile.path.pattern = "Assets/Test/{varName}";
            isMatch = profile.IsMatch(assetPath);
            Assert.IsFalse(isMatch);
        }

        [Test]
        public void ShouldNotMatchValidAssetPathWhenDisabled()
        {
            profile.enabled = false;
            var assetPath = "Assets/Test/SomeAsset/SomeAsset.png";

            var isMatch = profile.IsMatch(assetPath);
            Assert.IsFalse(isMatch);
        }

        [Test]
        public void ShouldMatchValidAssetPathThatDoesNotContainExclusion()
        {
            profile.pathExclusions.Add(new PathFilter {ignoreCase = true, matchType = StringMatchType.Contains, pattern = "/VFX/"});
            var assetPath = "Assets/Test/SomeAsset/SomeAsset.png";

            var isMatch = profile.IsMatch(assetPath);
            Assert.IsTrue(isMatch);
        }

        [Test]
        public void ShouldNotMatchValidAssetPathThatContainsExclusion()
        {
            profile.pathExclusions.Add(new PathFilter {ignoreCase = true, matchType = StringMatchType.Contains, pattern = "/VFX/"});
            var assetPath = "Assets/Test/SomeAsset/VFX/SomeAsset.png";

            var isMatch = profile.IsMatch(assetPath);
            Assert.IsFalse(isMatch);
        }

        [Test]
        public void ShouldRemoveNullFilters()
        {
            var filter = ScriptableObject.CreateInstance<AssetFilter>();
            profile.assetFilters.Add(filter);
            profile.assetFilters.Add(null);
            profile.assetFilters.Add(null);

            profile.RemoveNullFilters();
            Assert.AreEqual(1, profile.assetFilters.Count);

            Object.DestroyImmediate(filter, true);
        }
    }
}