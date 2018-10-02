﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileStructures
{
    public class DataRegister
    {

        private object key;

        private byte[] block;
        public long Position { get; set; }
        public long NextPtr { get; set; }
        public object Key { get => key; }

        private List<object> fields;
        private List<Attribute> template;

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
            //this.template = template;
            Position = -1;
            NextPtr = -1;
            CreateBlock(values,template);

        }

        public DataRegister(byte[] block, long pos, long nextPtr, List<Attribute> template)
        {
            this.block = block;
            this.Position = pos;
            this.NextPtr = nextPtr;
            UnPack(block,template);



        }

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
                    this.key = value;

                offset += template[i].Length;
            }
        }

        private void CreateBlock(List<string> values, List<Attribute> template)
        {
            if (values.Count == template.Count)
            {
                block = new byte[template.Sum(x => x.Length)];
                fields = new List<object>();
                object value = null;
                byte[] valueBytes = new byte[8];  // 8 is could be any value, is just for initializing the variable 
                int offset = 0;

                //BitConverter.GetBytes(Position).CopyTo(block, 0);
                //BitConverter.GetBytes(NextPtr).CopyTo(block,8);

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
                        key = value;

                    fields.Add(value);

                    for (int b = 0; b < valueBytes.Length; b++)
                        block[offset + b] = valueBytes[b];
                    offset += valueBytes.Length;
                }

            }
            else
                throw new Exception("Los Parametros proporcionados no son correctos"); 

        }




            //public List<object> Fields
            //{
            //    get => default(List<object>);
            //    set
            //    {
            //    }
            //}

            //public List<char> DataTemplate
            //{
            //    get => default(List<char>);
            //    set
            //    {
            //    }
            //}
        }
}