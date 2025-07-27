using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Avalonia.Media;
using Avalonia.Platform;
using PropertyModels.Collections;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantArgumentDefaultValue
// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Avalonia.PropertyGrid.Samples.FeatureDemos.Models
{
    [AutoCollapseCategories("AutoCollapse")]
    public class SimpleObject : ReactiveObject
    {
        public readonly string Description;

        public SimpleObject(string description)
        {
            Description = description;

            using var stream = AssetLoader.Open(new Uri($"avares://{GetType().Assembly.GetName().Name}/Assets/avalonia-banner.png"));
            AvaloniaBanner = new Media.Imaging.Bitmap(stream);

            // foreach (var name in new[] { "au.png", "bl.png", "ca.png", "cn.png" })
            // {
            //     using var stream = AssetLoader.Open(new Uri($"avares://{GetType().Assembly.GetName().Name}/Assets/country-flags/{name}"));
            //     var image = new Media.Imaging.Bitmap(stream);
            //     ImageList.Add(image);
            // }
        }

        public override string ToString() => $"({GetHashCode()}){Description}";

        [Category("Imaging")]
        [Description("The Banner of Avalonia")]
        public IImage AvaloniaBanner { get; set; }

        // [Category("ImagingList")]
        //[ImagePreviewMode(Stretch = StretchType.None)]
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        // public BindingList<IImage> ImageList { get; set; } = [];

        [Category("Path")]
        [DisplayName("Target Path")]
        [PathBrowsable(Filters = "Image Files(*.jpg;*.png;*.bmp;*.tag)|*.jpg;*.png;*.bmp;*.tag")]
        [Watermark("Image Path")]
        [Description("Select an image file")]
        public string ImagePath { get; set; } = "";

        [Category("Path")]
        [DisplayName("User Home")]
        [PathBrowsable(PathBrowsableType.Directory, InitialFileName = "C:\\Users")]
        [Watermark("Select Home")]
        [Description("Select the user's home directory")]
        public string UserHome { get; set; } = "";

        [Category("Path")]
        [DisplayName("My Pictures")]
        [PathBrowsable(PathBrowsableType.Directory)]
        [Description("Directory where your pictures are stored")]
        public string MyPicturesDirectory { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

        [Category("String")]
        [DisplayName("Target Name")]
        [Watermark("Your Target Name")]
        [ControlClasses("clearButton")]
        [Description("Enter the target name")]
        public string Name { get; set; } = "";

        [Category("String")]
        [PasswordPropertyText(true)]
        [Watermark("Input your password")]
        [Description("Enter your password")]
        public string Password { get; set; } = "";

        [Category("Boolean")]
        [Description("Encrypt data for security")]
        public bool EncryptData { get; set; } = true;

        [Category("Boolean")]
        [Description("Enable or disable safe mode")]
        public bool SafeMode { get; set; } = false;

        [Category("Boolean")]
        [Description("Three-state boolean value")]
        public bool? ThreeStates { get; set; } = null;

        // This property will be shown by Custom filter
        [Category("Boolean")]
        [IgnoreDataMember]
        [Description("Three-state boolean value with custom filter")]
        public bool? ThreeStates2 { get; set; } = null;

        [IgnoreDataMember]
        [Category("Boolean")]
        [Description("Boolean value that is always hidden")]
        public bool AlwaysHiddenBoolean { get; set; }

        [Category("Enum")]
        [SelectableListDisplayMode(SelectableListDisplayMode.Default)]
        [Description("Select phone service with default view")]
        public PhoneService ServiceDefaultView { get; set; } = PhoneService.None;
        
        [Category("Enum")]
        [SelectableListDisplayMode(SelectableListDisplayMode.Vertical)]
        [Description("Select phone service with vertical view")]
        public PhoneService ServiceVerticalView { get; set; } = PhoneService.None;
        
        [Category("Enum")]
        [SelectableListDisplayMode(SelectableListDisplayMode.Horizontal)]
        [Description("Select phone service with horizontal view")]
        public PhoneService ServiceHorizontalView { get; set; } = PhoneService.None;

        [Category("Enum")]
        [SingleSelectionMode(SingleSelectionMode.Default)]
        [Description("Select gender with default view")]
        public GenderType GenderDefaultView { get; set; } = GenderType.Unknown;

        [Category("Enum")]
        [SingleSelectionMode(SingleSelectionMode.RadioButton)]
        [Description("Select gender using radio buttons")]
        public GenderType GenderRadioView { get; set; } = GenderType.Male;

        [Category("Enum")]
        [SingleSelectionMode(SingleSelectionMode.ToggleButtonGroup, ElementMinWidth = 100)]
        [Description("Select gender using toggle button group")]
        public GenderType GenderToggleGroupView { get; set; } = GenderType.Female;

        [Category("Enum")]
        [EnumPermitValues<PhoneService>(PhoneService.Cell, PhoneService.Fax)]
        [Description("Select phone service allowing only Cell and Fax")]
        public PhoneService ServiceAllowCellFax { get; set; } = PhoneService.None;

        [Category("Enum")]
        [Description("Current platform")]
        public static PlatformID CurrentPlatform => Environment.OSVersion.Platform;

        [Category("Enum")]
        [Description("Select platform with default view")]
        public PlatformID PlatformDefaultView { get; set; } = Environment.OSVersion.Platform;
        
        [Category("Enum")]
        [SingleSelectionMode(SingleSelectionMode.RadioButton)]
        [SelectableListDisplayMode(SelectableListDisplayMode.Default)]
        [Description("Select platform using radio buttons with default view")]
        public PlatformID PlatformRadioButtonDefaultView { get; set; } = Environment.OSVersion.Platform;
        
        [Category("Enum")]
        [SingleSelectionMode(SingleSelectionMode.RadioButton)]
        [SelectableListDisplayMode(SelectableListDisplayMode.Vertical)]
        [Description("Select platform using radio buttons with vertical view")]
        public PlatformID PlatformRadioButtonVerticalView { get; set; } = Environment.OSVersion.Platform;
        
        [Category("Enum")]
        [SingleSelectionMode(SingleSelectionMode.RadioButton)]
        [SelectableListDisplayMode(SelectableListDisplayMode.Horizontal)]
        [Description("Select platform using radio buttons with horizontal view")]
        public PlatformID PlatformRadioButtonHorizontalView { get; set; } = Environment.OSVersion.Platform;
        
        [Category("Enum")]
        [SelectableListDisplayMode(SelectableListDisplayMode.Default)]
        [SingleSelectionMode(SingleSelectionMode.ToggleButtonGroup)]
        [Description("Select platform using toggle buttons with default view")]
        public PlatformID PlatformToggleButtonDefaultView { get; set; } = Environment.OSVersion.Platform;
        
        [Category("Enum")]
        [SelectableListDisplayMode(SelectableListDisplayMode.Vertical)]
        [SingleSelectionMode(SingleSelectionMode.ToggleButtonGroup)]
        [Description("Select platform using toggle buttons with vertical view")]
        public PlatformID PlatformToggleButtonVerticalView { get; set; } = Environment.OSVersion.Platform;
        
        [Category("Enum")]
        [SelectableListDisplayMode(SelectableListDisplayMode.Horizontal)]
        [SingleSelectionMode(SingleSelectionMode.ToggleButtonGroup)]
        [Description("Select platform using toggle buttons with horizontal view")]
        public PlatformID PlatformToggleButtonHorizontalView{ get; set; } = Environment.OSVersion.Platform;

        [Category("Enum")]
        [EnumProhibitValues<PlatformID>(PlatformID.Unix)]
        [Description("Select platform excluding Unix")]
        public PlatformID PlatformNoUnix { get; set; } = Environment.OSVersion.Platform;

        [Category("Enum")]
        [Description("Enum with display names")]
        public PlatformType EnumWithDisplayName { get; set; } = PlatformType.Windows;

        [Category("Selectable List")]
        [Description("Select login name from list")]
        public SelectableList<string> LoginName { get; set; } = new(["John", "David", "bodong"], "bodong");
        
        [Category("Selectable List")]
        [Description("Select login name from list without default")]
        public SelectableList<string> LoginNameNoDefault { get; set; } = new(["John", "David", "bodong"]);

        [Category("Selectable List")]
        [SingleSelectionMode(SingleSelectionMode.RadioButton)]
        [Description("Select login name using radio buttons")]
        public SelectableList<string> LoginNameRadioMode { get; set; } = new(["John", "David", "bodong"]);

        [Category("Selectable List")]
        [SingleSelectionMode(SingleSelectionMode.ToggleButtonGroup)]
        [Description("Select login name using toggle button group")]
        public SelectableList<string> LoginNameToggleGroupMode { get; set; } = new(["John", "David", "bodong"], "bodong");
        
        [Category("Selectable List")]
        [SingleSelectionMode(SingleSelectionMode.ToggleButtonGroup)]
        [SelectableListDisplayMode(SelectableListDisplayMode.Vertical)]
        [Description("Select login name using toggle button group with vertical view")]
        public SelectableList<string> LoginNameToggleGroupModeVerticalView { get; set; } = new(["John", "David", "bodong"], "David");

        [Category("Selectable List")]
        [Description("Select ID from list")]
        public SelectableList<int> IdList { get; set; } = new([100, 1000, 1024], 1000);

        private string _SourceImagePath = "";
        [Category("DataValidation")]
        [PathBrowsable(Filters = "Image Files(*.jpg;*.png;*.bmp;*.tag)|*.jpg;*.png;*.bmp;*.tag")]
        [Watermark("This path can be validated")]
        [Description("Path to source image with validation")]
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

        [Category("Numeric")]
        [Range(10, 200)]
        [Unit("m")]
        [Description("Integer value with range between 10 and 200")]
        public int iValue { get; set; } = 100;

        [Category("Numeric")]
        [Range(0.1f, 10.0f)]
        [Unit("cm")]
        [Description("Float value with range between 0.1 and 10.0")]
        public float fValue { get; set; } = 0.5f;

        [Category("Numeric")]
        [Range(0.1f, 10.0f)]
        [FloatPrecision(3)]
        [Unit("mm")]
        [Description("Float value with precision of 3 decimal places")]
        public float fValuePrecision { get; set; } = 0.5f;

        [Category("Numeric")]
        [Range(0.1f, 10.0f)]
        [Unit("rad")]
        [Description("Double value with range between 0.1 and 10.0")]
        public double dValue { get; set; } = 5.5f;

        [Category("Numeric")]
        [Unit("deg")]
        [Description("Long integer value representing degrees")]
        public long i64Value { get; set; } = 1000000000;

        [Category("Numeric")]
        [Unit("m/s\u00b2")]
        [Description("Large long integer value representing acceleration")]
        public long i64ValueBig { get; set; } = 583792581039233983;

        [Category("Numeric")]
        [Unit("km/h")]
        [Description("Decimal value representing speed in kilometers per hour")]
        public decimal decValue { get; set; } = 100.00M;

        [Category("Numeric")]
        [Range(typeof(decimal), "10.00001", "1000.9999", ParseLimitsInInvariantCulture = true)]
        [FloatPrecision(3)]
        [Unit("mp/h")]
        [Description("Decimal value with range and precision representing speed in miles per hour")]
        [InnerRightContentString("mp/h")]
        public decimal decValueWithRange { get; set; } = 100.00M;

        [Category("Numeric")]
        [Progress]
        [Unit("%")]
        [Description("Progress value represented as a percentage")]
        public double progressValue { get; set; } = 47;

        [Category("Numeric")]
        [Trackable(0, 100, Increment = 0.1, FormatString = "{0:0.0}")]
        [Unit("kg")]
        [Description("Trackable double value representing weight in kilograms")]
        public double trackableDoubleValue
        {
            get => progressValue;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (progressValue != value)
                {
                    progressValue = value;
                    RaisePropertyChanged(nameof(progressValue));
                }
            }
        }

        [Category("Numeric")]
        [Trackable(-1000, 1000, Increment = 1, FormatString = "{0:0}")]
        [Unit("t")]
        [Description("Trackable integer value representing mass in metric tons")]
        public int trackableIntValue { get; set; } = 10;
        
        [Category("Numeric")]
        [Trackable(-1000, 1000, Increment = 1, FormatString = "{0:0}")]
        [Description("Trackable integer value without unit")]
        public int trackableIntValueNoUnit { get; set; } = 10;
        
        [Category("Numeric")]
        [Trackable(-1000, 1000, Increment = 1, FormatString = "{0:0}")]
        [PropertyOperationVisibility(PropertyOperationVisibility.Visible)]
        [Description("Integer value with operations visible")]
        public int NumberWithOperations { get; set; } = 100;
        
        [Category("Array")]
        [Description("Array of strings")]
        public string[] stringArray { get; set; } = ["bodong", "china"];
        
        [Category("Array")]
        [Description("Array of boolean values")]
        public bool[] boolArray { get; set; } = [true, false];
        
        [Category("Array")]
        [Description("Array of platform IDs")]
        public PlatformID[] enumArray { get; set; } = [PlatformID.Win32NT, PlatformID.MacOSX, PlatformID.Unix];

        [Category("Array")] 
        [Description("Array of 3D vectors")]
        public Vector3[] Vec3Array { get; set; } = [new(1, 2, 3), new(7, 8, 9)];

        [Category("List")] 
        [Description("List of strings")]
        public List<string> stringList { get; set; } = ["John", "David", "bodong"];
        
        [Category("List")] 
        [Description("List of boolean values")]
        public List<bool> boolList { get; set; } = [true, false];
        
        [Category("List")]
        [Description("List of platform IDs")]
        public List<PlatformID> enumList { get; set; } = [PlatformID.MacOSX, PlatformID.Unix];
        
        [Category("List")]
        [Description("List of 3D vectors")]
        public List<Vector3> Vec3List { get; set; } = [new(1, 2, 3), new(7, 8, 9)];
        
        [Category("BindingList")]
        [Description("Binding list of strings")]
        public BindingList<string> stringBindingList { get; set; } = ["bodong", "china"];

        [Category("BindingList")]
        [DisplayName("Not Editable")]
        [Editable(false)]
        [Description("Binding list of strings that is not editable")]
        public BindingList<string> stringBindingListNotEditable { get; set; } = ["bodong", "china"];

        [Category("BindingList")]
        [DisplayName("Readonly List")]
        [ReadOnly(true)]
        [Description("Binding list of strings that is readonly")]
        public BindingList<string> stringBindingListReadonly { get; set; } = ["bodong", "china"];

        [Category("BindingList")]
        [Description("Binding list of boolean values")]
        public BindingList<bool> boolBindingList { get; set; } = [true, false];

        [Category("BindingList")]
        [Description("Binding list of platform IDs")]
        public BindingList<PlatformID> enumBindingList { get; set; } = [PlatformID.Win32NT, PlatformID.Unix];

        [Category("BindingList")]
        [Description("Binding list of 3D vectors")]
        public BindingList<Vector3> Vec3BindingList { get; set; } = [new(1024.0f, 2048.0f, 4096.0f)];

        [Category("BindingList")]
        [Length(1, 10)]
        [Description("Binding list of integers with length limit")]
        public BindingList<int> LengthLimitedList { get; set; } = [10, 20, 30];
        
        [Category("Collections")]
        [Description("Observable collection of strings")]
        public ObservableCollection<string> stringCollection { get; set; } = ["bodong", "china"];
        
        [Category("Collections")]
        [Description("Observable collection of boolean values")]
        public ObservableCollection<bool> boolCollection { get; set; } = [true, false];
                [Category("Collections")]
        [Description("Observable collection of platform IDs")]
        public ObservableCollection<PlatformID> enumCollection { get; set; } = [PlatformID.Win32NT, PlatformID.Unix];
        
        [Category("Collections")]
        [Description("Observable collection of 3D vectors")]
        public ObservableCollection<Vector3> Vec3Collection { get; set; } = [new(1024.0f, 2048, 4096.0)];

        [Category("Checked List")]
        [Description("Checked list of strings")]
        public CheckedList<string> CheckedListString { get; set; } = new(["bodong", "John", "David"], ["bodong"]);
        
        [Category("Checked List")]
        [SelectableListDisplayMode(SelectableListDisplayMode.Vertical)]
        [Description("Checked list of strings with vertical view")]
        public CheckedList<string> CheckedListStringVerticalView { get; set; } = new(["bodong", "John", "David"], ["bodong", "John"]);
        
        [Category("Checked List")]
        [SelectableListDisplayMode(SelectableListDisplayMode.Horizontal)]
        [Description("Checked list of strings with horizontal view")]
        public CheckedList<string> CheckedListStringHorizontalView { get; set; } = new(["bodong", "John", "David"], ["David"]);

        [Category("Checked List")]
        [Description("Checked list of integers")]
        public CheckedList<int> CheckedListInt { get; set; } = new([100, 200, 300, 400, 1024, 2048, 4096, 8192], [1024, 8192]);

        [Category("Date Time")]
        [Description("Date and time value")]
        public DateTime dateTime { get; set; } = DateTime.Now;

        [Category("Date Time")]
        [Description("Nullable date and time value")]
        public DateTime? dateTimeNullable { get; set; }

        [Category("Date Time")]
        [Description("Date and time with offset value")]
        public DateTimeOffset dateTimeOffset { get; set; } = DateTimeOffset.Now;

        [Category("Date Time")]
        [Description("Nullable date and time with offset value")]
        public DateTimeOffset? dateTimeOffsetNullable { get; set; }

        [Category("Date Time")]
        [ReadOnly(true)]
        [Description("Readonly start date")]
        public DateTime startDate { get; set; } = DateTime.Now;

        [Category("Date Time")]
        [Description("Time span value")]
        public TimeSpan time { get; set; } = DateTime.Now.TimeOfDay;

        [Category("Date Time")]
        [Description("Nullable time span value")]
        public TimeSpan? timeNullable { get; set; }

        [Category("Date Time")]
        [ReadOnly(true)]
        [Description("Readonly time span value")]
        public TimeSpan timeReadonly { get; set; } = DateTime.Now.TimeOfDay;

        [Category("Expandable")]
        [Description("Expandable 3D vector")]
        public Vector3 vec3 { get; set; } = new(1, 2, 3);

        [Category("Color")]
        [Description("Color value representing red")]
        public System.Drawing.Color RedColor { get; set; } = System.Drawing.Color.Red;

        [Category("Color")]
        [Description("Custom color value")]
        public System.Drawing.Color Color2 { get; set; } = System.Drawing.Color.FromArgb(255, 122, 50, 177);

        [Category("Color")]
        [Description("Binding list of colors")]
        public BindingList<System.Drawing.Color> ColorBindingList { get; set; } = [System.Drawing.Color.Pink, System.Drawing.Color.Purple];

        [Category("Expandable")]
        [DisplayName("Login User Data")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
        [Description("Expandable login information")]
        public LoginInfo loginInfo { get; set; } = new();

        [Category("Font")]
        [Description("Font family selection")]
        public FontFamily FontFamily { get; set; } = new("Courier New");

        [Category("Readonly")]
        [PathBrowsable(PathBrowsableType.Directory)]
        [ReadOnly(true)]
        [Description("Readonly path to a directory")]
        public string ReadonlyPath { get; set; } = @"C:\Windows\System32";

        [Category("Readonly")]
        [ReadOnly(true)]
        [Description("Readonly string")]
        public string ReadonlyString { get; set; } = "You can copy but you can't edit it";

        [Category("Readonly")]
        [ReadOnly(true)]
        [Description("Readonly boolean value")]
        public bool ReadonlyBoolean { get; set; } = false;

        [Category("Readonly")]
        [ReadOnly(true)]
        [Description("Readonly phone service type")]
        public PhoneService ReadonlyPhoneService { get; set; }

        [Category("Readonly")]
        [ReadOnly(true)]
        [Description("Readonly platform type")]
        public PlatformType ReadonlyPlatformType { get; set; }

        [Category("Readonly")]
        [ReadOnly(true)]
        [Description("Readonly integer value")]
        public int ReadonlyInt32 { get; set; }

        [Category("Readonly")]
        [ReadOnly(true)]
        [Description("Readonly float value")]
        public float ReadonlyFloat { get; set; }

        [Category("Readonly")]
        [ReadOnly(true)]
        [Description("Readonly binding list of integers")]
        public BindingList<int> ReadonlyIntBindingList { get; set; } = [1, 2, 3];

        [Category("Readonly")]
        [ReadOnly(true)]
        [Description("Readonly binding list of strings")]
        public BindingList<string> ReadonlyStringBindingList { get; set; } = ["Hello", "World", "!!!"];

        [Category("Readonly")]
        [ReadOnly(true)]
        [Description("Readonly selectable list of strings")]
        public SelectableList<string> ReadonlySelectableList { get; set; } = new(["John", "David", "bodong"]);

        [Category("Readonly")]
        [ReadOnly(true)]
        [Description("Readonly 3D vector")]
        public Vector3 ReadonlyVector3 { get; set; } = new(1, 2, 3);

        [Category("Readonly")]
        [ReadOnly(true)]
        [Description("Readonly checked list of platform IDs")]
        public CheckedList<PlatformID> ReadonlyPlatforms { get; set; } = new(Enum.GetValues<PlatformID>(), [PlatformID.Win32NT, PlatformID.Unix]);

        [Category("Readonly")]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Description("Readonly login information")]
        public LoginInfo ReadonlyLoginInfo { get; set; } = new();

        [Category("AutoCollapse")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Description("Collapsed login information")]
        public LoginInfo CollapsedLoginInfo { get; set; } = new();
    }

    [Flags]
    public enum PhoneService
    {
        [EnumDisplayName("Default")]
        None = 0,
        LandLine = 1,
        Cell = 2,
        Fax = 4,
        Internet = 8,
        Other = 16,

        [EnumExclude]
        CanNotSeeThis = 32
    }

    public enum PlatformType
    {
        [EnumDisplayName("Microsoft.Windows")]
        Windows,
        [EnumDisplayName("Apple.MacOS")]
        MacOS,
        [EnumDisplayName("Apple.IOS")]
        Ios,

        [EnumDisplayName("Unknown.Other")]
        Other,

        [EnumExclude]
        CanNotSeeThis
    }

    public enum GenderType
    {
        Unknown,
        Male,
        Female
    }

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
}