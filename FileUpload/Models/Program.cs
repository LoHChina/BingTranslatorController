using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

    class Program
    {
        static void Main(string[] args)
        {
            XlifParser parser = new XlifParser();
            String xpath = "d:\\test.xlf";
            String testpaht = "d:\\Books.Xml";
            //Console.WriteLine( parser.run(xpath));
            string str = parser.Translate("cell", "en", "fr");
            Console.WriteLine(str);
            //BingLocale bl = new BingLocale();
            /*Console.WriteLine(xpath);
            Xlif result= parser.ReadXlif(xpath);
            Console.WriteLine(result.source_language + "2" + result.target_language);
            foreach (string t in result.wordList)
            {
                Console.WriteLine(t);
            }
            string str = parser.Translate("cell", "en", "fr");
            Console.WriteLine(str);*/
            /*String xpath = "d:\\test.xlf";
            XmlTextReader reader = new XmlTextReader (xpath);
            while (reader.Read()) 
            {
                switch (reader.NodeType) 
                {
                    case XmlNodeType.Element: // The node is an element.
                        if (reader.Name == "file")
                        {
                            Console.Write("<" + reader.GetAttribute(1));
                        }
                        
                        Console.Write("<" + reader.Name);
                        Console.WriteLine(">");
                        break;
                    case XmlNodeType.Text: //Display the text in each element.
                        Console.WriteLine (reader.Value);
                        break;
                    case XmlNodeType.EndElement: //Display the end of the element.
                        Console.Write("</" + reader.Name);
                        Console.WriteLine(">");
                        break;
                }
            }
            Console.ReadLine();*/

        }
    }

