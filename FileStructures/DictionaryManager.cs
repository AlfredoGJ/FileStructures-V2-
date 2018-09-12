using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Windows.Storage;

namespace FileStructures
{
    public class DictionaryManager 
    {
        public delegate void  ItemsOnFileChanged ();
        public event ItemsOnFileChanged itemsOnFileChanged;
        private long headerValue;
        private long fileLength;
        private string name;
        private List<Entity> entities;
        
        public string Name { get =>  name; }
        public List<Entity> Entities { get => entities;}
       

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
                    if ( comparison== -1)
                        break;
                }

                if (i == entities.Count)
                {
                    if (i == 0)
                    {
                        entity.NextPtr = headerValue;
                        headerValue = entity.Position;
                        entities.Insert(i, entity);
                    }
                    else
                    {
                        entities[i - 1].NextPtr = entity.Position;
                        entities.Add(entity);

                    }           
                }
                else 
                {
                    entity.NextPtr = entities[i-1].NextPtr;
                    entities[i-1].NextPtr = entity.Position;
                    entities.Insert(i,entity);
                }
               

            WriteBack();
                
        }
        
        public void RemoveEntity()
        {
            throw new System.NotImplementedException();
        }

        public async void WriteBack()
        {
            StorageFolder localFolder = KnownFolders.PicturesLibrary;
            StorageFolder projectsFolder = await localFolder.GetFolderAsync("Projects");
            using (BinaryWriter writer = new BinaryWriter(await projectsFolder.OpenStreamForWriteAsync(name, CreationCollisionOption.OpenIfExists)))
            {
                writer.Seek(0,SeekOrigin.Begin);
                writer.Write(headerValue);
                foreach (Entity entity in entities)
                {
                    WriteEntity(entity,writer.BaseStream.Position,writer);
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
                headerValue= reader.ReadInt64();
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

        private void WriteEntity(Entity entity, long pos, BinaryWriter writer)
        {
            
            writer.Seek((int)pos, SeekOrigin.Begin);

            writer.Write(entity.ArrayName);
            writer.Write(entity.Position);
            writer.Write(entity.AttributesPtr);
            writer.Write(entity.DataPtr);
            writer.Write(entity.NextPtr);
        }

        private Entity  ReadEntity(BinaryReader reader, long pos)
        {
            reader.BaseStream.Seek(pos,SeekOrigin.Begin);
            string name = new string(reader.ReadChars(30));
            long position = reader.ReadInt64();
            long atrPtr = reader.ReadInt64();
            long dataPtr = reader.ReadInt64();
            long nextPtr = reader.ReadInt64();
            //FindAtributos(Stream, E); TODO find the Entity atributes on the file
            return  new Entity(name, position,atrPtr,dataPtr,nextPtr);
        }

        private void WriteAttribute()
        {
            throw new System.NotImplementedException();
        }

        private void ReadAttribute()
        {
            throw new System.NotImplementedException();
        }
    }
}