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
    public sealed partial class EntityContentDialog : ContentDialog
    {

        private Entity entity;

        public EntityContentDialog( string title, Entity entity)
        {
            this.InitializeComponent();
            this.Title = title;
            this.entity = entity;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {


            if (String.IsNullOrEmpty(Name.Text))
            {
                // mensaje
                args.Cancel = true;

            }
            else if (App.CurrentProject.Entities.Any(x => x.Name == Name.Text))
            {
                if (entity.Name != Name.Text)
                    // mensaje
                    args.Cancel = true;

            }

            else
            {
                if (entity.Name == "Uninitialized")
                    App.CurrentProject.Entities.Add(entity);

                entity.Name = Name.Text;
            }

            



        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }


    }
}
