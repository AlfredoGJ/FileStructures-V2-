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


    public enum DataTypes {Integer,String}
    [Serializable]
    public class Attribute
    {
        DataTypes Type;
        string Name;


    }
}