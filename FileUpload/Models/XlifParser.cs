using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Web;
using System.Text.RegularExpressions;
using FileUpload.Models;


    public class Xlif
    {
        public List<string> wordList;
        public string source_language;
        public string target_language;
        public Xlif()
        {
            wordList = new List<string>();
        }
    }

    public class AdmAccessToken
    {

        public string access_token { get; set; }

        public string token_type { get; set; }

        public string expires_in { get; set; }

        public string scope { get; set; }
    }

    class XlifParser
    {
        public string Xlif2String(string xlifPath)
        {
            string content = "";
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(@xlifPath);
            content = xdoc.OuterXml;
            return content;
        }
        public Xlif ReadXlif(string xlifPath)
        {
            Xlif _xlifFile=new Xlif();
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(@xlifPath);
            //Console.WriteLine(xdoc.DocumentElement.Value);
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(xdoc.NameTable);
            nsMgr.AddNamespace("ns", "urn:oasis:names:tc:xliff:document:1.2");

            XmlNode root = xdoc.SelectSingleNode("/ns:xliff/ns:file",nsMgr);
            if (root != null)
            {
                string source_language = root.Attributes["source-language"].Value;
                _xlifFile.source_language = source_language != null ? source_language : "en";
                string target_language = root.Attributes["target-language"].Value;
                _xlifFile.target_language = target_language != null ? target_language : "sw";

            }
            //Console.WriteLine(root.InnerXml);
            XmlNodeList nodelist = xdoc.SelectNodes("/ns:xliff/ns:file/ns:body/ns:group", nsMgr);
            foreach (XmlNode it in nodelist)
            {
                //Console.WriteLine(it.SelectSingleNode("//ns:source", nsMgr).InnerText);
                //Console.WriteLine("--------------");
                XmlNodeList childnodelist=it.SelectNodes("ns:group/ns:trans-unit", nsMgr);
                foreach (XmlNode childnode in childnodelist)
                {
                    if (childnode.Attributes["translate"].Value == "yes")
                    {
                        string word = childnode.SelectSingleNode("ns:source", nsMgr).InnerText;
                        _xlifFile.wordList.Add(word);
                        //Console.WriteLine(word);
                    }

                }
            }
            return _xlifFile;
        }
		public string run(string fileLocalPath, string fileName)
        {
            int exindex=fileName.LastIndexOf('.');
            string extention = fileName.Substring(exindex+1);
            if (extention == "zip")
            {
                zipRun(fileLocalPath,fileName);
                return "Zip File translated!";
            }
            else if (extention == "xlf")
            {
                return xliffRun(fileLocalPath);

            }
            return "Unsupported file type";
        }
        public void zipRun(string zipLocalPath,string zipName)
        {
            string zipDirectory = zipLocalPath + "_\\";
            zipFileApi.UnZipFiles(zipLocalPath, zipDirectory);
            DirectoryInfo dir=new DirectoryInfo(zipDirectory);
            readDictoryAndTranslate(dir);
            int fatherDirPos=zipLocalPath.LastIndexOf("\\");
             zipFileApi.CreateZip(zipDirectory, "d:\\ftp\\" + zipName);
        }

        public void readDictoryAndTranslate(DirectoryInfo directoryinfo)
        {
            foreach( FileInfo fileinfo in directoryinfo.GetFiles("*.xlf") ){
                xliffRun(fileinfo.FullName);
            }
        }
        public string xliffRun(string xlifPath)
        {
            //default source and target language
            string source_language = "en";
            string target_language = "fr";
            BingLocale bingLocals = new BingLocale();
            
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(@xlifPath);
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(xdoc.NameTable);
            nsMgr.AddNamespace("ns", "urn:oasis:names:tc:xliff:document:1.2");
            XmlNode root = xdoc.SelectSingleNode("/ns:xliff/ns:file", nsMgr);
            if (root != null)
            {
                source_language = root.Attributes["source-language"].Value;
                target_language = root.Attributes["target-language"].Value;
                source_language = bingLocals.localeTrans(source_language);
                target_language = bingLocals.localeTrans(target_language);
            }

            XmlNodeList nodelist = xdoc.SelectNodes("/ns:xliff/ns:file/ns:body/ns:group", nsMgr);
            foreach (XmlNode it in nodelist)
            {
                XmlNodeList childnodelist = it.SelectNodes("ns:group/ns:trans-unit", nsMgr);
                foreach (XmlNode childnode in childnodelist)
                {
                    if (childnode.Attributes["translate"].Value == "yes")
                    {
                        string word = childnode.SelectSingleNode("ns:source", nsMgr).InnerText;
                        string translated_word = Translate(word,source_language,target_language);
                        //string[] wordlist = Regex.Split(translated_word, " ", RegexOptions.IgnoreCase);
                        childnode.SelectSingleNode("ns:target", nsMgr).InnerText = translated_word;
                    }

                }
            }
            xdoc.Save(xlifPath);
            return "success";
        }

        public String getToken()
        {
            string clientID = "BingMT";
            string clientSecret = "G5hO8YdCtKvFQaFDaWwb6kLPePOfeUWL3tso8qJrjtc=";

            String strTranslatorAccessURI = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
            String strRequestDetails = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", HttpUtility.UrlEncode(clientID), HttpUtility.UrlEncode(clientSecret));

            System.Net.WebRequest webRequest = System.Net.WebRequest.Create(strTranslatorAccessURI);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(strRequestDetails);
            webRequest.ContentLength = bytes.Length;
            using (System.IO.Stream outputStream = webRequest.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
            System.Net.WebResponse webResponse = webRequest.GetResponse();
            //StreamReader sr = new StreamReader(webResponse.GetResponseStream());
            //string content = sr.ReadToEnd();
            //DataTable dt = JsonConvert.DeserializeObject<DataTable>(content);


            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(AdmAccessToken));
            //Get deserialized object from JSON stream 
            AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());

            string headerValue = "Bearer " + token.access_token;
            return headerValue;
        }

        public String Translate(String strTranslateString, string flang, string tlang)
        {




            //string token = "http%3a%2f%2fschemas.xmlsoap.org%2fws%2f2005%2f05%2fidentity%2fclaims%2fnameidentifier=BingMT&http%3a%2f%2fschemas.microsoft.com%2faccesscontrolservice%2f2010%2f07%2fclaims%2fidentityprovider=https%3a%2f%2fdatamarket.accesscontrol.windows.net%2f&Audience=http%3a%2f%2fapi.microsofttranslator.com&ExpiresOn=1407564528&Issuer=https%3a%2f%2fdatamarket.accesscontrol.windows.net%2f&HMACSHA256=kgCIsXwymdgTZkdE1yP3xitOBePVM%2b0w3G4%2fQyD2KxI%3d";
            string headerValue = getToken();
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?text=" + HttpUtility.UrlEncode(strTranslateString) + "&from=" + flang + "&to=" + tlang + "";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Headers.Add("Authorization", headerValue);
            WebResponse response = null;

            try
            {
                response = httpWebRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    string translated = reader.ReadToEnd();
                    int head = translated.IndexOf('>');
                    int tail = translated.LastIndexOf('<');
                    translated=translated.Substring(head+1,tail-head-1);
                    return translated;
                }
            }
            catch (WebException)
            {
                return "翻译失败";
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }
        }

    }

