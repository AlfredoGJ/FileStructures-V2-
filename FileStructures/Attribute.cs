using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileStructures
{
    /// <summary>
    /// Clase que representa un atributo 
    /// </summary>
    /// 


    public enum DataTypes {Integer,String,Character,Float,Boolean,Long}
    public enum KeyTypes {NoKey,Primary,Foreign }
    [Serializable]
    public class Attribute
    {
        
        public KeyTypes KeyType { get; set; }
        public DataTypes DataType { get; set; }
        public string Name { get; set; }
        public string AsociatedEntity { get; set; }

        public string KeyTypeString
        {
            get
            {
                switch (KeyType)
                {
                    case KeyTypes.Foreign:
                        return "Foreign Key";
                        break;

                    case KeyTypes.NoKey:
                        return "No Key";
                        break;

                    case KeyTypes.Primary:
                        return "Primary Key";
                        break;

                    default:
                        return "Error";
                        break;
                }
            }
        }

        public string DataTypeString
        {
            get
            {
                switch (DataType)
                {
                    case DataTypes.Boolean:
                        return "Boolean";
                        break;
                    default:
                        return "Error";
                        break;
                }
            }
        }

        public Attribute()
        {

        }

        public Attribute(Attribute attribute)
        {
            this.KeyType = attribute.KeyType;
            this.DataType = attribute.DataType;
            this.Name = attribute.Name;
            this.AsociatedEntity = attribute.AsociatedEntity;

        }

    }


}