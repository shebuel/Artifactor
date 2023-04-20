using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifactor
{
    public class Checks
    {
        public string testID { get; set; }
        public string testName { get; set; }
        public string testDescription { get; set; }
        public string testType { get; set; }
        public bool checkCompleted { get; set; }
        public List<string> filePath { get; set; }

        public Checks()
        {
            filePath = new List<string>();
            checkCompleted = false;
        }
    
    }

}
