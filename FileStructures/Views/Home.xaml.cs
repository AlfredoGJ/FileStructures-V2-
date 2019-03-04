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
    public sealed partial class Home : Page
    {
        public Home()
        {
            this.InitializeComponent();

            if (App.CurrentProject!=null)
            {
                CurrentFileName.Text = App.CurrentProject.Name;
                Header.Text = "Current opened file:";
            }
            else
            {
                Header.Text = "There is no an opened file";
            }
        }

        private async void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            picker.FileTypeFilter.Add(".DBP");
            picker.FileTypeFilter.Add(".dbp");

            StorageFile file =  await picker.PickSingleFileAsync();

            
            if (file != null)
            {
                CurrentFileName.Text = file.Name;
                App.CurrentProject =  await App.DeserializeProject(file.Path);

            }

            
        }

        private async void NewFile_Click(object sender, RoutedEventArgs e)
        {
            ContentDialogResult dialog = await NewFileDialog.ShowAsync();
            if (dialog == ContentDialogResult.Primary)
            {


                StorageFolder localFolder = KnownFolders.MusicLibrary;
                StorageFolder projectsFolder=  await localFolder.CreateFolderAsync("SGBDProjects", CreationCollisionOption.OpenIfExists);
                CurrentFileName.Text = FileNameTextBox.Text + ".dbp";
                App.CurrentProject = new Project(FileNameTextBox.Text + ".dbp");
                App.SerializeProject();

            }
        }
    }
}
