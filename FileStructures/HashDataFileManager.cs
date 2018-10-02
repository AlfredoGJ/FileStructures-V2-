using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileStructures
{
    public class HashDataFileManager : DataFileManager 
    {
        public HashDataFileManager(string name, List<Attribute> template) : base(name, template)
        { }
       
    }
}