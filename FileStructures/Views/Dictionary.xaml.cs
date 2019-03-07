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
      
        public Dictionary()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.CurrentProject!=null)
            {
                AddEntityButton.IsEnabled = true;
                Entities.ItemsSource = null;
                Entities.ItemsSource = App.CurrentProject.Entities;
                App.SerializeProject();

            }
            else
            {
                AddEntityButton.IsEnabled = false;
               
            }
        }

       

      

        private async void AddEntity_Click(object sender, RoutedEventArgs e)
        {
            Entity entity= new Entity("Uninitialized");
            ContentDialog cd = new EntityContentDialog("Add Entity",entity);

            ContentDialogResult result= await cd.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                
                Entities.ItemsSource = null;
                Entities.ItemsSource = App.CurrentProject.Entities;
                
            }
            
        }

        private async void DeleteEntityButton_Click(object sender, RoutedEventArgs e)
        {
            Entity entity = Entities.SelectedItem as Entity;
            ContentDialog cd = new ContentDialog();
            cd.CloseButtonText = "No";
            cd.PrimaryButtonText = "Yes";
            cd.Title = "Delete Entity";
            cd.Content = "Are you sure you want to delete the Entity "+entity.Name+ "?";
            var result = await cd.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                App.CurrentProject.DeleteEntity(entity);
                Entities.ItemsSource = null;
                Entities.ItemsSource = App.CurrentProject.Entities;

            }
        }

        private async void EditEntityButton_Click(object sender, RoutedEventArgs e)
        {

            
            Entity entity = Entities.SelectedItem as Entity;
            ContentDialog cd =  new EntityContentDialog("Edit Entity", entity);
            ContentDialogResult result = await cd.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                Entities.ItemsSource = null;
                Entities.ItemsSource = App.CurrentProject.Entities;

            }
        }


        private async  void AddAttributeButton_Click(object sender, RoutedEventArgs e)
        {
            Entity entity = Entities.SelectedItem as Entity;
            if (entity != null)
            {

                ContentDialog dialog = new EditAttributeContentDialog(entity);
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {

                }

            }
            
        }
        

        private async void addAttributeContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
           
           
        }

        private async void DeleteAttribute_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog cd = new ContentDialog();
            cd.CloseButtonText = "No";
            cd.PrimaryButtonText = "Yes";
            cd.Title = "Delete Attribute";
            cd.Content = "Are you sure?";
            var result = await cd.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                Entity entity = Entities.SelectedItem as Entity;
                //Attribute attribute = (sender as Control).DataContext as Attribute;
                Attribute attribute = Attributes.SelectedItem as Attribute;
                //entity.RemoveAttribute(attribute);
               
            }
        }

        private async void EditAttribute_Click(object sender, RoutedEventArgs e)
        {
            //Attribute attribute = (sender as Control).DataContext as Attribute;
            Attribute attribute = Attributes.SelectedItem as Attribute;
            Entity entity = Entities.SelectedItem as Entity;
            EditAttributeContentDialog editAttributeContentDialog = new EditAttributeContentDialog(attribute, entity);
            var result=await editAttributeContentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
                ;
               

        }

        

      

        private void Entities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Entities.SelectedItem != null)
            {
                EditEntityButton.IsEnabled = true;
                DeleteEntityButton.IsEnabled = true;

            }
            else
            {
                EditEntityButton.IsEnabled = false;
                DeleteEntityButton.IsEnabled = false;
            }
        }

        private void Attributes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Attributes.SelectedItem != null)
            {
                EditAttributeButton.IsEnabled = true;
                DeleteAttributeButton.IsEnabled = true;

            }
            else
            {
                EditAttributeButton.IsEnabled = false;
                DeleteAttributeButton.IsEnabled =false;
            }
        }

       
    }
}
