using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;

namespace BestBuyProductDataToCSV
{
    class Program
    {
        static void Main(string[] args)
        {
            // Make sure a file path was passed as an argument.
            if (args.Length < 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Must supply input file path and output folder via command line.");
                Console.ResetColor();
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
                Environment.Exit(1);
            }

            // Open an xml reader for that file.
            var inFile = args[0];
            var inFileName = Path.GetFileName(inFile);
            var outputFolder = args[1];
            var xmlReader = XmlReader.Create(new FileStream(inFile, FileMode.OpenOrCreate, FileAccess.Read));

            // Make sure the folders exist.
            var outputCsvFolder = $"{outputFolder}outputcsv/";
            Directory.CreateDirectory(outputCsvFolder);
            var outputArjunFolder = $"{outputFolder}arjun/";
            Directory.CreateDirectory(outputArjunFolder);

            // Writer for product csv data
            var csvFstream = new FileStream($"{outputFolder}outputcsv/{inFileName}.csv", FileMode.OpenOrCreate, FileAccess.Write);
            var csvWriter = new StreamWriter(csvFstream);
            // Writer for arjun's data
            var arjunFstream = new FileStream($"{outputFolder}arjun/{inFileName}.arjun.csv", FileMode.OpenOrCreate, FileAccess.Write);
            var arjunWriter = new StreamWriter(arjunFstream);
            // Writer for categories data.
            var catFstream = new FileStream($"{outputFolder}categories.csv", FileMode.Append, FileAccess.Write);
            var categoryWriter = new StreamWriter(catFstream);

            // List of categories from the best buy data.
            // <categoryId, categoryName>
            // TODO: Read this list in from a file, so we can write to a file without duplicating categories.
            var categories = new List<Tuple<string, string>>();


            // While their are still products to be processed.
            Console.Write("Reading xml... ");
            while (!xmlReader.EOF)
            {
                // * Find a product in the xml
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name.Equals("product"))
                {
                    // * Process the product
                    var productElem = xmlReader.ReadSubtree();
                    var p = ProcessProductXml(productElem);
                    // Skip this product if it is missing important data.
                    if (p.HasAllImportantData())
                    {
                        // * Add to <Department, Department id> list
                        categories.Add(new Tuple<string, string>(p.CategoryId, p.Category));
                        // * Write out product as a row in csv. ToString give csv format.
                        csvWriter.WriteLine($"{p.ProductId}\t{p.Name}\t{p.Description}\t{p.ImagePath}\t{p.Price}\t{p.CategoryId}");
                        // * Write this product in CSV for Arjun
                        arjunWriter.WriteLine($"{p.ProductId}, {p.Name}, {p.Category}, {p.Description}");
                    }
                }
                xmlReader.Read();
            }
            Console.WriteLine("Done.");

            // Sort categories
            var sortedList = categories.OrderBy(x => x.Item1);
            // Write categories to file
            foreach (var tuple in sortedList)
            {
                categoryWriter.WriteLine($"{tuple.Item1}, {tuple.Item2}");
            }

            // Make sure all fstreams are closed.
            csvWriter.Close();
            arjunWriter.Close();
            categoryWriter.Close();
        }

        /// <summary>
        /// Takes an <see cref="XmlReader"/> and generates ProductData.
        /// </summary>
        /// <param name="xml">An XmlReader corresponding
        /// to a product element.</param>
        /// <returns><see cref="ProductData"/> found in the root of the XmlReader.</returns>
        static ProductData ProcessProductXml(XmlReader xml)
        {
            var data = new ProductData();
            // Find xml data
            while (!xml.EOF)
            {
                if (xml.NodeType == XmlNodeType.Element)
                {
                    var elem = xml.Name;
                    xml.Read();
                    // Nothing to do if the next element is not text.
                    if (xml.NodeType != XmlNodeType.Text) continue;

                    // No need to assign if it is just blank.
                    if (xml.Value.Length > 0)
                    {
                        // Don't allow new lines or commas in data.
                        var value = xml.Value.Replace('\n', ' ').Replace(",", "&#044;");
                        switch (elem)
                        {
                            case "productId":
                                if (data.ProductId.Length > 0) break;
                                data.ProductId = value;
                                break;
                            case "name":
                                if (data.Name.Length > 0) break;
                                data.Name = value;
                                break;
                            case "regularPrice":
                                if (data.Price.Length > 0) break;
                                data.Price = value;
                                break;
                            case "shortDescription":
                                if (data.Description.Length > 0) break;
                                data.Description = value;
                                break;
                            case "departmentId":
                                if (data.CategoryId.Length > 0) break;
                                data.CategoryId = value;
                                break;
                            case "department":
                                if (data.Category.Length > 0) break;
                                data.Category = value;
                                break;
                            case "image":
                                if (data.ImagePath.Length > 0) break;
                                data.ImagePath = value;
                                break;
                        }
                    }
                }
                xml.Read();
            }
            return data;
        }
    }
}
