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
    public sealed partial class DataRegisterViewControl : UserControl
    {

        private List<object> fields;
        private DataRegister register;
        public List<object> Fields
        {
            get
            {
                return register.Fields;
            }

            set
            {
                fields = value;
                Fill();
            }
        }

        public DataRegisterViewControl()
        {
            this.InitializeComponent();
        }

        private void Fill()
        {

            ControlContent.ColumnDefinitions.Clear();
            ControlContent.Children.Clear();
            int i;
            for (i = 0; i < fields.Count; i++)
            {
                ControlContent.ColumnDefinitions.Add(new ColumnDefinition());
                TextBlock tb = new TextBlock();
                tb.Text = fields[i].ToString();
                tb.FontSize = 18;
                //tb.HorizontalAlignment = HorizontalAlignment.Stretch;
                Grid.SetColumn(tb, i);
                ControlContent.Children.Add(tb);
            }

            ControlContent.ColumnDefinitions.Add(new ColumnDefinition());
            TextBlock pos = new TextBlock();
            pos.Text = register.Position.ToString();
            pos.FontSize = 18;
            Grid.SetColumn(pos, i);
            ControlContent.Children.Add(pos);

            ControlContent.ColumnDefinitions.Add(new ColumnDefinition());
            TextBlock next = new TextBlock();
            next.Text = register.NextPtr.ToString();
            next.FontSize = 18;
            Grid.SetColumn(next, i+1);
            ControlContent.Children.Add(next);


        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            register = (DataContext as DataRegister);

            Fields = register.Fields;
            

        }
    }
}
