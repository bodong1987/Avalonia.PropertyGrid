using PropertyModels.Collections;
using PropertyModels.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;
using Avalonia.Media;
using Avalonia.Platform;
using System.Runtime.Serialization;

namespace Avalonia.PropertyGrid.Samples.Models
{
    [AutoCollapseCategories("AutoCollapse")]
    public class SimpleObject : ReactiveObject
    {
        public readonly string Description;

        public SimpleObject(string description)
        {
            Description = description;

            using (var stream = AssetLoader.Open(new Uri($"avares://{GetType().Assembly.GetName().Name}/Assets/avalonia-banner.png")))
            {
                AvaloniaBanner = new Avalonia.Media.Imaging.Bitmap(stream);
            }

            foreach(var name in new string[] {"au.png", "bl.png", "ca.png", "cn.png"})
            {
                using (var stream = AssetLoader.Open(new Uri($"avares://{GetType().Assembly.GetName().Name}/Assets/country-flags/{name}")))
                {
                    var image = new Avalonia.Media.Imaging.Bitmap(stream);
                    ImageList.Add(image);
                }
            }
        }

        public override string ToString()
        {
            return $"({GetHashCode()}){Description}";
        }

        [Category("Imaging")]
        public IImage AvaloniaBanner { get; set; }

        [Category("Imaging")]
        [ImagePreviewMode(Stretch = StretchType.None)]
        public BindingList<IImage> ImageList { get; set; } = new BindingList<IImage>();

        [Category("Path")]
        [DisplayName("Target Path")]
        [PathBrowsable(Filters = "Image Files(*.jpg;*.png;*.bmp;*.tag)|*.jpg;*.png;*.bmp;*.tag")]
        [Watermark("Image Path")]
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
        public PhoneService Service { get; set; } = PhoneService.None;


        [Category("Enum")]
        [EnumPermitValues<PhoneService>(PhoneService.Cell, PhoneService.Fax)]
        public PhoneService ServiceAllowCellFax { get; set; } = PhoneService.None;

        [Category("Enum")]
        public PlatformID CurrentPlatform => Environment.OSVersion.Platform;

        [Category("Enum")]
        public PlatformID Platform { get; set; } = Environment.OSVersion.Platform;

        [Category("Enum")]
        [EnumProhibitValues<PlatformID>(PlatformID.Unix)]
        public PlatformID PlatformNoUnix { get; set; } = Environment.OSVersion.Platform;

        [Category("Enum")]
        public PlatformType EnumWithDisplayName { get; set; } = PlatformType.Windows;

        [Category("Selectable List")]
        public SelectableList<string> LoginName { get; set; } = new SelectableList<string>(new string[] { "John", "David", "bodong" }, "bodong");

        [Category("Selectable List")]
        public SelectableList<string> LoginName2 { get; set; } = new SelectableList<string>(new string[] { "John", "David", "bodong" });

        [Category("Selectable List")]
        public SelectableList<int> IdList { get; set; } = new SelectableList<int>(new int[] { 100, 1000, 1024 }, 1000);

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

        [Category("DataValidation")]
        [Required(ErrorMessage ="Can not be null")]
        public string? ValidateString { get; set; }

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

        [Category("Numeric")]
        [Range(0.1f, 10.0f)]
        public double dValue { get; set; } = 5.5f;

        [Category("Numeric")]
        public Int64 i64Value { get; set; } = 1000000000;

        [Category("Numeric")]
        public Int64 i64ValueBig { get; set; } = 583792581039233983;

        [Category("Numeric")]
        [ProgressAttribute]
        public double progressValue { get; set; } = 47;

        [Category("Numeric")]
        [Trackable(0, 100, Increment = 0.1, FormatString = "{0:0.0}")]
        public double trackableDoubleValue
        {
            get => progressValue;
            set
            {
                if(progressValue != value)
                {
                    progressValue = value;
                    RaisePropertyChanged(nameof(progressValue));
                }
            }
        }

        [Category("Numeric")]
        [Trackable(-1000, 1000, Increment = 1, FormatString = "{0:0}")]
        public int trackableIntValue { get; set; } = 10;

        [Category("Array")]
        public BindingList<string> stringList { get; set; } = new BindingList<string>() { "bodong", "china" };

        [Category("Array")]
        [DisplayName("Not Editable")]
        [Editable(false)]
        public BindingList<string> stringListNotEditable { get; set; } = new BindingList<string>() { "bodong", "china" };

        [Category("Array")]
        [DisplayName("Readonly List")]
        [ReadOnly(true)]
        public BindingList<string> stringListReadonly { get; set; } = new BindingList<string>() { "bodong", "china" };

        [Category("Array")]
        public BindingList<Boolean> boolList { get; set; } = new BindingList<bool> { true, false };

        [Category("Array")]
        public BindingList<PlatformID> enumList { get; set; } = new BindingList<PlatformID>() { PlatformID.Win32NT, PlatformID.Unix };

        [Category("Array")]
        public BindingList<Vector3> Vec3List { get; set; } = new BindingList<Vector3>() { new Vector3(1024.0f, 2048.0f, 4096.0f) };

        [Category("Checked List")]
        public CheckedList<string> CheckedListString { get; set; } = new CheckedList<string>(new string[] { "bodong", "John", "David" }, new string[] { "bodong" });

        [Category("Checked List")]
        public CheckedList<int> CheckedListInt { get; set; } = new CheckedList<int>(new int[] { 1024, 2048, 4096, 8192 }, new int[] { 1024, 8192 });

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
        public Vector3 vec3 { get; set; } = new Vector3(1, 2, 3);

        [Category("Color")]
        public System.Drawing.Color RedColor { get; set; } = System.Drawing.Color.Red;

        [Category("Color")]
        public System.Drawing.Color Color2 { get; set; } = System.Drawing.Color.FromArgb(255, 122, 50, 177);

        [Category("Color")]
        public BindingList<System.Drawing.Color> ColorList { get; set; } = new BindingList<System.Drawing.Color>() { System.Drawing.Color.Pink, System.Drawing.Color.Purple };

        [DisplayName("Login User Data")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Expandable")]
        public LoginInfo loginInfo { get; set; } = new LoginInfo();

        [Category("Font")]
        public Avalonia.Media.FontFamily FontFamily { get; set; } = new FontFamily("Courier New");

        [Category("Readonly")]        
        [PathBrowsable(PathBrowsableType.Directory)]
        [ReadOnly(true)]
        public string ReadonlyPath { get; set; } = "C:\\Windows\\System32";

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
        public BindingList<int> ReadonlyIntArray { get; set; } = new BindingList<int>() { 1, 2, 3 };

        [Category("Readonly")]
        [ReadOnly(true)]
        public BindingList<string> ReadonlyStringArray { get; set; } = new BindingList<string>() { "Hello", "World", "!!!" };

        [Category("Readonly")]
        [ReadOnly(true)]
        public SelectableList<string> ReadonlySelectableList { get; set; } = new SelectableList<string>(new string[] { "John", "David", "bodong" });

        [Category("Readonly")]
        [ReadOnly(true)]
        public Vector3 ReadonlyVector3 { get; set; } = new Vector3(1, 2, 3);

        [Category("Readonly")]
        [ReadOnly(true)]
        public CheckedList<PlatformID> ReadonlyPlatforms { get; set; } = new CheckedList<PlatformID>(Enum.GetValues(typeof(PlatformID)).Cast<PlatformID>(), new PlatformID[] {PlatformID.Win32NT, PlatformID.Unix});

        [Category("Readonly")]
        [ReadOnly(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]        
        public LoginInfo ReadonlyLoginInfo { get; set; } = new LoginInfo();

        [Category("AutoCollapse")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public LoginInfo CollapsedLoginInfo { get; set; } = new LoginInfo();
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
        CanNotSeeThis = 32,
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
