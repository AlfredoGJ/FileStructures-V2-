using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileStructures
{
    /// <summary>
    /// Clase que representa un registro de datos
    /// </summary>
    public class DataRegister
    {

        /// <summary>
        /// Clave primaria del registro
        /// </summary>
        private object key;
        /// <summary>
        /// Bloque binario que representa los datos del registro
        /// </summary>
        private byte[] block;
        /// <summary>
        /// Posicion del registro en el archivo
        /// </summary>
        public long Position { get; set; }
        /// <summary>
        /// Apuntador al siguiente registro
        /// </summary>
        public long NextPtr { get; set; }
        /// <summary>
        /// Clave primaria del registro
        /// </summary>
        public object Key { get => key; }
        private Attribute keyAttribute;

        private List<object> fields;
        private List<Attribute> template;

        /// <summary>
        /// Lista de campos del registro
        /// </summary>
        public List<object> Fields { get =>fields; }

        public byte[] Block
        {
            get
            {
                //CreateBlock(fields.Cast<string>().ToList(),template);
                return block;
            }
        }


        public DataRegister(List<string> values, List<Attribute> template)
        {
            this.template = template;
            Position = -1;
            NextPtr = -1;
            CreateBlock(values,template);

        }

        public DataRegister(byte[] block, long pos, long nextPtr, List<Attribute> template)
        {
            this.template = template;
            this.block = block;
            this.Position = pos;
            this.NextPtr = nextPtr;
            UnPack(block,template);



        }
        /// <summary>
        /// Desempaqueta un bloque de datos binario y crea a partie de el un registro
        /// </summary>
        /// <param name="block"> Bloque de datos a desempaquetar</param>
        /// <param name="template">Lista de atributos que describen el registro </param>
        private void UnPack(byte[] block, List<Attribute> template)
        {
            int offset = 0;
            fields = new List<object>();
            object value = null;
            byte[] valueBytes = new byte[8];  // 8 is could be any value, is just for initializing the variable 

            //Position= BitConverter.ToInt64(block,0);
            //NextPtr = BitConverter.ToInt64(block,8);

            for (int i=0; i < template.Count;i++)
            {
                switch (template[i].Type)
                {
                    case 'I':

                        //value = Convert.ToInt32(values[i]);
                        //valueBytes = new byte[template[i].Length];
                        //for (int s = 0; s < valueBytes.Length; s++)
                        //{
                        //    valueBytes[s] = block[offset + s];
                        //}

                        value = BitConverter.ToInt32(block,offset);
                        fields.Add(value);
                        break;

                    case 'S':

                        string sValue="";
                        valueBytes = new byte[template[i].Length];

                        for (int j = 0; j < valueBytes.Length; j++)
                        {
                            valueBytes[j] = block[offset + j];
                            sValue += (char)valueBytes[j];
                        }
                             


                        fields.Add(sValue);

                        //fields.Add(BitConverter.ToString(valueBytes,10));
                        break;
                }
                if (template[i].IndexType == 2)
                {
                    this.key = value;
                    this.keyAttribute = template[i];
                }
                    

                offset += template[i].Length;
            }
        }
        /// <summary>
        /// Crea un bloque binario de datos a partir de una lista de cadenas
        /// </summary>
        /// <param name="values"> Lista de cadenas que contiene los datos del registro</param>
        /// <param name="template"> Lista de atributos que descru¿iben el registro</param>
        private void CreateBlock(List<string> values, List<Attribute> template)
        {
            if (values.Count == template.Count)
            {
                block = new byte[template.Sum(x => x.Length)];
                fields = new List<object>();
                object value = null;
                byte[] valueBytes = new byte[8];  // 8 is could be any value, is just for initializing the variable 
                int offset = 0;

                for (int i = 0; i < values.Count; i++)
                {
                    //TextBox tb = Container.Children[i] as TextBox;

                    switch (template[i].Type)
                    {
                        case 'I':

                            value = Convert.ToInt32(values[i]);
                            valueBytes = BitConverter.GetBytes((Int32)value);
                            break;

                        case 'S':
                            //value= tb.Text;
                            string str = values[i];
                            valueBytes = new byte[template[i].Length];
                            value = str;

                            for (int j = 0; j < str.Length; j++)
                                valueBytes[j] = BitConverter.GetBytes(str[j])[0];
                            break;
                    }

                    if (template[i].IndexType == 2)
                    {
                        key = value;
                        keyAttribute = template[i];
                    }
                        

                    fields.Add(value);

                    for (int b = 0; b < valueBytes.Length; b++)
                        block[offset + b] = valueBytes[b];
                    offset += valueBytes.Length;
                }

            }
            else
                throw new Exception("Los Parametros proporcionados no son correctos"); 

        }


        /// <summary>
        /// Copia los datos de un registro a otro
        /// </summary>
        /// <param name="register"> Registro donde se vaciarán los datos del registro actual</param>
        public void PasteTo(DataRegister register)
        {
            register.Position = Position;
            register.key =  App.CloneObjPrimitive(key, keyAttribute.Type);
            register.block = (byte[])block.Clone();
            register.template = template;
            register.NextPtr = NextPtr;
            register.fields = new List<object>();

            foreach (object o in fields)
                register.fields.Add(App.CloneObjPrimitive(o, template[fields.IndexOf(o)].Type));

        }
    }
}