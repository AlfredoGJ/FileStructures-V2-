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
    public sealed partial class Config : Page
    {
        public Config()
        {
            this.InitializeComponent();
        }

        private void tree_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentFileOrganization != FileOrganization.Tree)
                App.CurrentFileName = "";
            App.CurrentFileOrganization = FileOrganization.Tree;
        }

        private void indexed_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentFileOrganization != FileOrganization.Indexed)
                App.CurrentFileName = "";
            App.CurrentFileOrganization = FileOrganization.Indexed;
        }

        private void ordered_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentFileOrganization != FileOrganization.Ordered)
                App.CurrentFileName = "";
            App.CurrentFileOrganization = FileOrganization.Ordered;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.CurrentFileOrganization == FileOrganization.Indexed)
                indexed.IsChecked = true;

            if (App.CurrentFileOrganization == FileOrganization.Ordered)
                ordered.IsChecked = true;

            if (App.CurrentFileOrganization == FileOrganization.Tree)
                tree.IsChecked = true;
        }
    }
}
