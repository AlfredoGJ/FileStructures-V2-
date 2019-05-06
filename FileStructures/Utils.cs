using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStructures
{
    class Utils
    {

        public static Field StringToField(string strValue, Attribute attribute)
        {

            object value = null;
            switch (attribute.DataType)
            {

                case DataTypes.Integer:
                    value = int.Parse(strValue);
                    break;

                case DataTypes.String:
                    value = strValue;
                    break;

                case DataTypes.Boolean:
                    value = bool.Parse(strValue);
                    break;

                case DataTypes.Character:
                    value = char.Parse(strValue);
                    break;

                case DataTypes.Float:
                    value = float.Parse(strValue);
                    break;

                case DataTypes.Long:
                    value = long.Parse(strValue);
                    break;




            }

            Field f;
            f.value = value;
            f.dataType = attribute.DataType;

            return f;
        }

    }
}
