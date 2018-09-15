using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace FileStructures
{
    public class DictionaryManager
    {
        public delegate void ItemsOnFileChanged();
        public event ItemsOnFileChanged itemsOnFileChanged;
        private long headerValue;
        private long fileLength;
        private string name;
        private List<Entity> entities;

        public string Name { get => name; set => value = name; }
        public long Header { get => headerValue; }
        public List<Entity> Entities { get => entities; }
        public long FileLength { get => fileLength; } 


        public DictionaryManager(string fileName)
        {

            this.name = fileName;
            ReadFromFile();
        }

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


        public void RemoveEntity(Entity entity)
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
            WriteBack();
        }

        public async void WriteBack()
        {
            StorageFolder localFolder = KnownFolders.PicturesLibrary;
            StorageFolder projectsFolder = await localFolder.GetFolderAsync("Projects");
            using (BinaryWriter writer = new BinaryWriter(await projectsFolder.OpenStreamForWriteAsync(name, CreationCollisionOption.OpenIfExists)))
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

        private async void ReadFromFile()
        {
            StorageFolder localFolder = KnownFolders.PicturesLibrary;
            StorageFolder projectsFolder = await localFolder.GetFolderAsync("Projects");

            using (BinaryReader reader = new BinaryReader(await projectsFolder.OpenStreamForReadAsync(name)))
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
            }

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


            //FindAtributos(Stream, E); TODO find the Entity atributes on the file
            return new Entity(name, position, atrPtr, dataPtr, nextPtr,this);
        }

       

        public void  UpdateEntity(Entity entity, string newName)
        {

            int index = entities.FindIndex(x => x.Name == entity.Name);
            entities[index].Name = newName;
            WriteBack();
                
        }

        private void WriteAttribute(BinaryWriter writer, Attribute attribute)
        {
            writer.BaseStream.Seek(attribute.Position, SeekOrigin.Begin);
            writer.Write(attribute.ArrayName);
            writer.Write(attribute.Position);
            writer.Write(attribute.Type);
            writer.Write(attribute.Length);
            writer.Write(attribute.IndexType);
            writer.Write(attribute.IndexPtr);
            writer.Write(attribute.NextPtr);
        }

        public async Task <List<Attribute> >ReadAttributes(long pos)
        {

            StorageFolder localFolder = KnownFolders.PicturesLibrary;
            StorageFolder projectsFolder = await localFolder.GetFolderAsync("Projects");

            using (BinaryReader reader = new BinaryReader(await projectsFolder.OpenStreamForReadAsync(name)))
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
            Attribute attribute = new Attribute(name, position, type, length, indexType, indexPtr, nextPtr);
            return attribute;


        }

    }
}