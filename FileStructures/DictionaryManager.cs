using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace FileStructures
{

    /// <summary>
    /// Clase que se encarga de manejar el archivo de diccionario de datos Escritura /Lectura
    /// </summary>
    public class DictionaryManager
    {
        /// <summary>
        /// delegado para actualizar la vista cuando haya cambios en el diccionario
        /// </summary>
        public delegate void ItemsOnFileChanged();
        /// <summary>
        /// Evento que se invoca cuando hay cambios en el diccionario de datos
        /// </summary>
        public event ItemsOnFileChanged itemsOnFileChanged;
        private long headerValue;
        private long fileLength;
        private string name;
        private List<Entity> entities;

        /// <summary>
        /// Nombre del diccionario de datos
        /// </summary>
        public string Name { get => name; set => value = name; }
        /// <summary>
        /// CAbecera del diccionario de datos
        /// </summary>
        public long Header { get => headerValue; }
        /// <summary>
        /// Lista de entidades del diccionario de datos
        /// </summary>
        public List<Entity> Entities { get => entities; }
        /// <summary>
        /// Longitud del archivo de diccionario de datos
        /// </summary>
        public long FileLength { get => fileLength; }

        private StorageFolder projectFolder;


        public  DictionaryManager(string fileName)
        {

            this.name = fileName;
            StorageFolder localFolder = KnownFolders.PicturesLibrary;
            var T= localFolder.GetFolderAsync("Projects");
            do { }
            while (T.Status != Windows.Foundation.AsyncStatus.Completed);
            projectFolder = T.GetResults(); 
            ReadFromFile();
        }

        /// <summary>
        /// Metodo para agregar una entidad al diccionario de datos
        /// </summary>
        /// <param name="entity">Entidad a agregar</param>
        public void AddEntity(Entity entity)
        {

            entity.Position = fileLength;

            int i;
            for (i = 0; i < entities.Count; i++)
            {
                int comparison = string.Compare(entity.Name, entities[i].Name, StringComparison.CurrentCulture);
                if (comparison == -1)
                    break;
            }

            if (i == 0)
            {
                entity.NextPtr = headerValue;
                headerValue = entity.Position;
                entities.Insert(i, entity);
            }
            else
            {
                if (i == entities.Count)
                {
                    entities[i - 1].NextPtr = entity.Position;
                    entities.Add(entity);

                }
                else
                {
                    entity.NextPtr = entities[i - 1].NextPtr;
                    entities[i - 1].NextPtr = entity.Position;
                    entities.Insert(i, entity);
                }

            }
            WriteBack();
        }


        /// <summary>
        /// Metodo que elimina una entidad del diccionario de datos
        /// </summary>
        /// <param name="entity">Entidad a eliminar</param>
        /// <param name="writeBack">Variable que indica si se escribira el archivo despues de la eliminacion</param>
        public void RemoveEntity(Entity entity,bool  writeBack)
        {
            int index = entities.FindIndex(e => e.Name == entity.Name);

            if (index == 0)
            {
                headerValue = entities[0].NextPtr;
                entities.RemoveAt(0);
            }
            else
            {
                entities[index - 1].NextPtr = entities[index].NextPtr;
                entities.RemoveAt(index);
            }
            if(writeBack)
                WriteBack();
        }

        public async void WriteBack()
        {
           
            using (BinaryWriter writer = new BinaryWriter(await projectFolder.OpenStreamForWriteAsync(name, CreationCollisionOption.OpenIfExists)))
            {
                writer.BaseStream.Seek(0, SeekOrigin.Begin);
                writer.Write(headerValue);
                foreach (Entity entity in entities)
                {
                    WriteEntity(entity, writer);
                }

                fileLength = writer.BaseStream.Length;
            }

        }

        /// <summary>
        /// Metodo que lee las entidades y atributos desde el archivo de diccionario de datos
        /// </summary>
        private async void ReadFromFile()
        {
            using (BinaryReader reader = new BinaryReader(await projectFolder.OpenStreamForReadAsync(name)))
            {
                List<Entity> entidades = new List<Entity>();
                
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                headerValue = reader.ReadInt64();
                Int64 ApAux = headerValue;
                while (ApAux != -1)
                {
                    Entity EAux = ReadEntity(reader, ApAux);
                    ApAux = EAux.NextPtr;
                    entidades.Add(EAux);
                   

                }

                fileLength = reader.BaseStream.Length;
                entities = entidades;
                itemsOnFileChanged.Invoke();
                reader.Close();
            }

        }

        /// <summary>
        /// Metodo para escribir una entidad en el diccionario de datos
        /// </summary>
        /// <param name="entity">Entidad a escribir</param>
        /// <returns>Regresa true si se escribio correctamente</returns>
        public async Task<bool> WriteEntity(Entity entity)
        {
            using (BinaryWriter writer = new BinaryWriter(await projectFolder.OpenStreamForWriteAsync(name, CreationCollisionOption.OpenIfExists)))
            {
                writer.Seek((int)entity.Position, SeekOrigin.Begin);
                writer.Write(entity.ArrayName);
                writer.Write(entity.Position);
                writer.Write(entity.AttributesPtr);
                writer.Write(entity.DataPtr);
                writer.Write(entity.NextPtr);
                fileLength = writer.BaseStream.Length;                    
            }
            
            return true;
        }

        private void WriteEntity(Entity entity, BinaryWriter writer)
        {
            writer.Seek((int)entity.Position, SeekOrigin.Begin);
            writer.Write(entity.ArrayName);
            writer.Write(entity.Position);
            writer.Write(entity.AttributesPtr);
            writer.Write(entity.DataPtr);
            writer.Write(entity.NextPtr);
        }

        private Entity ReadEntity(BinaryReader reader, long pos)
        {
            
            reader.BaseStream.Seek(pos, SeekOrigin.Begin);
            string name = new string(reader.ReadChars(30));
            name = name.Trim('\0');
            long position = reader.ReadInt64();
            long atrPtr = reader.ReadInt64();
            long dataPtr = reader.ReadInt64();
            long nextPtr = reader.ReadInt64();


            List<Attribute> attributes = ReadAttributes(atrPtr,reader); 
            return new Entity(name, position, atrPtr, dataPtr, nextPtr,this, attributes);
        }

       

        /// <summary>
        /// Metodo que actualiza una entidad en el diccionario de atos
        /// </summary>
        /// <param name="entity">Entidad a actualizar</param>
        /// <param name="newName">Nuevo nombre de la entidad</param>
        public void  UpdateEntity(Entity entity, string newName)
        {

            
            //int index = entities.FindIndex(x => x.Name == entity.Name);
            RemoveEntity(entity, false);
            entity.Name = newName;
            AddEntity(entity);
            //entities[index].Name = newName;
            //WriteBack();

            itemsOnFileChanged?.Invoke();
                
        }

        /// <summary>
        /// Metodo para escribir un atributo en el diccionario de datos
        /// </summary>
        /// <param name="attribute">Atributo a escribir</param>
        /// <returns>Regresa true si se escribio correctamente</returns>
        public async Task<bool> WriteAttribute( Attribute attribute)
        {
            using (BinaryWriter writer = new BinaryWriter(await projectFolder.OpenStreamForWriteAsync(name,CreationCollisionOption.OpenIfExists)))
            {
                writer.BaseStream.Seek(attribute.Position, SeekOrigin.Begin);
                writer.Write(attribute.ArrayName);
                writer.Write(attribute.Position);
                writer.Write(attribute.Type);
                writer.Write(attribute.Length);
                writer.Write(attribute.IndexType);
                writer.Write(attribute.IndexPtr);
                writer.Write(attribute.NextPtr);
                fileLength = writer.BaseStream.Length;

            }
            return true;
        }

        /// <summary>
        /// Lee los atributos  desde el archivo de diccionario de datos
        /// </summary>
        /// <param name="pos"> Posicion de comienzo de los atributos</param>
        /// <param name="reader">Lector binario</param>
        /// <returns>Regresa la lista de atributos leidos</returns>
        public  List<Attribute> ReadAttributes(long pos, BinaryReader reader)
        {     
                long ptr = pos;
                List<Attribute> attributes = new List<Attribute>();
                while (ptr != -1)
                {
                    Attribute attrAux = ReadAttribute(reader, ptr);
                    ptr = attrAux.NextPtr;
                    attributes.Add(attrAux);
                }    
                return attributes;       
        }

        private Attribute ReadAttribute(BinaryReader reader, long pos)
        {
            reader.BaseStream.Seek(pos, SeekOrigin.Begin);
            string name = new string(reader.ReadChars(30));
            name = name.Trim('\0');
            long position = reader.ReadInt64();
            char type = reader.ReadChar();
            int length = reader.ReadInt32();
            int indexType = reader.ReadInt32();
            long indexPtr = reader.ReadInt64();
            long nextPtr = reader.ReadInt64();
            Attribute attribute = new Attribute(name, type, length, indexType,position, indexPtr, nextPtr);
            return attribute;
        }



    }
}