using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStructures
{
    class Index
    {
        public long pos;
        public char type;
        public int lenght;
        public int slotsNumber;
        private int mainTableEntries;
        public long next;
        public long[] MainTable;
        public Tuple<object, long>[] SecondaryTable;
        // Index Size=8+1+4+4= 19
        private int mainTableStart = 9;
        private long dataAreaStart;

        public int SizeInBytes
        {
            get
            {
                return mainTableStart + MainTable.Count() * 8 + (lenght + 8) * slotsNumber;
            }
        }
        
        

        public Index( char type, int lenght, int slots)
        {
            pos = -1;
            this.type = type;
            this.lenght = lenght;
            slotsNumber = slots;

            if (type == 'I')
                mainTableEntries = 10;
            if (type == 'S')
                mainTableEntries = 26;

            dataAreaStart = pos + MainTable.Count() * 8;
            MainTable = new long[mainTableEntries];
            SecondaryTable = new Tuple<object, long>[slots*mainTableEntries ];
           
        }

        public bool HasFreeSlot(int tableEntry)
        {
            
            if (tableEntry < mainTableEntries)
            {
                return SecondaryTable.Skip(tableEntry * mainTableEntries).Take(slotsNumber).Any(x => x.Item2 == -1);
            }
            return false;
        }

        public void InsertOnEntry(int tableEntry, object value, long pointer)
        {
            if (tableEntry < mainTableEntries)
            {
                if (MainTable[tableEntry] == -1)
                {
                    MainTable[tableEntry] = dataAreaStart + tableEntry * mainTableEntries;

                    for (int i = tableEntry * mainTableEntries; i < tableEntry * mainTableEntries + 1; i++)
                    {
                        if (SecondaryTable[i].Item2 == -1)
                            SecondaryTable[i] = new Tuple<object, long>(value, pointer);
                    }
                }
                
            }
           
        }

        
    }
}
