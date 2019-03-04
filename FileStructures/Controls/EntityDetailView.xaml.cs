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

// La plantilla de elemento Control de usuario está documentada en https://go.microsoft.com/fwlink/?LinkId=234236

namespace FileStructures.Controls
{
    public delegate void EntityChangedHandler (Entity entity, char chage);
    public sealed partial class EntityDetailView : UserControl
    {

        public EntityChangedHandler entityChanged;
        public Entity entity { get; set; }
        public EntityDetailView()
        {
            this.InitializeComponent();
            

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            int i = 0;
            var dc = DataContext;
            entity = this.DataContext as Entity;
            //Name.Text = entity.Name;
            //Pos.Text = entity.Position.ToString();
            //AtrrPtr.Text = entity.AttributesPtr.ToString();
            //DataPtr.Text = entity.DataPtr.ToString();
            //NextPtr.Text = entity.NextPtr.ToString();

            //entityChanged+= 
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            entityChanged.Invoke(entity,'d');
        }
    }
}
