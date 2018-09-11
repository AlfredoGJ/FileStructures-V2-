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

            if (entities.Count != 0)
            {
                int i;
                for (i = 0; i < entities.Count; i++)
                {
                    int comparison = string.Compare(entity.Name, entities[i].Name, StringComparison.CurrentCulture);
                    if ( comparison== 1 || i+1==entities.Count)
                        break;

                }

                if (i == 0)
                {
                    entity.Position = fileLength;
                    entity.NextPtr = headerValue;
                    headerValue = entity.Position;
                    entities.Insert(i, entity);
                }
                else
                {
                    entity.Position = fileLength;
                    entity.NextPtr = entities[i].NextPtr;
                    entities[i].NextPtr = entity.Position;
                    entities.Insert(i, entity);
                }
                

            }
            else
            {
                entity.Position = fileLength;
                headerValue = fileLength;
                entities.Add(entity);
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
                Int64 ApAux = reader.ReadInt64();
                while (ApAux != -1)
                {
                    Entity EAux = ReadEntity(reader, ApAux);            
                    ApAux = EAux.NextPtr;

                }

                fileLength = reader.BaseStream.Length;
                entities = entidades;
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

            string name = reader.ReadChars(30).ToString() ;
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