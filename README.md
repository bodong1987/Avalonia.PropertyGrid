# Avalonia.PropertyGrid
This is a PropertyGrid implementation for Avalonia, you can use it in Avalonia Applications.  
Its main features are:  
* Support automatic analysis of class's Properties like DevExpress's PropertyGridControl and display and edit
* Support simultaneous editing of multiple objects in one PropertyGrid
* Support custom types that implement the ICustomTypeDescriptor interface or TypeDescriptionProvider
* Support array editing, support for creating, inserting, deleting and clearing in the control
* Support data verification
* Support Built-in undo and redo framework
* Support global `ReadOnly` setting
* Support for automatically adjusting the visibility of properties based on conditions
* Support path picking
* Support three display modes: category-based, alphabetical sorting and builtin sorting  
* Support text filtering, regular expression filtering, and supports ignoring case settings  
* Support fast filtering by Category
* Support data automatic reloading
* Support automatic expansion of sub-objects
* Support adjust the width of the property name and property value by drag the title
* Support localization and realtime change language without restart
* Support custom property cell editors

## How To Use
Use the source code of this project directly or use NUGet Packages:  
    https://www.nuget.org/packages/bodong.Avalonia.PropertyGrid   
Then add PropertyGrid to your project, and bind the object to be displayed and edited to the `DataContext` property. If you want to bind multiple objects, just bind IEnumerable<T> directly

## Detail Description
### Data Modeling
If you want to edit an object in PropertyGrid, you only need to directly set this object to the `DataContext` property of PropertyGrid, PropertyGrid will automatically analyze the properties that can support editing, and edit it with the corresponding CellEdit. At the same time, you can also use Attributes in System.ComponentModel and System.ComponentModel.DataAnnotations to mark these properties, so that these properties have some special characteristics.  
Support but not limited to these:
```
System.ComponentModel.CategoryAttribute                     /* set property category */  
System.ComponentModel.BrowsableAttribute                    /* used to hide a property */    
System.ComponentModel.ReadOnlyAttribute                     /* make property readonly */  
System.ComponentModel.DisplayNameAttribute                  /* set friendly name */  
System.ComponentModel.DescriptionAttribute                  /* set long description text */  
System.ComponentModel.PasswordPropertyTextAttribute         /* mark text property is password */  
System.ComponentModel.DataAnnotations.EditableAttribute     /* mark list property can add/remove/clear elements */  
System.ComponentModel.DataAnnotations.RangeAttribute        /* set numeric range */  
System.Runtime.Serialization.IgnoreDataMemberAttribute      /* used to hide a property */  
```
In addition, there are other classes that can be supported in PropertyModels.ComponentModel and PropertyModels.ComponentModel.DataAnnotations, which can assist in describing class properties.  
If you want to have some associations between your class properties, for example, some properties depend on other properties in implementation, then you can try to mark this dependency with PropertyModels.ComponentModel.DataAnnotations.DependsOnPropertyAttribute  
but you need to inherit your class from PropertyModels.ComponentModel.ReactiveObject, otherwise you need to maintain this relationship by yourself, just trigger the PropertyChanged event of the target property when the dependent property changes.  

```C# 
PropertyModels.ComponentModel.FloatPrecisionAttribute                               /* set float percision */  
PropertyModels.ComponentModel.IntegerIncrementAttribute                             /* set integer increment by button*/  
PropertyModels.ComponentModel.WatermarkAttribute                                    /* set water mark, it is text hint*/  
PropertyModels.ComponentModel.MultilineTextAttribute                                /* make text edit can edit multi line text */  
PropertyModels.ComponentModel.ProgressAttribute                                     /* use progress bar to dipslay numeric value property, readonly */   
PropertyModels.ComponentModel.TrackableAttribute                                    /* use trackbar to edit numeric value property */  
PropertyModels.ComponentModel.EnumDisplayNameAttribute                              /* set friendly name for each enum vlaues */
PropertyModels.ComponentModel.EnumExcludeAttribute                                  /* Globally prohibit an enumerated field from appearing in the PropertyGrid. To configure it for a single property, you can use EnumPermitValuesAttribute or EnumProhibitValuesAttribute. */
PropertyModels.ComponentModel.AutoCollapseCategoriesAttribute                       /* By configuring this Attribute for a class, some Categories can be automatically collapsed in the initial state. */
PropertyModels.ComponentModel.DataAnnotations.DependsOnPropertyAttribute            /* mark this property is depends on the other property */  
PropertyModels.ComponentModel.DataAnnotations.FileNameValidationAttribute           /* mark this property is filename, so control will validate the string directly */  
PropertyModels.ComponentModel.DataAnnotations.PathBrowsableAttribute                /* mark string property is path, so it will provide a button to show path browser*/  
PropertyModels.ComponentModel.DataAnnotations.VisibilityPropertyConditionAttribute  /* set this property will auto refresh all visiblity when this proeprty value changed. */  
PropertyModels.ComponentModel.DataAnnotations.EnumPermitValuesAttribute<T>          /* For a single attribute configuration, force the allowed enumeration values ​​to be set */
PropertyModels.ComponentModel.DataAnnotations.EnumProhibitValuesAttribute<T>        /* For individual attribute configurations, certain enumeration values ​​are forcibly prohibited from appearing in the candidate list */
PropertyModels.ComponentModel.DataAnnotations.ImagePreviewModeAttribute             /* set image display mode */
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
    Avalonia.Media.IImage
    Avalonia.Media.FontFamily      
    PropertyModels.Collections.ICheckedList  
    PropertyModels.Collections.ISelectableList  
    object which support TypeConverter.CanConvertFrom(typeof(string))  

**By default, structure properties are not supported. All structure properties need to be customized before they can be displayed.**  

### Extra Data Structure
* PropertyModels.Collections.SelectableList<T>  
    You can initialize this list with some objects, and you can only select one object in this list. ProeprtyGrid uses ComboBox by default to edit the properties of this data structure
* PropertyModels.Collections.CheckedList<T>
    like SelectableList<T>, you can initialize it with some objects, but you can select multiple objects in it. ProeprtyGrid uses a set of CheckBoxes by default to edit the properties of this data structure, for example:
    ![CheckList](./Docs/Images/CheckList.png)

### Data Reloading
Implement from System.ComponentModel.INotifyPropertyChanged and trigger the PropertyChanged event when the property changes. PropertyGrid will listen to these events and automatically refresh the view data.  
if you implementing from PropertyModels.ComponentModel.INotifyPropertyChanged instead of System.ComponentModel.INotifyPropertyChanged will gain the additional ability to automatically fire the PropertyChanged event when an edit occurs in the PropertyGrid without having to handle each property itself.  
You can also directly inherit PropertyModel.ComponentModel.MiniReactiveObject, PropertyModel.ComponentModel.ReactiveObject. The former only has data change notification capabilities, while the latter also has data dynamic visibility refresh support. If you use ReactiveUI.ReactiveObject directly, then you will not have dynamic visibility support. At this time, you need to monitor the relevant properties yourself, rather than using the RaisePropertyChanged method to throw the corresponding property change event.

### Custom Property Filter
```xml
<pgc:PropertyGrid 
    x:Name="propertyGrid_Basic" 
    Margin="4" 
    CustomPropertyDescriptorFilter="OnCustomPropertyDescriptorFilter"
    DataContext="{Binding simpleObject}"
    >
</pgc:PropertyGrid>
```  
set CustomPropertyDescriptorFilter, and add your custom process.  
```C#
private void OnCustomPropertyDescriptorFilter(object? sender, CustomPropertyDescriptorFilterEventArgs e)
{
	if(e.TargetObject is SimpleObject simpleObject&& e.PropertyDescriptor.Name == "ThreeStates2")
    {
        e.IsVisible = true;
        e.Handled = true;
    }
}
```  
check MainDemoView.axaml.cs for more information.


### Change Size
You can change the width of namelabel and cell edit by drag here:
![Dragging](./Docs/Images/ChangeSize.png)
Or set the NameWidth property of PropertyGrid directly.  

### Multiple Objects Edit

If you want to edit multiple objects at the same time, you only need to set the object to DataContext as IEnumerable, for example:

```C#
public IEnumerable<SimpleObject> multiObjects => new SimpleObject[] { multiObject0, multiObject1 };
```
```xml
<pgc:PropertyGrid x:Name="propertyGrid_MultipleObjects" Margin="2" DataContext="{Binding multiObjects}"></pgc:PropertyGrid>
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
    string? _SourceImagePath;
    [Category("DataValidation")]
    [PathBrowsable(Filters = "Image Files(*.jpg;*.png;*.bmp;*.tag)|*.jpg;*.png;*.bmp;*.tag")]
    [Watermark("This path can be validated")]
    public string? SourceImagePath
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
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
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


***The implementation behind this depends on IReactiveObject in PropertyModels, you can implement it yourself, or directly derive your Model from ReactiveObject.***

**AbstractVisiblityConditionAttribute**.  
One thing to pay special attention to is **that any property that needs to be used as a visibility condition for other properties needs to be marked with [ConditionTarget].**   
The purpose is to let PropertyGrid know that when this property changes, it needs to notify the upper layer to refresh the visibility information.

### User Localization
Implement your PropertyModels.Services.ILocalizationService class, and register its instance by :
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
To customize CellEdit, you need to implement a Factory class from AbstractCellEditFactory, and then append this class instance to CellEditFactoryService.Default, such as:
```C#
public class TestExtendPropertyGrid : Controls.PropertyGrid
{
    static TestExtendPropertyGrid()
    {
        CellEditFactoryService.Default.AddFactory(new Vector3CellEditFactory());
        CellEditFactoryService.Default.AddFactory(new CountryInfoCellEditFactory());
        CellEditFactoryService.Default.AddFactory(new ToggleSwitchCellEditFactory());
    }
}
```
Refer to Extends in Samples for more information.

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

### Extends
![Extends](./Docs/Images/extends.png)
In custom AbstractCellEditFactory, there are only two methods that must be overridden:  
* HandleNewProperty is used to create the control you want to edit the property, 
You need to pass the value out through the interface of the framework after the UI edits the data, so as to ensure that other related objects receive the message notification and save the undo redo command.  
```C#
    public override Control? HandleNewProperty(PropertyCellContext context)
    {
        var propertyDescriptor = context.Property;
        var target = context.Target;

        if (propertyDescriptor.PropertyType != typeof(bool))
        {
            return null;
        }

        ToggleSwitch control = new ToggleSwitch();
        control.SetLocalizeBinding(ToggleSwitch.OnContentProperty, "On");
        control.SetLocalizeBinding(ToggleSwitch.OffContentProperty, "Off");

        control.IsCheckedChanged += (s, e) =>
        {
            SetAndRaise(context, control, control.IsChecked);
        };

        return control;
    }
```
* HandleProeprtyChanged method is used to synchronize external data. When the external data changes, the data is reacquired and synchronized to the control.
```C#
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
```
* AbstractCellEditFactory also has a overrideable property ImportPriority. This value determines the order in which the PropertyGrid triggers these Factories. The larger the value, the earlier the trigger.   
* Overriding the Accept method allows your Factory to only take effect when appropriate.  
* Override the HandlePropagateVisibility method to customize the filtering scheme

For example:  
Under normal circumstances, PropertyGrid does not automatically handle structure properties, because structures have certain particularities. To support such internally unsupported types, you need to extend PropertyGrid yourself. This example shows how to support and edit the structure SVector3.  
There is also an example of SelectableList customization for reference.  
More details can be seen in the file TestExtendPropertyGrid.cs.  

### Dynamic Visibility
![DynamicVisibility](./Docs/Images/DynamicVisibility.png)
Show Dynamic Visibility  
If you check 'IsShowPath', the Path can be edited.  
If you select Unix in Platform and input anything in UnixVersion, you can edit the extra properties.

### UndoRedo
![RedoUndo](./Docs/Images/undoredo.png)
This example shows how to implement undo and redo functions based on the built-in undo-redo framework.

### Self's Properties
![Self's Properties](./Docs/Images/self-properties.png)
Show PropertyGrid's properties.  

## Avalonia.PropertyGrid.NugetSamples
This example shows how to use PropertyGrid through the Nuget package. 

### Major changes  
v11.0.4.1  
* The data modeling module has been extracted into an independent project (PropertyModels) to facilitate the implementation of a project organization structure that separates data and performance. Therefore, after this version, you need to install two NUGET packages.

v11.0.6.2  
* Set the `SelectedObject` property to obsolete. It is recommended to use the DataContext mechanism directly to provide data to the PropertyGrid;
add custom property visibility filter support.

v11.0.10.1  
* `SelectedObject` property is deleted, please use `DataContext` directly.

v11.1.1.1  

* There is no need to associate the bodong.PropertyModels Nuget package separately. You can automatically reference this package when you reference Avalonia.PropertyGrid. If you need to reference PropertyModels in your own data model project, please ensure that their version numbers are consistent with the version referenced by PropertyGrid.

v11.1.4.1  

* The ShowStyle attribute is split into three: ShowStyle, PropertyOrderStyle, and CategoryOrderStyle. They are respectively used to indicate: whether to display categories, attribute sorting method, and category sorting method.
* ICellEditFactory adds an interface `HandleReadOnlyStateChanged` to notify the processing of ReadOnly tags. You can customize your ReadOnly behavior through this interface. For example, by default ReadOnly is handled by setting IsEnabled to false. If your control supports better read-only effects, you can customize it by overriding this method. For example, when it comes to String and number, the IsReadOnly property is more suitable than IsEnabled because it can better support users to copy the content in the control.

v11.1.4.2  

* Enable Nullable

