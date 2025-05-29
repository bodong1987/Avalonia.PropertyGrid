using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;
using Avalonia.PropertyGrid.Controls.Factories.Builtins;
using Avalonia.PropertyGrid.Localization;
using Avalonia.PropertyGrid.Samples.Models;
using Avalonia.PropertyGrid.Services;
using PropertyModels.Collections;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;

namespace Avalonia.PropertyGrid.Samples.Views
{
    public class TestExtendPropertyGrid : Controls.PropertyGrid
    {
        static TestExtendPropertyGrid()
        {
            CellEditFactoryService.Default.AddFactory(new Vector3CellEditFactory());
            CellEditFactoryService.Default.AddFactory(new CountryInfoCellEditFactory());
            CellEditFactoryService.Default.AddFactory(new ToggleSwitchCellEditFactory());
        }

        private int _customLabelClickCount;

        public TestExtendPropertyGrid()
        {
            CustomNameBlock += (s, e) =>
            {
                if(e.Context.Property.Name == nameof(TestExtendsObject.customLabel))
                {
                    var button = new Button { Content = LocalizationService.Default[e.Context.Property.DisplayName] };
                    
                    button.Click += (ss, ee) =>
                    {
                        button.Content = $"{LocalizationService.Default[e.Context.Property.DisplayName]} {++_customLabelClickCount}";
                    };

                    LocalizationService.Default.OnCultureChanged += (ss, ee) =>
                    {
                        button.Content =
                            $"{LocalizationService.Default[e.Context.Property.DisplayName]} {_customLabelClickCount}";
                    };

                    e.CustomNameBlock = button;
                }                
            };

            CustomPropertyOperationControl += OnCustomPropertyOperationControl;
            CustomPropertyOperationMenuOpening += OnCustomPropertyOperationMenuOpening;
        }

        private static void OnCustomPropertyOperationMenuOpening(object? sender, CustomPropertyDefaultOperationEventArgs e)
        {
            if (!e.Context.Property.IsDefined<PropertyOperationVisibilityAttribute>())
            {
                return;
            }

            if (e.Context.Property.Name == nameof(TestExtendsObject.CustomOperationMenuNumber))
            {
                if (e.StageType == PropertyDefaultOperationStageType.Init)
                {
                    e.DefaultButton.SetLocalizeBinding(ContentProperty, "Operation");    
                }
                else if (e.StageType == PropertyDefaultOperationStageType.MenuOpening)
                {
                    // If you don't want to create the menu every time, you can move it to Init, so you don't have to Clear it here.
                    e.Menu.Items.Clear();
                        
                    var minMenuItem = new MenuItem();
                    minMenuItem.SetLocalizeBinding(HeaderedSelectingItemsControl.HeaderProperty, "Min");
                    minMenuItem.Click += (s, args) => 
                    {
                        e.Context.Factory!.SetPropertyValue(e.Context, 0);
                    };

                    var maxMenuItem = new MenuItem();
                    maxMenuItem.SetLocalizeBinding(HeaderedSelectingItemsControl.HeaderProperty, "Max");
                    maxMenuItem.Click += (s, args) => 
                    {
                        e.Context.Factory!.SetPropertyValue(e.Context, 1024); 
                    };
                
                    var errorMenuItem = new MenuItem();
                    errorMenuItem.SetLocalizeBinding(HeaderedSelectingItemsControl.HeaderProperty, "GenError");
                    errorMenuItem.Click += (s, args) =>
                    {
                        e.Context.Factory!.SetPropertyValue(e.Context, 1024000);
                    };

                    e.Menu.Items.Add(minMenuItem);
                    e.Menu.Items.Add(maxMenuItem);
                    e.Menu.Items.Add(errorMenuItem);
                }
            }
        }

        private static void OnCustomPropertyOperationControl(object? sender, CustomPropertyOperationControlEventArgs e)
        {
            if (!e.Context.Property.IsDefined<PropertyOperationVisibilityAttribute>())
            {
                return;
            }

            if (e.Context.Property.Name == nameof(TestExtendsObject.CustomOperationControlNumber))
            {
                var stackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                var minButton = new Button();
                minButton.SetLocalizeBinding(ContentProperty, "Min");
                minButton.Click += (ss, ee) =>
                {
                    // please use factory interface, so you can raise command event 
                    e.Context.Factory!.SetPropertyValue(e.Context, 0);
                };
                
                stackPanel.Children.Add(minButton);
                
                var maxButton = new Button();
                maxButton.SetLocalizeBinding(ContentProperty, "Max");
                maxButton.Click += (ss, ee) =>
                {
                    // please use factory interface, so you can raise command event 
                    e.Context.Factory!.SetPropertyValue(e.Context, 1024);
                };

                stackPanel.Children.Add(maxButton);

                e.CustomControl = stackPanel;
            }
        }
    }

    #region SVector3
    public class SVector3ViewModel : MiniReactiveObject
    {
        public SVector3 Vec;

        public float X
        {
            get => Vec.x;
            set => this.RaiseAndSetIfChanged(ref Vec.x, value);
        }

        public float Y
        {
            get => Vec.y;
            set => this.RaiseAndSetIfChanged(ref Vec.y, value);
        }

        public float Z
        {
            get => Vec.z;
            set => this.RaiseAndSetIfChanged(ref Vec.z, value);
        }
    }

    internal class Vector3CellEditFactory : AbstractCellEditFactory
    {
        public override bool Accept(object accessToken) => accessToken is TestExtendPropertyGrid;

        public override Control? HandleNewProperty(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            _ = context.Target;

            if (propertyDescriptor.PropertyType != typeof(SVector3))
            {
                return null;
            }

            var control = new Vector3View();

            return control;
        }

        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;
            var control = context.CellEdit!;

            if (propertyDescriptor.PropertyType != typeof(SVector3))
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            if (control is Vector3View vv)
            {
                var vec = (SVector3)propertyDescriptor.GetValue(target)!;

                var model = new SVector3ViewModel { Vec = vec };
                vv.DataContext = model;

                model.PropertyChanged += (s, e) => SetAndRaise(context, control, model.Vec);

                return true;
            }

            return false;
        }
    }
    #endregion

    #region Countries
    internal class CountryInfoCellEditFactory : SelectableListCellEditFactory
    {
        public override int ImportPriority => base.ImportPriority + 100;

        public override bool Accept(object accessToken) => accessToken is TestExtendPropertyGrid;

        public override Control? HandleNewProperty(PropertyCellContext context)
        {
            if (context.Property.PropertyType != typeof(SelectableList<CountryInfo>))
            {
                return null;
            }

            var control = base.HandleNewProperty(context);

            if (control is ComboBox cb)
            {
                cb.ItemTemplate = new FuncDataTemplate<CountryInfo>((value, _) => new CountryView());
            }

            return control;
        }
    }
    #endregion

    #region Bool
    internal class ToggleSwitchCellEditFactory : AbstractCellEditFactory
    {
        // make this extend factor only effect on TestExtendPropertyGrid
        public override bool Accept(object accessToken) => accessToken is TestExtendPropertyGrid;

        public override Control? HandleNewProperty(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            // var target = context.Target;

            if (propertyDescriptor.PropertyType != typeof(bool))
            {
                return null;
            }

            var control = new ToggleSwitch();
            control.SetLocalizeBinding(ToggleSwitch.OnContentProperty, "On");
            control.SetLocalizeBinding(ToggleSwitch.OffContentProperty, "Off");

            control.IsCheckedChanged += (s, e) => SetAndRaise(context, control, control.IsChecked);

            return control;
        }

        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;
            var control = context.CellEdit!;

            if (propertyDescriptor.PropertyType != typeof(bool))
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            if (control is ToggleSwitch ts)
            {
                ts.IsChecked = (bool)propertyDescriptor.GetValue(target)!;

                return true;
            }

            return false;
        }
    }

    #endregion
}
