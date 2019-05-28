using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace FileStructures.Views
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class SQLPage : Page
    {
        public SQLPage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SQLParser parser = new SQLParser();
            var query=parser.Tokenize(QueryBox.Text);
            int pivotIndex;
            string compareTo;

            if (query.Error == "")
            {
                // Se localiza la entidad 
                if (App.CurrentProject.Entities.Any(x => x.Name == query.Table))
                {
                    var entity = App.CurrentProject.Entities.Find(x => x.Name == query.Table);


                    // Se localizan los indices de los campos
                    List<int> fieldIndexes = new List<int>(); 

                    foreach (string campo in query.fields)
                    {
                        int index = entity.Attributes.FindIndex(attr => attr.Name == campo);
                        if (index != -1)
                            fieldIndexes.Add(index);
                        else
                            return;
                    }

                    // Condicion
                    if (query.Comparer != "")
                    {


                        // Se checa el campo de la condicion a cmparar
                        int index = entity.Attributes.FindIndex(attr => attr.Name == query.OpA);
                        if (index != -1)
                        {
                            pivotIndex = index;
                            compareTo = query.OpB;
                        }
                        else
                        {
                            index = entity.Attributes.FindIndex(attr => attr.Name == query.OpA);
                            if (index != -1)
                            {
                                pivotIndex = index;
                                compareTo = query.OpA;
                            }
                            else
                                return;
                        }


                        // Aqui ya se vacian todos los registros

                        DataTable tabla = new DataTable();

                        foreach (DataRegister register in entity.Registers)
                        {

                            if (register == entity.Registers[0])
                            {
                                foreach (int fieldIndex in fieldIndexes)
                                {
                                    DataColumn col = new DataColumn();
                                    col.ColumnName = entity.Attributes[fieldIndex].Name;
                                    tabla.Columns.Add(col);
                                    col.ReadOnly = true;

                                }
                            }



                            if (EvalCondition(register,pivotIndex,compareTo,query.Comparer))
                            {

                                DataRow r = tabla.NewRow();

                                foreach (int fieldIndex in fieldIndexes)
                                {

                                    r[entity.Attributes[fieldIndex].Name] = register.Fields[fieldIndex].value;

                                }
                                tabla.Rows.Add(r);
                            }

                        }
                        QueryResults.ItemsSource = tabla.DefaultView;





                    }



                }

            }

        }

        private bool EvalCondition(DataRegister register, int pivotIndex, string compareTo,string comparer)
        {

            switch (register.Fields[pivotIndex].dataType)
            {
                case DataTypes.Integer:
                    switch (comparer)
                    {

                        case "=":
                            return (int)(register.Fields[pivotIndex].value) == int.Parse( compareTo);
                           

                        case ">":
                            return (int)(register.Fields[pivotIndex].value) > int.Parse(compareTo);
                            break;

                        case "<":
                            return (int)(register.Fields[pivotIndex].value) < int.Parse(compareTo);
                            break;

                        case ">=":
                            return (int)(register.Fields[pivotIndex].value) >= int.Parse(compareTo);
                            break;

                        case "<=":
                            return (int)(register.Fields[pivotIndex].value) <= int.Parse(compareTo);
                            break;

                        case "<>":
                            return (int)(register.Fields[pivotIndex].value) != int.Parse(compareTo);
                            break;


                    }
                    
                    break;

                case DataTypes.Float:
                    switch (comparer)
                    {

                        case "=":
                            return (float)(register.Fields[pivotIndex].value) == float.Parse(compareTo);

                        case ">":
                            return (float)(register.Fields[pivotIndex].value) > float.Parse(compareTo);
                        case "<":
                            return (float)(register.Fields[pivotIndex].value) < float.Parse(compareTo);

                        case ">=":
                            return (float)(register.Fields[pivotIndex].value) >= float.Parse(compareTo);
                        case "<=":
                            return (float)(register.Fields[pivotIndex].value) <= float.Parse(compareTo); break;

                        case "<>":
                            return (float)(register.Fields[pivotIndex].value) != float.Parse(compareTo); break;


                    }

                    break;

                case DataTypes.String:
                    switch (comparer)
                    {

                        case "=":
                            return register.Fields[pivotIndex].value.ToString() == compareTo;

                        //case ">":
                        //    return register.Fields[pivotIndex].value.ToString() > compareTo;

                        //case "<":
                        //    return register.Fields[pivotIndex].value < compareTo;

                        //case ">=":
                        //    return register.Fields[pivotIndex].value >= compareTo;

                        //case "<=":
                        //    return register.Fields[pivotIndex].value <= compareTo;
                            

                        case "<>":
                            return register.Fields[pivotIndex].value.ToString() != compareTo;


                    }

                    break;

            }
            return false;
           




        }
    }
}
