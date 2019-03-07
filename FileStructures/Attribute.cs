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
        public DataTypes Type { get; set; }
        public string Name { get; set; }


    }
}