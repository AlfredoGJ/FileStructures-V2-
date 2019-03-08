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

// La plantilla de elemento del cuadro de diálogo de contenido está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace FileStructures.Controls
{
    public sealed partial class EditAttributeContentDialog : ContentDialog
    {
        private Attribute auxAttribute;
        private Attribute attribute;
        private Entity entity;
        private bool isNew;

        public EditAttributeContentDialog(Attribute attribute, Entity entity)
        {
            this.InitializeComponent();
            this.attribute = attribute;
            this.entity = entity;
            auxAttribute = new Attribute(attribute);
            AsociatedEntity.ItemsSource = App.CurrentProject.Entities;
            isNew = false;


        }

        public EditAttributeContentDialog(Entity entity)
        {
            this.InitializeComponent();
            this.entity = entity;
            this.auxAttribute = new Attribute();
            AsociatedEntity.ItemsSource = App.CurrentProject.Entities;
            isNew = true;

        }

        private void DataType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (DataType.SelectedIndex)
            {
                case  0:
                    auxAttribute.DataType = DataTypes.Integer;
                    break;

                case 1:
                    auxAttribute.DataType = DataTypes.String;
                    break;

                case 2:
                    auxAttribute.DataType = DataTypes.Character;
                    break;

                case 3:
                    auxAttribute.DataType = DataTypes.Float;
                    break;

                case 4:
                    auxAttribute.DataType = DataTypes.Boolean;
                    break;

                case 5:
                    auxAttribute.DataType = DataTypes.Long;
                    break;

               
            }
        }

        private void IndexType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (IndexType.SelectedIndex)
            {
                case 0:
                    auxAttribute.KeyType = KeyTypes.NoKey;
                    AsociatedEntity.Visibility = Visibility.Collapsed;
                    DataType.IsEnabled = true;
                    auxAttribute.AsociatedEntity = "";
                    break;

                case 1:
                    auxAttribute.KeyType = KeyTypes.Primary;
                    AsociatedEntity.Visibility = Visibility.Collapsed;
                    DataType.IsEnabled = true;
                    auxAttribute.AsociatedEntity = "";
                    break;


                case 2:
                    auxAttribute.KeyType = KeyTypes.Foreign;
                    AsociatedEntity.Visibility = Visibility.Visible;
                    DataType.SelectedIndex = 5;
                    DataType.IsEnabled = false;
                    break;
            }
        }


        private void addAttributeEditContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            auxAttribute.Name = AttributeName.Text;
            if (!String.IsNullOrWhiteSpace(AttributeName.Text) && DataType.SelectedValue != null && IndexType.SelectedValue != null)
            {
                // An attribute is being added, not edited, "attribute" is passed in the constructor 
                // When its mean to be edited.
                if (attribute== null)
                {
                    if (!entity.Attributes.Any(x => x.Name == auxAttribute.Name))
                    {
                        if (entity.Attributes.Any(x => x.KeyType == KeyTypes.Primary) && auxAttribute.KeyType == KeyTypes.Primary)
                        {
                            args.Cancel = true;
                            Warning.Text = "Error: This entity already contains a primary key.";
                        }
                        else
                        {
                            auxAttribute.Name = AttributeName.Text;
                            entity.AddAttribute(auxAttribute);
                        }

                    }
                    else
                    {
                        args.Cancel = true;
                        Warning.Text = "Error: Already exists an attribute with ths name.";
                    }
                }
                else
                {
                    //if (auxAttribute.Name == attribute.Name && auxAttribute.KeyType == attribute.KeyType)
                    //{

                    //}
                    if (auxAttribute.KeyType != attribute.KeyType && auxAttribute.KeyType == KeyTypes.Primary && entity.Attributes.Any(x => x.KeyType == KeyTypes.Primary))
                    {
                        args.Cancel = true;
                        Warning.Text = "Error: This entity already contains a primary key.";
                    }


                    if (auxAttribute.Name != attribute.Name && entity.Attributes.Any(x => x.Name == auxAttribute.Name))
                    {
                        args.Cancel = true;
                        Warning.Text = "Error: Already exists an attribute with ths name.";
                    }

                }
                   
               
            }
            else
            {
                args.Cancel = true;
                Warning.Text = "Error: Complete all the fields correctly";
            }


        }

        private void AnyField_GotFocus(object sender, RoutedEventArgs e)
        {
            Warning.Text = "";
        }

        private void AsociatedEntity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            auxAttribute.AsociatedEntity = (AsociatedEntity.SelectedItem as Entity).Name; 
        }
    }
}
