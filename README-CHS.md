# Avalonia.PropertyGrid  
[View English Document](./README.md)  

这是一个用于Avalonia的PropertyGrid实现，可以在Avalonia应用程序中使用。  
其主要功能包括：  
* 支持自动分析类的属性（类似于DevExpress的PropertyGridControl）并显示和编辑
* 支持在一个PropertyGrid中同时编辑多个对象
* 支持实现ICustomTypeDescriptor接口或TypeDescriptionProvider的自定义类型
* 支持自定义属性标签的控件
* 支持自定义任意属性的栅格编辑器(CellEdit)
* 支持额外的属性操作区及其自定义操作
* 支持数组编辑，支持数组在控件中创建、插入、删除和清除
* 支持数据验证
* 支持内置的撤销和重做框架
* 支持全局`只读`设置
* 支持根据条件自动调整属性的可见性
* 支持文件路径选择
* 支持三种显示模式：基于类别、按字母排序和内置排序  
* 支持文本过滤、正则表达式过滤，并支持忽略大小写设置  
* 支持按类别快速过滤
* 支持数据自动重新加载
* 支持子对象的自动展开
* 支持通过拖动标题调整属性名称和属性值的宽度
* 支持本地化和实时更改语言而无需重启
* 支持Web  

<a href="https://bodong1987.github.io/Avalonia.PropertyGrid/" target="_blank"> → 查看在线演示</a>

## 如何使用
直接使用此项目的源代码或使用NUGET包：  
    https://www.nuget.org/packages/bodong.Avalonia.PropertyGrid   
然后将PropertyGrid添加到您的项目中，并将要显示和编辑的对象绑定到`DataContext`属性。  
如果要绑定多个对象，只需直接绑定`IEnumerable<T>`

## 如何调试
打开`Avalonia.PropertyGrid.sln`，将配置更改为`Development`，选择`Avalonia.PropertyGrid.Samples.Desktop`项目作为启动项目，编译和运行它即可。  

## 如何构建在线演示
在`Samples/Avalonia.PropertyGrid.Samples.Browser`下运行`dotnet publish`。这可能需要几分钟。    
**由于.net 8.0中的一个bug，您必须使用.net 9.0，否则在运行时会遇到错误**  

## 详细描述
### 数据建模
如果您想在PropertyGrid中编辑一个对象，只需将此对象直接设置为PropertyGrid的`DataContext`属性，PropertyGrid将自动分析支持编辑的属性，并使用相应的栅格编辑器(`CellEdit`)进行编辑。同时，您还可以使用`System.ComponentModel`、`System.ComponentModel.DataAnnotations`和`PropertyModels.ComponentModel`中的属性来标记这些属性，使这些属性具有一些特殊特性。  
支持但不限于以下：
```C#
System.ComponentModel.CategoryAttribute                     /* 设置属性类别 */
System.ComponentModel.BrowsableAttribute                    /* 用于隐藏属性 */
System.ComponentModel.ReadOnlyAttribute                     /* 使属性只读 */
System.ComponentModel.DisplayNameAttribute                  /* 设置友好名称 */
System.ComponentModel.DescriptionAttribute                  /* 设置长描述文本 */
System.ComponentModel.PasswordPropertyTextAttribute         /* 标记文本属性为密码 */
System.ComponentModel.DataAnnotations.EditableAttribute     /* 标记列表属性可以添加/删除/清除元素 */  
System.ComponentModel.DataAnnotations.RangeAttribute        /* 设置数值范围 */
System.Runtime.Serialization.IgnoreDataMemberAttribute      /* 用于隐藏属性 */
```

此外，在PropertyModels.ComponentModel和PropertyModels.ComponentModel.DataAnnotations中还有其他类可以支持，帮助描述类属性。  
如果您希望类属性之间有一些关联，例如某些属性在实现上依赖于其他属性，那么您可以尝试使用PropertyModels.ComponentModel.DataAnnotations.DependsOnPropertyAttribute标记这种依赖关系  
但您需要从PropertyModels.ComponentModel.ReactiveObject继承您的类，否则您需要自己维护这种关系，只需在依赖属性更改时触发目标属性的PropertyChanged事件。

```C# 
PropertyModels.ComponentModel.FloatPrecisionAttribute                               /* 设置浮点精度 */  
PropertyModels.ComponentModel.IntegerIncrementAttribute                             /* 通过按钮设置整数增量 */  
PropertyModels.ComponentModel.WatermarkAttribute                                    /* 设置水印，它是文本提示 */  
PropertyModels.ComponentModel.MultilineTextAttribute                                /* 使文本编辑可以编辑多行文本 */  
PropertyModels.ComponentModel.ProgressAttribute                                     /* 使用进度条显示数值属性，只读 */   
PropertyModels.ComponentModel.TrackableAttribute                                    /* 使用滑块编辑数值属性 */  
PropertyModels.ComponentModel.EnumDisplayNameAttribute                              /* 为每个枚举值设置友好名称 */
PropertyModels.ComponentModel.EnumExcludeAttribute                                  /* 全局禁止枚举字段出现在PropertyGrid中。要为单个属性配置，可以使用EnumPermitValuesAttribute或EnumProhibitValuesAttribute。 */
PropertyModels.ComponentModel.AutoCollapseCategoriesAttribute                       /* 通过为类配置此属性，可以在初始状态下自动折叠某些类别。 */
PropertyModels.ComponentModel.DataAnnotations.DependsOnPropertyAttribute            /* 标记此属性依赖于其他属性 */  
PropertyModels.ComponentModel.DataAnnotations.FileNameValidationAttribute           /* 标记此属性为文件名，因此控件将直接验证字符串 */  
PropertyModels.ComponentModel.DataAnnotations.PathBrowsableAttribute                /* 标记字符串属性为路径，因此它将提供一个按钮以显示路径浏览器 */  
PropertyModels.ComponentModel.DataAnnotations.VisibilityPropertyConditionAttribute  /* 设置此属性将在属性值更改时自动刷新所有可见性。 */  
PropertyModels.ComponentModel.DataAnnotations.EnumPermitValuesAttribute<T>          /* 对于单个属性配置，强制设置允许的枚举值 */
PropertyModels.ComponentModel.DataAnnotations.EnumPermitNamesAttribute              /* 基于枚举名称 */
PropertyModels.ComponentModel.DataAnnotations.EnumProhibitValuesAttribute<T>        /* 对于单个属性配置，强制禁止某些枚举值出现在候选列表中 */
PropertyModels.ComponentModel.DataAnnotations.EnumProhibitNamesAttribute            /* 基于枚举名称 */
PropertyModels.ComponentModel.DataAnnotations.IEnumValueAuthorizeAttribute          /* 基于此接口创建自定义枚举值过滤器 */
PropertyModels.ComponentModel.DataAnnotations.ImagePreviewModeAttribute             /* 设置图像显示模式 */
PropertyModels.ComponentModel.DataAnnotations.FloatingNumberEqualToleranceAttribute /* 你可以用这个标签来标记一个浮点数的容忍值，当变化差小于这个值时，就认为没有变化。 */  
PropertyModels.ComponentModel.SingleSelectionModeAttribute                          /* 设置枚举/可选列表视图模式，使用组合框/切换按钮组/单选按钮组 */  
PropertyModels.ComponentModel.UnitAttribute                                         /* 用于在显示名称旁显示单位 */  
PropertyModels.ComponentModel.PropertyOperationVisibilityAttribute                  /* 用于指定额外操作区域是否可见，默认值是由属性自行决定。*/  
PropertyModels.ComponentModel.SelectableListDisplayModeAttribute                    /* 用于指定可选列表类型的显示视图类别，默认是水平排列+自动换行，也可配置成其它的。 */  
```

### 支持的内置类型
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
System.ComponentModel.BindingList<>  
System.DateTime/System.DateTimeOffset/System.DateTime?/System.DateTimeOffset?  
System.TimeSpan/System.TimeSpan?      
System.Drawing.Color/Avalonia.Media.Color  
Avalonia.Media.IImage
Avalonia.Media.FontFamily      
PropertyModels.Collections.ICheckedList  
PropertyModels.Collections.ISelectableList  
object which support TypeConverter.CanConvertFrom(typeof(string))  
```
**默认情况下，不支持结构属性。所有结构属性需要自定义才能显示。**

### 额外的数据结构
* `PropertyModels.Collections.SelectableList<T>`  
您可以使用一些对象初始化此列表，并且只能在此列表中选择一个对象。PropertyGrid默认使用ComboBox编辑此数据结构的属性
* `PropertyModels.Collections.CheckedList<T>`    
类似于`SelectableList<T>`，您可以使用一些对象初始化它，但可以在其中选择多个对象。PropertyGrid默认使用一组CheckBoxes编辑此数据结构的属性，例如：
![CheckList](./Docs/Images/CheckList.png)

### 数据重新加载
实现`System.ComponentModel.INotifyPropertyChanged`接口，并在属性更改时触发`PropertyChanged`事件。`PropertyGrid`将监听这些事件并自动刷新视图数据。  
如果您实现的是`PropertyModels.ComponentModel.INotifyPropertyChanged`而不是`System.ComponentModel.INotifyPropertyChanged`，那么在`PropertyGrid`中进行编辑时，将获得自动触发`PropertyChanged`事件的额外能力，而无需自己处理每个属性。  
您还可以直接继承`PropertyModel.ComponentModel.MiniReactiveObject`或`PropertyModel.ComponentModel.ReactiveObject`。前者仅具有数据更改通知功能，而后者还支持数据动态可见性刷新。如果您直接使用`ReactiveUI.ReactiveObject`，那么您将没有动态可见性支持。这时，您需要自己监控相关属性，而不是使用`RaisePropertyChanged`方法抛出相应的属性更改事件。  

### 自定义属性过滤器
```xml
<pgc:PropertyGrid 
    x:Name="propertyGrid_Basic" 
    Margin="4" 
    CustomPropertyDescriptorFilter="OnCustomPropertyDescriptorFilter"
    DataContext="{Binding simpleObject}"
    >
</pgc:PropertyGrid>
```  
设置`CustomPropertyDescriptorFilter`，并添加自定义处理逻辑。    
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
查看[MainView.axaml.cs](./Samples/Avalonia.PropertyGrid.Samples/Views/MainView.axaml.cs)以获取更多信息。  

### 更改大小
您可以通过拖动来更改名称标签和单元格编辑的宽度：
![Dragging](./Docs/Images/ChangeSize.png)
或者直接设置PropertyGrid的NameWidth属性。

### 多对象编辑

如果您想同时编辑多个对象，只需将对象设置为DataContext为IEnumerable，例如： 

```C#
public IEnumerable<SimpleObject> multiObjects => new SimpleObject[] { multiObject0, multiObject1 };
```
```xml
<pgc:PropertyGrid x:Name="propertyGrid_MultipleObjects" Margin="2" DataContext="{Binding multiObjects}"></pgc:PropertyGrid>
```
**由于复杂性考虑，许多复杂类型的多对象编辑不被支持！！！**

### ICustomTypeDescriptor
您可以在Samples中直接找到使用示例。  

### 数组支持
`PropertyGrid`支持数组编辑。这里的数组属性只能使用`BindingList`声明。设置**[Editable(false)]**可以禁用创建和删除功能，这与Array的行为一致。  
**此外，为了支持创建功能，`BindingList`的模板参数只能是非纯虚类。**  
**不支持结构属性。**  

### 展开类属性
当`PropertyGrid`未提供内置的CellEdit来编辑目标属性时，有几种可能性：

1. 如果属性或属性的PropertyType标记了TypeConverter，则PropertyGrid将尝试使用TextBox来编辑对象。当文本更改时，它将主动尝试使用TypeConverter将字符串转换为目标对象。  
2. 如果属性使用ExpandableObjectConverter，则PropertyGrid将尝试在原地展开对象。  
3. 如果两者都不满足，则PropertyGrid将尝试使用只读TextBox显示目标属性的ToString()值。  
  
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

### 数据验证
提供数据验证功能有两种方法：
1. 直接在属性的setter中抛出异常。但我个人不太推荐这种方法，因为如果您在代码中设置此属性，可能会意外导致错误。例如：
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
2. 第二种方法是使用`System.ComponentModel.DataAnnotations.ValidationAttribute`来标记目标属性，可以使用系统提供的或用户自定义的验证属性。例如：
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

### 动态可见性
通过设置属性，您可以使某些属性仅在满足条件时显示。例如：
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
在此示例中，您可以先勾选IsShowPath，然后将Platform设置为Unix，再在UnixVersion中输入内容，您将看到unixLoginInfo字段。
要实现这一点，您只需用自定义Attribute标记属性即可。如果您需要实现自己的规则，只需从中实现自己的规则。

**其背后的实现依赖于`PropertyModels`中的`IReactiveObject`，您可以自行实现，或直接从`ReactiveObject`派生您的模型。**  
**AbstractVisiblityConditionAttribute**  
需要特别注意的一点是，**任何需要用作其他属性可见性条件的属性都需要标记为[`ConditionTarget`]。**  
这样做的目的是让PropertyGrid知道当此属性更改时，需要通知上层刷新可见性信息。


### 用户本地化
实现您的PropertyModels.Services.ILocalizationService类，并通过以下方式注册其实例：
```C#
    LocalizationService.Default.AddExtraService(new YourLocalizationService());
```
如果您想为内置文本提供相应的语言包，请将相应的文件添加到`/Sources/Avalonia.PropertyGrid/Assets/Localizations`，并使用语言的`CultureInfo.Name`命名。例如：  
```
    en-US.json
    ru-RU.json
    zh-CN.json
```  

### 自定义属性标题控件
你只需要注册事件`CustomNameBlock`，然后将你的自定义标题控件分配给`e.CustomNameBlock`即可:
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
![Custom-Label](./Docs/Images/custom-label.png)

### 自定义栅格编辑(CellEdit)
要自定义CellEdit，您需要从AbstractCellEditFactory实现一个工厂类，然后将此类实例附加到CellEditFactoryService.Default，例如：
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
有关更多信息，请参考Samples中的Extends。  

### 自定义属性操作
要自定义属性操作，你需要先配置`PropertyGrid`的`PropertyOperationVisibility`属性为Default或者Visible，前者由属性自行决定是否显示，后者全部显示；Hidden表示禁止所有操作区显示。  
然后你需要注册`PropertyGrid`的事件`CustomPropertyOperationControl`或者`CustomPropertyOperationMenuOpening`，这允许你配置默认的操作按钮以及弹出菜单，当然你也可以全部替换该区域的所有控件为你自己的控件，你通过事件回调中的`PropertyCellContext`获取到该属性对应的所有上下文信息，比如PropertyDescriptor、Target、RootPropertyGrid等等。  
**需要注意的是，如果你涉及到属性变更，要通过Context的Factory相关接口，这样可以确保你的修改可以基于命令模式，这样可以产生必要的命令队列，使得操作全部融入框架中。**  
如：  
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
![Proeprty Operation Example](./Docs/Images/operation-example.png)

## 示例描述
### 特性示例
* 基本功能  
此页面展示了PropertyGrid的基本功能，包括各种属性的显示和默认编辑器等,另外还测试所有可调外观属性。
![Styles](./Docs/Images/Styles.png)

* 数据同步  
![DataSync](./Docs/Images/data-sync.png)
在这里您可以验证数据更改和自动重新加载功能。

* 多对象编辑  
![Multi-Views](./Docs/Images/Multi-Objects.png)
您可以在这里验证多对象编辑的功能。注意：  
  
**某些属性不支持同时编辑多个对象。**

* 自定义对象  
![CustomObject](./Docs/Images/CustomObject.png)
这里展示了如何基于ICustomTypeDescriptor创建自定义对象。  

* 扩展
![Extends](./Docs/Images/extends.png)
在自定义AbstractCellEditFactory中，只有两个方法必须被重写：
1. HandleNewProperty 用于创建您想要编辑属性的控件，
您需要在UI编辑数据后通过框架的接口传递值，以确保其他相关对象接收到消息通知并保存撤销重做命令。
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
2. HandlePropertyChanged 方法用于同步外部数据。当外部数据发生变化时，重新获取数据并同步到控件。  
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

3. AbstractCellEditFactory 还有一个可重写的属性 ImportPriority。该值决定了 PropertyGrid 触发这些工厂的顺序。值越大，触发越早。
4. 重写 Accept 方法可以让您的工厂仅在适当的时候生效。
5. 重写 HandlePropagateVisibility 方法以自定义过滤方案。

例如：  
在正常情况下，PropertyGrid 不会自动处理结构属性，因为结构具有某些特殊性。要支持此类内部不支持的类型，您需要自行扩展 PropertyGrid。此示例展示了如何支持和编辑结构 SVector3。  
还有一个关于 SelectableList 自定义的示例供参考。  
更多细节可以在文件 TestExtendPropertyGrid.cs 中看到。

* 动态可见性
![DynamicVisibility](./Docs/Images/DynamicVisibility.png)
展示动态可见性  
如果您勾选 'IsShowPath'，则可以编辑路径。  
如果您在 Platform 中选择 Unix 并在 UnixVersion 中输入任何内容，则可以编辑额外的属性。

* 撤销重做
![RedoUndo](./Docs/Images/undoredo.png)
此示例展示了如何基于内置的撤销重做框架实现撤销和重做功能。

### 设置示例
一个简单的示例用于展示如何基于PropertyGrid非常简单方便的实现一个系统设置界面。  
![settings-demo](./Docs/Images/settings-demo.png)  

### 画笔示例
一个简单的画笔示例，你可以通过该Demo查看如何将PropertyGrid用于工程化软件，用于快速展示和编辑数据。  
![painter-demo](./Docs/Images/painter-demo.png)

### 主要更改  
v11.0.4.1  
* 数据建模模块已提取为独立项目（PropertyModels），以便于实现数据与表现分离的项目组织结构。因此，在此版本之后，您需要安装两个 NUGET 包。

v11.0.6.2  
* 设置 `SelectedObject` 属性为过时。建议直接使用 DataContext 机制为 PropertyGrid 提供数据；添加自定义属性可见性过滤器支持。

v11.0.10.1  
* 删除 `SelectedObject` 属性，请直接使用 `DataContext`。

v11.1.1.1  
* 无需单独关联 bodong.PropertyModels Nuget 包。引用 Avalonia.PropertyGrid 时可以自动引用此包。如果需要在自己的数据模型项目中引用 PropertyModels，请确保其版本号与 PropertyGrid 引用的版本一致。

v11.1.4.1  
* ShowStyle 属性拆分为三个：ShowStyle、PropertyOrderStyle 和 CategoryOrderStyle。它们分别用于指示：是否显示类别、属性排序方法和类别排序方法。
* ICellEditFactory 添加了一个接口 `HandleReadOnlyStateChanged` 来通知处理 ReadOnly 标签。您可以通过此接口自定义您的 ReadOnly 行为。例如，默认情况下通过将 IsEnabled 设置为 false 来处理 ReadOnly。如果您的控件支持更好的只读效果，可以通过重写此方法进行自定义。例如，对于字符串和数字，IsReadOnly 属性比 IsEnabled 更适合，因为它可以更好地支持用户复制控件中的内容。

v11.1.4.2  
* 启用 Nullable