using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStructures
{

    /// <summary>
    /// Clase que representa una entidad 
    /// </summary>
    [Serializable]
    public class Entity 
    {
        List<Attribute> Attributes;
        List<DataRegister> Registers;
        public string Name { get;  set; }
        public Entity(string name)
        {
            Name = name;
        }
       
    }
}