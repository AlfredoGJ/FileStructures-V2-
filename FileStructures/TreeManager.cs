using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStructures
{
    public class TreeManager
    {
        private string name;
        private List<Attribute> attributes;
        private Tree tree;
        public Tree Tree{get =>  tree ;set => tree= value;}
        private bool initialized;

        public TreeManager(string name)
        {
            initialized = false;
            this.name = name;
            tree = new Tree(2);
        }

        public void Initialize(List<DataRegister> registers)
        {
            if (!initialized)
            {
                foreach (DataRegister register in registers)
                    this.tree.Insert(register.Key,register.Position);

                initialized = true;
            }
        }

        //public void AddKey(int key, long address)
        //{
        //    tree.Insert(key,address);
        //}

        //public void AddKey(string key, long address)
        //{
        //    tree.Insert(key, address);
        //}

        public void AddKey(object key, long address)
        {
            tree.Insert(key, address);
        }


    }
}
