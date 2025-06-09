[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/bodong1987/Avalonia.PropertyGrid)
![License](https://img.shields.io/github/license/avaloniaui/avalonia.svg)

# Avalonia.PropertyGrid
[查看简体中文文档](./README-CHS.md)  

This is a PropertyGrid implementation for Avalonia, you can use it in Avalonia Applications.  
Its main features are:  
* Support automatic analysis of class's Properties like DevExpress's PropertyGridControl and display and edit
* Support simultaneous editing of multiple objects in one PropertyGrid
* Support custom types that implement the ICustomTypeDescriptor interface or TypeDescriptionProvider
* Support custom property label controls
* Support custom property cell edit  
* Support for additional property operation areas and their custom operations
* Support collections editing, support for adding, inserting, deleting and clearing in the control
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
* Support web  

<a href="https://bodong1987.github.io/Avalonia.PropertyGrid/" target="_blank"> → View Online Demo</a>  

## How To Use
Use the source code of this project directly or use NUGET Packages:  
    https://www.nuget.org/packages/bodong.Avalonia.PropertyGrid   
Then add PropertyGrid to your project, and bind the object to be displayed and edited to the `DataContext` property. If you want to bind multiple objects, just bind `IEnumerable<T>` directly

## How to Debug
Open `Avalonia.PropertyGrid.sln`, change config to `Development`, set `Avalonia.PropertyGrid.Samples.Desktop` as startup project, build and run it.  

## How to build Online Demo
run `dotnet publish` under `Samples/Avalonia.PropertyGrid.Samples.Browser`. This may end up taking several minutes.    
**Because of the bug in .net 8.0, you must use .net 9.0, otherwise you will encounter errors when running**  

## Detail Description
### Data Modeling
If you want to edit an object in PropertyGrid, you only need to directly set this object to the `DataContext` property of PropertyGrid, PropertyGrid will automatically analyze the properties that can support editing, and edit it with the corresponding `CellEdit`. At the same time, you can also use Attributes in `System.ComponentModel`, `System.ComponentModel.DataAnnotations` and `PropertyModels.ComponentModel` to mark these properties, so that these properties have some special characteristics.  
Support but not limited to these:
```C#
System.ComponentModel.CategoryAttribute                     /* set property category */  
System.ComponentModel.BrowsableAttribute                    /* used to hide a property */    
System.ComponentModel.ReadOnlyAttribute                     /* make property readonly */  
System.ComponentModel.DisplayNameAttribute                  /* set friendly name */  
System.ComponentModel.DescriptionAttribute                  /* set long description text, When you point the mouse to the title, the corresponding Tooltip will appear. */  
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
PropertyModels.ComponentModel.DataAnnotations.PropertyVisibilityConditionAttribute  /* set this property will auto refresh all visiblity when this proeprty value changed. */  
PropertyModels.ComponentModel.DataAnnotations.EnumPermitValuesAttribute<T>          /* For a single attribute configuration, force the allowed enumeration values ​​to be set */
PropertyModels.ComponentModel.DataAnnotations.EnumPermitNamesAttribute              /* based on enum names */
PropertyModels.ComponentModel.DataAnnotations.EnumProhibitValuesAttribute<T>        /* For individual attribute configurations, certain enumeration values ​​are forcibly prohibited from appearing in the candidate list */
PropertyModels.ComponentModel.DataAnnotations.EnumProhibitNamesAttribute            /* based on enum names */
PropertyModels.ComponentModel.DataAnnotations.IEnumValueAuthorizeAttribute          /* create your custom enum value filter based on this interface */
PropertyModels.ComponentModel.DataAnnotations.ImagePreviewModeAttribute             /* set image display mode */
PropertyModels.ComponentModel.DataAnnotations.FloatingNumberEqualToleranceAttribute /* You can use this tag to mark the tolerance value of a floating point number. When the change difference is less than this value, it is considered that there is no change. */  
PropertyModels.ComponentModel.SingleSelectionModeAttribute                          /* set enum/selectablelist view mode, use combobox/togglebutton group/radiobox group */
PropertyModels.ComponentModel.UnitAttribute                                         /* used to display a unit next to the display name */  
PropertyModels.ComponentModel.PropertyOperationVisibilityAttribute                  /* Used to specify whether the additional operation area is visible. The default value is determined by the property. */  
PropertyModels.ComponentModel.SelectableListDisplayModeAttribute                    /* Used to specify the display view category of the selectable list type. The default is horizontal arrangement + automatic line wrap, and it can also be configured to other types. */   
PropertyModels.ComponentModel.ExpandableObjectDisplayModeAttribute                  /* You can control the display mode of the sub-objects to be expanded. Through these configurations, you can control whether the sub-objects display categories, sorting, etc. */  
```

### Supported Builtin Types
```C#
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
    /* All containers and arrays that implement the IList<T> interface support display and editing, but containers that do not implement the IBindingList or INotifyCollectionChanged interface do not support data synchronization. */
    T[] /* Add/Insert/Remove/Clear and datasync is not supported */  
    System.Collections.Generic.List<T> /* datasync is not supported */  
    System.ComponentModel.BindingList<T>  
    System.Collections.ObjectModel.ObservableCollection<T>
    System.DateTime/System.DateTimeOffset/System.DateTime?/System.DateTimeOffset?  
    System.TimeSpan/System.TimeSpan?      
    System.Drawing.Color/Avalonia.Media.Color  
    Avalonia.Media.IImage
    Avalonia.Media.FontFamily      
    PropertyModels.Collections.ICheckedList  
    PropertyModels.Collections.ISelectableList  
    object which support TypeConverter.CanConvertFrom(typeof(string))  
```

**By default, structure properties are not supported. All structure properties need to be customized before they can be displayed.**  

### Extra Data Structure
* PropertyModels.Collections.SelectableList<T>  
    You can initialize this list with some objects, and you can only select one object in this list. ProeprtyGrid uses ComboBox by default to edit the properties of this data structure
* PropertyModels.Collections.CheckedList<T>
    like SelectableList<T>, you can initialize it with some objects, but you can select multiple objects in it. ProeprtyGrid uses a set of CheckBoxes by default to edit the properties of this data structure.

### Data Reloading
Implement from `System.ComponentModel.INotifyPropertyChanged` and trigger the `PropertyChanged` event when the property changes. `PropertyGrid` will listen to these events and automatically refresh the view data.  
if you implementing from `PropertyModels.ComponentModel.INotifyPropertyChanged` instead of `System.ComponentModel.INotifyPropertyChanged` will gain the additional ability to automatically fire the `PropertyChanged` event when an edit occurs in the PropertyGrid without having to handle each property itself.  
You can also directly inherit `PropertyModel.ComponentModel.MiniReactiveObject`, `PropertyModel.ComponentModel.ReactiveObject`. The former only has data change notification capabilities, while the latter also has data dynamic visibility refresh support. If you use `ReactiveUI.ReactiveObject` directly, then you will not have dynamic visibility support. At this time, you need to monitor the relevant properties yourself, rather than using the `RaisePropertyChanged` method to throw the corresponding property change event.

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
check [MainView.axaml.cs](./Samples/Avalonia.PropertyGrid.Samples/Views/MainView.axaml.cs) for more information.


### Change Size
You can change the width of namelabel and cell edit by drag here:
![Dragging](./Docs/Images/name-width.png)
Or set the NameWidth property of PropertyGrid directly.  

### Multiple Objects Edit

If you want to edit multiple objects at the same time, you only need to set the object to DataContext as IEnumerable, for example:

```C#
public IEnumerable<SimpleObject> multiObjects => new SimpleObject[] { multiObject0, multiObject1 };
```
```xml
<pgc:PropertyGrid x:Name="propertyGrid_MultipleObjects" Margin="2" DataContext="{Binding multiObjects}"></pgc:PropertyGrid>
```  
![multi-objects](./Docs/Images/multi-objects-edit.png)
**Due to complexity considerations, there are many complex types of multi-object editing that are not supported!!!**

### Custom Object & Virtual Object
You can implement your own custom objects or virtual objects based on TypeDescriptionProvider and ICustomTypeDescriptor, and PropertyGrid will automatically recognize and use them.  
![custom-objects](./Docs/Images/custom-objects.png)

### Collection Support
`PropertyGrid` supports collection editing.  
All containers implemented from IList will be automatically recognized and displayed, but only containers or types that implement IBindingList or INotifyCollectionChanged interface will support data synchronization, because data synchronization requires the property type to throw messages itself, and there is no way to help it from the outside.   
Setting **[Editable(false)]** can disable the creation and deletion functions, making the original addable and deleteable container become the same as a normal array, only allowing display and modification, not allowing the addition and deletion of child elements.   
![collection-support](./Docs/Images/collection-support.png)  
**In addition, in order to support the creation function, the template parameter of the container can only be a non-pure virtual class. ** 
**Structural properties are not supported and need to be customized. **

### Expandable Class Properties
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
![expandable-objects](./Docs/Images/expandable-objects.png)   

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
2. The second method is to use `System.ComponentModel.DataAnnotations.ValidationAttribute` to mark the target property, both system-provided and user-defined. for example:
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
    [Required(ErrorMessage = "Can not be null")]
    [Description("String that must not be null")]
    public string ValidateString { get; set; } = "";

    [Category("DataValidation")]
    [Description("Select platforms with validation")]
    [ValidatePlatform]
    public CheckedList<PlatformID> Platforms { get; set; } = new(Enum.GetValues<PlatformID>());
    
    [Category("DataValidation")]
    [Range(0, 100)]
    [Trackable(0, 200)]
    public int ValidateInteger { get; set; } = 100;

    [Category("DataValidation")]
    [MinLength(3), MaxLength(6)]
    public ObservableCollection<int> ValidateIntegerCollection { get; set; } = [1, 2, 3];
```  
![data-validation](./Docs/Images/data-validation.png)  

### Dynamic Visibilty
By setting Attribute, you can make certain Properties only displayed when conditions are met. for example: 
```C#
    public class DynamicVisibilityObject : ReactiveObject
    {
        [ConditionTarget]
        public bool IsShowPath { get; set; } = true;

        [PropertyVisibilityCondition(nameof(IsShowPath), true)]
        [PathBrowsable(Filters = "Image Files(*.jpg;*.png;*.bmp;*.tag)|*.jpg;*.png;*.bmp;*.tag")]
        public string Path { get; set; } = "";

        [ConditionTarget]
        public PlatformID Platform { get; set; } = PlatformID.Win32NT;

        [PropertyVisibilityCondition(nameof(Platform), PlatformID.Unix)]
        [ConditionTarget]
        public string UnixVersion { get; set; } = "";

        // show more complex conditions...
        [Browsable(false)]
        [DependsOnProperty(nameof(IsShowPath), nameof(Platform), nameof(UnixVersion))]
        [ConditionTarget]
        public bool IsShowUnixLoginInfo => IsShowPath && Platform == PlatformID.Unix && UnixVersion.IsNotNullOrEmpty();

        [PropertyVisibilityCondition(nameof(IsShowUnixLoginInfo), true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public LoginInfo unixLogInInfo { get; set; } = new LoginInfo();
    }
```  
![dynamic-visibility](./Docs/Images/dynamic-visibility.png)  
In this example, you can check `IsShowPath` first, then set the `Platform` to Unix, and then enter anything in `UnixVersion`, and you will see the `unixLoginInfo` field.
To do this, you only need to mark the property with a custom Attribute. If you need to implement your own rules, just implement your own rules from.  

**The implementation behind this depends on `IReactiveObject` in `PropertyModels`, you can implement it yourself, or directly derive your Model from `ReactiveObject`.**

**AbstractVisiblityConditionAttribute**.  
One thing to pay special attention to is **that any property that needs to be used as a visibility condition for other properties needs to be marked with [ConditionTarget].**   
The purpose is to let PropertyGrid know that when this property changes, it needs to notify the upper layer to refresh the visibility information.

### User Localization
Implement your PropertyModels.Services.ILocalizationService class, and register its instance by :
```C#
    LocalizationService.Default.AddExtraService(new YourLocalizationService());
```
If you want to provide the corresponding language pack for the built-in text, please add the corresponding file to `/Sources/Avalonia.PropertyGrid/Assets/Localizations`, and name it with the `CultureInfo.Name` of the language. for example:  
```
    en-US.json
    ru-RU.json
    zh-CN.json
```
![localization](./Docs/Images/localization-demo.png)  

### Custom Property Label Controls
To custom label control, use `CustomNameBlock` event on PropertyGrid, and assign your custom control to `e.CustomNameBlock`, for example:
```C#
public class TestExtendPropertyGrid : Controls.PropertyGrid
{
    private int CustomLabelClickCount = 0;

    public TestExtendPropertyGrid()
    {
        CustomNameBlock += (s, e) =>
        {
            if(e.PropertyDescriptor.Name == "customLabel")
            {
                var button = new Button() { Content = LocalizationService.Default[e.PropertyDescriptor.DisplayName] };
                
                button.Click += (ss, ee) =>
                {
                    button.Content = $"{LocalizationService.Default[e.PropertyDescriptor.DisplayName]} {++CustomLabelClickCount}";
                };

                e.CustomNameBlock = button;
            }                
        };
    }
}
```
![Custom-Label](./Docs/Images/custom-name-control.png) 

### Custom Cell Edit
To customize a CellEdit, you need to implement a new factory class from `AbstractCellEditFactory` or other existing factory classes, and then register an instance of this factory class with the `PropertyGrid`, for example:

```C#
public class TestExtendPropertyGrid : Controls.PropertyGrid
{
    public TestExtendPropertyGrid()
    {
        Factories.AddFactory(new Vector3CellEditFactory());
        Factories.AddFactory(new CountryInfoCellEditFactory());
        Factories.AddFactory(new ToggleSwitchCellEditFactory());

        // ...
    }
}
```

![Custom-CellEdit](./Docs/Images/custom-celledit.png)  

To implement a custom factory class, you may need to focus on the following methods that can be overridden. The first two are mandatory to override, while the others are optional:  

1. `HandleNewProperty` is used to create the control you want to use to edit the property. You need to pass the value through the framework's interface after editing the data in the UI to ensure that other related objects receive the notification and save the undo/redo command.  
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
        // use this, don't change value directly
        SetAndRaise(context, control, control.IsChecked); 
    };

    return control;
}
```
2. `HandlePropertyChanged` method is used to synchronize external data. When external data changes, retrieve the data again and synchronize it to the control.  
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
3. Override `ImportPriority` to determine the priority of your factory class. The larger the value, the higher the priority, and the earlier it is triggered.  
4. Override the `Accept` method to make your factory effective only when appropriate.
5. Override `HandlePropagateVisibility` method to customize the filtering scheme.
6. Override `HandleReadOnlyStateChanged` method to customize the read-only effect of your control. By default, `IsEnabled=value` is used to achieve the read-only effect. You can provide your implementation by overriding this method. For example, if your control is inherently in read-only mode, you can use `IsReadOnly=value` to replace the default behavior.
7. Override `CheckIsPropertyChanged` method to customize the method of comparing property sizes. For example, when comparing floating-point numbers, if the difference is less than a certain value, it can be considered unchanged.

### Custom Property Operations

To customize property operations, you first need to configure the `PropertyGrid`'s `PropertyOperationVisibility` property to either Default or Visible. The former allows the property itself to decide whether to display, while the latter displays everything. Hidden means all operation areas are prohibited from displaying.

Then, you need to register the `PropertyGrid` events `CustomPropertyOperationControl` or `CustomPropertyOperationMenuOpening`. This allows you to configure the default operation buttons and popup menus. Of course, you can also replace all controls in this area with your own. You can obtain all context information corresponding to the property through the `PropertyCellContext` in the event callback, such as PropertyDescriptor, Target, RootPropertyGrid, etc.

**It is important to note that if you involve property changes, you must use the Factory-related interfaces of the Context. This ensures that your modifications can be based on the command pattern, creating the necessary command queue so that operations are fully integrated into the framework.**

```C#
private void OnCustomPropertyOperationMenuOpening(object? sender, CustomPropertyDefaultOperationEventArgs e)
{
    if (!e.Context.Property.IsDefined<PropertyOperationVisibilityAttribute>())
    {
        return;
    }

    if (e.Context.Property.Name == nameof(TestExtendsObject.CustomOperationMenuNumber))
    {
        if (e.StageType == PropertyDefaultOperationStageType.Init)
        {
            e.DefaultButton.SetLocalizeBinding(Button.ContentProperty, "Operation");    
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

private void OnCustomPropertyOperationControl(object? sender, CustomPropertyOperationControlEventArgs e)
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
        minButton.SetLocalizeBinding(Button.ContentProperty, "Min");
        minButton.Click += (ss, ee) =>
        {
            // please use factory interface, so you can raise command event 
            e.Context.Factory!.SetPropertyValue(e.Context, 0);
        };
        
        stackPanel.Children.Add(minButton);
        
        var maxButton = new Button();
        maxButton.SetLocalizeBinding(Button.ContentProperty, "Max");
        maxButton.Click += (ss, ee) =>
        {
            // please use factory interface, so you can raise command event 
            e.Context.Factory!.SetPropertyValue(e.Context, 1024);
        };

        stackPanel.Children.Add(maxButton);

        e.CustomControl = stackPanel;
    }
}
```
![Proeprty Operation Example](./Docs/Images/custom-property-operations.png)  

## Description of Samples
### Feature Demos
A set of cases is used to showcase the basic features of `Avalonia.PropertyGrid`, including but not limited to: property analysis, dynamic configuration adjustment, data synchronization, simultaneous editing of multiple objects, custom objects, custom grid editors, dynamic visibility, undo-redo framework, custom appearance, etc.  
![basic-features](./docs/Images/basic-features.png)

### Settings Demo
A simple example is used to show how to implement a system settings interface based on PropertyGrid very simply and conveniently.  
![settings-demo](./Docs/Images/settings-demo.png)  

### Painter Demo
A simple brush example. You can use this demo to see how to use PropertyGrid in engineering software to quickly display and edit data.  
![painter-demo](./Docs/Images/painter-demo.png)  

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

v11.3.0.18
* Removed multiple buttons in the header area, integrated them into a pop-up menu of one button, and opened up more options.  
* The names of some Attributes have been adjusted and some new options have been added. You can see these changes in the code in the documentation and examples.  
* Built-in scroll bar, by default, PropertyGrid header is fixed at the top when scrolling  

v11.3.0.21
* New display and editing support for common containers has been added, but there are certain limitations. Containers that implement observable interfaces will not have the ability to dynamically add, delete, clear, and other operations.