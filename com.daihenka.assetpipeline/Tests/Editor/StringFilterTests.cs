using Daihenka.AssetPipeline.Filters;
using NUnit.Framework;

namespace Daihenka.AssetPipeline.Tests
{
    class StringFilterTests
    {
        [Test]
        public void ShouldMatchContainsPatternCaseInsensitive()
        {
            var filter = new StringFilter(StringMatchType.Contains, "Test", true);
            var isMatch = filter.IsMatch("SomeTestString");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("TestSomeString");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("SomeStringTest");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("SometestString");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("testSomeString");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("SomeStringtest");
            Assert.IsTrue(isMatch);
        }

        [Test]
        public void ShouldMatchContainsPatternCaseSensitive()
        {
            var filter = new StringFilter(StringMatchType.Contains, "Test");
            var isMatch = filter.IsMatch("SomeTestString");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("TestSomeString");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("SomeStringTest");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("SometestString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("testSomeString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("SomeStringtest");
            Assert.IsFalse(isMatch);
        }

        [Test]
        public void ShouldMatchStartsWithPatternCaseInsensitive()
        {
            var filter = new StringFilter(StringMatchType.StartsWith, "Test", true);
            var isMatch = filter.IsMatch("SomeTestString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("TestSomeString");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("SomeStringTest");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("SometestString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("testSomeString");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("SomeStringtest");
            Assert.IsFalse(isMatch);
        }

        [Test]
        public void ShouldMatchStartsWithPatternCaseSensitive()
        {
            var filter = new StringFilter(StringMatchType.StartsWith, "Test");
            var isMatch = filter.IsMatch("SomeTestString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("TestSomeString");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("SomeStringTest");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("SometestString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("testSomeString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("SomeStringtest");
            Assert.IsFalse(isMatch);
        }

        [Test]
        public void ShouldMatchEndsWithPatternCaseInsensitive()
        {
            var filter = new StringFilter(StringMatchType.EndsWith, "Test", true);
            var isMatch = filter.IsMatch("SomeTestString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("TestSomeString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("SomeStringTest");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("SometestString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("testSomeString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("SomeStringtest");
            Assert.IsTrue(isMatch);
        }

        [Test]
        public void ShouldMatchEndsWithPatternCaseSensitive()
        {
            var filter = new StringFilter(StringMatchType.EndsWith, "Test");
            var isMatch = filter.IsMatch("SomeTestString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("TestSomeString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("SomeStringTest");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("SometestString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("testSomeString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("SomeStringtest");
            Assert.IsFalse(isMatch);
        }

        [Test]
        public void ShouldMatchEqualsPatternCaseInsensitive()
        {
            var filter = new StringFilter(StringMatchType.Equals, "Test", true);
            var isMatch = filter.IsMatch("SomeTestString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("TestSomeString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("SomeStringTest");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("Test");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("SometestString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("testSomeString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("SomeStringtest");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("test");
            Assert.IsTrue(isMatch);
        }

        [Test]
        public void ShouldMatchEqualsPatternCaseSensitive()
        {
            var filter = new StringFilter(StringMatchType.Equals, "Test");
            var isMatch = filter.IsMatch("SomeTestString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("TestSomeString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("SomeStringTest");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("Test");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("SometestString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("testSomeString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("SomeStringtest");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("test");
            Assert.IsFalse(isMatch);
        }

        [Test]
        public void ShouldMatchWildcardPatternCaseInsensitive()
        {
            var filter = new StringFilter(StringMatchType.Wildcard, "T*t", true);
            var isMatch = filter.IsMatch("SomeTestString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("TestSomeString");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("SomeStringTest");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("Test");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("SometestString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("testSomeString");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("SomeStringtest");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("test");
            Assert.IsTrue(isMatch);
        }

        [Test]
        public void ShouldMatchWildcardPatternCaseSensitive()
        {
            var filter = new StringFilter(StringMatchType.Wildcard, "T*t");
            var isMatch = filter.IsMatch("SomeTestString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("TestSomeString");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("SomeStringTest");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("Test");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("SometestString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("testSomeString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("SomeStringtest");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("test");
            Assert.IsFalse(isMatch);
        }

        [Test]
        public void ShouldMatchRegexPatternCaseInsensitive()
        {
            var filter = new StringFilter(StringMatchType.Regex, "(test)+", true);
            var isMatch = filter.IsMatch("SomeTestString");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("TestSomeString");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("SomeStringTest");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("Test");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("SometestString");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("testSomeString");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("SomeStringtest");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("test");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("someString");
            Assert.IsFalse(isMatch);
        }

        [Test]
        public void ShouldMatchRegexPatternCaseSensitive()
        {
            var filter = new StringFilter(StringMatchType.Regex, "(test)+");
            var isMatch = filter.IsMatch("SomeTestString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("TestSomeString");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("SomeStringTest");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("Test");
            Assert.IsFalse(isMatch);

            isMatch = filter.IsMatch("SometestString");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("testSomeString");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("SomeStringtest");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("test");
            Assert.IsTrue(isMatch);

            isMatch = filter.IsMatch("someString");
            Assert.IsFalse(isMatch);
        }
    }
}