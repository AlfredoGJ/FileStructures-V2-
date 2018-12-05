using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStructures
{
    internal class SecondaryIndex
    {

        public long pos;
        public char type;
        public int lenght;
        public int slotsNumber;
        public int mainTableEntries;
        public long next;
        public object[] MainTable;
        public Tuple<object, long>[] SecondaryTable;
        // Index Size=8+1+4+4= 17
        private int mainTableStart = 17;
        public long dataAreaStart;


        public int SizeInBytes
        {
            get
            {
                return mainTableStart + (MainTable.Count() * lenght) + 8 * slotsNumber * mainTableEntries;
            }
        }

        public SecondaryIndex(char type, int lenght,int slots, int entries,long pos ) : base()
        {
            this.pos = pos;
            this.type = type;
            this.lenght = lenght;
            slotsNumber = slots;
            mainTableEntries = entries;
            MainTable = new object[mainTableEntries];

            for (int i = 0; i < mainTableEntries; i++)
                MainTable[i] = -1;

            SecondaryTable = new Tuple<object, long>[slots * mainTableEntries];

            for (int i = 0; i < slots * mainTableEntries; i++)
            {
                if (type == 'I')
                    SecondaryTable[i] = new Tuple<object, long>(-1, -1);
                if (type == 'S')
                    SecondaryTable[i] = new Tuple<object, long>(new string('\0', lenght), -1);
            }

            dataAreaStart = pos + MainTable.Count() * 8;
        }

        public bool HasFreeSlot(object value)
        {

            int index = -1;
            for (int i = 0; i < MainTable.Count(); i++)
            {
                if (MainTable[i] == value)
                {
                    index = i;
                    break;
                }
            }
                return SecondaryTable.Skip(index * slotsNumber).Take(slotsNumber).Any(x => x.Item2 == -1);
            
            return false;
        }

        public void InsertOnEntry(object value, long pointer)
        {

            bool found = false;
            int index = -1;
            for (int i = 0; i < MainTable.Count(); i++)
            {
                if (MainTable[i] == value)
                {
                    index = i;
                    found = true;
                    break;
                }
                else if ((int)MainTable[i] == -1)
                {
                    index = i;
                }

            }

            if (index != -1)
            {
                if (!found)
                    MainTable[index] = value;

                for (int i = index * slotsNumber; i < (index + 1) * slotsNumber; i++)
                {
                    if (SecondaryTable[i].Item2 == -1)
                    {
                        SecondaryTable[i] = new Tuple<object, long>(value, pointer);
                        break;
                    }
                }
            }

        }

        public void ClearEntry(object value, long address)
        {


            int index = -1;
            for (int i = 0; i < MainTable.Count(); i++)
            {
                if (MainTable[i] == value)
                {
                    index = i;
                    break;
                }
            }


            if (index != -1)
            {
            }

            for (int i = index * slotsNumber; i < (index + 1) * slotsNumber; i++)
            {
                if (SecondaryTable[i].Item1 == value)
                {
                    SecondaryTable[i] = new Tuple<object, long>(-1, -1);
                    break;
                }
            }
        }


        public List<Tuple<object, long>> GetEntrySlots(object value)
        {
            List<Tuple<object, long>> result = new List<Tuple<object, long>>();

            int index = -1;
            for (int i = 0; i < MainTable.Count(); i++)
            {
                if (MainTable[i] == value)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                

                if (index < mainTableEntries)
                {
                    for (int i = index * slotsNumber; i < (index * slotsNumber) + slotsNumber; i++)
                        result.Add(SecondaryTable[i]);

                }
                
            }
            return result;


        }



    }
}
