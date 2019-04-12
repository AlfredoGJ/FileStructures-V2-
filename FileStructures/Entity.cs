using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
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
        
        public List<Attribute> Attributes { get; set; }
        public List<DataRegister> Registers;
        public string Name { get; set; }
        public Attribute Key { get; set; }
        public Entity(string name)
        {
            Name = name;
            Attributes = new List<Attribute>();
            Registers = new List<DataRegister>();
        }

        public void AddAttribute(Attribute attribute)
        {
            Attributes.Add(attribute);
            if (attribute.KeyType == KeyTypes.Primary)
                Key = attribute;
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (Attributes == null)
                Attributes = new List<Attribute>();

            if (Registers == null)
                Registers = new List<DataRegister>();
        }

        internal void RemoveAttribute(Attribute attribute)
        {
            Attributes.Remove(attribute);
            if (attribute.KeyType == KeyTypes.Primary)
                Key = null;
        }
    }
}