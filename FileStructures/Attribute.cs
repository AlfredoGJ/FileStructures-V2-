﻿using System;
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


        public Attribute(string name, char type, int length, int indexType)
        {
            this.name = name;
            Position = -1 ;
            Type = type;
            Length = length;
            IndexType =indexType;
            IndexPtr = -1;
            NextPtr = -1;
        }

        public Attribute(string name, char type, int length, int indexType, long position, long indexptr, long nextptr)
        {
            this.name = name;
            Position = position;
            Type = type;
            Length = length;
            IndexType = indexType;
            IndexPtr = indexptr;
            NextPtr = nextptr;
        }


        public void PasteTo(Attribute attribute)
        {
           attribute.name= this.name;
           attribute.Position= Position;
           attribute.Type= Type ;
           attribute.Length= Length ;
           attribute.IndexType= IndexType;
           attribute.IndexPtr= IndexPtr;
           attribute.NextPtr= NextPtr;
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