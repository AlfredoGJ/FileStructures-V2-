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
        public List<Index> Indexes { get; }
        long currentLenght;
        StorageFile file;
        private StorageFolder projectFolder;

        public IndexManager(List<Attribute> template)
        {
            indexes = new List<Index>();
            StorageFolder localFolder = KnownFolders.PicturesLibrary;
            var T = localFolder.GetFolderAsync("Projects");
            do { }
            while (T.Status != Windows.Foundation.AsyncStatus.Completed);
            projectFolder = T.GetResults();

            currentLenght = 8;

            foreach (Attribute attr in template)
            {
                switch (attr.IndexType)
                {
                    case 2:
                        if (attr.Type == 'S')
                        {
                            Index index = new Index(currentLenght, 'C', attr.Length, App.PrimaryKeyIndexNumber);
                            currentLenght += (26 * 8) + (App.PrimaryKeyIndexNumber * (attr.Length+8));
                            if (attr != template.Last())
                                index.next = currentLenght;
                            indexes.Add(index);
                        }

                        if (attr.Type == 'I')
                        {
                            Index index = new Index(currentLenght, 'I', 4, App.PrimaryKeyIndexNumber);
                            if (attr != template.Last())
                                index.next = currentLenght;
                            currentLenght += (10 * 8) + (App.PrimaryKeyIndexNumber * 12);
                            indexes.Add(index);
                        }
                        break;

                    case 3:

                        if (attr.Type == 'S')
                        {
                            Index index = new Index(currentLenght, 'C', attr.Length, App.SecondaryKeyIndexNumber);
                            currentLenght += (26 * 8) + (App.PrimaryKeyIndexNumber * (attr.Length + 8));
                            if (attr != template.Last())
                                index.next = currentLenght;
                            indexes.Add(index);
                        }
                        if (attr.Type == 'I')
                        {
                            Index index = new Index(currentLenght, 'I', 4, App.SecondaryKeyIndexNumber);
                            currentLenght += (10 * 8) + (App.SecondaryKeyIndexNumber * 12);
                            if(attr!= template.Last())
                                index.next = currentLenght;
                            indexes.Add(index);
                        }
                            
                        break;

                }
            }
        }

        public IndexManager(string name)
        {
            this.name = name;

        }

        private async Task WriteCharIndexAsync(Index index)
        {

            StorageFile file = await projectFolder.CreateFileAsync(name + ".idx", CreationCollisionOption.OpenIfExists);
            using (BinaryWriter writer = new BinaryWriter(await file.OpenStreamForWriteAsync()))
            {
                writer.Seek((int)index.pos, SeekOrigin.Begin);
                writer.Write('S');
                writer.Write(index.lenght);
                writer.Write(index.slots);
                // Reservamos espacio para cada apuntador de letras
                for (int letters = 0; letters < 26; letters++)
                    writer.Write((long)(-1));
                // reservamos espacio para los apuntadores al archivo
                for (int s = 0; s < index.slots; s++)
                {
                    writer.Write(new string('\0', index.lenght));
                    writer.Write((long)(-1));
                }

            }

        }

        private async Task WriteIntIndexAsync( Index index)
        {

            StorageFile file = await projectFolder.CreateFileAsync(name + ".idx", CreationCollisionOption.OpenIfExists);
            using (BinaryWriter writer = new BinaryWriter(await file.OpenStreamForWriteAsync()))
            {
                writer.Seek((int)index.pos, SeekOrigin.Begin);
                writer.Write('I');
                writer.Write(index.slots);

                for (int num = 0; num < 10; num++)
                    writer.Write((long)-1);
                
                for (int num = 0; num < 10; num++)
                {
                    for (int i = 0; i < index.slots; i++)
                    {
                        writer.Write((int)(-1));
                        writer.Write((long)(-1));
                    }
                }

                

            }

        }
        private async void WriteToFile()
        {
            StorageFile file = await projectFolder.CreateFileAsync(name + ".idx", CreationCollisionOption.OpenIfExists);
            using (BinaryWriter writer = new BinaryWriter(await file.OpenStreamForWriteAsync()))
            {
                writer.Seek(0,SeekOrigin.Begin);
                writer.Write((long)8);
            }

                foreach (Index idx in indexes)
            {
                if (idx.type == 'S')
                    await WriteCharIndexAsync(idx);
                if (idx.type == 'I')
                    await WriteIntIndexAsync(idx);
            }
        }

        private async void ReadFromFile()
        {
            Indexes.Clear();
            StorageFile file = await projectFolder.CreateFileAsync(name + ".idx", CreationCollisionOption.OpenIfExists);
            using (BinaryReader reader = new BinaryReader(await file.OpenStreamForWriteAsync()))
            {
                long header=reader.ReadInt64();
                while (header != -1)
                {
                    Index index =  await ReadIndex(header);
                    header = index.next;
                    indexes.Add(index);

                }
                
            }


        }

        private async Task<Index> ReadIndex(long pos)
        {
            Index idx = null;
            StorageFile file = await projectFolder.CreateFileAsync(name + ".idx", CreationCollisionOption.OpenIfExists);
            using (BinaryReader reader = new BinaryReader(await file.OpenStreamForWriteAsync()))
            {
                reader.BaseStream.Seek(pos, SeekOrigin.Begin);
                char type = reader.ReadChar();
                switch (type)
                {
                    case 'I':
                        idx = new Index(pos,type,4,reader.ReadInt32());
                        break;
                    case 'S':
                        idx = new Index(pos, type, reader.ReadInt32(), reader.ReadInt32());
                        break;
                }

            }
            return idx;
        }

        public long GetAddr(string value, int indexNumber)
        {
            long ptr = -1;

            return ptr;
        }

        public async Task<long> GetAddress(int value, int indexNumber)
        {
            long ptr = -1;
            Index index = indexes[indexNumber];
            int placeInTable = App.GetIntFirstDigit(value);
            long tableStart = index.pos + 25;

            StorageFile file = await projectFolder.CreateFileAsync(name + ".idx", CreationCollisionOption.OpenIfExists);
            using (BinaryReader reader = new BinaryReader(await file.OpenStreamForWriteAsync()))
            {
                reader.BaseStream.Seek(tableStart + (placeInTable * 8), SeekOrigin.Begin);
                long ptr2 = reader.ReadInt64();
                if (ptr2 != -1)
                {
                    reader.BaseStream.Seek(ptr2,SeekOrigin.Begin);

                    for (int i = 0; i < index.slots; i++)
                    {
                        int intVal=reader.ReadInt32();
                        reader.BaseStream.Seek(index.lenght, SeekOrigin.Current);

                        if (intVal == value)
                            ptr = reader.ReadInt64();
                    }
                }
            }
                return ptr;
        }

        public async Task<bool> InsertValue(int value,long address , int indexNumber)
        {
            bool result = false;
            int placeInTable = App.GetIntFirstDigit(value);
            Index index = indexes[indexNumber];
            long slotIndex= await HasCapacity(index, placeInTable);
            long tableStart = index.pos + 25;
            long tableEnd = tableStart + (8 * index.slots);
            int slotSize = 12;
            long slotsStart = tableEnd + placeInTable * (slotSize * index.slots);
            if (slotIndex != 0)
            {
                StorageFile file = await projectFolder.CreateFileAsync(name + ".idx", CreationCollisionOption.OpenIfExists);
                using (BinaryWriter writer = new BinaryWriter(await file.OpenStreamForWriteAsync()))
                {
                    if (slotIndex == -1)
                    {
                        // Se escribe en la tabla el apuntador a los slots de la entrada de esa tabla
                        writer.Seek((int)((placeInTable*8) + tableStart), SeekOrigin.Begin);
                        writer.Write(slotsStart);
                    }
                    else
                    {
                        // se escriben los datos en el slot correspondiente
                        writer.Seek((int)(slotsStart + slotsStart * slotSize), SeekOrigin.Begin);
                        writer.Write(value);
                        writer.Write(address);
                    }

                }
            }
            else
                result = false;

            return result;
        }

        private async Task<long> HasCapacity(Index index, int place  )
        {
            long result = 0; ;
            long ptr = index.pos+ 25 + place;   // Index.pos+ 25 = Where the index data starts
            StorageFile file = await projectFolder.CreateFileAsync(name + ".idx", CreationCollisionOption.OpenIfExists);

            using (BinaryReader reader = new BinaryReader(await file.OpenStreamForWriteAsync()))
            {
                reader.BaseStream.Seek(ptr, SeekOrigin.Begin);
                long ptr2=reader.ReadInt64();
                if (ptr2 != -1)
                {
                    reader.BaseStream.Seek(ptr2,SeekOrigin.Begin);

                    for (int i = 0; i < index.slots; i++)
                    {

                        reader.BaseStream.Seek(index.lenght,SeekOrigin.Current);
                        long longVal = reader.ReadInt64();

                        if (longVal == -1)
                        {
                            result = i;
                            break;
                        }

                    }
                }
                else
                    result = ptr2;
                
                    

                

            }
            return result;
        }

        public bool InsertValue(string value, int indexNumber)
        {
            bool result = false;

            return result;
        }

    }
}
