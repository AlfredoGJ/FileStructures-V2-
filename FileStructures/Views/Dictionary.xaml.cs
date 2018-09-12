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

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace FileStructures.Views
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class Dictionary : Page
    {
        DictionaryManager manager;

        public Dictionary()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

           
            manager = new DictionaryManager(App.CurrentFileName);
            
            //manager.itemsOnFileChanged = new DictionaryManager.ItemsOnFileChanged(newmethod);
            manager.itemsOnFileChanged+= UpdateDictionaryData;
            //do { } while (manager.Entities == null);
           

        }

        private void UpdateDictionaryData()
        {
            Entities.ItemsSource = null;
            Entities.ItemsSource = manager.Entities;
        }

        private async void AddEntity_Click(object sender, RoutedEventArgs e)
        {
            Entity entity;
            ContentDialogResult result= await addEntityContentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                if (EntityName.Text != "")
                {
                    entity = new Entity(EntityName.Text);
                    manager.AddEntity(entity);
                }
                
            }

            Entities.ItemsSource = null;
            Entities.ItemsSource = manager.Entities;

        }
    }
}
