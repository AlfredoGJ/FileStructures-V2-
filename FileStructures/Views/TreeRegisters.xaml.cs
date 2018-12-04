using FileStructures.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
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
    public sealed partial class TreeRegisters : Page
    {
        DictionaryManager dataManager;
        public TreeRegisters()
        {
            this.InitializeComponent();
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(App.CurrentFileName))
            {
                dataManager = new DictionaryManager(App.CurrentFileName);
                dataManager.itemsOnFileChanged += UpdateDictionaryData;

            }
        }

        private void UpdateDictionaryData()
        {
            EntitiesList.ItemsSource = null;
            EntitiesList.ItemsSource = dataManager.Entities;
            foreach (Entity entity in dataManager.Entities)
                entity.itemsOnFileChanged += UpdateRegistersData;
        }

        private void UpdateRegistersData()
        {
            if (EntitiesList.SelectedItem != null)
            {
                Entity currentEntity = (EntitiesList.SelectedItem as Entity);
                RegistersList.ItemsSource = null;
                RegistersList.ItemsSource = currentEntity.Registers;
                currentEntity.treeManager.Tree.Draw(MyCanvas);
                //FillIndexesView((EntitiesList.SelectedItem as Entity).indexManager.Indexes);

                
            }
        }

        private void AddRegister_Click(object sender, RoutedEventArgs e)
        {
            if (EntitiesList.SelectedItem != null)
            {

                AddRegisterContentDialog dialog = new AddRegisterContentDialog(EntitiesList.SelectedItem as Entity);
                dialog.ShowAsync();



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
                pos.Text = "Position";
                pos.FontSize = 18;
                Grid.SetColumn(pos, i);
                Headers.Children.Add(pos);

                Headers.ColumnDefinitions.Add(new ColumnDefinition());
                TextBlock next = new TextBlock();
                next.Text = "NextPtr";
                next.FontSize = 18;
                Grid.SetColumn(next, i + 1);
                Headers.Children.Add(next);

                Headers.ColumnDefinitions.Add(new ColumnDefinition());
                Headers.ColumnDefinitions.Add(new ColumnDefinition());



                UpdateRegistersData();
                //FillIndexesView(E.indexManager.Indexes);
            }
        }

        //private void FillIndexesView(List<Index> indexes)
        //{
        //    Indexes.Children.Clear();
        //    foreach (Index index in indexes)
        //    {
        //        Indexes.Children.Add(new IndexControl(index));
        //    }
        //}

        async void DeleteRegisterButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            DataRegister register = (button.Parent as Grid).DataContext as DataRegister;

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
                    Attribute attribute = (sender as Control).DataContext as Attribute;
                    entity.RemoveRegister(register, true);

                }
            }


            //(e.OriginalSource as FrameworkElement).DataContext;

        }

        private void EditRegisterButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            DataRegister register = (button.Parent as Grid).DataContext as DataRegister;

            if (EntitiesList.SelectedItem != null)
            {
                Entity entity = EntitiesList.SelectedItem as Entity;

                AddRegisterContentDialog dialog = new AddRegisterContentDialog(EntitiesList.SelectedItem as Entity, register);
                dialog.ShowAsync();

            }
        }

        private async void ImportRegisters_Click(object sender, RoutedEventArgs e)
        {

            if (EntitiesList.SelectedItem != null)
            {
                Entity current = EntitiesList.SelectedItem as Entity;

                FileOpenPicker picker = new FileOpenPicker();
                picker.ViewMode = PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                picker.FileTypeFilter.Add(".txt");


                StorageFile file = await picker.PickSingleFileAsync();

                if (file != null)
                {
                    var stream = await file.OpenReadAsync();
                    byte[] data = new byte[stream.Size];
                    string sRead = "";

                    using (BinaryReader reader = new BinaryReader(await file.OpenStreamForReadAsync()))
                    {
                        reader.BaseStream.Seek(0, SeekOrigin.Begin);
                        reader.Read(data, 0, data.Length);
                        foreach (byte b in data)
                        {
                            sRead += (char)b;
                        }


                    }

                    List<DataRegister> registers = new List<DataRegister>();
                    List<List<string>> intermediate = App.CutStringToRegisters(sRead);

                    foreach (List<string> reg in intermediate)
                    {
                        DataRegister newRegister = new DataRegister(reg, current.Attributes);
                        //registers.Add(new DataRegister(reg, current.Attributes));

                        current.AddRegister(newRegister, false, false);
                        

                    }
                       



                }

            }
            
        }
    }
}
