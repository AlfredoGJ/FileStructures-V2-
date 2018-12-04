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

        //private delegate deleteButtonClick void (object sender, RoutedEventArgs eventArgs);
        public event RoutedEventHandler DeleteButtonClick;
        public event RoutedEventHandler EditButtonClick;

        //private Button deleteButton;

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

            ControlContent.ColumnDefinitions.Add(new ColumnDefinition());
            Button deleteButton = new Button();
            SymbolIcon symbol = new SymbolIcon(Symbol.Delete);
            deleteButton.Content = symbol;
            deleteButton.Click += DeleteClick;
            Grid.SetColumn(deleteButton, i + 2);
            ControlContent.Children.Add(deleteButton);

            ControlContent.ColumnDefinitions.Add(new ColumnDefinition());
            Button editButton = new Button();
            SymbolIcon symbolE = new SymbolIcon(Symbol.Edit);
            editButton.Content = symbolE;
            editButton.Click += EditClick;
            Grid.SetColumn(editButton, i + 3);
            ControlContent.Children.Add(editButton);


        }

        private void EditClick(object sender, RoutedEventArgs e)
        {
            EditButtonClick?.Invoke(sender, e);
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            DeleteButtonClick?.Invoke(sender,e);
        }

        //private void deleteButton_Click(object sender, RoutedEventArgs e)
        //{

        //    int i = 0;
        //    //DataRegister regis;
        //}

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                register = (DataContext as DataRegister);

                Fields = register.Fields;
            }

        }

        private void UserControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (DataContext != null)
            {
                register = (DataContext as DataRegister);

                Fields = register.Fields;
            }
        }
    }
}
