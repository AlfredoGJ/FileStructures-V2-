using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace FileStructures
{
    public class IndexManager
    {
        private string name;
        private List<Index> indexes;
        public List<Index> Indexes { get =>indexes; }
        long currentLenght;
        StorageFile file;
        private StorageFolder projectFolder;
        List<Attribute> template;



        public IndexManager(string name,List<Attribute> template)
        {
            this.name = name;
            indexes = new List<Index>();
            StorageFolder localFolder = KnownFolders.PicturesLibrary;
            var T = localFolder.GetFolderAsync("Projects");
            do { }
            while (T.Status != Windows.Foundation.AsyncStatus.Completed);
            projectFolder = T.GetResults();
            this.template = template;

           
            
        }

        public async  void initialize()
        {
            bool initialized = await ReadFromFile();
            if (!initialized)
            {
                long ptrAux = 8;
                foreach (Attribute attr in template)
                {
                    Index index = null;
                    switch (attr.IndexType)
                    {
                        case 2:
                            index = new Index(attr.Type, attr.Length, App.PrimaryKeyIndexNumber,ptrAux);
                            ptrAux += index.SizeInBytes;
                            indexes.Add(index);
                            break;
                        case 3:
                            index = new Index(attr.Type, attr.Length, App.SecondaryKeyIndexNumber,ptrAux);
                            ptrAux += index.SizeInBytes;
                            indexes.Add(index);
                            break;
                    }
                }
                foreach (Index idx in indexes)
                {
                    if (idx == indexes.Last())
                        idx.next = -1;
                    else
                        idx.next = indexes[indexes.IndexOf(idx) + 1].pos;
                }

                WriteToFile();
            }
        }

        private async void WriteIndex(BinaryWriter writer, Index index)
        {
            writer.Seek((int)index.pos, SeekOrigin.Begin);
            writer.Write((byte)index.type);
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
                {
                    char[]  S = (values.Item1 as string).ToCharArray();
                    for (int i = 0; i < index.lenght; i++)
                    {
                        if (i < S.Length)
                            writer.Write((byte)S[i]);
                        else
                            writer.Write('\0');
                    }
                        
                }

                writer.Write((long)values.Item2);
            }
        }

        private async Task<Index> ReadIndex(BinaryReader reader, long position)
        {

            reader.BaseStream.Seek(position,SeekOrigin.Begin);
            Index index = new Index((char)reader.ReadByte(), reader.ReadInt32(), reader.ReadInt32(), position);
            index.next = reader.ReadInt64();

            for (int i = 0; i < index.MainTable.Length;i++)
                index.MainTable[i] = reader.ReadInt64();

            for(int i=0;i< index.SecondaryTable.Length;i++)
            {
                if (index.type == 'I')
                {
                    int key = reader.ReadInt32();
                    long pos = reader.ReadInt64();
                    index.SecondaryTable[i] = new Tuple<object, long>(key,pos);
                }

                if (index.type == 'S')
                {
                    char[] S= new char[index.lenght];
                    for (int j = 0; j < index.lenght; j++)
                    {
                        S[j] = (char)reader.ReadByte();
                    }

                    index.SecondaryTable[i] = new Tuple<object, long>(new string(S), reader.ReadInt64());
                }
                    
               
            }
            return index;
        }



        public async void WriteToFile()
        {
            StorageFile file = await projectFolder.CreateFileAsync(name + ".idx", CreationCollisionOption.OpenIfExists);
            using (BinaryWriter writer = new BinaryWriter(await file.OpenStreamForWriteAsync()))
            {
                writer.Seek(0, SeekOrigin.Begin);
                

                if (indexes.Count > 0)
                {
                    writer.Write((long)8);
                    foreach (Index idx in indexes)
                    {
                        WriteIndex(writer, idx);

                    }
                }
                else
                    writer.Write((long)-1);

            }
        }

        public async Task<bool>ReadFromFile()
        {
            bool result = false;
            Indexes.Clear();
            StorageFile file = await projectFolder.CreateFileAsync(name + ".idx", CreationCollisionOption.OpenIfExists);
            using (BinaryReader reader = new BinaryReader(await file.OpenStreamForWriteAsync()))
            {
                if (reader.BaseStream.Length > 0)
                {
                    long header = reader.ReadInt64();
                    while (header != -1)
                    {
                        Index index = await ReadIndex(reader, header);
                        header = index.next;
                        indexes.Add(index);
                    }
                    result = true;
                }
                else
                    result = false;
            }
            return result;
        }

        public int HasSlots(List<int> entries)
        {
            int count = 0;

            for (int i = 0; i < indexes.Count; i++)
                if (indexes[i].HasFreeSlot(entries[i]))
                    count++;

            return count;
        }

        
        public void InsertIndexesOf(DataRegister register, List<int> idxIndexes, List<int> entries, long posInDataFile)
        {
            int freeIndexCount = 0;

            for (int i = 0; i < indexes.Count; i++)
            {
                indexes[i].InsertOnEntry(entries[i],register.Fields[idxIndexes[i]],posInDataFile);
            }

        }

        public void ClearIndexesOf(DataRegister register, List<int> idxIndexes, List<int> entries)
        {
            int freeIndexCount = 0;

            for (int i = 0; i < indexes.Count; i++)
            {
                indexes[i].ClearEntry(entries[i], register.Fields[idxIndexes[i]]);
                
            }

        }

    }
}
