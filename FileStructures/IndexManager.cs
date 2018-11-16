using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStructures
{
    class IndexManager
    {
        private string name;
        private List<Index> indexes;
        public List<Index> Indexes { get; }

        public IndexManager(List<Attribute> template)
        {

        }

        public IndexManager(string name)
        {
            this.name = name;

        }

        private void addCharIndex(int slots,int charLenght,long startAddr)
        {

        }

        private void addIntIndex(int slots, long startAddr)
        {

        }
        private void WriteToFile()
        {
        }

        private void ReadFromFile()
        {

        }

        public long GetAddr(string value)
        {
            long ptr = -1;

            return ptr;
        }

        public long GetAddr(int value)
        {
            long ptr = -1;

            return ptr;
        }

        public bool InsertValue(int value, int indexNumber)
        {
            bool result = false;

            return result;
        }

        public bool InsertValue(string value, int indexNumber)
        {
            bool result = false;

            return result;
        }



        public void 






    }
}
