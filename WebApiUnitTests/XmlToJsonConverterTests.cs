using NUnit.Framework;
using WebApi.Exceptions;
using WebApi.Services;

namespace WebApiUnitTests
{
    public class XmlToJsonConverterTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ConvertBasicTest()
        {
            var sourceXml = "<testdata><title>Test title</title><text>Test text</text></testdata>";
            var expectedJson = "{\"Title\":\"Test title\",\"Text\":\"Test text\"}";
            
            var converter = new XmlToJsonConvertorService();
            var resultJson =  converter.Convert(sourceXml);
            Assert.AreEqual(expectedJson, resultJson);
        }
        
        [Test]
        public void ConvertBasicBadXml()
        {
            var sourceXml = "<title>Test title</title><text>Test text</text>";

            var converter = new XmlToJsonConvertorService();
            Assert.Throws<ParsingInputFileException>(() => converter.Convert(sourceXml));
        }
    }
}