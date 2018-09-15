using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileStructures
{
    public class Attribute
    {


        // main fields
        private string name;
        private long nextPointer;

        // public properties
        public string Name { get => name; set => name = value; }
        public long Position { get; set; }
        public char Type { get; set; }
        public int Length { get; set; }
        public int IndexType { get; set; }
        public long IndexPtr { get; set; }
        public long NextPtr { get => nextPointer; set => nextPointer = value; }


        
        
        public char[] ArrayName
        {
            get
            {
                char[] arrName = new char[30];
                arrName.Initialize();
                for (int i = 0; i < name.Length; i++)
                {
                    arrName[i] = name[i];
                }
                return arrName;
            }
            
        }


        public Attribute(string name,long position, char type, int length, int indexType, long indexPrt, long nextPtr)
        {
            this.name = name;
            Position = position;
            Type = type;
            Length = length;
            IndexType =indexType;
            IndexPtr = IndexPtr;
            NextPtr = nextPtr ;
        }


       


        public bool Edited
        {
            get => default(bool);
            set
            {
            }
        }
    }
}