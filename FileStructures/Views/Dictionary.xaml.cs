using FileStructures.Controls;
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
        string entityNameAux="";
        public Dictionary()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(App.CurrentFileName))
            {
                AddEntityButton.IsEnabled = true;

                manager = new DictionaryManager(App.CurrentFileName);
                manager.itemsOnFileChanged += UpdateDictionaryData;

            }
            else
            {
                AddEntityButton.IsEnabled = false;
            }
        }

        private void UpdateDictionaryData()
        {
            Entities.ItemsSource = null;
            Entities.ItemsSource = manager.Entities;
            Header.Text = "  "+manager.Header.ToString(); ;
        }

        public void EntityChange(Entity entity, char chage)
        {
            Console.WriteLine("Matanga dijo la changa");
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
                    var entities = manager.Entities;
                    if (!entities.Any(x => x.Name == entity.Name))
                    {
                        manager.AddEntity(entity);
                    }
                    else
                    {
                        ContentDialog cd = new ContentDialog();
                        cd.CloseButtonText = "OK";
                        cd.Title = "Error";
                        cd.Content = "Already exist an entity with this name";
                        cd.ShowAsync();
                    }
                }
                
            }
            UpdateDictionaryData();
        }

        private async  void Delete_Click(object sender, RoutedEventArgs e)
        {

            ContentDialog cd = new ContentDialog();
            cd.CloseButtonText = "No";
            cd.PrimaryButtonText = "Yes";
            cd.Title = "Delete Entity";
            cd.Content = "Are you sure?";
            var result= await  cd.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                Entity entity = (sender as Control).DataContext as Entity;
                manager.RemoveEntity(entity);
                UpdateDictionaryData();
            }
          
            
        }

        private void Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            Console.WriteLine("Matanga dijo la changa");
        }

        private void Name_LostFocus(object sender, RoutedEventArgs e)
        {
            Entity entity = (sender as Control).DataContext as Entity;
            TextBox textBox = (sender as TextBox);
            string newName = textBox.Text;
            var entities = manager.Entities;

            // The name of the entity has been changed
            if (newName != entity.Name)
            {
                if (!entities.Any(x => x.Name == newName))
                    manager.UpdateEntity(entity, newName);
                else
                {
                    ContentDialog cd = new ContentDialog();
                    cd.CloseButtonText = "OK";
                    cd.Title = "Error";
                    cd.Content = "Already exist an entity with the name: "+newName ;
                    textBox.Text = entity.Name;
                    cd.ShowAsync();
                    
                }
            }
                
            

        }

        private async  void AddAttributeButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialogResult result = await addAttributeContentDialog.ShowAsync();
        }
    }
}
