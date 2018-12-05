using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStructures
{
    public class TreeManager
    {
        private string name;
        private List<Attribute> attributes;
        private Tree tree;
        /// <summary>
        /// Arbol manejado por la clase
        /// </summary>
        public Tree Tree{get =>  tree ;set => tree= value;}
        private bool initialized;

        public TreeManager(string name)
        {
            initialized = false;
            this.name = name;
            tree = new Tree(2);
        }

        /// <summary>
        /// Función que inicializa el manager
        /// </summary>
        /// <param name="registers">Lista de registros con que se debe inicializar</param>
        public void Initialize(List<DataRegister> registers)
        {
            if (!initialized)
            {
                foreach (DataRegister register in registers)
                    this.tree.Insert(register.Key,register.Position);

                initialized = true;
            }
        }

        /// <summary>
        /// Agrega una clave al arbol
        /// </summary>
        /// <param name="key">Clave que se agregará al arbol</param>
        /// <param name="address">Direccion en el archivo de datos de la clave a insertar</param>
        public void AddKey(object key, long address)
        {
            tree.Insert(key, address);
        }


    }
}
