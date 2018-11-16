﻿using System;
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

// La plantilla de elemento del cuadro de diálogo de contenido está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace FileStructures.Controls
{
    public sealed partial class EditAttributeContentDialog : ContentDialog
    {
        private Attribute attribute;
        private Entity entity;

        public EditAttributeContentDialog(Attribute attribute, Entity entity)
        {
            this.InitializeComponent();
            this.attribute = new Attribute(attribute.Name,attribute.Type,attribute.Length,attribute.IndexType,attribute.Position,attribute.IndexPtr,attribute.NextPtr);
            this.entity = entity;



            AttributeName.Text = attribute.Name;

            if (attribute.Type == 'I')
            {
                DataType.SelectedIndex = 0;
                Lenght.IsEnabled = false;
            }
            else
            {
                DataType.SelectedIndex = 1;
                Lenght.IsEnabled = true;
            }

            Lenght.Text = attribute.Length.ToString();
            IndexType.SelectedIndex = attribute.IndexType;

        }

        public EditAttributeContentDialog(Entity entity)
        {
            this.InitializeComponent();
            this.entity = entity;

        }

        private void DataType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (DataType.SelectedIndex)
            {
                case 0:
                    Lenght.Text = "4";
                    Lenght.IsEnabled = false;
                    break;

                case 1:
                    Lenght.Text = "1";
                    Lenght.IsEnabled = true;
                    break;
            }
        }

        private async  void addAttributeEditContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

            // Th attribute is being edited 
            if (attribute != null)
            {
                int lenght = 0;
                int.TryParse(Lenght.Text, out lenght);

                attribute.Name = AttributeName.Text;
                attribute.Type = DataType.SelectedValue.ToString()[0];
                attribute.Length = lenght;
                attribute.IndexType = IndexType.SelectedIndex;

                bool result = await entity.UpdateAttribute(attribute);
                if (!result)
                {
                    args.Cancel = true;
                    Warning.Text = "Error: Already exists an attribute with ths name.";
                }
                    
            }
            // The attribute is being created and inserted
            else
            {
                if (!String.IsNullOrWhiteSpace(AttributeName.Text) && DataType.SelectedValue != null && IndexType.SelectedValue != null)
                {

                    int lenght = 0;
                    int.TryParse(Lenght.Text, out lenght);
                    var attributes = entity.Attributes;

                    Attribute attribute = new Attribute(AttributeName.Text, DataType.SelectedValue.ToString()[0], lenght, IndexType.SelectedIndex);
                    if (!attributes.Any(x => x.Name == attribute.Name))
                    {
                        if (attributes.Any(x => x.IndexType == 2) && attribute.IndexType == 2)
                        {
                            args.Cancel = true;
                            Warning.Text = "Error: This entity already contains a primary key.";
                        }
                        else
                            entity.AddAttribute(attribute);
                    }
                    else
                    {
                        args.Cancel = true;
                        Warning.Text = "Error: Already exists an attribute with ths name.";
                    }
                    

                }
                else
                {
                    args.Cancel = true;
                    Warning.Text = "Error: Complete all the fields correctly";
                }
            }

            
           

        }

        private void AnyField_GotFocus(object sender, RoutedEventArgs e)
        {
            Warning.Text = "";
        }

   
    }
}
