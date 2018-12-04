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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            var context = (e.OriginalSource as Control).DataContext;

            if (context.GetType() == new Tuple<int, long>(1, 1).GetType())
            {
                
                int idx = (context as Tuple<int, long>).Item1;
                SecTable.ItemsSource= index.GetEntrySlots(idx);



            }

            if (context.GetType() == new Tuple<string, long>("1", 1).GetType())
            {
                string key = (context as Tuple<string, long>).Item1;
                int idx = App.Alphabet.IndexOf(Char .ToUpper(key[0]));
                SecTable.ItemsSource = index.GetEntrySlots(idx);
            }

        }
    }
}
