using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStructures
{
    public class Index
    {
        public long pos;
        public char type;
        public int lenght;
        public int slotsNumber;
        private int mainTableEntries;
        public long next;
        public long[] MainTable;
        public Tuple<object, long>[] SecondaryTable;
        // Index Size=8+1+4+4= 17
        private int mainTableStart = 17;
        private long dataAreaStart;

        public int SizeInBytes
        {
            get
            {
                return mainTableStart + (MainTable.Count() * 8) + (lenght + 8) * slotsNumber * mainTableEntries;
            }
        }



        public Index(char type, int lenght, int slots, long pos)
        {
            this.pos = pos;
            this.type = type;
            this.lenght = lenght;
            slotsNumber = slots;

            if (type == 'I')
                mainTableEntries = 10;
            if (type == 'S')
                mainTableEntries = 26;

            MainTable = new long[mainTableEntries];

            for (int i = 0; i < mainTableEntries; i++)
                MainTable[i] = -1;

            SecondaryTable = new Tuple<object, long>[slots * mainTableEntries];

            for (int i = 0; i < slots * mainTableEntries; i++)
            {
                if(type=='I')
                    SecondaryTable[i] = new Tuple<object, long>(-1, -1);
                if (type == 'S')
                    SecondaryTable[i] = new Tuple<object, long>(new string('\0',lenght), -1);
            }
                

            dataAreaStart = pos + MainTable.Count() * 8;
           
           
        }

        public bool HasFreeSlot(int tableEntry)
        {
            
            if (tableEntry < mainTableEntries)
            {
                return SecondaryTable.Skip(tableEntry * slotsNumber).Take(slotsNumber).Any(x => x.Item2 == -1);
            }
            return false;
        }

        public void InsertOnEntry(int tableEntry, object value, long pointer)
        {
            if (tableEntry < mainTableEntries)
            {
                if (MainTable[tableEntry] != -1)
                {
                    //MainTable[tableEntry] = dataAreaStart + tableEntry * mainTableEntries;

                    for (int i = tableEntry * slotsNumber; i < (tableEntry + 1) * slotsNumber ; i++)
                    {
                        if (SecondaryTable[i].Item2 == -1)
                        {
                            SecondaryTable[i] = new Tuple<object, long>(value, pointer);
                            break;
                        }
                    }
                }
                else
                {
                    MainTable[tableEntry] = dataAreaStart + tableEntry * slotsNumber;
                    SecondaryTable[tableEntry*slotsNumber] = new Tuple<object, long>(value, pointer);

                }
                
            }
           
        }

        public List<Tuple<object, long>> GetEntrySlots(int tableEntry)
        {
            List<Tuple<object, long>> result = new List<Tuple<object, long>>();

            if (tableEntry < mainTableEntries)
            {
                for (int i = tableEntry*slotsNumber; i < (tableEntry * slotsNumber) + slotsNumber; i++)
                    result.Add(SecondaryTable[i]);

            }
            return result;
        }

        
    }
}
