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
    /// Clase que se encarga de manejar los indices y su escritura lectura en el archivo de indices
    /// </summary>
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
        /// <summary>
        /// Metodo para inicializar el manejador de indices
        /// </summary>
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
                            index = new Index(attr.Type, attr.Length, App.PrimaryKeyIndexNumber,ptrAux,false);
                            ptrAux += index.SizeInBytes;
                            indexes.Add(index);
                            break;
                        case 3:
                            index = new Index(attr.Type, attr.Length, App.SecondaryKeyIndexNumber,ptrAux,true);
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

        /// <summary>
        /// Escribe un indice en el archivo de datos
        /// </summary>
        /// <param name="writer">Instanci BinaryWriter que se encarga de escribir en el archivo</param>
        /// <param name="index"> Indice a escribir en el archivo</param>
        private async void WriteIndex(BinaryWriter writer, Index index)
        {
            writer.Seek((int)index.pos, SeekOrigin.Begin);
            writer.Write((byte)index.type);
            writer.Write(index.lenght);
            writer.Write(index.slotsNumber);
            writer.Write(index.IT);
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

        /// <summary>
        /// Metodo para leer un indice desde el archivo de indices
        /// </summary>
        /// <param name="reader">Instancia BinaryReader que se encarga de leer el archivo </param>
        /// <param name="position"> Posicion en el archivo donde se encuentra el indice</param>
        /// <returns></returns>
        private async Task<Index> ReadIndex(BinaryReader reader, long position)
        {

            reader.BaseStream.Seek(position,SeekOrigin.Begin);
            Index index = new Index((char)reader.ReadByte(), reader.ReadInt32(), reader.ReadInt32(), position,reader.ReadBoolean());
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


        /// <summary>
        /// Metodo que se encarga de escribir los indices en el archivo de indices
        /// </summary>
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

        /// <summary>
        /// Metodo que se encarga de leer los indices desde el archivo de indices
        /// </summary>
        /// <returns>true si se leyeron correctamente los indices, de lo contrario regresa false </returns>
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
        /// <summary>
        /// Funcion que checa si hay espacio disponible en cierto indice
        /// </summary>
        /// <param name="entries"> lista de  cada entrada a checar</param>
        /// <returns></returns>
        public int HasSlots(List<int> entries)
        {
            int count = 0;

            for (int i = 0; i < indexes.Count; i++)
                if (indexes[i].HasFreeSlot(entries[i]))
                    count++;

            return count;
        }

        /// <summary>
        /// Inserta los indices de un registro en cada indice correspondiente
        /// </summary>
        /// <param name="register"> Registro a insertar</param>
        /// <param name="idxIndexes"> Lista de ID´s a usar para la insercion</param>
        /// <param name="entries">lissta de entradas a ingresar de cada indice</param>
        /// <param name="posInDataFile"> Posicion del indice en el archivo de datos</param>
        public void InsertIndexesOf(DataRegister register, List<int> idxIndexes, List<int> entries, long posInDataFile)
        {
            int freeIndexCount = 0;

            for (int i = 0; i < indexes.Count; i++)
            {
                indexes[i].InsertOnEntry(entries[i],register.Fields[idxIndexes[i]],posInDataFile);
            }

        }
        /// <summary>
        /// Remueve los indices ocupados por el registro
        /// </summary>
        /// <param name="register">Registro del cual se eliminarán los indices</param>
        /// <param name="idxIndexes">Indices de cada registro a usar para la eliminacion</param>
        /// <param name="entries">Entradas en cada registro a eliminar</param>
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
