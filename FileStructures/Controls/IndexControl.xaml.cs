using System;
using System.Collections.Generic;
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

// La plantilla de elemento Control de usuario está documentada en https://go.microsoft.com/fwlink/?LinkId=234236

namespace FileStructures.Controls
{
    public sealed partial class IndexControl : UserControl
    {
        Index index;
        public List<Tuple<object, long>> SecondaryTable { get; set; }
        public IndexControl()
        {
            this.InitializeComponent();
        }

        public IndexControl(Index index)
        {
            this.InitializeComponent();
            //Title.Text = index.pos.ToString();
            SecondaryTable = new List<Tuple<object, long>>();
            this.DataContext = SecondaryTable;
            this.index = index;
            Type.Text += index.type;
            Position.Text += index.pos.ToString();
            Size.Text += index.SizeInBytes.ToString();

            //Indice tipo primario
            if (!index.IT)
            {
                Title.Text = "Primary Index";
                if (index.type == 'I')
                {
                    List<Tuple<int, long>> mainTable = new List<Tuple<int, long>>();
                    for (int i = 0; i < index.MainTable.Count(); i++)
                        mainTable.Add(new Tuple<int, long>(i, index.MainTable[i]));

                    IndexDetail.ItemsSource = mainTable;
                }

                if (index.type == 'S')
                {
                    List<Tuple<string, long>> mainTable = new List<Tuple<string, long>>();
                    for (int i = 0; i < index.MainTable.Count(); i++)
                        mainTable.Add(new Tuple<string, long>(App.Alphabet[i].ToString(), index.MainTable[i]));

                    IndexDetail.ItemsSource = mainTable;
                }
            }
            // Indice tipo secundario
            else
            {
                Title.Text = "Secondary Index";

                
                if (index.type == 'I')
                {
                    List<Tuple<int, string>> mainTable = new List<Tuple<int, string>>();

                    for (int i = 0; i < index.SecondaryTable.Count(); i++)
                    {
                        if (!mainTable.Any(x => x.Item1 == (int)index.SecondaryTable[i].Item1))
                        {
                            if(index.SecondaryTable[i].Item2!=-1)
                                mainTable.Add(new Tuple<int, string>((int)index.SecondaryTable[i].Item1, ""));
                        }
                    }
                    IndexDetail.ItemsSource = mainTable;
                }

                if (index.type == 'S')
                {
                    List<Tuple<string, string>> mainTable = new List<Tuple<string, string>>();

                    for (int i = 0; i < index.SecondaryTable.Count(); i++)
                    {
                        if (!mainTable.Any(x => x.Item1 == (string)index.SecondaryTable[i].Item1))
                        {
                            if (index.SecondaryTable[i].Item2 != -1)
                                mainTable.Add(new Tuple<string, string>((string)index.SecondaryTable[i].Item1, ""));
                        }
                            
                    }

                    IndexDetail.ItemsSource = mainTable;
                }
            }
            

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            var context = (e.OriginalSource as Control).DataContext;

            // When the index is primary
            if (!index.IT)
            {
                if (context.GetType() == new Tuple<int, long>(1, 1).GetType())
                {

                    int idx = (context as Tuple<int, long>).Item1;
                    SecTable.ItemsSource = index.GetEntrySlots(idx);



                }

                if (context.GetType() == new Tuple<string, long>("1", 1).GetType())
                {
                    string key = (context as Tuple<string, long>).Item1;
                    int idx = App.Alphabet.IndexOf(Char.ToUpper(key[0]));
                    SecTable.ItemsSource = index.GetEntrySlots(idx);
                }
            }
            else
            {
                if (context.GetType() == new Tuple<int, string>(1, "1").GetType())
                {

                    int idx = (context as Tuple<int, string>).Item1;
                    SecTable.ItemsSource = index.SecondaryTable.ToList().FindAll(x=> (int)x.Item1==idx);



                }

                if (context.GetType() == new Tuple<string, string>("1", "1").GetType())
                {
                    string key = (context as Tuple<string, string>).Item1;
                    SecTable.ItemsSource = index.SecondaryTable.ToList().FindAll(x => (string)x.Item1 == key);
                    //int idx = App.Alphabet.IndexOf(Char.ToUpper(key[0]));
                    //SecTable.ItemsSource = index.GetEntrySlots(idx);
                }
            }
            

        }
    }
}
