# Avalonia.PropertyGrid
This is a PropertyGrid implementation for Avalonia, you can use it in Avalonia Applications.  
Its main features are:  
* Support automatically analyze object properties and display, like WinForms's PropertyGrid
* Support simultaneous editing of multiple objects (some types of properties cannot support simultaneous 
editing of multiple objects)  
* Support ICustomTypeDescriptor
* Support array editing, support dynamic addition and deletion. Arrays of objects of any type are supported. The array mentioned here is just a BindingList<> object
* Support data verification. You can directly throw an exception in the property setter to report a data error, and of course you can add validation to the property based on System.ComponentModel.DataAnnotations. The system also automatically analyzes and processes these checks.
* Supports dynamic visibility, that is, whether a property is visible can be dynamically determined by another property's value
* Supports two display modes: category-based and alphabetical sorting  
* Supports text filtering, regular expression filtering, and supports ignoring case settings  
* Support data automatic reloading, when data is modified from the outside or other PropertyGrid, it will automatically refresh the data, but the target object needs to implement the interface INotifyPropertyChanged
* Support automatic expansion of sub-objects, but need to mark attributes [TypeConverter(typeof(ExpandableObjectConverter))]
* Support to adjust the width of the property name and property value by dragging and dropping, and the optimal width will be automatically calculated when the SelectObject is set for the first time  
* Support localization, you can override built in Localization Service by reset PropertyGrid.LocalizationService interface
* The extension is very simple, you can use very simple code at any time to make the PropertyGrid support the type of editing that is not originally supported. The system has built-in support for some common property editing, but it is not complete, so when you need to edit a specific object through a special control, you need to expand it yourself, and this step is very simple.



## How To Use
Use the source code of this warehouse directly or add packages from nuget(https://www.nuget.org/packages/bodong.Avalonia.PropertyGrid).  
Then add PropertyGrid to your project, and bind the object to be displayed and edited to the SelectObject property. If you want to bind multiple objects, just bind IEnumerable<T> directly

## Samples
![Basic View](./Docs/Images/BasicView.png)
You can clone this project, and open Avalonia.PropertyGrid.sln, build it and run Avalonia.PropertyGrid.Samples, you can view this.

### Basic
This page shows the basic functions of PropertyGrid, including the display of various properties and the default editor, etc.  

### Views
![Views](./Docs/Images/Views.png)
You can see the PropertyGrid with filtering and grouping by category turned off. Just change Properties:
```xml
<StackPanel Orientation="Vertical">
    <TextBlock>Default View</TextBlock>
    <pgc:PropertyGrid Margin="4" SelectedObject="{Binding simpleObject}"></pgc:PropertyGrid>
</StackPanel>
<StackPanel Orientation="Vertical" Grid.Column="2">
    <TextBlock>Not Allow Filter</TextBlock>
    <pgc:PropertyGrid Margin="4" AllowFilter="False" SelectedObject="{Binding simpleObject}"></pgc:PropertyGrid>
</StackPanel>
<StackPanel Orientation="Vertical" Grid.Column="4">
    <TextBlock>Not Allow Filter And No Categories</TextBlock>
    <pgc:PropertyGrid Margin="4" AllowFilter="False" ShowStyle="Alphabetic" SelectedObject="{Binding simpleObject}"></pgc:PropertyGrid>
</StackPanel>
```

### DataSync
Here you can verify data changes and auto-reload functionality.

### MultiObjects
You can verify the function of multi-object editing here. Note:   
**some properties do not support editing multiple objects at the same time.**

### CustomObject
![CustomObject](./Docs/Images/CustomObject.png)
Here shows how to create a custom object based on ICustomTypeDescriptor.

### Custom Cell Edit
![CustomCellEdit](./Docs/Images/CustomCellEdit.png)
By default, PropertyGrid uses CheckBox to edit Boolean data, here shows how to use ToggleSwitch to edit Boolean data in a simple way, and how to make this function only effective locally without affecting the whole.
This is the Custom Cell Edit's Codes:
```C#
    // create a child property
    public class ToggleSwitchExtensionPropertyGrid : Controls.PropertyGrid
    {
        static ToggleSwitchExtensionPropertyGrid()
        {
            FactoryTemplates.AddFactory(new ToggleSwitchCellEditFactory());
        }
    }

    class ToggleSwitchCellEditFactory : AbstractCellEditFactory
    {
        // make this extend factor only effect on ToggleSwitchExtensionPropertyGrid
        public override bool Accept(object accessToken)
        {
            return accessToken is ToggleSwitchExtensionPropertyGrid;
        }

        public override Control HandleNewProperty(object target, PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.PropertyType != typeof(bool))
            {
                return null;
            }

            ToggleSwitch control = new ToggleSwitch();
            control.Checked += (s, e) => { SetAndRaise(control, propertyDescriptor, target, true); };
            control.Unchecked += (s, e) => { SetAndRaise(control, propertyDescriptor, target, false); };

            return control;
        }

        public override bool HandlePropertyChanged(object target, PropertyDescriptor propertyDescriptor, Control control)
        {
            if (propertyDescriptor.PropertyType != typeof(bool))
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            if (control is ToggleSwitch ts)
            {
                ts.IsChecked = (bool)propertyDescriptor.GetValue(target);

                return true;
            }

            return false;
        }
    }
```

### Dynamic Visibility
![DynamicVisibility](./Docs/Images/DynamicVisibility.png)
PropertyGrid supports dynamic visibility adjustment of properties. For example, you can let users see a certain property only when a certain property is True, otherwise it is invisible.  
In this example, you can check IsShowPath first, then set the Platform to Unix, and then enter something in UnixVersion, and you will see the unixLoginInfo field.

```C#
    public class DynamicVisibilityObject : ReactiveObject
    {
        [ConditionTarget]
        public bool IsShowPath { get; set; } = true;

        [VisibilityPropertyCondition(nameof(IsShowPath), true)]
        [PathBrowsable(Filters = "Image Files(*.jpg;*.png;*.bmp;*.tag)|*.jpg;*.png;*.bmp;*.tag")]
        public string Path { get; set; } = "";

        [ConditionTarget]
        public PlatformID Platform { get; set; } = PlatformID.Win32NT;

        [VisibilityPropertyCondition(nameof(Platform), PlatformID.Unix)]
        [ConditionTarget]
        public string UnixVersion { get; set; } = "";

        // show more complex conditions...
        [Browsable(false)]
        [DependsOnProperty(nameof(IsShowPath), nameof(Platform), nameof(UnixVersion))]
        [ConditionTarget]
        public bool IsShowUnixLoginInfo => IsShowPath && Platform == PlatformID.Unix && UnixVersion.IsNotNullOrEmpty();

        [VisibilityPropertyCondition(nameof(IsShowUnixLoginInfo), true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public LoginInfo unixLogInInfo { get; set; } = new LoginInfo();
    }
```
To do this, you only need to mark the property with a custom Attribute. If you need to implement your own rules, just implement your own rules from **AbstractVisiblityConditionAttribute**.  
One thing to pay special attention to is **that any property that needs to be used as a visibility condition for other properties needs to be marked with [ConditionTarget].**   
The purpose is to let PropertyGrid know that when this property changes, it needs to notify the upper layer to refresh the visibility information.

## Avalonia.PropertyGrid.NugetSamples
This example shows how to use PropertyGrid through the Nuget package. Its content is similar to the Sample that directly uses the source code, and it can also be used as a case for learning how to use it.  

**My Blog: https://www.cnblogs.com/bodong**  
**如果你来自中国或者认识中文，可以在这里获取更多相关信息:**  
    https://www.cnblogs.com/bodong/p/17342817.html


