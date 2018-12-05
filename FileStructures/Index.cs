using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStructures
{

    /// <summary>
    /// Clase que representa un indice
    /// </summary>
    public class Index
    {
        /// <summary>
        /// Posicion del indice en el archivo
        /// </summary>
        public long pos;
        /// <summary>
        /// Tipo de indice S o I, Entero o Cadena
        /// </summary>
        public char type;
        /// <summary>
        /// Longitud de el campo de el indice
        /// </summary>
        public int lenght;
        /// <summary>
        /// Numero de localidades que tendra el indice para cada entrada
        /// </summary>
        public int slotsNumber;
        /// <summary>
        /// Numero de enyradas en la tabla principal
        /// </summary>
        public int mainTableEntries;
        /// <summary>
        /// Apuntador al siguiente indice en el archivo de indices
        /// </summary>
        public long next;
        /// <summary>
        /// Tabla principal de indices
        /// </summary>
        public long[] MainTable;
        /// <summary>
        /// Tabla secundaria de Indices
        /// </summary>
        public Tuple<object, long>[] SecondaryTable;
        // Index Size=8+1+4+4= 17
        private int mainTableStart = 17;
        /// <summary>
        /// Posicion donde comienza el area de datos del indice
        /// </summary>
        public long dataAreaStart;

        /// <summary>
        /// Tamaño total que ocupa el indice en el archivo de indices
        /// </summary>
        public int SizeInBytes
        {
            get
            {
                return mainTableStart + (MainTable.Count() * 8) + (lenght + 8) * slotsNumber * mainTableEntries;
            }
        }

        public Index()
        { }

        public Index(char type, int lenght, int slots, long pos)
        {
            this.pos = pos;
            this.type = type;
            this.lenght = lenght;
            slotsNumber = slots;

            if (type == 'I')
                mainTableEntries = 10;
            if (type == 'S')
                mainTableEntries = 26;

            MainTable = new long[mainTableEntries];

            for (int i = 0; i < mainTableEntries; i++)
                MainTable[i] = -1;

            SecondaryTable = new Tuple<object, long>[slots * mainTableEntries];

            for (int i = 0; i < slots * mainTableEntries; i++)
            {
                if(type=='I')
                    SecondaryTable[i] = new Tuple<object, long>(-1, -1);
                if (type == 'S')
                    SecondaryTable[i] = new Tuple<object, long>(new string('\0',lenght), -1);
            }
                

            dataAreaStart = pos + MainTable.Count() * 8;
           
           
        }
        /// <summary>
        /// Checa si el indice de la entrada tiene localidades libres
        /// </summary>
        /// <param name="tableEntry">Entrada en la tabla principal a checar</param>
        /// <returns> Regresa true si hay lugar, false  si no</returns>
        public bool HasFreeSlot(int tableEntry)
        {
            
            if (tableEntry < mainTableEntries)
            {
                return SecondaryTable.Skip(tableEntry * slotsNumber).Take(slotsNumber).Any(x => x.Item2 == -1);
            }
            return false;
        }

        /// <summary>
        /// Inserta un valor para para la entrada de la tabla principal indicada
        /// </summary>
        /// <param name="tableEntry">  Entrada de la tabla principal</param>
        /// <param name="value">Valor a insertar</param>
        /// <param name="pointer"> Posicion en el archivo de datos del registro asociado al valor proporcionado</param>
        public void InsertOnEntry(int tableEntry, object value, long pointer)
        {
            if (tableEntry < mainTableEntries)
            {
                if (MainTable[tableEntry] != -1)
                {
                    //MainTable[tableEntry] = dataAreaStart + tableEntry * mainTableEntries;

                    for (int i = tableEntry * slotsNumber; i < (tableEntry + 1) * slotsNumber ; i++)
                    {
                        if (SecondaryTable[i].Item2 == -1)
                        {
                            SecondaryTable[i] = new Tuple<object, long>(value, pointer);
                            break;
                        }
                    }
                }
                else
                {
                    MainTable[tableEntry] = dataAreaStart + tableEntry * slotsNumber;
                    SecondaryTable[tableEntry*slotsNumber] = new Tuple<object, long>(value, pointer);

                }
                
            }
           
        }


        /// <summary>
        /// Libera el espacio ocupado por un valor en el indice
        /// </summary>
        /// <param name="tableEntry">Entradade la tabla principal donde se encuantra el valor</param>
        /// <param name="value"> Valor a eliminar</param>
        public void ClearEntry(int tableEntry, object value)
        {
            if (tableEntry < mainTableEntries)
            {
                if (MainTable[tableEntry] != -1)
                {
                    //MainTable[tableEntry] = dataAreaStart + tableEntry * mainTableEntries;

                    for (int i = tableEntry * slotsNumber; i < (tableEntry + 1) * slotsNumber; i++)
                    {
                        if (SecondaryTable[i].Item1 == value)
                        {
                            SecondaryTable[i] = new Tuple<object, long>(-1, -1);
                            break;
                        }
                    }
                }
                

            }
        }

        /// <summary>
        /// Regresa los todos los valores almacenados en la entrada indicada
        /// </summary>
        /// <param name="tableEntry">Entrada </param>
        /// <returns>Regresa una lista de tuplas <valor, posicion>  que representan a cada valor</returns>
        public List<Tuple<object, long>> GetEntrySlots(int tableEntry)
        {
            List<Tuple<object, long>> result = new List<Tuple<object, long>>();

            if (tableEntry < mainTableEntries)
            {
                for (int i = tableEntry*slotsNumber; i < (tableEntry * slotsNumber) + slotsNumber; i++)
                    result.Add(SecondaryTable[i]);

            }
            return result;
        }

        
    }
}
