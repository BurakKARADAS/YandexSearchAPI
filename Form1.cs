using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace YandexSearchAPI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static string GetValue(XElement group, string name)
        {
            try
            {
                return group.Element("doc").Element(name).Value;
            }
            catch
            {
                return string.Empty;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ServicePointManager.Expect100Continue = false;

            string aranacak = "c#";
            string useranme = "goren-ali";
            string key = "03.178948983:5f00fe5e625cdd51ef5819e61107cc0a";
            string yandexurl = @"https://yandex.com.tr/search/xml?user=" + useranme + @"&key=" + key + @"&lr=983&l10n=tr&filter=none";

            // Текст запроса в формате XML
            string command =
              @"<?xml version=""1.0"" encoding=""UTF-8""?>   
          <request>   
           <query>" + aranacak + @"</query>
           <page>" + 0 + @"</page>
           <groupings>
             <groupby attr=""d"" 
                    mode=""deep"" 
                    groups-on-page=""10"" 
                    docs-in-group=""1"" />   
           </groupings>   
          </request>";

            byte[] bytes = Encoding.UTF8.GetBytes(command);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(yandexurl);
            request.Method = "POST";
            request.ContentLength = bytes.Length;
            request.ContentType = "text/xml";

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            // Получаем ответ
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            XmlReader xmlReader = XmlReader.Create(response.GetResponseStream());
            XDocument xmlResponse = XDocument.Load(xmlReader);

            var groupQuery = from gr in xmlResponse.Elements().
                Elements("response").
                Elements("results").
                Elements("grouping").
                Elements("group")
                             select gr;

              

            for (int h = 0; h < groupQuery.Count(); h++)
            {
                dgv.Rows.Add(GetValue(groupQuery.ElementAt(h), "title"), GetValue(groupQuery.ElementAt(h), "url"));
             
            }
        }
    }
}
