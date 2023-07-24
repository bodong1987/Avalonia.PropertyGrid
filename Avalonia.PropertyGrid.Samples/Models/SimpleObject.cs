using Avalonia.PropertyGrid.Model.Collections;
using Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Model.Extensions;
using Avalonia.Media;

namespace Avalonia.PropertyGrid.Samples.Models
{
    public class SimpleObject : ReactiveObject
    {
        public readonly string Description;

        public SimpleObject(string description)
        {
            Description = description;
        }

        public override string ToString()
        {
            return $"({GetHashCode()}){Description}";
        }

        [Category("String")]
        [DisplayName("Target Name")]
        [Watermark("Your Target Name")]
        public string Name { get; set; }

        [Category("String")]
        [DisplayName("Target Path")]
        [PathBrowsable(Filters = "Image Files(*.jpg;*.png;*.bmp;*.tag)|*.jpg;*.png;*.bmp;*.tag")]
        [Watermark("Image Path")]
        public string Path { get; set; }

        [Category("String")]
        public string UUID { get; set; } = Guid.NewGuid().ToString();

        [Category("String")]
        [PasswordPropertyText(true)]
        public string Password { get; set; }

        [Category("Boolean")]
        public bool EncryptData { get; set; } = true;

        [Category("Boolean")]
        public bool SafeMode { get; set; } = false;

        [Category("Boolean")]
        public bool? ThreeStates { get; set; } = null;

        [Category("Enum")]
        public PhoneService Service { get; set; } = PhoneService.None;

        [Category("Enum")]
        public PlatformID CurrentPlatform => Environment.OSVersion.Platform;

        [Category("Enum")]
        public PlatformID Platform { get; set; } = Environment.OSVersion.Platform;

        [Category("Selectable List")]
        public SelectableList<string> LoginName { get; set; } = new SelectableList<string>(new string[] { "John", "David", "bodong" }, "bodong");

        [Category("Selectable List")]
        public SelectableList<int> IdList { get; set; } = new SelectableList<int>(new int[] { 100, 1000, 1024 }, 1000);

        string _SourceImagePath;
        [Category("DataValidation")]
        [PathBrowsable(Filters = "Image Files(*.jpg;*.png;*.bmp;*.tag)|*.jpg;*.png;*.bmp;*.tag")]
        [Watermark("This path can be validated")]
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

        [Category("Binding List")]
        public BindingList<string> stringList { get; set; } = new BindingList<string>() { "bodong", "china" };

        [Category("Binding List")]
        [DisplayName("Not Editable")]
        [Editable(false)]
        public BindingList<string> stringListNotEditable { get; set; } = new BindingList<string>() { "bodong", "china" };

        [Category("Binding List")]
        [DisplayName("Readonly List")]
        [ReadOnly(true)]
        public BindingList<string> stringListReadonly { get; set; } = new BindingList<string>() { "bodong", "china" };

        [Category("Binding List")]
        public BindingList<Boolean> boolList { get; set; } = new BindingList<bool> { true, false };

        [Category("Binding List")]
        public BindingList<PlatformID> enumList { get; set; } = new BindingList<PlatformID>() { PlatformID.Win32NT, PlatformID.Unix };

        [Category("Binding List")]
        public BindingList<Vector3> Vec3List { get; set; } = new BindingList<Vector3>() { new Vector3(1024.0f, 2048.0f, 4096.0f) };

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

        public Vector3 vec3 { get; set; } = new Vector3(1, 2, 3);

        [Category("Color")]
        public System.Drawing.Color RedColor { get; set; } = System.Drawing.Color.Red;

        [Category("Color")]
        public System.Drawing.Color Color2 { get; set; } = System.Drawing.Color.FromArgb(255, 122, 50, 177);

        [Category("Color")]
        public BindingList<System.Drawing.Color> ColorList { get; set; } = new BindingList<System.Drawing.Color>() { System.Drawing.Color.Pink, System.Drawing.Color.Purple };

        [DisplayName("Login User Data")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public LoginInfo loginInfo { get; set; } = new LoginInfo();
    }

    [Flags]
    public enum PhoneService
    {
        None = 0,
        LandLine = 1,
        Cell = 2,
        Fax = 4,
        Internet = 8,
        Other = 16
    }

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
}
