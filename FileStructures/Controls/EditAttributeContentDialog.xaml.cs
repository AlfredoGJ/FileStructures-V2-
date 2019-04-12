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
            isNew = false;
            List<Entity> dummy = new List<Entity>();
            dummy.Add(entity);
            AsociatedEntity.ItemsSource = App.CurrentProject.Entities.Except(dummy);

            this.AttributeName.Text = attribute.Name;
            this.DataType.SelectedIndex = (int)attribute.DataType;
            this.IndexType.SelectedIndex = (int)attribute.KeyType;



        }

        public EditAttributeContentDialog(Entity entity)
        {
            this.InitializeComponent();
            this.entity = entity;
            this.auxAttribute = new Attribute();
            isNew = true;

            List<Entity> dummy = new List<Entity>();
            dummy.Add(entity);
            AsociatedEntity.ItemsSource = App.CurrentProject.Entities.Except(dummy);
            

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
                    auxAttribute.AssociatedEntity = "N/A";
                    break;

                case 1:
                    auxAttribute.KeyType = KeyTypes.Primary;
                    AsociatedEntity.Visibility = Visibility.Collapsed;
                    DataType.IsEnabled = true;
                    auxAttribute.AssociatedEntity = "N/A";
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
            auxAttribute.Description = Description.Text;
            if (!String.IsNullOrWhiteSpace(AttributeName.Text) && DataType.SelectedValue != null && IndexType.SelectedValue != null)
            {
                // An attribute is being added, not edited, "attribute" is passed in the constructor 
                // When its meant to be edited.
                if (attribute== null)
                {
                    // Entity doesn't contain an attribute with this name 
                    if (!entity.Attributes.Any(x => x.Name == auxAttribute.Name))
                    {
                        // If already contains a primary key
                        if (entity.Key != null && auxAttribute.KeyType == KeyTypes.Primary) //Attributes.Any(x => x.KeyType == KeyTypes.Primary)
                        {
                            args.Cancel = true;
                            Warning.Text = "Error: This entity already contains a primary key.";
                        }
                        else if (auxAttribute.KeyType == KeyTypes.Foreign && AsociatedEntity.SelectedItem == null)
                        {
                            Warning.Text = "Please select a valid entity as Foreign Key";
                            args.Cancel = true;
                        }
                        else if (auxAttribute.KeyType == KeyTypes.Foreign && entity.Attributes.Any(x =>x.KeyType==KeyTypes.Foreign && x.AssociatedEntity== auxAttribute.AssociatedEntity))
                        {
                            Warning.Text = "This entity already contains Foreign Key from the selected Entity";
                            args.Cancel = true;
                        }
                        else
                        {
                            auxAttribute.Name = AttributeName.Text;
                            auxAttribute.Description = Description.Text;
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
                    if (auxAttribute.KeyType != attribute.KeyType && auxAttribute.KeyType == KeyTypes.Primary && entity.Key!=null)//Attributes.Any(x => x.KeyType == KeyTypes.Primary)
                    {
                        args.Cancel = true;
                        Warning.Text = "Error: This entity already contains a primary key.";
                        return;
                    }


                    if (auxAttribute.Name != attribute.Name && entity.Attributes.Any(x => x.Name == auxAttribute.Name))
                    {
                        args.Cancel = true;
                        Warning.Text = "Error: Already exists an attribute with ths name.";
                        return;
                    }



                    else if (auxAttribute.KeyType == KeyTypes.Foreign && AsociatedEntity.SelectedItem == null)
                    {
                        Warning.Text = "Please select a valid entity as Foreign Key";
                        args.Cancel = true;
                        return;
                    }
                    else if (auxAttribute.KeyType == KeyTypes.Foreign && entity.Attributes.Any(x => x.KeyType == KeyTypes.Foreign && x.AssociatedEntity == auxAttribute.AssociatedEntity))
                    {
                        Warning.Text = "This entity already contains Foreign Key from the selected Entity";
                        args.Cancel = true;
                        return;
                    }


                    auxAttribute.CopyTo(attribute);




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
            
            Entity entity = (AsociatedEntity.SelectedItem as Entity);

            if (entity != null)
            {
                if (entity.Key != null)
                {
                    auxAttribute.AssociatedEntity = entity.Name;
                    auxAttribute.DataType = entity.Key.DataType;
                }
                else
                {
                    AsociatedEntity.SelectedItem = null;
                    Warning.Text = "Please select an entity with a Primary Key";

                }
            }
        }
        
    }
}
