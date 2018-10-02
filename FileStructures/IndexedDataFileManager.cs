using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileStructures
{
    public class IndexedDataFileManager:DataFileManager
    {
        public IndexedDataFileManager(string name, List<Attribute> template) : base(name,template )
        { }
    }
}