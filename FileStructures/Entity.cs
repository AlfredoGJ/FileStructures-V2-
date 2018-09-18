using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace FileStructures
{
    public class Entity :INotifyPropertyChanged
    {

        // Private fields
        private DataFileManager dataManager;
        private DictionaryManager dictionaryManager;
        private List<Attribute> attributes;

        // main fields
        private string name;
        private long position;
        private long attributePointer;
        private long dataPointer;
        private long nextPointer;

        public event PropertyChangedEventHandler PropertyChanged;



        // public properties
        public string Name { get => name; set => name = value; }
        public long Position { get => position; set  => position = value; }
        public long AttributesPtr { get => attributePointer; }
        public long DataPtr { get => dataPointer; }
        public long NextPtr { get => nextPointer; set => nextPointer = value; }
        public List<Attribute> Attributes { get => attributes; }
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




        public Entity(string name, long position, long atrPtr, long dataPtr, long nextPtr, DictionaryManager dictionaryManager, List< Attribute> attributes)
        {
            this.name = name;
            this.position = position;
            this.attributePointer = atrPtr;
            this.dataPointer = dataPtr;
            this.nextPointer = nextPtr;
            this.dictionaryManager = dictionaryManager;
            this.attributes = attributes;


        }


        public Entity(string name, DictionaryManager manager)
        {
            this.name = name;
            position = -1;
            attributePointer = - 1;
            dataPointer = -1;
            nextPointer = -1;
            this.dictionaryManager = manager;
            attributes = new List<Attribute>();
            
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


        public async void AddAttribute(Attribute attribute)
        {
            attribute.Position = dictionaryManager.FileLength;

            int i;
            for (i = 0; i < attributes.Count; i++)
            {
                int comparison = string.Compare(attribute.Name, attributes[i].Name, StringComparison.CurrentCulture);
                if (comparison == -1)
                    break;
            }

            if (i == 0)
            {
                attribute.NextPtr = attributePointer;
                attributePointer = attribute.Position;
                attributes.Insert(i, attribute);
            }
            else
            {
                if (i == attributes.Count)
                {
                    attributes[i - 1].NextPtr = attribute.Position;
                    attributes.Add(attribute);

                }
                else
                {
                    attribute.NextPtr = attributes[i - 1].NextPtr;
                    attributes[i - 1].NextPtr = attribute.Position;
                    attributes.Insert(i, attribute);
                }

            }
            
            await dictionaryManager.WriteEntity(this);
            foreach (Attribute attr in attributes)
            {
                await dictionaryManager.WriteAttribute(attr);
            }

          
        }


        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public async void RemoveAttribute(Attribute attribute)
        {
            int index = attributes.FindIndex(e => e.Name == attribute.Name);

            if (index == 0)
            {
                attributePointer = attributes[0].NextPtr;
                attributes.RemoveAt(0);
            }
            else
            {
                attributes[index - 1].NextPtr = attributes[index].NextPtr;
                attributes.RemoveAt(index);
            }
            await dictionaryManager.WriteEntity(this);
            foreach (Attribute attr in Attributes)
            {
                await dictionaryManager.WriteAttribute(attr);
            }

        }

        //public void RemoveAttribute()
        //{
        //    throw new System.NotImplementedException();
        //}

        //public void SetPrimaryKeyAttribute()
        //{
        //    throw new System.NotImplementedException();
        //}

        //public void AddRegister()
        //{
        //    throw new System.NotImplementedException();
        //}

        //public void DeleteRegister()
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}