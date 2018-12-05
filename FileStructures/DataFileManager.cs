using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace FileStructures
{
    /// <summary>
    ///Clase que maneja el archivo de datos
    /// </summary>
    public class DataFileManager
    {
        private FileStream stream;
        private string filePath;
        private string name;
        private long fileLength;
        /// <summary>
        /// Longitud del archivo de datos
        /// </summary>
        public long FileLength { get => fileLength;  }

        private StorageFolder projectFolder;
        List<Attribute> template;

        /// <summary>
        /// Lista de regitros en el archivo de datos
        /// </summary>
        public List<DataRegister> registers;
        /// <summary>
        /// Delegado para la actualizacion de los datos
        /// </summary>
        public delegate void ItemsOnFileChanged();
        /// <summary>
        /// Evento  invocado cada que se actualizan los datos en el archivo
        /// </summary>
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


        /// <summary>
        /// Metodo que escribe un registro de datos en el archivo
        /// </summary>
        /// <param name="register">Registro a escribir</param>
        /// <returns>Regresa true si se escribio el registro correctamente</returns>
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

            //itemsOnFileChanged.Invoke();
            return true;
        }

        /// <summary>
        /// Lee todos los registros del archivo de datos
        /// </summary>
        /// <param name="pos">Posicion donde se comenzará a leer</param>
        public async void  ReadAllRegisters(long pos)
        {
            //Open files or create them if don't exist 
            StorageFile file = await projectFolder.CreateFileAsync(name + ".dat", CreationCollisionOption.OpenIfExists);
           // StorageFile fileIdx = await projectFolder.CreateFileAsync(name + ".idx", CreationCollisionOption.OpenIfExists);


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

       
    }
}