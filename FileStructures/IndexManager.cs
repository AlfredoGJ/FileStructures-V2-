using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace FileStructures
{
    class IndexManager
    {
        private string name;
        private List<Index> indexes;
        public List<Index> Indexes { get =>indexes; }
        long currentLenght;
        StorageFile file;
        private StorageFolder projectFolder;

        public IndexManager(string name,List<Attribute> template, bool Initialize)
        {
            this.name = name;
            indexes = new List<Index>();
            StorageFolder localFolder = KnownFolders.PicturesLibrary;
            var T = localFolder.GetFolderAsync("Projects");
            do { }
            while (T.Status != Windows.Foundation.AsyncStatus.Completed);
            projectFolder = T.GetResults();
            

            if (Initialize)
            {
                long ptrAux = 8;
                foreach (Attribute attr in template)
                {
                    Index index = null;
                    switch (attr.IndexType)
                    {
                        case 2:
                             index = new Index(attr.Type,attr.Length, App.PrimaryKeyIndexNumber);                               
                            break;

                        case 3:
                            index = new Index(attr.Type, attr.Length, App.SecondaryKeyIndexNumber);
                            break;

                    }

                    index.pos = ptrAux;
                    if (attr == template.Last())
                        index.next = -1;
                    else
                        index.next = index.SizeInBytes + ptrAux;

                    ptrAux = index.next;
                    indexes.Add(index);
                }
                WriteToFile();
            }
            
            
        }

        private async void WriteIndex(Index index)
        {
            StorageFile file = await projectFolder.CreateFileAsync(name + ".idx", CreationCollisionOption.OpenIfExists);
            using (BinaryWriter writer = new BinaryWriter(await file.OpenStreamForWriteAsync()))
            {
                writer.Seek((int)index.pos, SeekOrigin.Begin);
                writer.Write(index.type);
                writer.Write(index.lenght);
                writer.Write(index.slotsNumber);
                writer.Write(index.next);
                foreach (long ptr in index.MainTable)
                    writer.Write(ptr);

                foreach (Tuple<object, long> values in index.SecondaryTable)
                {
                    if(index.type=='I')
                        writer.Write((int)values.Item1);
                    if (index.type == 'S')
                        writer.Write((values.Item1 as string).ToCharArray());

                    writer.Write(values.Item2);
                }
            }
        }

        private async Task<Index> ReadIndex(BinaryReader reader, long position)
        {

            reader.BaseStream.Seek(position,SeekOrigin.Begin);
            Index index = new Index(reader.ReadChar(), reader.ReadInt32(), reader.ReadInt32());
            index.next = reader.ReadInt64();

            for (int i = 0; i < index.MainTable.Length;i++)
                index.MainTable[i] = reader.ReadInt64();

            for(int i=0;i< index.SecondaryTable.Length;i++)
            {
                if (index.type == 'I')
                    index.SecondaryTable[i] = new Tuple<object, long>(reader.ReadInt32(),reader.ReadInt64());
                    
                if (index.type == 'S')
                    index.SecondaryTable[i] = new Tuple<object, long>(new string(reader.ReadChars(index.lenght)), reader.ReadInt64());
               
            }
            return index;
        }



        private async void WriteToFile()
        {
            StorageFile file = await projectFolder.CreateFileAsync(name + ".idx", CreationCollisionOption.OpenIfExists);
            using (BinaryWriter writer = new BinaryWriter(await file.OpenStreamForWriteAsync()))
            {
                writer.Seek(0, SeekOrigin.Begin);
                writer.Write((long)8);

                foreach (Index idx in indexes)
                {
                    WriteIndex(idx);

                }
            }
        }

        public async void ReadFromFile()
        {
            Indexes.Clear();
            StorageFile file = await projectFolder.CreateFileAsync(name + ".idx", CreationCollisionOption.OpenIfExists);
            using (BinaryReader reader = new BinaryReader(await file.OpenStreamForWriteAsync()))
            {
                long header=reader.ReadInt64();
                while (header != -1)
                {
                    Index index =  await ReadIndex(reader,header);
                    header = index.next;
                    indexes.Add(index);
                }
            }
        }

        public int HasSlots(List<int> entries)
        {
            int count = 0;

            for (int i = 0; i < indexes.Count; i++)
                if (indexes[i].HasFreeSlot(entries[i]))
                    count++;

            return count;
        }

        // Falta el pointer  de la direccion en el archivo de datos;
        public void InsertIndexesOf(DataRegister register, List<int> idxIndexes, List<int> entries)
        {
            int freeIndexCount = 0;

            for (int i = 0; i < indexes.Count; i++)
            {
                indexes[i].InsertOnEntry(entries[i],register.Fields[idxIndexes[i]],);
            }

        }
    }
}
