using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileStructures
{
    public class Entity
    {

        // Private fields
        private DataFileManager dataManager;
        private long atrPtr;

        // main fields
        private string name;
        private long position;
        private long attributePointer;
        private long dataPointer;
        private long nextPointer;

        // public properties
        public string Name { get => name; }
        public long Position { get => position; set  => position = value; }
        public long AttributesPtr { get => attributePointer; }
        public long DataPtr { get => dataPointer; }
        public long NextPtr { get => nextPointer; set => nextPointer = value; }
        public char[] ArrayName
        {
            get
            {
                char[] arrName = new char[30];
                arrName.Initialize();
                for(int i=0; i<name.Length;i++)
                {
                    arrName[i] = name[i];
                }
                return arrName;
            }
        }




        public Entity(string name, long position, long atrPtr, long dataPtr, long nextPtr)
        {
            this.name = name;
            this.position = position;
            this.attributePointer = atrPtr;
            this.dataPointer = dataPtr;
            this.nextPointer = nextPtr;
        }


        public Entity(string name)
        {
            this.name = name;
            position = -1;
            attributePointer = - 1;
            dataPointer = -1;
            nextPointer = -1;
            
        }
        
       
        public List<Attribute> Attributes
        {
            get => default(List<Attribute>);
            set
            {
            }
        }

        public object PrimaryKeyAttribute
        {
            get => default(int);
            set
            {
            }
        }

        public List<DataRegister> Registers
        {
            get => default(List<DataRegister>);
            set
            {
            }
        }

        public bool Edited
        {
            get => default(bool);
            set
            {
            }
        }

        public void AddAttribute()
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAttribute()
        {
            throw new System.NotImplementedException();
        }

        public void SetPrimaryKeyAttribute()
        {
            throw new System.NotImplementedException();
        }

        public void AddRegister()
        {
            throw new System.NotImplementedException();
        }

        public void DeleteRegister()
        {
            throw new System.NotImplementedException();
        }
    }
}