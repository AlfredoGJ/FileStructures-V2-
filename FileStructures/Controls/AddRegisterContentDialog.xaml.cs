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
        byte[] block;
        int offset;

        public AddRegisterContentDialog( Entity entity)
        {
            this.InitializeComponent();
            this.entity = entity;
            int blockSise = entity.Attributes.Sum(x => x.Length);
            byte[] block = new byte[blockSise];
            offset = 0;

            foreach (Attribute attr in this.entity.Attributes)
            {
                TextBox tb = new TextBox();
                tb.Name = attr.Name;
                tb.PlaceholderText = attr.Name;
                tb.Margin = new Thickness(20, 0, 0, 0);
                
                switch (attr.Type)
                {
                    case 'I':
                        tb.Width = 80;
                        tb.MaxLength = 8;

                        break;

                    case 'S':
                        tb.Width = attr.Length * 8;
                        tb.MaxLength = attr.Length;
                        break;
                }

                Container.Children.Add(tb);
            }



        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

            List<string> values = new List<string>();
            for (int i = 0; i < Container.Children.Count; i++)
              values.Add( (Container.Children[i] as TextBox).Text);

            DataRegister register = new DataRegister(values, entity.Attributes);

            entity.AddRegister(register);

            }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }
    }
}
