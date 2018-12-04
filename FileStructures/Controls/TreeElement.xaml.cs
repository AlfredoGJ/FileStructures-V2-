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
    public sealed partial class TreeElement : UserControl
    {
        public int X { get; set;}
        public int Y { get; set;}

        public string Address
        {
            get { return AddressValue.Text; }
            set { AddressValue.Text = value; }
            
        }

        public string Key
        {
            get { return KeyValue.Text; }
            set { KeyValue.Text = value; }
            
        }

        public TreeElement()
        {
            this.InitializeComponent();

        }

        public TreeElement(int x, int y, string value, string address)
        {
            this.InitializeComponent();
            this.X = x;
            this.Y = y;

            Key = value;
            Address = address;
            this.DataContext = this;


        }

        public TreeElement(string value, string address)
        {
            this.InitializeComponent();
            Key = value;
            Address = address;
            this.DataContext = this;


        }

    }
}
