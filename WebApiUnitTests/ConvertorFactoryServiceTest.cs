using System;
using Moq;
using NUnit.Framework;
using WebApi.Exceptions;
using WebApi.Services;

namespace WebApiUnitTests
{
    public class ConvertorFactoryServiceTest
    {
        [Test]
        public void GetInstancePositiveTest()
        {
            var testFileName = "testFile.xml";
            
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(m => m.GetService(typeof(XmlToJsonConvertorService))).Returns(new XmlToJsonConvertorService());
            
            var convertorFactory = new ConvertorFactoryService(serviceProviderMock.Object);
            var convertor = convertorFactory.GetInstance(testFileName);
            
            Assert.IsTrue(convertor.GetType() == typeof(XmlToJsonConvertorService), "Wrong convertor type returned");
        }
        
        [Test]
        public void GetInstanceUnknownFileTest()
        {
            var testFileName = "testFile.exe";
            
            var serviceProviderMock = new Mock<IServiceProvider>();

            var convertorFactory = new ConvertorFactoryService(serviceProviderMock.Object);
            Assert.Throws<UnsupportedFileExtension>(() => convertorFactory.GetInstance(testFileName));
        }
    }
}