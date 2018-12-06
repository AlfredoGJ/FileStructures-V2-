using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStructures
{

    /// <summary>
    /// Clase que representa una entidad 
    /// </summary>
    public class Entity : INotifyPropertyChanged
    {

        /// <summary>
        /// Delegado creado para actualizar los cambios en la entidad
        /// </summary>
        public delegate void ItemsOnFileChanged();
        /// <summary>
        /// Evento que se invoca cada que hay cambios en la entidad
        /// </summary>
        public event ItemsOnFileChanged itemsOnFileChanged;
        // Private fields
        private DataFileManager dataManager;
        private DictionaryManager dictionaryManager;
        public IndexManager indexManager;
        /// <summary>
        /// Manejador de el arbol binario
        /// </summary>
        public TreeManager treeManager;
        private List<Attribute> attributes;
        private Attribute primaryKey;
        private List<DataRegister> registers;
        


        // main fields
        private string name;
        private long position;
        private long attributePointer;
        private long dataPointer;
        private long nextPointer;

        /// <summary>
        /// Evento qe se invoca cuando una propiedad cambia en la entidad
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;



        // public properties
        /// <summary>
        /// Nombre de la entidad
        /// </summary>
        public string Name { get => name; set => name = value; }
        /// <summary>
        /// Posicion en el dicccionario de datos de la entidad
        /// </summary>
        public long Position { get => position; set => position = value; }
        /// <summary>
        /// Apuntador a los atributos de la entidad
        /// </summary>
        public long AttributesPtr { get => attributePointer; }
        /// <summary>
        /// Apuntador a datos de la entidad
        /// </summary>
        public long DataPtr { get => dataPointer; }
        /// <summary>
        /// Apuntador a la siguiente entidad
        /// </summary>
        public long NextPtr { get => nextPointer; set => nextPointer = value; }
        /// <summary>
        /// Lista de atributos de la entidad
        /// </summary>
        public List<Attribute> Attributes { get => attributes; }
        /// <summary>
        /// Clave primaria de la entidad
        /// </summary>
        public Attribute PrimaryKey { get => primaryKey; }
        /// <summary>
        /// Lista de registros de datos de la entidad
        /// </summary>
        public List<DataRegister> Registers { get => registers; }


        /// <summary>
        /// Nombre de la entidad representado en un arreglo de caracteres
        /// </summary>
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


        public Entity(string name, long position, long atrPtr, long dataPtr, long nextPtr, DictionaryManager dictionaryManager, List<Attribute> attributes)
        {
            this.name = name;
            this.position = position;
            this.attributePointer = atrPtr;
            this.dataPointer = dataPtr;
            this.nextPointer = nextPtr;
            this.dictionaryManager = dictionaryManager;
            this.attributes = attributes;
            this.primaryKey = attributes.Find(X => X.IndexType == 2);
            this.dataManager = new DataFileManager(Name, attributes);
 

            dataManager.itemsOnFileChanged += UpdateChangesInView;
            dataManager.ReadAllRegisters(dataPtr);


            if (App.CurrentFileOrganization == FileOrganization.Indexed)
            {
                this.indexManager = new IndexManager(Name, attributes);
                indexManager.initialize();
            }

            if (App.CurrentFileOrganization == FileOrganization.Tree)
            {

                this.treeManager = new TreeManager(Name);
            }

            //UpdateChangesInView();
        }

        
        private void UpdateChangesInView()
        {
            this.registers = dataManager.registers;
            itemsOnFileChanged?.Invoke();


            if (App.CurrentFileOrganization == FileOrganization.Tree)
                this.treeManager.Initialize(dataManager.registers);

        }


        public Entity(string name, DictionaryManager manager)
        {
            this.name = name;
            position = -1;
            attributePointer = -1;
            dataPointer = -1;
            nextPointer = -1;
            this.dictionaryManager = manager;
            attributes = new List<Attribute>();

        }


        /// <summary>
        /// Agrega un atributo a la entidad
        /// </summary>
        /// <param name="attribute">Atributo que se agregará</param>
        public async void AddAttribute(Attribute attribute)
        {
            attribute.Position = dictionaryManager.FileLength;

            int i = attributes.Count;
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

        /// <summary>
        /// Agrega un atributo que ya existe en la entidad, usado para cuando se edita un atributo
        /// </summary>
        /// <param name="attribute"></param>
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
        /// <summary>
        /// Elimina un atributo de la entidad
        /// </summary>
        /// <param name="attribute">Atributo a eliminar</param>
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

        /// <summary>
        /// Actualiza un atributo en la entidad
        /// </summary>
        /// <param name="attribute">Atributo a actualizar</param>
        /// <returns>true si se actualizo correctamente</returns>
        public async Task<bool> UpdateAttribute(Attribute attribute)
        {
            Attribute findResult = attributes.Find(x => x.Name == attribute.Name);

            // There is no attribute named like the new one, it is rewrited in the same position with different name
            // Must be reordered acccording to its name
            if (findResult == null)
            {
                attribute.PasteTo(attributes.Find(x => x.Position == attribute.Position));
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



        /// <summary>
        /// Actualiza un registr de datos en la entidad
        /// </summary>
        /// <param name="register">Registro a actualizar</param>
        /// <returns>True si se actualizo correctamente</returns>
        public async Task<bool> UpdateDataRegister(DataRegister register)
        {
            bool insertResult= false;

            if (register.Key != null)
            {
                DataRegister findResult = registers.Find(x => x.Key.ToString() == register.Key.ToString());

                // There is no attribute named like the new one, it is rewrited in the same position with different Key
                // Must be reordered acccording to its Key
                if (findResult == null)
                {
                    DataRegister oldRegister = registers.Find(x => x.Position == register.Position);
                    RemoveRegister(oldRegister, false);
                    AddRegister(register, true,true);

                    //register.PasteTo(registers.Find(x => x.Position == register.Position));


                    //await dataManager.WriteRegister(register);
                    insertResult = true;
                }
                // Already exist an attribute named like this
                else
                {
                    // The attribute with the same name as this is actually the same attribute
                    if (findResult.Position == register.Position)
                    {
                        await dataManager.WriteRegister(register);
                        UpdateChangesInView();
                        insertResult = true;
                    }

                    // else: The attribute with the same name is another attribute, no operation is done and an error msg shows to the user


                }


            }
            else
            {
                //Insertion of edited reisters must be unordered
            }
            
            return insertResult;

        }



    
        /// <summary>
        /// Agrega un registro a la entidad
        /// </summary>
        /// <param name="register">Registro a agregar</param>
        /// <param name="writeBack">variable que especifica si se escribira el archvo de datos despues de la insercion</param>
        /// <param name="existent">variable que indica si el registro existe actualmente en la entidad, usado para la edicion de registros</param>
        public void AddRegister(DataRegister register, bool writeBack, bool existent)
        {
            switch (App.CurrentFileOrganization)
            {
                case FileOrganization.Ordered:
                    if (register.Key != null)
                    {
                        InsertDataOrdered(register, writeBack, existent);
                    }
                    else
                    {
                      InsertDataUnordered(register,writeBack,existent);
                    }
                    break;

                case FileOrganization.Indexed:
                    AddRegisterIndexed(register);
                  
                    break;

                case FileOrganization.Tree:
                     AddRegisterTree(register);

                    break;

            }
            
        }


        private async void AddRegisterTree( DataRegister register)
        {
            InsertDataUnordered(register,true, false);
            treeManager.AddKey(register.Key, register.Position) ;
        }

        private async void AddRegisterIndexed(DataRegister register)
        {
            var fields = register.Fields;
            List<int> entries = new List<int>();
            List<int> eIndexes = new List<int>();

            for (int i = 0; i < fields.Count(); i++)
            {
                Attribute E = Attributes[i];

                // If its an index
                if (E.IndexType == 2 || E.IndexType == 3)
                {
                    int entry = -1;

                    if (E.Type == 'S')
                        entry = App.Alphabet.IndexOf((char.ToUpper((fields[i] as string)[0])));

                    if (E.Type == 'I')
                        entry = App.GetIntFirstDigit((int)fields[i]);

                    entries.Add(entry);
                    eIndexes.Add(i);
                }
            }
            
            int numFreeSlots = indexManager.HasSlots(entries);

            // All entries can be inserted 
            if (numFreeSlots == entries.Count())
            {
                InsertDataUnordered(register,true,false);
                indexManager.InsertIndexesOf(register, eIndexes, entries, register.Position);
                indexManager.WriteToFile();
            }
            // Just some entries can be insertesd 
            else if (numFreeSlots > 0)
            {
            }
            // No entry can be inserted
            else
            {
            }
            

        }

        private async void InsertDataOrdered(DataRegister register, bool writeBack, bool existent )
        {
            if(!existent)
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
                    register.NextPtr = -1;
                    registers.Add(register);

                }
                else
                {
                    register.NextPtr = registers[i - 1].NextPtr;
                    registers[i - 1].NextPtr = register.Position;
                    registers.Insert(i, register);
                }

            }
            if (writeBack)
            {
                await dictionaryManager.WriteEntity(this);
                foreach (DataRegister reg in registers)
                {
                    await dataManager.WriteRegister(reg);
                }
                UpdateChangesInView();
            }
            

        }

        private async Task<long> InsertDataUnordered(DataRegister register, bool writeBack, bool existent)
        {
            if (!existent)
                register.Position = dataManager.FileLength;

            int i = registers.Count;

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
                    register.NextPtr = -1;
                    registers.Add(register);

                }
                else
                {
                    register.NextPtr = registers[i - 1].NextPtr;
                    registers[i - 1].NextPtr = register.Position;
                    registers.Insert(i, register);
                }

            }
            if (writeBack)
            {
                await dictionaryManager.WriteEntity(this);
                foreach (DataRegister reg in registers)
                {
                    await dataManager.WriteRegister(reg);
                }
                UpdateChangesInView();
            }

            return register.Position; 
        }


        /// <summary>
        /// Elimina un registro de la entidad en modo indedxado
        /// </summary>
        /// <param name="register">Registro a eliminar</param>
        public async void RemoveRegisterIndexed(DataRegister register)
        {
            RemoveRegister(register, true);

            var fields = register.Fields;
            List<int> entries = new List<int>();
            List<int> eIndexes = new List<int>();

            for (int i = 0; i < fields.Count(); i++)
            {
                Attribute E = Attributes[i];

                // If its an index
                if (E.IndexType == 2 || E.IndexType == 3)
                {
                    int entry = -1;

                    if (E.Type == 'S')
                        entry = App.Alphabet.IndexOf((char.ToUpper((fields[i] as string)[0])));

                    if (E.Type == 'I')
                        entry = App.GetIntFirstDigit((int)fields[i]);

                    entries.Add(entry);
                    eIndexes.Add(i);
                }
            }


           
            indexManager.ClearIndexesOf(register, eIndexes, entries);
            indexManager.WriteToFile();
            



        }
        /// <summary>
        /// Elimina un registro de la entidad
        /// </summary>
        /// <param name="register">Registro a eliminar</param>
        /// <param name="writeBack">Variable que indica se se escribira el archivo de datos despues de la eliminacion</param>
        public async void RemoveRegister(DataRegister register, bool writeBack)
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

            if (writeBack)
            {
                await dictionaryManager.WriteEntity(this);
                foreach (DataRegister reg in registers)
                {
                    await dataManager.WriteRegister(reg);
                }
                UpdateChangesInView();
            }
            

        }
    }
}