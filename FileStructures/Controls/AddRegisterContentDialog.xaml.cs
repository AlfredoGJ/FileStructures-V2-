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

// La plantilla de elemento del cuadro de diálogo de contenido está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace FileStructures.Controls
{
    public sealed partial class AddRegisterContentDialog : ContentDialog
    {
        Entity entity;
        DataRegister register;
        bool RefIntegrity = true;
        public AddRegisterContentDialog( Entity entity)
        {
            this.InitializeComponent();
            this.entity = entity;

            CreateFields(null);


        }

        private void OnlyText(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            string tAux = "";
            string text = (sender as TextBox).Text;
            foreach (char c in text)
            {
                if (Char.IsLetter(c))
                    tAux += c;

            }
             (sender as TextBox).Text = tAux;
            (sender as TextBox).SelectionStart = tAux.Length;
            (sender as TextBox).SelectionLength = 0;

        }

        private void NumbersOnly(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            string tAux = "";
            string text = (sender as TextBox).Text;
            foreach (char c in text)
            {
                if (Char.IsDigit(c))
                    tAux+=c;

            }
             (sender as TextBox).Text = tAux;
            (sender as TextBox).SelectionStart = tAux.Length;
            (sender as TextBox).SelectionLength = 0;
        }

        public AddRegisterContentDialog(Entity entity, DataRegister register)
        {
            this.InitializeComponent();
            this.entity = entity;
            this.register = register;
            CreateFields(register);

        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            List<string> values = ConvertToStrings();

            if (RefIntegrity && values != null)
            {

                DataRegister register = new DataRegister(values, entity.Attributes);
                // If we are adding the register
                if (this.register == null)
                {
                    if (!entity.Registers.Any(x => x.Key == register.Key))
                        entity.AddRegister(register);
                    else
                    {
                        Warning.Margin = new Thickness(20, 10, 10, 0);
                        Warning.Text = "Error: This entity already contains a register with the key " + register.Key.value.ToString();
                        args.Cancel = true;
                    }
                }


                // If we are Editing an existent Register
                else
                {

                    if (this.register.Key == register.Key)
                        this.register.Fields = register.Fields;
                    else
                    {
                        if (entity.Registers.All(x => x.Key != register.Key))
                        {
                            this.register.Fields = register.Fields;
                            this.register.Key = register.Key;
                        }
                        else
                        {
                            Warning.Margin = new Thickness(20, 10, 10, 0);
                            Warning.Text = "Error: This entity already contains a register with the key " + register.Key.value.ToString();
                            args.Cancel = true;
                        }
                    }

                }
            }
            else
                args.Cancel = true;

        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }


        private void CreateFields(DataRegister r)
        {

            for (int i = 0; i < this.entity.Attributes.Count; i++)
            {

                TextBox tb = new TextBox();
                CheckBox cb = new CheckBox();
                
                switch (entity.Attributes[i].DataType)
                {


                    case DataTypes.Boolean:
                        cb.Name= entity.Attributes[i].Name;
                        cb.DataContext = entity.Attributes[i];
                        if (r != null)
                            cb.IsChecked = Boolean.Parse( r.Fields[i].value.ToString());
                        Container.Children.Add(cb);
                        break;


                    case DataTypes.Character:
                        
                        tb.Name = entity.Attributes[i].Name;
                        tb.DataContext = entity.Attributes[i];
                        tb.PlaceholderText = entity.Attributes[i].Name;
                        tb.Margin = new Thickness(20, 0, 0, 0);
                        tb.MaxLength = 1;
                        if (r != null)
                            tb.Text = r.Fields[i].value.ToString();
                        if (entity.Attributes[i].KeyType == KeyTypes.Foreign)
                            tb.LostFocus += ValidateReferentialIntegrity;
                        Container.Children.Add(tb);

                        
                        break;

                    case DataTypes.Float:
                        tb = new TextBox();
                        tb.Name = entity.Attributes[i].Name;
                        tb.DataContext = entity.Attributes[i];
                        tb.PlaceholderText = entity.Attributes[i].Name;
                        tb.Margin = new Thickness(20, 0, 0, 0);
                        if (r != null)
                            tb.Text = r.Fields[i].value.ToString();
                        if (entity.Attributes[i].KeyType == KeyTypes.Foreign)
                            tb.LostFocus += ValidateReferentialIntegrity;
                        Container.Children.Add(tb);


                        break;

                    case DataTypes.Integer:
                        tb = new TextBox();
                        tb.Name = entity.Attributes[i].Name;
                        tb.DataContext = entity.Attributes[i];
                        tb.PlaceholderText = entity.Attributes[i].Name;
                        tb.Margin = new Thickness(20, 0, 0, 0);
                        tb.Width = 120;
                        tb.MaxLength = 8;
                        tb.TextChanging += NumbersOnly;
                        if (r != null)
                            tb.Text = r.Fields[i].value.ToString();
                        if (entity.Attributes[i].KeyType == KeyTypes.Foreign)
                            tb.LostFocus += ValidateReferentialIntegrity;
                        Container.Children.Add(tb);
                        break;


                    case DataTypes.Long:
                        tb = new TextBox();
                        tb.Name = entity.Attributes[i].Name;
                        tb.DataContext = entity.Attributes[i];
                        tb.PlaceholderText = entity.Attributes[i].Name;
                        tb.Margin = new Thickness(20, 0, 0, 0);
                        tb.TextChanging += NumbersOnly;
                        if (r != null)
                            tb.Text = r.Fields[i].value.ToString();
                        if (entity.Attributes[i].KeyType == KeyTypes.Foreign)
                            tb.LostFocus += ValidateReferentialIntegrity;
                        Container.Children.Add(tb);
                        break;

                    case DataTypes.String:

                        tb = new TextBox();
                        tb.Name = entity.Attributes[i].Name;
                        tb.DataContext = entity.Attributes[i];
                        tb.PlaceholderText = entity.Attributes[i].Name;
                        tb.Margin = new Thickness(20, 0, 0, 0);
                        tb.Width = 120;
                        tb.MaxLength = 120;
                        tb.TextChanging += OnlyText;
                        if (r != null)
                            tb.Text = r.Fields[i].value.ToString();
                        if (entity.Attributes[i].KeyType == KeyTypes.Foreign)
                            tb.LostFocus += ValidateReferentialIntegrity;
                        Container.Children.Add(tb);
                        break;

                }

               
            }
        }

        private void ValidateReferentialIntegrity(object sender, RoutedEventArgs e)
        {
            if (sender.GetType() == typeof(TextBox))
            {
                TextBox textbox = sender as TextBox;
                Attribute attribute = textbox.DataContext as Attribute;
                Entity asociatedEntity = App.CurrentProject.Entities.Find(x => x.Name== attribute.AssociatedEntity);


                if (string.IsNullOrEmpty(textbox.Text))
                {

                    RefIntegrity = false;
                    Warning.Margin = new Thickness(20, 10, 10, 0);
                    Warning.Text = "Please provide a value for the field " + attribute.Name;
                }
                else if (!asociatedEntity.Registers.Any(x => x.Key == Utils.StringToField(textbox.Text, attribute)))
                {
                    RefIntegrity = false;
                    Warning.Margin = new Thickness(20, 10, 10, 0);
                    Warning.Text = "Error: The value provided as Secodary Key does not exist";
                }
                else
                {
                    
                    Warning.Text = "";
                }

            }
        }

        // Hay luego compa
        private bool ValidateFields()
        {
            bool result = false;
            for (int i = 0; i < Container.Children.Count; i++)
            {
               
            }
            return result;

        }

        private List<string> ConvertToStrings()
        {
            List<string> s = new List<string>();

            for (int i = 0; i < Container.Children.Count; i++)
            {
                if (Container.Children[i].GetType() == typeof(TextBox))
                {
                    TextBox tb = (Container.Children[i] as TextBox);
                    if (!string.IsNullOrEmpty(tb.Text))
                        s.Add(tb.Text);
                    else
                        return null;
                }
                    
                if (Container.Children[i].GetType() == typeof(CheckBox))
                    s.Add((Container.Children[i] as CheckBox).IsChecked.ToString());
            }


            return s;
        }

       
    }
}
