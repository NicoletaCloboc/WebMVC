using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace WebMVC.Models
{
    public class Upload
    {
        /// <summary>
        /// Xml file with orders
        /// </summary>
        [BindProperty]
        public IFormFile XML_File { get; set; }

        /// <summary>
        /// Message of upload process
        /// </summary>
        public string UploadMessage { get; set; }

        public string CopyXMLFile(IWebHostEnvironment _hostingEnvironment)
        {
            try
            {
                //List<string> permittedExtensions = new List<string>(){ ".xml"};
                string ext = Path.GetExtension(XML_File.FileName).ToLowerInvariant();

                if (!string.IsNullOrEmpty(ext) && ext == ".xml") //permittedExtensions.Contains(ext))
                {
                    string uniqueFileName = null;

                    if (XML_File != null)
                    {
                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "XML");
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + XML_File.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (FileStream fs = new FileStream(filePath, FileMode.Create))
                        {
                            XML_File.CopyTo(fs);
                        }

                        if (UploadMessage == null) UploadMessage = string.Empty;
                        if (!ValidateXML(_hostingEnvironment, filePath))
                        {
                            if (File.Exists(filePath))
                                File.Delete(filePath);
                            UploadMessage += "XML Order not valid.";
                        }
                        else
                        {
                            //check if orders are valid
                            try
                            {
                                GetOrdersFromOneXml(uploadsFolder, uniqueFileName);
                            }
                            catch (Exception e)
                            {
                                UploadMessage += Environment.NewLine + e.Message;
                            }
                        }
                        if (string.IsNullOrEmpty(UploadMessage))
                            UploadMessage += Environment.NewLine + "Order sent successfully.";
                    }
                }
                else
                    UploadMessage += Environment.NewLine + "File must have XML type.";
            }
            catch
            {
                UploadMessage += Environment.NewLine + "File with wrong format.";
            }
            return UploadMessage;
        }

        public bool ValidateXML(IWebHostEnvironment _hostingEnvironment, string filePath)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;

            // read in schema file
            settings.Schemas.Add(null, Path.Combine(_hostingEnvironment.WebRootPath, "XSD", "OrderImport.xsd"));

            settings.ValidationFlags |= System.Xml.Schema.XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= System.Xml.Schema.XmlSchemaValidationFlags.ReportValidationWarnings;
            XmlReader xmlReader = null;
            try
            {
                xmlReader = XmlReader.Create(filePath, settings);
                while (xmlReader.Read()) { }
                return true;
            }
            catch //(Exception e)
            {
                //return "Validation not passed " + e.Message;
                return false;
            }
            finally
            {
                if (xmlReader != null)
                    xmlReader.Dispose();
            }
        }

        public static List<Order> GetOrdersFromOneXml(string folderPath, string fileName)
        {
            //Load the XML file in XmlDocument.
            XmlDocument doc = new XmlDocument();
            List<Order> orders = new List<Order>();

            doc.Load(Path.Combine(folderPath, fileName));

            //Loop through the selected Nodes.
            foreach (XmlNode node in doc.SelectNodes("/BigShoeDataImport/Order"))
            {
                orders.Add(new Order
                {
                    CustomerName = node.Attributes["CustomerName"].InnerText,
                    CustomerEmail = node.Attributes["CustomerEmail"].InnerText,
                    Quantity = short.Parse(node.Attributes["Quantity"].InnerText),
                    Notes = node.Attributes["CustomerEmail"].InnerText,
                    Size = float.Parse(node.Attributes["Size"].InnerText),
                    DateRequired = DateTime.Parse(node.Attributes["DateRequired"].InnerText)
                });
            }
            return orders;
        }
    }
}