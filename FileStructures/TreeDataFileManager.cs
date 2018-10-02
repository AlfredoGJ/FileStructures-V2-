using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileStructures
{
    public class TreeDataFileManager:DataFileManager
    {
        public TreeDataFileManager(string name, List<Attribute> template) : base(name,template)
        { }
    }
}