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
    public sealed partial class Registers : Page
    {
       
        public Registers()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.EntitiesList.ItemsSource = App.CurrentProject.Entities;
        }

        private void UpdateDictionaryData()
        {
            //EntitiesList.ItemsSource = null;
            //EntitiesList.ItemsSource = manager.Entities;
            //foreach (Entity entity in manager.Entities)
            //    entity.itemsOnFileChanged += UpdateRegistersData;
        }

        private void UpdateRegistersData()
        {
            if (EntitiesList.SelectedItem != null)
            {
                RegistersList.ItemsSource = null;
                RegistersList.ItemsSource = (EntitiesList.SelectedItem as Entity).Registers;
            }
        }

        private async void AddRegister_Click(object sender, RoutedEventArgs e)
        {
            if (EntitiesList.SelectedItem != null)
            {

                AddRegisterContentDialog dialog = new AddRegisterContentDialog(EntitiesList.SelectedItem as Entity);
                var result = await  dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    App.SerializeProject();
                    UpdateRegistersData();
                }
                    







            }

        }

        private void EntitiesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (EntitiesList.SelectedItem != null)
            {
                Headers.ColumnDefinitions.Clear();
                Headers.Children.Clear();
                Entity E = EntitiesList.SelectedItem as Entity;
                //E.itemsOnFileChanged += UpdateRegistersData;
                int i;
                for (i = 0; i < E.Attributes.Count; i++)
                {
                    Headers.ColumnDefinitions.Add(new ColumnDefinition());
                    TextBlock tb = new TextBlock();
                    tb.Text = E.Attributes[i].Name;
                    tb.FontSize = 18;
                    tb.Foreground = new SolidColorBrush(color: Windows.UI.Color.FromArgb(255, 255, 255, 255));
                    Grid.SetColumn(tb, i);
                    Headers.Children.Add(tb);
                }


                UpdateRegistersData();
            }
        }

         async void DeleteRegisterButtonClick(object sender, RoutedEventArgs e)
        {

            DataRegister register = (DataRegister)RegistersList.SelectedItem;

            if (EntitiesList.SelectedItem != null)
            {
                Entity entity = EntitiesList.SelectedItem as Entity;
                ContentDialog cd = new ContentDialog();
                cd.CloseButtonText = "No";
                cd.PrimaryButtonText = "Yes";
                cd.Title = "Delete Register";
                cd.Content = "Are you sure?";
                var result = await cd.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                   
                    entity.RemoveRegister (register);
                    UpdateRegistersData();
                    App.SerializeProject();
                   
                }
            }

        }

        private async void EditRegisterButtonClick(object sender, RoutedEventArgs e)
        {
            DataRegister register = RegistersList.SelectedItem as DataRegister;

            if (EntitiesList.SelectedItem != null)
            {
                Entity entity = EntitiesList.SelectedItem as Entity;

                AddRegisterContentDialog dialog = new AddRegisterContentDialog(EntitiesList.SelectedItem as Entity, register);
                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    App.SerializeProject();
                    UpdateRegistersData();
                }

            }
        }
    }
}
