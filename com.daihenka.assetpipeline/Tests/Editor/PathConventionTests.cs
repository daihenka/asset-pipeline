using System;
using System.Collections.Generic;
using Daihenka.AssetPipeline.NamingConvention;
using NUnit.Framework;

namespace Daihenka.AssetPipeline.Tests
{
    class TemplateTests
    {
        ITemplateResolver TemplateResolver()
        {
            var resolver = new GenericResolver();
            resolver.templates.Add(new Template("reference", "{variable}", templateResolver: resolver));
            resolver.templates.Add(new Template("nested", "/root/{@reference}", templateResolver: resolver));
            return resolver;
        }

        public static IEnumerable<TestCaseData> TestValidPatternGenerator()
        {
            yield return new TestCaseData("").SetName("empty");
            yield return new TestCaseData("{variable}").SetName("single variable");
            yield return new TestCaseData("{dotted.variable}").SetName("dotted variable");
            yield return new TestCaseData("{variable}/{variable}").SetName("duplicate variable");
            yield return new TestCaseData(@"{variable:\w+?}").SetName("custom expression");
            yield return new TestCaseData(@"{variable:\snake}").SetName("custom snake case expression");
        }

        [Test]
        [TestCaseSource(nameof(TestValidPatternGenerator))]
        public void TestValidPattern(string pattern)
        {
            Assert.DoesNotThrow(() => new Template("test", pattern));
        }

        public static IEnumerable<TestCaseData> TestInvalidPatternGenerator()
        {
            yield return new TestCaseData("{}").SetName("empty placeholder");
            yield return new TestCaseData("{variable-dashed}").SetName("invalid placeholder character");
            yield return new TestCaseData("{variable:(?<missing_closing_angle_bracket)}").SetName("invalid placeholder expression");
        }

        [Test]
        [TestCaseSource(nameof(TestInvalidPatternGenerator))]
        public void TestInvalidPattern(string pattern)
        {
            Assert.Throws<ValueException>(() => new Template("test", pattern));
        }

        public static IEnumerable<TestCaseData> TestMatchingParseGenerator()
        {
            yield return new TestCaseData("/static/string", "/static/string").Returns(new TemplateData()).SetName("static string");
            yield return new TestCaseData("/single/{variable}", "/single/value").Returns(new TemplateData {{"variable", new TemplateParsedData("value", StringConvention.None)}}).SetName("single variable");
            yield return new TestCaseData("/{variable}/{variable}", "/first/second").Returns(new TemplateData {{"variable", new TemplateParsedData("second", StringConvention.None)}}).SetName("duplicate variable");
            yield return new TestCaseData(@"/static/{variable:\d\{4\}}", "/static/1234").Returns(new TemplateData {{"variable", new TemplateParsedData("1234", StringConvention.None)}}).SetName("custom variable expression");
            yield return new TestCaseData(@"/{a}/static/{b}", "/first/static/second").Returns(new TemplateData {{"a", new TemplateParsedData("first", StringConvention.None)}, {"b", new TemplateParsedData("second", StringConvention.None)}}).SetName("mix of static and variables");
            yield return new TestCaseData(@"/{a.b.c}/static/{a.b.d}", "/first/static/second").Returns(new TemplateData {{"a.b.c", new TemplateParsedData("first", StringConvention.None)}, {"a.b.d", new TemplateParsedData("second", StringConvention.None)}})
                .SetName("mix of static and dotted variables");
            yield return new TestCaseData(@"/{a}_{b}", "/first_second").Returns(new TemplateData {{"a", new TemplateParsedData("first", StringConvention.None)}, {"b", new TemplateParsedData("second", StringConvention.None)}}).SetName("neighbouring variables");
            yield return new TestCaseData(@"/single/{@reference}", "/single/value").Returns(new TemplateData {{"variable", new TemplateParsedData("value", StringConvention.None)}}).SetName("single reference");
            yield return new TestCaseData(@"{@nested}/reference", "/root/value/reference").Returns(new TemplateData {{"variable", new TemplateParsedData("value", StringConvention.None)}}).SetName("nested reference");
            yield return new TestCaseData(@"/single/{variable:\snake}", "/single/single_test").Returns(new TemplateData {{"variable", new TemplateParsedData("single_test", StringConvention.SnakeCase)}}).SetName("snake case");
            yield return new TestCaseData(@"/single/{variable:\usnake}", "/single/SINGLE_TEST").Returns(new TemplateData {{"variable", new TemplateParsedData("SINGLE_TEST", StringConvention.UpperSnakeCase)}}).SetName("upper snake case");
            yield return new TestCaseData(@"/single/{variable:\camel}", "/single/singleTest").Returns(new TemplateData {{"variable", new TemplateParsedData("singleTest", StringConvention.CamelCase)}}).SetName("camel case");
            yield return new TestCaseData(@"/single/{variable:\kebab}", "/single/single-test").Returns(new TemplateData {{"variable", new TemplateParsedData("single-test", StringConvention.KebabCase)}}).SetName("kebab case");
            yield return new TestCaseData(@"/single/{variable:\pascal}", "/single/SingleTest").Returns(new TemplateData {{"variable", new TemplateParsedData("SingleTest", StringConvention.PascalCase)}}).SetName("pascal case");
            yield return new TestCaseData(@"/single/{variable:\upper}", "/single/SINGLE-TEST").Returns(new TemplateData {{"variable", new TemplateParsedData("SINGLE-TEST", StringConvention.UpperCase)}}).SetName("upper case");
            yield return new TestCaseData(@"/single/{variable:\lower}", "/single/single_test").Returns(new TemplateData {{"variable", new TemplateParsedData("single_test", StringConvention.LowerCase)}}).SetName("lower case");
        }

        [Test]
        [TestCaseSource(nameof(TestMatchingParseGenerator))]
        public TemplateData TestMatchingParse(string pattern, string path)
        {
            var template = new Template("test", pattern, templateResolver: TemplateResolver());
            var data = template.Parse(path);
            return data;
        }

        public static IEnumerable<TestCaseData> TestNonMatchingParseGenerator()
        {
            yield return new TestCaseData("/static/string", "/static/").SetName("string too short");
            yield return new TestCaseData("/static/{variable}", "/static/").SetName("missing variable");
            yield return new TestCaseData(@"/static/{variable:\d\{4\}}", "/static/foo").SetName("mismatching custom expression");
            yield return new TestCaseData(@"/static/{variable}/{@reference}", "/static/value/").SetName("string not accounting for reference");
            yield return new TestCaseData(@"{@nested}/reference", "/root/value").SetName("string not accounting for nested reference");
            yield return new TestCaseData(@"/single/{variable:\snake}", "/single/singleTest").SetName("mismatching snake case");
            yield return new TestCaseData(@"/single/{variable:\usnake}", "/single/singleTest").SetName("mismatching upper snake case");
            yield return new TestCaseData(@"/single/{variable:\kebab}", "/single/singleTest").SetName("mismatching kebab case");
            yield return new TestCaseData(@"/single/{variable:\camel}", "/single/single_test").SetName("mismatching camel case");
            yield return new TestCaseData(@"/single/{variable:\pascal}", "/single/singleTest").SetName("mismatching pascal case");
            yield return new TestCaseData(@"/single/{variable:\upper}", "/single/singleTest").SetName("mismatching upper case");
            yield return new TestCaseData(@"/single/{variable:\lower}", "/single/singleTest").SetName("mismatching lower case");
        }

        [Test]
        [TestCaseSource(nameof(TestNonMatchingParseGenerator))]
        public void TestNonMatchingParse(string pattern, string path)
        {
            var template = new Template("test", pattern, templateResolver: TemplateResolver());
            Assert.Throws<ParseException>(() => template.Parse(path, true));
        }

        public static IEnumerable<TestCaseData> TestMatchingDefaultConventionParseGenerator()
        {
            yield return new TestCaseData(@"/single/{variable}", "/single/single_test", StringConvention.SnakeCase).Returns(new TemplateData {{"variable", new TemplateParsedData("single_test", StringConvention.SnakeCase)}}).SetName("snake case");
            yield return new TestCaseData(@"/single/{variable}", "/single/SINGLE_TEST", StringConvention.UpperSnakeCase).Returns(new TemplateData {{"variable", new TemplateParsedData("SINGLE_TEST", StringConvention.UpperSnakeCase)}}).SetName("upper snake case");
            yield return new TestCaseData(@"/single/{variable}", "/single/singleTest", StringConvention.CamelCase).Returns(new TemplateData {{"variable", new TemplateParsedData("singleTest", StringConvention.CamelCase)}}).SetName("camel case");
            yield return new TestCaseData(@"/single/{variable}", "/single/single-test", StringConvention.KebabCase).Returns(new TemplateData {{"variable", new TemplateParsedData("single-test", StringConvention.KebabCase)}}).SetName("kebab case");
            yield return new TestCaseData(@"/single/{variable}", "/single/SingleTest", StringConvention.PascalCase).Returns(new TemplateData {{"variable", new TemplateParsedData("SingleTest", StringConvention.PascalCase)}}).SetName("pascal case");
            yield return new TestCaseData(@"/single/{variable}", "/single/SINGLE-TEST", StringConvention.UpperCase).Returns(new TemplateData {{"variable", new TemplateParsedData("SINGLE-TEST", StringConvention.UpperCase)}}).SetName("upper case");
            yield return new TestCaseData(@"/single/{variable}", "/single/single_test", StringConvention.LowerCase).Returns(new TemplateData {{"variable", new TemplateParsedData("single_test", StringConvention.LowerCase)}}).SetName("lower case");
        }

        [Test]
        [TestCaseSource(nameof(TestMatchingDefaultConventionParseGenerator))]
        public TemplateData TestMatchingDefaultConventionParse(string pattern, string path, StringConvention convention)
        {
            var template = new Template("test", pattern, convention, templateResolver: TemplateResolver());
            var data = template.Parse(path);
            return data;
        }

        public static IEnumerable<TestCaseData> TestNonMatchingDefaultConventionParseGenerator()
        {
            yield return new TestCaseData(@"/single/{variable}", "/single/singleTest", StringConvention.SnakeCase).SetName("mismatching snake case");
            yield return new TestCaseData(@"/single/{variable}", "/single/singleTest", StringConvention.UpperSnakeCase).SetName("mismatching upper snake case");
            yield return new TestCaseData(@"/single/{variable}", "/single/single_test", StringConvention.CamelCase).SetName("mismatching camel case");
            yield return new TestCaseData(@"/single/{variable}", "/single/singleTest", StringConvention.KebabCase).SetName("mismatching kebab case");
            yield return new TestCaseData(@"/single/{variable}", "/single/singleTest", StringConvention.PascalCase).SetName("mismatching pascal case");
            yield return new TestCaseData(@"/single/{variable}", "/single/singleTest", StringConvention.UpperCase).SetName("mismatching upper case");
            yield return new TestCaseData(@"/single/{variable}", "/single/singleTest", StringConvention.LowerCase).SetName("mismatching lower case");
        }

        [Test]
        [TestCaseSource(nameof(TestNonMatchingDefaultConventionParseGenerator))]
        public void TestNonMatchingDefaultConventionParse(string pattern, string path, StringConvention convention)
        {
            var template = new Template("test", pattern, convention, templateResolver: TemplateResolver());
            Assert.Throws<ParseException>(() => template.Parse(path, true));
        }

        public static IEnumerable<TestCaseData> TestKeyConventionsGenerator()
        {
            yield return new TestCaseData(@"/single/{variable}").Returns(new TemplateKeys {{"variable", StringConvention.None}}).SetName("default");
            yield return new TestCaseData(@"/single/{variable:\snake}").Returns(new TemplateKeys {{"variable", StringConvention.SnakeCase}}).SetName("snake case");
            yield return new TestCaseData(@"/single/{variable:\usnake}").Returns(new TemplateKeys {{"variable", StringConvention.UpperSnakeCase}}).SetName("upper snake case");
            yield return new TestCaseData(@"/single/{variable:\camel}").Returns(new TemplateKeys {{"variable", StringConvention.CamelCase}}).SetName("camel case");
            yield return new TestCaseData(@"/single/{variable:\kebab}").Returns(new TemplateKeys {{"variable", StringConvention.KebabCase}}).SetName("kebab case");
            yield return new TestCaseData(@"/single/{variable:\pascal}").Returns(new TemplateKeys {{"variable", StringConvention.PascalCase}}).SetName("pascal case");
            yield return new TestCaseData(@"/single/{variable:\upper}").Returns(new TemplateKeys {{"variable", StringConvention.UpperCase}}).SetName("upper case");
            yield return new TestCaseData(@"/single/{variable:\lower}").Returns(new TemplateKeys {{"variable", StringConvention.LowerCase}}).SetName("lower case");
        }

        [Test]
        [TestCaseSource(nameof(TestKeyConventionsGenerator))]
        public TemplateKeys TestKeyConventions(string pattern)
        {
            var template = new Template("test", pattern, templateResolver: TemplateResolver());
            var data = template.KeyConventions();
            return data;
        }

        public static IEnumerable<TestCaseData> TestValidParseInStrictModeGenerator()
        {
            yield return new TestCaseData("/{variable}/{variable}", "/value/value").Returns(new TemplateData {{"variable", new TemplateParsedData("value", StringConvention.None)}}).SetName("simple duplicate");
            yield return new TestCaseData(@"/static/{variable:\d\{4\}}/other/{variable}", "/static/1234/other/1234").Returns(new TemplateData {{"variable", new TemplateParsedData("1234", StringConvention.None)}}).SetName("duplicate with one specialised expression");
            yield return new TestCaseData("/{a.b.c}/static/{a.b.c}", "/value/static/value").Returns(new TemplateData {{"a.b.c", new TemplateParsedData("value", StringConvention.None)}}).SetName("dotted duplicate variable");
            yield return new TestCaseData(@"/{a}/{b}/other/{a}_{b}", "/a/b/other/a_b").Returns(new TemplateData {{"a", new TemplateParsedData("a", StringConvention.None)}, {"b", new TemplateParsedData("b", StringConvention.None)}}).SetName("multiple duplicates");
            yield return new TestCaseData(@"{@nested}/{variable}", "/root/value/value").Returns(new TemplateData {{"variable", new TemplateParsedData("value", StringConvention.None)}}).SetName("duplicate from reference");
        }

        [Test]
        [TestCaseSource(nameof(TestValidParseInStrictModeGenerator))]
        public TemplateData TestValidParseInStrictMode(string pattern, string path)
        {
            var template = new Template("test", pattern, duplicatePlaceholderMode: DuplicatePlaceholderMode.Strict, templateResolver: TemplateResolver());
            var data = template.Parse(path);
            return data;
        }

        public static IEnumerable<TestCaseData> TestInvalidParseInStrictModeGenerator()
        {
            yield return new TestCaseData("/{variable}/{variable}", "/a/b").SetName("simple duplicate");
            yield return new TestCaseData(@"/static/{variable:\d\{4\}}/other/{variable}", "/static/1234/other/2345").SetName("duplicate with one specialised expression");
            yield return new TestCaseData("/{a.b.c}/static/{a.b.c}", "/c1/static/c2").SetName("dotted duplicate variable");
            yield return new TestCaseData(@"/{a}/{b}/other/{a}_{b}", "/a/b/other/c_d").SetName("multiple duplicates");
            yield return new TestCaseData(@"{@nested}/{variable}", "/root/different/value").SetName("duplicate from reference");
        }

        [Test]
        [TestCaseSource(nameof(TestInvalidParseInStrictModeGenerator))]
        public void TestInvalidParseInStrictMode(string pattern, string path)
        {
            var template = new Template("test", pattern, duplicatePlaceholderMode: DuplicatePlaceholderMode.Strict, templateResolver: TemplateResolver());
            Assert.Throws<ParseException>(() => template.Parse(path)).Message.Contains("Different extracted values");
        }

        public static IEnumerable<TestCaseData> TestAnchorGenerator()
        {
            yield return new TestCaseData("/static/value/extra", Anchor.Start, true).SetName("anchor_start:matching string");
            yield return new TestCaseData("/static/", Anchor.Start, false).SetName("anchor_start:non-matching string");
            yield return new TestCaseData("/extra/static/value", Anchor.End, true).SetName("anchor_end:matching string");
            yield return new TestCaseData("/static/value/extra", Anchor.End, false).SetName("anchor_end:non-matching string");
            yield return new TestCaseData("/static/value", Anchor.Exact, true).SetName("anchor_both:matching string");
            yield return new TestCaseData("extra/static/value", Anchor.Exact, false).SetName("anchor_both:non-matching string prefix");
            yield return new TestCaseData("/static/value/extra", Anchor.Exact, false).SetName("anchor_both:non-matching string suffix");
            yield return new TestCaseData("extra/static/value/extra", Anchor.Contains, true).SetName("anchor_none:matching string");
            yield return new TestCaseData("extra/non/matching/extra", Anchor.Contains, false).SetName("anchor_none:non-matching string");
        }

        [Test]
        [TestCaseSource(nameof(TestAnchorGenerator))]
        public void TestAnchor(string path, Anchor anchor, bool expected)
        {
            var pattern = "/static/{variable}";
            var template = new Template("test", pattern, anchor);
            if (!expected)
            {
                Assert.Throws<ParseException>(() => template.Parse(path));
            }
            else
            {
                var data = template.Parse(path);

                Assert.AreEqual(data, new TemplateData {{"variable", new TemplateParsedData("value", StringConvention.None)}});
            }
        }

        public static IEnumerable<TestCaseData> TestFormatGenerator()
        {
            yield return new TestCaseData("/static/string", new Dictionary<string, string>(), "/static/string").SetName("static string");
            yield return new TestCaseData("/single/{variable}", new Dictionary<string, string> {{"variable", "value"}}, "/single/value").SetName("single variable");
            yield return new TestCaseData("/{variable}/{variable}", new Dictionary<string, string> {{"variable", "value"}}, "/value/value").SetName("duplicate variable");
            yield return new TestCaseData(@"/static/{variable:\d\{4\}}", new Dictionary<string, string> {{"variable", "1234"}}, "/static/1234").SetName("custom variable expression");
            yield return new TestCaseData(@"/{a}/static/{b}", new Dictionary<string, string> {{"a", "first"}, {"b", "second"}}, "/first/static/second").SetName("mix of static and variables");
            yield return new TestCaseData(@"/{a.b.c}/static/{a.b.d}", new Dictionary<string, string> {{"a.b.c", "first"}, {"a.b.d", "second"}}, "/first/static/second").SetName("mix of static and dotted variables");
            yield return new TestCaseData(@"/single/{@reference}", new Dictionary<string, string> {{"variable", "value"}}, "/single/value").SetName("reference");
            yield return new TestCaseData(@"{@nested}/reference", new Dictionary<string, string> {{"variable", "value"}}, "/root/value/reference").SetName("nested reference");
        }

        [Test]
        [TestCaseSource(nameof(TestFormatGenerator))]
        public void TestFormat(string pattern, Dictionary<string, string> data, string expected)
        {
            var template = new Template("test", pattern, templateResolver: TemplateResolver());
            var formatted = template.Format(data);
            Assert.AreEqual(formatted, expected);
        }

        public static IEnumerable<TestCaseData> TestFormatFailureGenerator()
        {
            yield return new TestCaseData("/single/{variable}", new Dictionary<string, string>()).SetName("missing single variable");
            yield return new TestCaseData("/{variable_a}/{variable_b}", new Dictionary<string, string> {{"variable_a", "value"}}).SetName("partial data");
            yield return new TestCaseData(@"{nested.variable}", new Dictionary<string, string>()).SetName("missing dotted variable");
            yield return new TestCaseData(@"/single/{@reference}", new Dictionary<string, string> {{"some", "value"}}).SetName("reference");
            yield return new TestCaseData(@"{@nested}/reference", new Dictionary<string, string> {{"some", "value"}}).SetName("nested reference");
        }

        [Test]
        [TestCaseSource(nameof(TestFormatFailureGenerator))]
        public void TestFormatFailure(string pattern, Dictionary<string, string> data)
        {
            var template = new Template("test", pattern, templateResolver: TemplateResolver());
            Assert.Throws<FormatException>(() => template.Format(data));
        }

        [Test]
        public void TestEscapingPattern()
        {
            var template = new Template("test", @"{filename}.{index:\d\{4\}}.{ext}");
            var expected = new TemplateData {{"filename", new TemplateParsedData("filename", StringConvention.None)}, {"index", new TemplateParsedData("0001", StringConvention.None)}, {"ext", new TemplateParsedData("ext", StringConvention.None)}};
            Assert.AreEqual(template.Parse("filename.0001.ext"), expected);
        }
    }
}