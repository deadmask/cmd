using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace cmd
{
    public class settings
    {
        public bool app_autostart { set; get; }
        public bool server_autostart { set; get; }
        public bool form_closing { set; get; }
        public string path_to_repo { set; get; }
        public bool protect_repo { set; get; }
        public string login { set; get; }
        public string password { set; get; }
        public string server_port { set; get; }
    
    }
}

public class XMLFile<T>
{
    Type dataType;
    string filePath;

    public XMLFile(string filePath)
    {
        this.filePath = filePath + ".xml";
        dataType = typeof(T);
    }

    public T Load()
    {       
            TextReader tr = new StreamReader(filePath);
            XmlSerializer reader = new XmlSerializer(dataType);
            T instance = (T)reader.Deserialize(tr);
            tr.Close();
            return instance;        
    }

    public void Save(object Obj)
    {
        TextWriter tw = new StreamWriter(filePath);
        XmlSerializer writer = new XmlSerializer(dataType);
        writer.Serialize(tw, Obj);
        tw.Close();
    }
}