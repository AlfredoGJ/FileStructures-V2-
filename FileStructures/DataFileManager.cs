using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace FileStructures
{
    public class DataFileManager
    {
        private FileStream stream;
        private string filePath;
        private string name;
        private long fileLength;
        public long FileLength { get => fileLength;  }

        private StorageFolder projectFolder;
        List<Attribute> template;

        public List<DataRegister> registers;
        public delegate void ItemsOnFileChanged();
        public event ItemsOnFileChanged itemsOnFileChanged;

        public DataFileManager(string name, List<Attribute> template)
        {
            this.template = template;
            this.name = name;
            StorageFolder localFolder = KnownFolders.PicturesLibrary;
            var T = localFolder.GetFolderAsync("Projects");
            do { }
            while (T.Status != Windows.Foundation.AsyncStatus.Completed);
            projectFolder = T.GetResults();
            //ReadAllRegisters(-1);


        }



        private DataRegister ReadRegister(BinaryReader reader, long pos)
        {
            reader.BaseStream.Seek(pos,SeekOrigin.Begin);

            byte[] block = new byte[template.Sum(x => x.Length)];
            long position = reader.ReadInt64();
            reader.Read(block,0,block.Length);
            long nextPtr = reader.ReadInt64();

            return new DataRegister(block, position, nextPtr, template);
        }

        public async Task<bool>  WriteRegister(DataRegister register)
        {
            StorageFile file = await projectFolder.CreateFileAsync(name+".dat", CreationCollisionOption.OpenIfExists);
            using (BinaryWriter writer = new BinaryWriter(await file.OpenStreamForWriteAsync()))
            {
                writer.BaseStream.Seek(register.Position,SeekOrigin.Begin);
                writer.Write(register.Position);
                writer.Write(register.Block);
                writer.Write(register.NextPtr);
                fileLength = writer.BaseStream.Length;
            }
            itemsOnFileChanged.Invoke();
            return true;
        }

        public async void  ReadAllRegisters(long pos)
        {

            StorageFile file = await projectFolder.CreateFileAsync(name + ".dat", CreationCollisionOption.OpenIfExists);
            using (BinaryReader reader = new BinaryReader(await projectFolder.OpenStreamForReadAsync(name+".dat")))
            {
                List<DataRegister> registers = new List<DataRegister>();

                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                Int64 ApAux = pos;
                while (ApAux != -1)
                {
                    DataRegister drAux = ReadRegister(reader, ApAux);
                    ApAux = drAux.NextPtr;
                    registers.Add(drAux);


                }

                fileLength = reader.BaseStream.Length;

                this.registers = registers;
                itemsOnFileChanged.Invoke();
                //return registers;
               
            }


        }

        public void WriteRegisters()
        {
            throw new System.NotImplementedException();
        }
    }
}