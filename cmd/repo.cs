using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd
{   
    public class repo
    {
        public string name { set; get; }
        public string author { set; get; }
        public DateTime date_creation { set; get; }
        public DateTime updated { set; get; }
        public int filecount { set; get; }
        public List<file> file_info = new List<file>();
        public List<users> user_list = new List<users>();

    }

    public class file
    {  
        public string crc { set; get; }
        public FileAttributes Attributes { set; get; }
        public string Extension { set; get; }
        public string FullName { set; get; }
        public long Length { set; get; }
        public string Name { set; get; }     
    }
     public class users
    {
        public string login { set; get; }
        public string pass { set; get; }
        public string role { set; get; }
      
    }

}
