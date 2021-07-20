using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XSLT
{
    [Serializable]
    [XmlRoot("bookstore")]
    public class Bookstore
    {
        [XmlElement("book")]
        public List<Book> books;

        public Bookstore() { books = new List<Book>(); }
    }
}
