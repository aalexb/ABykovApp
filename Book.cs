using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkApp
{
    public class Book
    {
        public String title;


        public string ReadXML()
        {
            // First write something so that there is something to read ...  
            var b = new Book { title = "Serialization Overview" };
            var writer = new System.Xml.Serialization.XmlSerializer(typeof(Book));
            string newPath = System.IO.Path.GetTempPath();
            var wfile = new System.IO.StreamWriter(newPath+"SerializationOverview.xml");
            writer.Serialize(wfile, b);
            wfile.Close();

            // Now we can read the serialized book ...  
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(Book));
            System.IO.StreamReader file = new System.IO.StreamReader(newPath+"SerializationOverview.xml");
            Book overview = (Book)reader.Deserialize(file);
            file.Close();
            return newPath;
            //return overview.title;

        }
    }
}
