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
    public sealed partial class TreeNode : UserControl
    {
        public TreeNode()
        {
            this.InitializeComponent();
        }

        public TreeNode(List<TreeElement> elements)
        {
            this.InitializeComponent();
            foreach (TreeElement element in elements)
                Childs.Children.Add(element);

        }
        public void Add(TreeElement element)
        {
            Childs.Children.Add(element);
        }
    }
}
