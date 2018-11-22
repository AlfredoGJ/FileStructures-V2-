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

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace FileStructures
{

    public enum FileOrganization {Ordered, Indexed,Tree }

    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Navigation_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (NavigationViewItemBase item in navigation.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "Home_Page")
                {
                    navigation.SelectedItem = item;
                    break;
                }
            }
            ContentFrame.Navigate(typeof(Views.Home));
        }

        private void navigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            switch ((args.SelectedItem as NavigationViewItem).Content)
            {
                case "Archivo":
                         ContentFrame.Navigate(typeof(Views.Home));
                    break;


                case "Diccionario":
                    ContentFrame.Navigate(typeof(Views.Dictionary));
                    break;

                case "Datos":
                    if(App.CurrentFileOrganization== FileOrganization.Ordered)
                        ContentFrame.Navigate(typeof(Views.Registers));
                    if(App.CurrentFileOrganization == FileOrganization.Indexed)
                        ContentFrame.Navigate(typeof(Views.IndexedRegisters));
                    break;

                case "Configuración":
                    ContentFrame.Navigate(typeof(Views.Config));
                    break;
            }
           
        }
    }
}
