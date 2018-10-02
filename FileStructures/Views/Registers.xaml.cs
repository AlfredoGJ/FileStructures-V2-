﻿using FileStructures.Controls;
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
        DictionaryManager manager;
        public Registers()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(App.CurrentFileName))
            {
                manager = new DictionaryManager(App.CurrentFileName);
                manager.itemsOnFileChanged += UpdateDictionaryData;
               
            }
        }

        private void UpdateDictionaryData()
        {
            EntitiesList.ItemsSource = null;
            EntitiesList.ItemsSource = manager.Entities;
            foreach (Entity entity in manager.Entities)
                entity.itemsOnFileChanged += UpdateRegistersData;
        }

        private void UpdateRegistersData()
        {
            if (EntitiesList.SelectedItem != null)
            {
                RegistersList.ItemsSource = null;
                RegistersList.ItemsSource = (EntitiesList.SelectedItem as Entity).Registers;
            }            
        }

        private void AddRegister_Click(object sender, RoutedEventArgs e)
        {
            if (EntitiesList.SelectedItem != null)
            {

                AddRegisterContentDialog dialog = new AddRegisterContentDialog(EntitiesList.SelectedItem as Entity);
                dialog.ShowAsync();

                UpdateRegistersData();

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
                    Grid.SetColumn(tb, i);
                    Headers.Children.Add(tb);
                }

                Headers.ColumnDefinitions.Add(new ColumnDefinition());
                TextBlock pos = new TextBlock();
                pos.Text ="Position";
                pos.FontSize = 18;
                Grid.SetColumn(pos, i);
                Headers.Children.Add(pos);

                Headers.ColumnDefinitions.Add(new ColumnDefinition());
                TextBlock next = new TextBlock();
                next.Text = "NextPtr";
                next.FontSize = 18;
                Grid.SetColumn(next, i+1);
                Headers.Children.Add(next);


                UpdateRegistersData();
            }
        }

      
    }
}
