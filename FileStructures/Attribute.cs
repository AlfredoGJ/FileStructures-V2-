using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileStructures
{
    /// <summary>
    /// Clase que representa un atributo 
    /// </summary>
    public class Attribute
    {


        // main fields
        private string name;
        private long nextPointer;

        // public properties
        /// <summary>
        /// Nombre del atributo
        /// </summary>
        public string Name { get => name; set => name = value; }
        /// <summary>
        /// Posicion en el archivo
        /// </summary>
        public long Position { get; set; }
        /// <summary>
        /// Tipo del atributo
        /// </summary>
        public char Type { get; set; }
        /// <summary>
        /// Longitud del atributo
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// Tipo de indice del atributo
        /// </summary>
        public int IndexType { get; set; }

        /// <summary>
        /// Apuntador al indice
        /// </summary>
        public long IndexPtr { get; set; }
        /// <summary>
        /// Apuntador al siguiente atributo
        /// </summary>
        public long NextPtr { get => nextPointer; set => nextPointer = value; }
        /// <summary>
        /// Nombre del atributo en un arreglo de caracteres
        /// </summary>
        public char[] ArrayName
        {
            get
            {
                char[] arrName = new char[30];
                arrName.Initialize();
                for (int i = 0; i < name.Length; i++)
                {
                    arrName[i] = name[i];
                }
                return arrName;
            }
            
        }


        public Attribute(string name, char type, int length, int indexType)
        {
            this.name = name;
            Position = -1 ;
            Type = type;
            Length = length;
            IndexType =indexType;
            IndexPtr = -1;
            NextPtr = -1;
        }

        public Attribute(string name, char type, int length, int indexType, long position, long indexptr, long nextptr)
        {
            this.name = name;
            Position = position;
            Type = type;
            Length = length;
            IndexType = indexType;
            IndexPtr = indexptr;
            NextPtr = nextptr;
        }


        /// <summary>
        /// Copia las caracteristicas del atributo a otro atributo
        /// </summary>
        /// <param name="attribute">Atributo al que seran copiadas las caracteristicas</param>
        public void PasteTo(Attribute attribute)
        {
           attribute.name= this.name;
           attribute.Position= Position;
           attribute.Type= Type ;
           attribute.Length= Length ;
           attribute.IndexType= IndexType;
           attribute.IndexPtr= IndexPtr;
           attribute.NextPtr= NextPtr;
        }
    }
}