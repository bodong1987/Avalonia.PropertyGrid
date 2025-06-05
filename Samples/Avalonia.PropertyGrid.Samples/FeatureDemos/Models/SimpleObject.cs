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
        public string? ImagePath { get; set; }

        [Category("Path")]
        [DisplayName("User Home")]
        [PathBrowsable(PathBrowsableType.Directory, InitialFileName = "C:\\Users")]
        [Watermark("Select Home")]
        public string? UserHome { get; set; }

        [Category("Path")]
        [DisplayName("My Pictures")]
        [PathBrowsable(PathBrowsableType.Directory)]
        public string? MyPicturesDirectory { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

        [Category("String")]
        [DisplayName("Target Name")]
        [Watermark("Your Target Name")]
        [ControlClasses("clearButton")]
        public string? Name { get; set; }

        [Category("String")]
        [PasswordPropertyText(true)]
        [Watermark("Input your password")]
        public string? Password { get; set; }

        [Category("Boolean")]
        public bool EncryptData { get; set; } = true;

        [Category("Boolean")]
        public bool SafeMode { get; set; } = false;

        [Category("Boolean")]
        public bool? ThreeStates { get; set; } = null;

        // This property will be shown by Custom filter
        [Category("Boolean")]
        [IgnoreDataMember]
        public bool? ThreeStates2 { get; set; } = null;

        [IgnoreDataMember]
        [Category("Boolean")]
        public bool AlwaysHiddenBoolean { get; set; }

        [Category("Enum")]
        [SelectableListDisplayMode(SelectableListDisplayMode.Default)]
        public PhoneService ServiceDefaultView { get; set; } = PhoneService.None;
        
        [Category("Enum")]
        [SelectableListDisplayMode(SelectableListDisplayMode.Vertical)]
        public PhoneService ServiceVerticalView { get; set; } = PhoneService.None;
        
        [Category("Enum")]
        [SelectableListDisplayMode(SelectableListDisplayMode.Horizontal)]
        public PhoneService ServiceHorizontalView { get; set; } = PhoneService.None;

        [Category("Enum")]
        [SingleSelectionMode(SingleSelectionMode.Default)]
        public GenderType GenderDefaultView { get; set; } = GenderType.Unknown;

        [Category("Enum")]
        [SingleSelectionMode(SingleSelectionMode.RadioButton)]
        public GenderType GenderRadioView { get; set; } = GenderType.Male;

        [Category("Enum")]
        [SingleSelectionMode(SingleSelectionMode.ToggleButtonGroup, ElementMinWidth = 100)]
        public GenderType GenderToggleGroupView { get; set; } = GenderType.Female;

        [Category("Enum")]
        [EnumPermitValues<PhoneService>(PhoneService.Cell, PhoneService.Fax)]
        public PhoneService ServiceAllowCellFax { get; set; } = PhoneService.None;

        [Category("Enum")]
        public static PlatformID CurrentPlatform => Environment.OSVersion.Platform;

        [Category("Enum")]
        public PlatformID PlatformDefaultView { get; set; } = Environment.OSVersion.Platform;
        
        [Category("Enum")]
        [SingleSelectionMode(SingleSelectionMode.RadioButton)]
        [SelectableListDisplayMode(SelectableListDisplayMode.Default)]
        public PlatformID PlatformRadioButtonDefaultView { get; set; } = Environment.OSVersion.Platform;
        
        [Category("Enum")]
        [SingleSelectionMode(SingleSelectionMode.RadioButton)]
        [SelectableListDisplayMode(SelectableListDisplayMode.Vertical)]
        public PlatformID PlatformRadioButtonVerticalView { get; set; } = Environment.OSVersion.Platform;
        
        [Category("Enum")]
        [SingleSelectionMode(SingleSelectionMode.RadioButton)]
        [SelectableListDisplayMode(SelectableListDisplayMode.Horizontal)]
        public PlatformID PlatformRadioButtonHorizontalView { get; set; } = Environment.OSVersion.Platform;
        
        [Category("Enum")]
        [SelectableListDisplayMode(SelectableListDisplayMode.Default)]
        [SingleSelectionMode(SingleSelectionMode.ToggleButtonGroup)]
        public PlatformID PlatformToggleButtonDefaultView { get; set; } = Environment.OSVersion.Platform;
        
        [Category("Enum")]
        [SelectableListDisplayMode(SelectableListDisplayMode.Vertical)]
        [SingleSelectionMode(SingleSelectionMode.ToggleButtonGroup)]
        public PlatformID PlatformToggleButtonVerticalView { get; set; } = Environment.OSVersion.Platform;
        
        [Category("Enum")]
        [SelectableListDisplayMode(SelectableListDisplayMode.Horizontal)]
        [SingleSelectionMode(SingleSelectionMode.ToggleButtonGroup)]
        public PlatformID PlatformToggleButtonHorizontalView{ get; set; } = Environment.OSVersion.Platform;

        [Category("Enum")]
        [EnumProhibitValues<PlatformID>(PlatformID.Unix)]
        public PlatformID PlatformNoUnix { get; set; } = Environment.OSVersion.Platform;

        [Category("Enum")]
        public PlatformType EnumWithDisplayName { get; set; } = PlatformType.Windows;

        [Category("Selectable List")]
        public SelectableList<string> LoginName { get; set; } = new(["John", "David", "bodong"], "bodong");
        
        [Category("Selectable List")]
        public SelectableList<string> LoginNameNoDefault { get; set; } = new(["John", "David", "bodong"]);

        [Category("Selectable List")]
        [SingleSelectionMode(SingleSelectionMode.RadioButton)]
        public SelectableList<string> LoginNameRadioMode { get; set; } = new(["John", "David", "bodong"]);

        [Category("Selectable List")]
        [SingleSelectionMode(SingleSelectionMode.ToggleButtonGroup)]
        public SelectableList<string> LoginNameToggleGroupMode { get; set; } = new(["John", "David", "bodong"], "bodong");
        
        [Category("Selectable List")]
        [SingleSelectionMode(SingleSelectionMode.ToggleButtonGroup)]
        [SelectableListDisplayMode(SelectableListDisplayMode.Vertical)]
        public SelectableList<string> LoginNameToggleGroupModeVerticalView { get; set; } = new(["John", "David", "bodong"], "David");

        [Category("Selectable List")]
        public SelectableList<int> IdList { get; set; } = new([100, 1000, 1024], 1000);

        private string? _SourceImagePath;
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

        [Category("DataValidation")]
        [Required(ErrorMessage = "Can not be null")]
        public string? ValidateString { get; set; }

        [Category("DataValidation")]
        [Description("Select platforms")]
        [ValidatePlatform]
        public CheckedList<PlatformID> Platforms { get; set; } = new(Enum.GetValues<PlatformID>());

        [Category("Numeric")]
        [Range(10, 200)]
        [Unit("m")]
        public int iValue { get; set; } = 100;

        [Category("Numeric")]
        [Range(0.1f, 10.0f)]
        [Unit("cm")]
        public float fValue { get; set; } = 0.5f;

        [Category("Numeric")]
        [Range(0.1f, 10.0f)]
        [FloatPrecision(3)]
        [Unit("mm")]
        public float fValuePrecision { get; set; } = 0.5f;

        [Category("Numeric")]
        [Range(0.1f, 10.0f)]
        [Unit("rad")]
        public double dValue { get; set; } = 5.5f;

        [Category("Numeric")]
        [Unit("deg")]
        public long i64Value { get; set; } = 1000000000;

        [Category("Numeric")]
        [Unit("m/s\u00b2")]
        public long i64ValueBig { get; set; } = 583792581039233983;

        [Category("Numeric")]
        [Unit("km/h")]
        public decimal decValue { get; set; } = 100.00M;

        [Category("Numeric")]
        [Range(typeof(decimal), "10.00001", "1000.9999", ParseLimitsInInvariantCulture = true)]
        [FloatPrecision(3)]
        [Unit("mp/h")]
        public decimal decValueWithRange { get; set; } = 100.00M;

        [Category("Numeric")]
        [Progress]
        [Unit("%")]
        public double progressValue { get; set; } = 47;

        [Category("Numeric")]
        [Trackable(0, 100, Increment = 0.1, FormatString = "{0:0.0}")]
        [Unit("kg")]
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
        public int trackableIntValue { get; set; } = 10;
        
        [Category("Numeric")]
        [Trackable(-1000, 1000, Increment = 1, FormatString = "{0:0}")]
        public int trackableIntValueNoUnit { get; set; } = 10;
        
        [Category("Numeric")]
        [Trackable(-1000, 1000, Increment = 1, FormatString = "{0:0}")]
        [PropertyOperationVisibility(PropertyOperationVisibility.Visible)]
        public int NumberWithOperations { get; set; } = 100;
        
        [Category("Array")]
        public string[] stringArray { get; set; } = ["bodong", "china"];
        
        [Category("Array")]
        public bool[] boolArray { get; set; } = [true, false];
        
        [Category("Array")]
        public PlatformID[] enumArray { get; set; } = [PlatformID.Win32NT, PlatformID.MacOSX, PlatformID.Unix];

        [Category("Array")] 
        public Vector3[] Vec3Array { get; set; } = [new(1, 2, 3), new(7, 8, 9)];

        [Category("List")] 
        public List<string> stringList { get; set; } = ["John", "David", "bodong"];
        
        [Category("List")] 
        public List<bool> boolList { get; set; } = [true, false];
        
        [Category("List")]
        public List<PlatformID> enumList { get; set; } = [PlatformID.MacOSX, PlatformID.Unix];
        
        [Category("List")]
        public List<Vector3> Vec3List { get; set; } = [new(1, 2, 3), new(7, 8, 9)];
        
        [Category("BindingList")]
        public BindingList<string> stringBindingList { get; set; } = ["bodong", "china"];

        [Category("BindingList")]
        [DisplayName("Not Editable")]
        [Editable(false)]
        public BindingList<string> stringBindingListNotEditable { get; set; } = ["bodong", "china"];

        [Category("BindingList")]
        [DisplayName("Readonly List")]
        [ReadOnly(true)]
        public BindingList<string> stringBindingListReadonly { get; set; } = ["bodong", "china"];

        [Category("BindingList")]
        public BindingList<bool> boolBindingList { get; set; } = [true, false];

        [Category("BindingList")]
        public BindingList<PlatformID> enumBindingList { get; set; } = [PlatformID.Win32NT, PlatformID.Unix];

        [Category("BindingList")]
        public BindingList<Vector3> Vec3BindingList { get; set; } = [new(1024.0f, 2048.0f, 4096.0f)];

        [Category("BindingList")]
        [Length(1, 10)]
        public BindingList<int> LengthLimitedList { get; set; } = [10, 20, 30];
        
        [Category("Collections")]
        public ObservableCollection<string> stringCollection { get; set; } = ["bodong", "china"];
        
        [Category("Collections")]
        public ObservableCollection<bool> boolCollection { get; set; } = [true, false];
        
        [Category("Collections")]
        public ObservableCollection<PlatformID> enumCollection { get; set; } = [PlatformID.Win32NT, PlatformID.Unix];
        
        [Category("Collections")]
        public ObservableCollection<Vector3> Vec3Collection { get; set; } = [new(1024.0f, 2048, 4096.0)];

        [Category("Checked List")]
        public CheckedList<string> CheckedListString { get; set; } = new(["bodong", "John", "David"], ["bodong"]);
        
        [Category("Checked List")]
        [SelectableListDisplayMode(SelectableListDisplayMode.Vertical)]
        public CheckedList<string> CheckedListStringVerticalView { get; set; } = new(["bodong", "John", "David"], ["bodong", "John"]);
        
        [Category("Checked List")]
        [SelectableListDisplayMode(SelectableListDisplayMode.Horizontal)]
        public CheckedList<string> CheckedListStringHorizontalView { get; set; } = new(["bodong", "John", "David"], ["David"]);

        [Category("Checked List")]
        public CheckedList<int> CheckedListInt { get; set; } = new([100, 200, 300, 400, 1024, 2048, 4096, 8192], [1024, 8192]);

        [Category("Date Time")]
        public DateTime dateTime { get; set; } = DateTime.Now;

        [Category("Date Time")]
        public DateTime? dateTimeNullable { get; set; }

        [Category("Date Time")]
        public DateTimeOffset dateTimeOffset { get; set; } = DateTimeOffset.Now;

        [Category("Date Time")]
        public DateTimeOffset? dateTimeOffsetNullable { get; set; }

        [Category("Date Time")]
        [ReadOnly(true)]
        public DateTime startDate { get; set; } = DateTime.Now;

        [Category("Date Time")]
        public TimeSpan time { get; set; } = DateTime.Now.TimeOfDay;

        [Category("Date Time")]
        public TimeSpan? timeNullable { get; set; }

        [Category("Date Time")]
        [ReadOnly(true)]
        public TimeSpan timeReadonly { get; set; } = DateTime.Now.TimeOfDay;

        [Category("Expandable")]
        public Vector3 vec3 { get; set; } = new(1, 2, 3);

        [Category("Color")]
        public System.Drawing.Color RedColor { get; set; } = System.Drawing.Color.Red;

        [Category("Color")]
        public System.Drawing.Color Color2 { get; set; } = System.Drawing.Color.FromArgb(255, 122, 50, 177);

        [Category("Color")]
        public BindingList<System.Drawing.Color> ColorBindingList { get; set; } = [System.Drawing.Color.Pink, System.Drawing.Color.Purple];

        [Category("Expandable")]
        [DisplayName("Login User Data")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
        public LoginInfo loginInfo { get; set; } = new();

        [Category("Font")]
        public FontFamily FontFamily { get; set; } = new("Courier New");

        [Category("Readonly")]
        [PathBrowsable(PathBrowsableType.Directory)]
        [ReadOnly(true)]
        public string ReadonlyPath { get; set; } = @"C:\Windows\System32";

        [Category("Readonly")]
        [ReadOnly(true)]
        public string ReadonlyString { get; set; } = "You can copy but you can't edit it";

        [Category("Readonly")]
        [ReadOnly(true)]
        public bool ReadonlyBoolean { get; set; } = false;

        [Category("Readonly")]
        [ReadOnly(true)]
        public PhoneService ReadonlyPhoneService { get; set; }

        [Category("Readonly")]
        [ReadOnly(true)]
        public PlatformType ReadonlyPlatformType { get; set; }

        [Category("Readonly")]
        [ReadOnly(true)]
        public int ReadonlyInt32 { get; set; }

        [Category("Readonly")]
        [ReadOnly(true)]
        public float ReadonlyFloat { get; set; }

        [Category("Readonly")]
        [ReadOnly(true)]
        public BindingList<int> ReadonlyIntBindingList { get; set; } = [1, 2, 3];

        [Category("Readonly")]
        [ReadOnly(true)]
        public BindingList<string> ReadonlyStringBindingList { get; set; } = ["Hello", "World", "!!!"];

        [Category("Readonly")]
        [ReadOnly(true)]
        public SelectableList<string> ReadonlySelectableList { get; set; } = new(["John", "David", "bodong"]);

        [Category("Readonly")]
        [ReadOnly(true)]
        public Vector3 ReadonlyVector3 { get; set; } = new(1, 2, 3);

        [Category("Readonly")]
        [ReadOnly(true)]
        public CheckedList<PlatformID> ReadonlyPlatforms { get; set; } = new(Enum.GetValues<PlatformID>(), [PlatformID.Win32NT, PlatformID.Unix]);

        [Category("Readonly")]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public LoginInfo ReadonlyLoginInfo { get; set; } = new();

        [Category("AutoCollapse")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
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
