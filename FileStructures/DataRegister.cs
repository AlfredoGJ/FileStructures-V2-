using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileStructures
{
    /// <summary>
    /// Clase que representa un registro de datos
    /// </summary>
    [Serializable]
    public class DataRegister
    {
        public List<Attribute> Template {get; set;}
        public List<Field> Fields {get; set;}
        public Field Key { get; set; }

        public DataRegister(List<string> values, List<Attribute> template)
        {
            Fields = new List<Field>();
            Template = template;
            for(int i=0;i<values.Count;i++)
            {

                Field f = Utils.StringToField(values[i],template[i]);
                if (Template[i].KeyType == KeyTypes.Primary)
                    this.Key = f;
                Fields.Add(f);
            }

        }
    }

    [Serializable]
    public struct Field
    {
        public object value;
        public DataTypes dataType;


        public static bool operator == (Field left,Field right)
        {
            if (left.dataType != right.dataType)
                return false;

            switch (left.dataType)
            {
                case DataTypes.Boolean:
                    return (bool)left.value == (bool)right.value;
                   

                case DataTypes.Character:
                    return (char)left.value == (char)right.value;
                   

                case DataTypes.Float:
                    return (float)left.value == (float)right.value;
                   

                case DataTypes.Integer:
                    return (int)left.value == (int)right.value;
                  

                case DataTypes.Long:
                    return (long)left.value == (long)right.value;

                case DataTypes.String:
                    return (string)left.value == (string)right.value;
                    
            }
            return false;
        }

        public static bool operator !=(Field left, Field right)
        {
            return !(left == right);
        }


        }
}