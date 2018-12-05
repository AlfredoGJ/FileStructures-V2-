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

            if (!String.IsNullOrEmpty( App.CurrentFileName))
            {
                CurrentFileName.Text = App.CurrentFileName;
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
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".ddc");
            picker.FileTypeFilter.Add(".idd");
            picker.FileTypeFilter.Add(".tdd");



            StorageFile file =  await picker.PickSingleFileAsync();
            
            if (file != null)
            {
                CurrentFileName.Text = file.Name;
                App.CurrentFileName = file.Name;
                if (file.FileType == ".ddc")
                    App.CurrentFileOrganization =  FileOrganization.Ordered;
                else if(file.FileType==".idd")
                    App.CurrentFileOrganization =  FileOrganization.Indexed; 
                else
                    App.CurrentFileOrganization = FileOrganization.Tree;

            }

            
        }

        private async void NewFile_Click(object sender, RoutedEventArgs e)
        {
            ContentDialogResult dialog = await NewFileDialog.ShowAsync();
            if (dialog == ContentDialogResult.Primary)
            {
               
                StorageFolder localFolder = KnownFolders.PicturesLibrary;
                StorageFolder projectsFolder=  await localFolder.CreateFolderAsync("Projects", CreationCollisionOption.OpenIfExists);
                StorageFile myNewFile;
                if (App.CurrentFileOrganization== FileOrganization.Ordered)
                    myNewFile= await projectsFolder.CreateFileAsync(FileNameTextBox.Text + ".ddc");
                else if(App.CurrentFileOrganization == FileOrganization.Indexed)
                    myNewFile = await projectsFolder.CreateFileAsync(FileNameTextBox.Text + ".idd");
                else
                    myNewFile = await projectsFolder.CreateFileAsync(FileNameTextBox.Text + ".tdd");

                BinaryWriter writer = new BinaryWriter(  await myNewFile.OpenStreamForWriteAsync());
                writer.Write(((long)(-1)));
                writer.Close();


                CurrentFileName.Text = myNewFile.Name ;
                Header.Text = "Current opened file:";
                App.CurrentFileName = myNewFile.Name;

            }
        }
    }
}
