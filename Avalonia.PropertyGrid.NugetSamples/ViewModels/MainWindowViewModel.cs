using Avalonia.PropertyGrid.Model.Collections;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations;
using Avalonia.PropertyGrid.Model.Extensions;
using Avalonia.PropertyGrid.ViewModels;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography;

namespace Avalonia.PropertyGrid.NugetSamples.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        readonly TestObject _SimpleObject = new TestObject();
        public TestObject simpleObject => _SimpleObject;

        PropertyGridShowStyle _ShowStyle = PropertyGridShowStyle.Category;

        public PropertyGridShowStyle ShowStyle
        {
            get => _ShowStyle;
            set
            {
                if (_ShowStyle != value)
                {
                    this.RaiseAndSetIfChanged(ref _ShowStyle, value);

                    this.RaisePropertyChanged(nameof(IsShowCategory));
                }
            }
        }

        bool _AllowFilter = true;
        public bool AllowFilter
        {
            get => _AllowFilter;
            set => this.RaiseAndSetIfChanged(ref _AllowFilter, value);
        }

        public bool IsShowCategory
        {
            get => ShowStyle == PropertyGridShowStyle.Category;
            set
            {
                PropertyGridShowStyle newStyle = value ? PropertyGridShowStyle.Category : PropertyGridShowStyle.Alphabetic;

                if (ShowStyle != newStyle)
                {
                    ShowStyle = newStyle;
                    this.RaisePropertyChanged(nameof(IsShowCategory));
                }
            }
        }

        public MainWindowViewModel()
        {

        }
    }

    public class TestObject : Avalonia.PropertyGrid.Model.ComponentModel.ReactiveObject
    {
        #region BuiltIn Datas
        [Category("BuiltIn")]
        public bool BooleanValue { get; set; }

        [Category("BuiltIn")]
        public bool? BooleanValueNullable { get; set; }

        [Category("BuiltIn")]
        public int IntValue { get; set; }

        [Category("BuiltIn")]
        public double DoubleValue { get; set; }

        [Category("BuiltIn")]
        public System.Int64 Int64Value { get; set; }

        [Category("BuiltIn")]
        public string StrValue { get; set; } = "bodong";

        [Category("BuiltIn")]
        [PathBrowsable(Filters = "Image Files(*.jpg;*.png;*.bmp;*.tag)|*.jpg;*.png;*.bmp;*.tag")]
        public string ImagePath { get; set; } = "";

        [Category("BuiltIn")]
        public PlatformID EnumValue { get; set; } = Environment.OSVersion.Platform;

        [Category("BuiltIn")]
        public PropertyVisibility FlagsEnumValue { get; set; } = PropertyVisibility.AlwaysVisible;

        [Category("BuiltIn")]
        public DateTime DateTimeValue { get; set; } = DateTime.Now;

        [Category("BuiltIn")]
        public DateTimeOffset DateTimeOffsetValue { get; set; } = DateTimeOffset.Now;

        [Category("BuiltIn")]
        public DateTimeOffset? DateTimeOffsetValueNullable { get; set; }

        [Category("BuiltIn")]
        public TimeSpan TimeSpanValue { get; set; } = DateTime.Now.TimeOfDay;

        [Category("BuiltIn")]
        public TimeSpan? TimeSpanValueNullable { get; set; } = DateTime.Now.TimeOfDay;

        [Category("BuiltIn")]
        public BindingList<string> StringList { get; set; } = new BindingList<string>() { "bodong", "china" };

        [Category("BuiltIn")]
        public BindingList<PlatformID> EnumList { get; set; } = new BindingList<PlatformID>() { PlatformID.Win32NT, PlatformID.Win32Windows };

        [Category("BuiltIn")]
        [Editable(false)]
        public BindingList<int> IntList { get; set; } = new BindingList<int> { 1, 2, 3 };

        [Category("BuiltIn")]
        public CheckedList<string> CheckedListValue { get; set; } = new CheckedList<string>(new string[] { "bodong", "John", "David" }, new string[] {"bodong"});

        [Category("BuiltIn")]
        public SelectableList<PlatformID> SelectableListValue { get; set; } = new SelectableList<PlatformID>(new PlatformID[] { PlatformID.Win32Windows, PlatformID.Unix, PlatformID.Other }, PlatformID.Unix);
        #endregion

        #region Data Validation
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

        [Category("DataValidation")]
        [Description("Select platforms")]
        [ValidatePlatform]
        public CheckedList<PlatformID> Platforms { get; set; } = new CheckedList<PlatformID>(Enum.GetValues(typeof(PlatformID)).Cast<PlatformID>());

        [Category("DataValidation")]
        [Range(10, 200)]
        public int IntValue10To200 { get; set; } = 100;
        #endregion

        #region Expandable
        [DisplayName("Expand Object")]
        [Category("Expandable")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public LoginInfo loginInfo { get; set; } = new LoginInfo();
        #endregion
    }

    public class LoginInfo : MiniReactiveObject
    {
        public string UserName { get; set; }

        [PasswordPropertyText(true)]
        public string Password { get; set; }

        public PlatformID ServerType { get; set; } = PlatformID.Unix;

        public EncryptData EncryptPolicy { get; set; } = new EncryptData();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class EncryptData : MiniReactiveObject
    {
        public EncryptionPolicy Policy { get; set; } = EncryptionPolicy.RequireEncryption;

        public RSAEncryptionPaddingMode PaddingMode { get; set; } = RSAEncryptionPaddingMode.Pkcs1;
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