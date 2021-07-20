using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Xml.Serialization;
using System.Diagnostics;

namespace XSLT
{
    public partial class mainForm : Form
    {
        private Bookstore bookStore;

        public mainForm()
        {
            InitializeComponent();
            bookStore = new Bookstore();
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            string filePath = string.Empty;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }

            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(filePath);
                XmlElement bookstore = xDoc.DocumentElement;

                dataGridView.Rows.Clear();

                foreach (XmlNode bookNode in bookstore)
                {
                    dataGridView.Rows.Add();
                    BlockRow(dataGridView.RowCount - 2);
                    if (bookNode.Attributes.Count > 0)
                    {
                        XmlNode attr = bookNode.Attributes.GetNamedItem("category");
                        if (attr != null)
                        {
                            dataGridView[Category.Index, dataGridView.RowCount - 2].Value = attr.InnerText;
                        }
                    }
                    foreach (XmlNode datanode in bookNode.ChildNodes)
                    {
                        if (datanode.Name == "title")
                        {
                            dataGridView[Title.Index, dataGridView.RowCount - 2].Value = datanode.InnerText;
                        }
                        if (datanode.Name == "author")
                        {
                            dataGridView[Author.Index, dataGridView.RowCount - 2].Value += datanode.InnerText + ";";
                        }
                        if (datanode.Name == "price")
                        {
                            dataGridView[Price.Index, dataGridView.RowCount - 2].Value = datanode.InnerText;
                        }
                    }
                    dataGridView[Author.Index, dataGridView.RowCount - 2].Value =
                        dataGridView[Author.Index, dataGridView.RowCount - 2].Value.ToString().Remove(
                            dataGridView[Author.Index, dataGridView.RowCount - 2].Value.ToString().Length - 1);
                }
            }
            catch (Exception exc) { MessageBox.Show(exc.Message); }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                SaveInBookStore();
                SerializeObject(filePath);
            }
        }

        private void htmlButton_Click(object sender, EventArgs e)
        {
            SaveInBookStore();
            SerializeObject("out.xml");
            Process.Start("out.xml");
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                try
                {
                    dataGridView.Rows.Remove(row);
                }
                catch (Exception exc) { MessageBox.Show("Empty row!"); }
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            for (int i = bookStore.books.Count; i < dataGridView.RowCount - 1; i++)
            {
                BlockRow(i);
            }
        }

        private XmlSerializer CreateOverrider()
        {
            XmlAttributeOverrides xOver = new XmlAttributeOverrides();
            XmlAttributes xAttrs = new XmlAttributes();

            XmlAttributeAttribute xAttribute = new XmlAttributeAttribute("category");
            xAttrs.XmlAttribute = xAttribute;

            xOver.Add(typeof(Book), "category", xAttrs);

            return new XmlSerializer(typeof(Bookstore), xOver);
        }

        private void SerializeObject(string filename)
        {
            XmlSerializer mySerializer = CreateOverrider();
            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.Encoding = Encoding.UTF8;
            XmlWriter writer = XmlWriter.Create(filename, settings);

            writer.WriteProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\"transform.xsl\"");

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            mySerializer.Serialize(writer, bookStore, ns);
            writer.Close();
        }

        private void SaveInBookStore()
        {
            bookStore.books.Clear();
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                try
                {
                    Book book = new Book();
                    book.title = dataGridView[Title.Index, row.Index].Value.ToString();
                    book.author = dataGridView[Author.Index, row.Index].Value.ToString().Split(';').ToList<string>();
                    book.category = dataGridView[Category.Index, row.Index].Value.ToString();
                    book.price = Double.Parse(dataGridView[Price.Index, row.Index].Value.ToString(), CultureInfo.InvariantCulture);
                    bookStore.books.Add(book);
                }
                catch (Exception exc) { }
            }
        }

        private void BlockRow(int index)
        {
            dataGridView.Rows[index].ReadOnly = true;
            foreach (DataGridViewCell cell in dataGridView.Rows[index].Cells)
                cell.Style.BackColor = Color.FromArgb(255, 208, 208, 255);
        }
    }
}
