using System;
using System.Xml.Linq;
using Newtonsoft.Json;
using WebApi.BusinessModel;
using WebApi.Exceptions;
using WebApi.Interfaces.Services;

namespace WebApi.Services
{
    public class XmlToJsonConvertorService : IConvertor
    {
        public string Convert(string source)
        {
            // Good improvement would be to validate XML against some scheme here
            
            try
            {
                var xdoc = XDocument.Parse(source);
                var doc = new DocumentBo
                {
                    Title = xdoc?.Root?.Element("title")?.Value,
                    Text = xdoc?.Root?.Element("text")?.Value
                };

                var serializedDoc = JsonConvert.SerializeObject(doc);

                return serializedDoc;
            }
            catch (Exception)
            {
                throw new ParsingInputFileException("Unknown problem when parsing XML file");
            }
        }
    }
}