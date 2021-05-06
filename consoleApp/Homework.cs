using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Notino.Homework
{
    public class Document
    {
        public string Title { get; set; }
        public string Text { get; set; }
    }

    static class Program
    {
        // I would create separate methods like ReadFile, Convert, WriteFile. All methods should catch possible exception
        // and report proper message to user, when something is wrong. And the best way in some real life project is to have 
        // separate class from file operation and separate class for convertions. But for this small project it's useless.
        static void Main(string[] args)
        {
            // here would be nice to get file names and paths as an input argument. For that purpose there exists some 
            // libraries for input argument definition. Also we should check if input is valid (e.g. not empty or it is really a path)
            var sourceFileName = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\Source Files\\Document1.xml");
            var targetFileName = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\Target Files\\Document1.json");
            // good question here is why do we go 3 levels down in path 

            try
            {
                // I prefer to use var everywhere, instead of types (or at least to have common way, for whole project)
                var sourceStream = File.Open(sourceFileName, FileMode.Open);
                var reader = new StreamReader(sourceStream);
                var input = reader.ReadToEnd();
            }
            // here we should catch also other more specific exception, to inform user properly what was wrong
            catch (Exception)
            {
                Console.WriteLine("I am sorry, I crashed with unknown reason");
                // Some log should be added here or eventually crash report sent 
            }
            
            // XML file should be validated, prior to parse
            
            // The input variable is not visible here
            // Whole this block should be try/catch also
            var xdoc = XDocument.Parse(/*input*/"");
            var doc = new Document
            {
                Title = xdoc?.Root?.Element("title")?.Value,
                Text = xdoc?.Root?.Element("text")?.Value
            };

            var serializedDoc = JsonConvert.SerializeObject(doc);

            var targetStream = File.Open(targetFileName, FileMode.Create, FileAccess.Write);
            var sw = new StreamWriter(targetStream);
            sw.Write(serializedDoc);
        }
    }
}
