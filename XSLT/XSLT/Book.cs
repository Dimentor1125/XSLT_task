using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace XSLT
{
    public class Book
    {
        [XmlAttribute]
        public string category { get; set; }

        public string title { get; set; }

        [XmlElement("author")]
        public List<string> author { get; set; }
        public int year { get; set; }
        public double price { get; set; }

        public Book()
        {
            category = string.Empty;
            title = string.Empty;
            author = new List<string>();
            year = 2021;
            price = -1;
        }
    }
}
