using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;


    public class BingLocale
    {
        public List<string> locales;
        string confPath = "d:\\binglocal.xml";
        public BingLocale()
        {
            locales = new List<string>();
            ReadLocales(confPath);
        }
        public void ReadLocales(string confPath)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(@confPath);

            XmlNodeList nodelist = xdoc.SelectNodes("//code");
            foreach (XmlNode it in nodelist)
            {
                //Console.WriteLine(it.InnerText);
                locales.Add(it.InnerText);
            }
        }
    }

