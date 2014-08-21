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
		public string localeTrans(string inLocale)
        {
            string outLocale = "zh-CHS";
            if (locales.Contains(inLocale)) return inLocale;
            if (!inLocale.Contains("-")) return outLocale;
            if(inLocale=="zh-Hant"||inLocale=="zh-HK"||inLocale=="zh-TW") return "zh-CHT";
            int index = inLocale.IndexOf("-");
            inLocale = inLocale.Substring(0, index);
            if (locales.Contains(inLocale)) return inLocale;
            if (inLocale == "zh") return "zh-CHS";

            return outLocale;
        }
    }

