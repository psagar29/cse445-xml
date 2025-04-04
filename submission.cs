
using System;
using System.Xml.Schema; // Needed for XSD validation
using System.Xml;        // Needed for XML reading and document handling
using Newtonsoft.Json; // Needed for JSON conversion
using System.IO;         // Potentially useful

namespace ConsoleApp1 // Required namespace
{
    public class Program
    {
        // --- URLs for XML and XSD files ---
        public static string xmlURL = "https://psagar29.github.io/cse445-xml/Hotels.xml";
        public static string xmlErrorURL = "https://psagar29.github.io/cse445-xml/HotelsErrors.xml";
        public static string xsdURL = "https://psagar29.github.io/cse445-xml/Hotels.xsd";

        // --- Main Program Execution ---
        public static void Main(string[] args)
        {
            Console.WriteLine("--- CSE445 Assignment 4 ---");

            // --- Task 1: Verification ---
            Console.WriteLine("\n[1] Verifying VALID XML (Hotels.xml) against Schema (Hotels.xsd)...");
            string result1 = Verification(xmlURL, xsdURL);
            Console.WriteLine($"Result: {result1}");

            Console.WriteLine("\n[2] Verifying INVALID XML (HotelsErrors.xml) against Schema (Hotels.xsd)...");
            string result2 = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine("Result(s):");
            Console.WriteLine(result2); // Should list errors found

            // --- Task 2: XML to JSON Conversion ---
            Console.WriteLine("\n[3] Converting VALID XML (Hotels.xml) to JSON...");
            string jsonResult = Xml2Json(xmlURL);
            Console.WriteLine("JSON Output:");
            Console.WriteLine(jsonResult); // Should be JSON without XML declaration

            Console.WriteLine("\n--- Program Finished ---");
        }

        // --- Q2.1: XML Schema Verification Method ---
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            string errors = "";
            bool hasErrors = false;

            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

                settings.ValidationEventHandler += (sender, e) => {
                    errors += $"[{e.Severity}] {e.Message} (Line: {e.Exception?.LineNumber ?? 0}, Pos: {e.Exception?.LinePosition ?? 0})\n";
                    hasErrors = true;
                };

                XmlSchemaSet schemaSet = new XmlSchemaSet();
                 using (XmlReader schemaReader = XmlReader.Create(xsdUrl))
                 {
                    schemaSet.Add(null, schemaReader);
                 }
                settings.Schemas = schemaSet;

                using (XmlReader reader = XmlReader.Create(xmlUrl, settings))
                {
                    try
                    {
                        while (reader.Read()) { }
                    }
                    catch (XmlException ex)
                    {
                        errors += $"[Error] XML Well-Formedness Error: {ex.Message} (Line: {ex.LineNumber}, Pos: {ex.LinePosition})\n";
                        hasErrors = true;
                    
                    }
                }
            }
            catch (Exception ex)
            {
                errors += $"[Error] General Exception during Verification: {ex.Message}\n";
                hasErrors = true;
            }

            return hasErrors ? errors.TrimEnd('\n', '\r') : "No Error";
        }

        // --- Q2.2: XML to JSON Conversion Method ---
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlUrl);

                if (xmlDoc.FirstChild?.NodeType == XmlNodeType.XmlDeclaration)
                {
                    xmlDoc.RemoveChild(xmlDoc.FirstChild);
                }

                // Convert the modified XmlDocument to JSON
                string jsonText = JsonConvert.SerializeXmlNode(xmlDoc, Newtonsoft.Json.Formatting.Indented, false);

                // Replace "@" attribute prefix with "_"
                jsonText = jsonText.Replace("\"@", "\"_");

              
                return jsonText;
            }
            catch (Exception ex)
            {
                return $"[Error] Exception during XML to JSON conversion: {ex.Message}";
            }
        }
    }
}