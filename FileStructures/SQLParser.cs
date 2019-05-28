using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileStructures
{
    public class SQLParser
    {
        //Regex select = new Regex(@"SELECT",RegexOptions.IgnoreCase);
        //Regex fields = new Regex(@"((\b\w+\b),)+", RegexOptions.IgnoreCase );
        List<string> comparers = new List<string> { "=", ">","<",">=","<=", "<>"  };
        public SQLParser()
        {
            
        }

        public SQLSelectQuery Tokenize(string query)
        {

            SQLSelectQuery selectQuery = new SQLSelectQuery();

            query = query.Trim();
            var words=query.Split(" ",StringSplitOptions.RemoveEmptyEntries);
            words= words.Reverse().ToArray();
            Stack<string> qWords = new Stack<string>(words);
           
            if (qWords.Pop().ToUpper()== "SELECT")
            {
                // campos
                List<string> fields = new List<string>();
                var field = "";
                field = qWords.Pop();
                fields.Add(field);
                while (field.ToUpper() != "FROM")
                {

                    if (!fields.Contains("*"))
                        fields.Add(field);
                    else
                    {
                        if (field != "*")
                        {
                            fields.Add("Error: Se esperaba FROM");
                            break;
                        }
                    }
                    field = qWords.Pop();
                }
                selectQuery.fields = fields;
                // Agregar validacion de comas o algo


                // Se agrega Tabla
                selectQuery.Table = qWords.Pop();

                // Se checa si tiene condición
                try
                {
                    qWords.Pop();
                    try
                    {
                        // condicion y operandos
                        selectQuery.OpA = qWords.Pop();
                        selectQuery.Comparer = qWords.Pop();
                        selectQuery.OpB = qWords.Pop();
                    }
                    catch
                    {
                        if(selectQuery.OpA==null)
                            selectQuery.Error = "Error: No se econtro un primer operando";
                        if (selectQuery.OpB == null)
                            selectQuery.Error = "Error: No se econtro un segundo operando";
                        if (!comparers.Contains(selectQuery.Comparer))
                            selectQuery.Error = "Error: El operador de comparacion no es valido";

                    }

                }
                // Si no tiene condicion
                catch
                {

                }

            }
            else
            {
                selectQuery.Error="Error: No es una clausula SELECT";
            }

            return selectQuery;
        }




        struct Token
        {
            string val;
            string type;
        }


    
    }

    public class SQLSelectQuery
    {
        public string Error { get; set; }
        public List<string> fields { get; set; }
        public string Table { get; set; }
        public string OpA { get; set; }
        public string OpB { get; set; }
        public string Comparer { get; set; }
    }
}
