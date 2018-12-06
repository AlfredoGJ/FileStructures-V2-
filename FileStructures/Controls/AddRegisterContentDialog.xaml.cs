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
        public AddRegisterContentDialog( Entity entity)
        {
            this.InitializeComponent();
            this.entity = entity;
            //int blockSise = entity.Attributes.Sum(x => x.Length);
            //byte[] block = new byte[blockSise];
            

            foreach (Attribute attr in this.entity.Attributes)
            {
                
                TextBox tb = new TextBox();
                tb.Name = attr.Name;
                tb.PlaceholderText = attr.Name;
                tb.Margin = new Thickness(20, 0, 0, 0);
                
                switch (attr.Type)
                {
                    case 'I':
                        tb.Width = 120;
                        tb.MaxLength = 8;
                        tb.TextChanging += NumbersOnly;
                        break;

                    case 'S':
                        tb.Width = attr.Length * 12;
                        tb.MaxLength = attr.Length;
                        tb.TextChanging += OnlyText;
                        break;
                }

                Container.Children.Add(tb);
            }

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

            foreach (Attribute attr in this.entity.Attributes)
            {
                TextBox tb = new TextBox();
                tb.Name = attr.Name;
                tb.PlaceholderText = attr.Name;
                tb.Margin = new Thickness(20, 0, 0, 0);
                tb.Text = register.Fields[entity.Attributes.IndexOf(attr)].ToString();

                switch (attr.Type)
                {
                    case 'I':
                        tb.Width = 120;
                        tb.MaxLength = 8;
                        break;

                    case 'S':
                        tb.Width = attr.Length * 12;
                        tb.MaxLength = attr.Length;
                        break;
                }
                Container.Children.Add(tb);
            }
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            List<string> values = new List<string>();
            for (int i = 0; i < Container.Children.Count; i++)
                values.Add((Container.Children[i] as TextBox).Text);

            DataRegister register = new DataRegister(values, entity.Attributes);

            // If we are adding the register
            if (this.register == null)
            {
                if (!entity.Registers.Any(x => App.CompareObjects(x.Key, register.Key) == 0))
                    entity.AddRegister(register, true, false);
                else
                {
                    Warning.Margin = new Thickness(20, 10, 10, 0);
                    Warning.Text = "Error: This entity already contains a register with the key " + register.Key.ToString();
                    args.Cancel = true;
                }
            }
                

            // If we are Editing an existent Register
            else
            {
                register.Position = this.register.Position;
                register.NextPtr = this.register.NextPtr;
                bool result = await entity.UpdateDataRegister(register);

                if (!result)
                    args.Cancel = true;


            }

        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }
    }
}
