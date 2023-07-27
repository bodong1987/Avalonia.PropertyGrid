# Avalonia.PropertyGrid
This is a PropertyGrid implementation for Avalonia, you can use it in Avalonia Applications.  
Its main features are:  
* Support automatic analysis of class's Properties like DevExpress's PropertyGridControl and display and edit
* Support simultaneous editing of multiple objects in one PropertyGrid
* Support custom types that implement the ICustomTypeDescriptor interface
* Support array editing, support for creating, inserting, deleting and clearing in the control
* Support data verification
* Support Built-in undo and redo framework
* Support for automatically adjusting the visibility of properties based on conditions
* Support path picking
* Support two display modes: category-based and alphabetical sorting  
* Support text filtering, regular expression filtering, and supports ignoring case settings  
* Support fast filtering by Category
* Support data automatic reloading
* Support automatic expansion of sub-objects
* Support adjust the width of the property name and property value by drag the title
* Support localization
* Support custom property cell editors

## How To Use
Use the source code of this project directly or add packages from nuget(https://www.nuget.org/packages/bodong.Avalonia.PropertyGrid).  
Then add PropertyGrid to your project, and bind the object to be displayed and edited to the SelectObject property. If you want to bind multiple objects, just bind IEnumerable<T> directly

## Detail Description
### Data Modeling
If you want to edit an object in PropertyGrid, you only need to directly set this object to the SelectedObject property of PropertyGrid, PropertyGrid will automatically analyze the properties that can support editing, and edit it with the corresponding CellEdit. At the same time, you can also use Attributes in System.ComponentModel and System.ComponentModel.DataAnnotations to mark these properties, so that these properties have some special characteristics.  
Support but not limited to these:
```
System.ComponentModel.CategoryAttribute
System.ComponentModel.BrowsableAttribute
System.ComponentModel.ReadOnlyAttribute
System.ComponentModel.DisplayNameAttribute
System.ComponentModel.DescriptionAttribute
System.ComponentModel.PasswordPropertyTextAttribute
System.ComponentModel.DataAnnotations.EditableAttribute
System.ComponentModel.DataAnnotations.RangeAttribute  
```
In addition, there are other classes that can be supported in Avalonia.PropertyGrid.Model.ComponentModel and Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations, which can assist in describing class properties.  
If you want to have some associations between your class properties, for example, some properties depend on other properties in implementation, then you can try to mark this dependency with Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations.DependsOnPropertyAttribute  
but you need to inherit your class from Avalonia.PropertyGrid.Model.ComponentModel.ReactiveObject, otherwise you need to maintain this relationship by yourself, just trigger the PropertyChanged event of the target property when the dependent property changes.  

```
Avalonia.PropertyGrid.Model.ComponentModel.FloatPrecisionAttribute  
Avalonia.PropertyGrid.Model.ComponentModel.IntegerIncrementAttribute
Avalonia.PropertyGrid.Model.ComponentModel.WatermarkAttribute
Avalonia.PropertyGrid.Model.ComponentModel.MultilineTextAttribute
Avalonia.PropertyGrid.Model.ComponentModel.ProgressAttribute
Avalonia.PropertyGrid.Model.ComponentModel.TrackableAttribute
Avalonia.PropertyGrid.Model.ComponentModel.EnumDisplayNameAttribute
Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations.DependsOnPropertyAttribute
Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations.FileNameValidationAttribute
Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations.PathBrowsableAttribute
Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations.VisibilityPropertyConditionAttribute
```

### Supported Builtin Types
    bool  
    bool?  
    sbyte  
    byte  
    short  
    ushort  
    int  
    uint  
    Int64  
    UInt64  
    float  
    double  
    string  
    enum/[Flags]enum  
    System.ComponentModel.BindingList<>  
    System.DateTime/System.DateTimeOffset/System.DateTime?/System.DateTimeOffset?  
    System.TimeSpan/System.TimeSpan?      
    System.Drawing.Color/Avalonia.Media.Color  
    System.Drawing.Image/Avalonia.Media.IImage
    Avalonia.Media.FontFamily      
    Avalonia.PropertyGrid.Model.Collections.ICheckedList  
    Avalonia.PropertyGrid.Model.Collections.ISelectableList  
    object which support TypeConverter.CanConvertFrom(typeof(string))  

**Struct properties are not supported.**  

### Extra Data Structure
* Avalonia.PropertyGrid.Model.Collections.SelectableList<T>  
    You can initialize this list with some objects, and you can only select one object in this list. ProeprtyGrid uses ComboBox by default to edit the properties of this data structure
* Avalonia.PropertyGrid.Model.Collections.CheckedList<T>
    like SelectableList<T>, you can initialize it with some objects, but you can select multiple objects in it. ProeprtyGrid uses a set of CheckBoxes by default to edit the properties of this data structure, for example:
    ![CheckList](./Docs/Images/CheckList.png)

### Data Reloading
Implement from System.ComponentModel.INotifyPropertyChanged and trigger the PropertyChanged event when the property changes. PropertyGrid will listen to these events and automatically refresh the view data.  
if you implementing from Avalonia.PropertyGrid.Model.ComponentModel.INotifyPropertyChanged instead of System.ComponentModel.INotifyPropertyChanged will gain the additional ability to automatically fire the PropertyChanged event when an edit occurs in the PropertyGrid without having to handle each property itself.

### Change Size
You can change the width of namelabel and cell edit by drag here:
![Dragging](./Docs/Images/ChangeSize.png)
Or set the NameWidth property of PropertyGrid directly.  

### Multiple Objects Edit

If you want to edit multiple objects at the same time, you only need to set the object to SelectedObject as IEnumerable, for example:

```C#
public IEnumerable<SimpleObject> multiObjects => new SimpleObject[] { multiObject0, multiObject1 };
```
```xml
<pgc:PropertyGrid x:Name="propertyGrid_MultipleObjects" Margin="2" SelectedObject="{Binding multiObjects}"></pgc:PropertyGrid>
```
**Due to complexity considerations, there are many complex types of multi-object editing that are not supported!!!**

### ICustomTypeDescriptor
You can find usage examples directly in Samples

### Array Support
PropertyGrid supports array editing. The array properties here can only be declared using BindingList. Setting **[Editable(false)]** can disable the creation and deletion functions, which is consistent with the behavior of Array. In addition, in order to support creation functions, **the template parameters of BindingList can only be non-pure virtual classes.**   
**Struct properties are not supported.**

### Expand Class Properties
When PropertyGrid does not provide a built-in CellEdit to edit the target property, there are several possibilities:
1. If the property or the PropertyType of property is marked with TypeConverter, then the PropertyGrid will try to use the TextBox to edit the object. When the text is changed, it will actively try to use TypeConverter to convert the string into the target object.
2. If the property uses ExpandableObjectConverter, then PropertyGrid will try to expand the object in place.
3. If neither is satisfied, then PropertyGrid will try to use a read-only TextBox to display the ToString() value of the target property.

```C#
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class EncryptData : MiniReactiveObject
    {
        public EncryptionPolicy Policy { get; set; } = EncryptionPolicy.RequireEncryption;

        public RSAEncryptionPaddingMode PaddingMode { get; set; } = RSAEncryptionPaddingMode.Pkcs1;
    }

    #region Expandable
    [DisplayName("Expand Object")]
    [Category("Expandable")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public LoginInfo loginInfo { get; set; } = new LoginInfo();
    #endregion
```

### Data Validation
There are two ways to provide data validation capabilities:
1. Throw an exception directly in the setter of the property. But I personally don't recommend this method very much, because if you set this property in the code, it may cause errors by accident. like:

```C#
    string _SourceImagePath;
    [Category("DataValidation")]
    [PathBrowsable(Filters = "Image Files(*.jpg;*.png;*.bmp;*.tag)|*.jpg;*.png;*.bmp;*.tag")]
    public string SourceImagePath
    {
        get => _SourceImagePath;
        set
        {
            if (value.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(SourceImagePath));
            }

            if (!System.IO.Path.GetExtension(value).iEquals(".png"))
            {
                throw new ArgumentException($"{nameof(SourceImagePath)} must be .png file.");
            }

            _SourceImagePath = value;
        }
    }
```
2. The second method is to use System.ComponentModel.DataAnnotations.ValidationAttribute to mark the target property, both system-provided and user-defined. for example:
```C#
    public class ValidatePlatformAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is CheckedList<PlatformID> id)
            {
                if (id.Contains(PlatformID.Unix) || id.Contains(PlatformID.Other))
                {
                    return new ValidationResult("Can't select Unix or Other");
                }
            }

            return ValidationResult.Success;
        }
    }

    [Category("DataValidation")]
    [Description("Select platforms")]
    [ValidatePlatform]
    public CheckedList<PlatformID> Platforms { get; set; } = new CheckedList<PlatformID>(Enum.GetValues(typeof(PlatformID)).Cast<PlatformID>());

    [Category("Numeric")]
    [Range(10, 200)]
    public int iValue { get; set; } = 100;

    [Category("Numeric")]
    [Range(0.1f, 10.0f)]
    public float fValue { get; set; } = 0.5f;
    
    [Category("Numeric")]
    [Range(0.1f, 10.0f)]
    [FloatPrecision(3)]
    public float fValuePrecision { get; set; } = 0.5f;
```

### Dynamic Visibilty
By setting Attribute, you can make certain Properties only displayed when conditions are met. for example: 
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
In this example, you can check IsShowPath first, then set the Platform to Unix, and then enter something in UnixVersion, and you will see the unixLoginInfo field.
To do this, you only need to mark the property with a custom Attribute. If you need to implement your own rules, just implement your own rules from.  


***The implementation behind this depends on IReactiveObject in Avalonia.PropertyGrid.Model, you can implement it yourself, or directly derive your Model from ReactiveObject.***

**AbstractVisiblityConditionAttribute**.  
One thing to pay special attention to is **that any property that needs to be used as a visibility condition for other properties needs to be marked with [ConditionTarget].**   
The purpose is to let PropertyGrid know that when this property changes, it needs to notify the upper layer to refresh the visibility information.

### User Localization
Implement your Avalonia.PropertyGrid.Model.Services.ILocalizationService class, and register its instance by :
```C#
    LocalizationService.Default.AddExtraService(new YourLocalizationService());
```
If you want to provide the corresponding language pack for the built-in text, please add the corresponding file to Avalonia.PropertyGrid/Assets/Localizations, and name it with the CultureInfo.Name of the language. for example:  
```
    en-US.json
    ru-RU.json
    zh-CN.json
```

### Custom Cell Edit
To customize CellEdit, you need to implement a Factory class from AbstractCellEditFactory, and then append this class instance to PropertyGrid.FactoryTemplates, such as:
```C#
   public class ToggleSwitchExtensionPropertyGrid : Controls.PropertyGrid
    {
        static ToggleSwitchExtensionPropertyGrid()
        {
            CellEditFactoryService.Default.AddFactory(new ToggleSwitchCellEditFactory());
        }
    }

    class ToggleSwitchCellEditFactory : AbstractCellEditFactory
    {
        // make this extend factor only effect on ToggleSwitchExtensionPropertyGrid
        public override bool Accept(object accessToken)
        {
            return accessToken is ToggleSwitchExtensionPropertyGrid;
        }

        public override Control HandleNewProperty(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;
            
            if (propertyDescriptor.PropertyType != typeof(bool))
            {
                return null;
            }

            ToggleSwitch control = new ToggleSwitch();
            control.IsCheckedChanged += (s, e) =>
            {
                SetAndRaise(context, control, control.IsChecked);
            };

            return control;
        }

        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;
            var control = context.CellEdit;

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
There are only two methods that must be overridden:   
HandleNewProperty is used to create the control you want to edit the property, 
You need to pass the value out through the interface of the framework after the UI edits the data, so as to ensure that other related objects receive the message notification and save the undo redo command.  
HandleProeprtyChanged method is used to synchronize external data. When the external data changes, the data is reacquired and synchronized to the control.
AbstractCellEditFactory also has a overrideable property ImportPriority. This value determines the order in which the PropertyGrid triggers these Factories. The larger the value, the earlier the trigger.   
Overriding the Accept method allows your Factory to only take effect when appropriate.

*** 
You can also use this method to extend PropertyGrid so that you can edit types that are not supported by the built-in functionality.


## Description of Samples
![Basic View](./Docs/Images/BasicView.png)
You can clone this project, and open Avalonia.PropertyGrid.sln, build it and run Avalonia.PropertyGrid.Samples, you can view this.

### Basic
This page shows the basic functions of PropertyGrid, including the display of various properties and the default editor, etc.  

### Styles
![Styles](./Docs/Images/Styles.png)
Test all adjustable appearance properties.

### DataSync
![DataSync](./Docs/Images/data-sync.png)
Here you can verify data changes and auto-reload functionality.

### MultiObjects
![Multi-Views](./Docs/Images/Multi-Objects.png)
You can verify the function of multi-object editing here. Note:   
  
**some properties do not support editing multiple objects at the same time.**

### CustomObject
![CustomObject](./Docs/Images/CustomObject.png)
Here shows how to create a custom object based on ICustomTypeDescriptor.

### Custom Cell Edit
![CustomCellEdit](./Docs/Images/CustomCellEdit.png)
By default, PropertyGrid uses CheckBox to edit Boolean data, here shows how to use ToggleSwitch to edit Boolean data in a simple way, and how to make this function only effective locally without affecting the whole.

### Extends
![Extends](./Docs/Images/extends.png)
Under normal circumstances, PropertyGrid does not automatically handle structure properties, because structures have certain particularities. To support such internally unsupported types, you need to extend PropertyGrid yourself. This example shows how to support and edit the structure SVector3:
```C#
namespace Avalonia.PropertyGrid.Samples.Models
{
    public struct SVector3
    {
        public float x, y, z;

        public override string ToString()
        {
            return string.Format("{0:0.0}, {1:0.0}, {2:0.0}", x, y, z);
        }
    }

    public class TestExtendsObject : MiniReactiveObject
    {
        [Category("Struct")]
        public Vector3 vec3Object { get; set; } = new Vector3();

        [Category("Struct")]
        public SVector3 vec3Struct { get; set; }

        [Category("Struct")]
        public BindingList<SVector3> vec3BindingList { get; set; } = new BindingList<SVector3>()
        {
            new SVector3(){ x = 7.8f, y = 3.14f, z = 0.0f }
        };
                
        [Category("SelectableList")]
        public SelectableList<CountryInfo> Countries { get; set; }


        public TestExtendsObject()
        {
            List<CountryInfo> list = new List<CountryInfo>();

            var assets = AssetLoader.GetAssets(new Uri($"avares://{GetType().Assembly.GetName().Name}/Assets/country-flags"), null);
            foreach (var asset in assets)
            {
                list.Add(new CountryInfo(asset));
            }

            Countries = new SelectableList<CountryInfo>(list, list.Find(x => x.Code == "cn"));
        }
    }

    // ignore some codes
}
```
There is also an example of SelectableList customization for reference.  
More details can be seen in the file TestExtendPropertyGrid.cs.  

### Dynamic Visibility
![DynamicVisibility](./Docs/Images/DynamicVisibility.png)
Show Dynamic Visibility  
If you check 'IsShowPath', the Path can be edited.  
If you select Unix in Platform and input anything in UnixVersion, you can edit the extra properties.

### RodoUndo
![RedoUndo](./Docs/Images/undoredo.png)
This example shows how to implement undo and redo functions based on the built-in undo-redo framework.

### Self's Properties
![Self's Properties](./Docs/Images/self-properties.png)
Show PropertyGrid's properties.  

## Avalonia.PropertyGrid.NugetSamples
This example shows how to use PropertyGrid through the Nuget package. 

