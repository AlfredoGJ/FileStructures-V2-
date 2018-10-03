using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStructures
{
    public class Entity :INotifyPropertyChanged
    {


        public delegate void ItemsOnFileChanged();
        public event ItemsOnFileChanged itemsOnFileChanged;
        // Private fields
        private DataFileManager dataManager;
        private DictionaryManager dictionaryManager;
        private List<Attribute> attributes;
        private Attribute primaryKey;
        private List<DataRegister> registers;


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
        public Attribute PrimaryKey { get => primaryKey; }

        public List<DataRegister> Registers { get => registers; }
       


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
            this.primaryKey = attributes.Find(X => X.IndexType == 2);
            this.dataManager = new DataFileManager(Name,attributes);
            dataManager.itemsOnFileChanged += UpdateRegistersData;
            dataManager.ReadAllRegisters(dataPtr);
           


        }

        private void UpdateRegistersData()
        {
            this.registers = dataManager.registers;
            itemsOnFileChanged?.Invoke();
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

            int i= attributes.Count;
            //for (i = 0; i < attributes.Count; i++)
            //{
            //    int comparison = string.Compare(attribute.Name, attributes[i].Name, StringComparison.CurrentCulture);
            //    if (comparison == -1)
            //        break;
            //}


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

        //This method is provitional, must be integrated on  AddAttribute later
        public async void AddAttributeExistent(Attribute attribute)
        {
            //attribute.Position = dictionaryManager.FileLength;

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
               // attributes.Insert(i, attribute);
            }
            else
            {
                if (i == attributes.Count)
                {
                    attributes[i - 1].NextPtr = attribute.Position;
                   // attributes.Add(attribute);

                }
                else
                {
                    attribute.NextPtr = attributes[i - 1].NextPtr;
                    attributes[i - 1].NextPtr = attribute.Position;
                    //attributes.Insert(i, attribute);
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

        public async Task<bool> UpdateAttribute(Attribute attribute)
        {
            Attribute findResult = attributes.Find(x => x.Name == attribute.Name);

            // There is no attribute named like the new one, it is rewrited in the same position with different name
            // Must be reordered acccording to its name
            if (findResult == null)
            {
                attribute.PasteTo(attributes.Find(x=> x.Position==attribute.Position));
               // AddAttributeExistent(attribute);  //implementation paused
                await dictionaryManager.WriteAttribute(attribute);
            }
            // Already exist an attribute named like this
            else
            {
                // The attribute with the same name as this is actually the same attribute
                if (findResult.Position == attribute.Position)
                {
                    attribute.PasteTo(attributes.Find(x => x.Position == attribute.Position));
                    await dictionaryManager.WriteAttribute(attribute);
                }
                //The attribute with the same name is another attribute, no operation is done and an error msg shows to the user
                else
                    return false;

            }

            return true;
        }

       

        //public void SetPrimaryKeyAttribute()
        //{
        //    throw new System.NotImplementedException();
        //}

        public void AddRegister(DataRegister register)
        {
            if (register.Key != null)
            {
                InsertDataOrdered(register);
            }
            else
            {

            }
        }


        private async void InsertDataOrdered(DataRegister register)
        {
            register.Position = dataManager.FileLength;
            int i=0;

            switch (primaryKey.Type)
            {
                case 'S':
                    for (i = 0; i < registers.Count; i++)
                    {
                        int comparison = string.Compare(register.Key as string, registers[i].Key as string, StringComparison.CurrentCulture);
                        if (comparison == -1)
                            break;
                    }
                    break;

                case 'I':

                    for (i = 0; i < registers.Count; i++)
                    {
                       if ((int)register.Key  < (int)registers[i].Key)
                            break;
                    }
                    break;
            }
         
            if (i == 0)
            {
                register.NextPtr = dataPointer;
                dataPointer = register.Position;
                registers.Insert(i, register);
            }

            else
            {
                if (i == registers.Count)
                {
                    registers[i - 1].NextPtr = register.Position;
                    registers.Add(register);

                }
                else
                {
                    register.NextPtr = registers[i - 1].NextPtr;
                    registers[i - 1].NextPtr = register.Position;
                    registers.Insert(i, register);
                }

            }

            await dictionaryManager.WriteEntity(this);
            foreach (DataRegister reg in registers)
            {
                await dataManager.WriteRegister(reg);
            }

        }

        public async void RemoveRegister(DataRegister register)
        {
            int index = registers.FindIndex(e => e.Position == register.Position);

            if (index == 0)
            {
                dataPointer = registers[0].NextPtr;
                registers.RemoveAt(0);
            }
            else
            {
                registers[index - 1].NextPtr = registers[index].NextPtr;
                registers.RemoveAt(index);
            }
            await dictionaryManager.WriteEntity(this);
            foreach (DataRegister reg in registers)
            {
                await dataManager.WriteRegister(reg);
            }

        }
    }
}