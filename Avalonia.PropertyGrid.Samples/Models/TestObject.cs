using Avalonia.PropertyGrid.Model.Collections;
using Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations;
using Avalonia.PropertyGrid.ViewModels;
using Avalonia.PropertyGrid.Model.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Samples.Models
{
    public class TestObject : Avalonia.PropertyGrid.Model.ComponentModel.ReactiveObject
    {
        #region BuiltIn Datas
        [Category("BuiltIn")]
        public bool BooleanValue { get; set; }

        [Category("BuiltIn")]
        [Browsable(false)]
        public bool BooleanValueHidden { get; set; }

        [Category("BuiltIn")]
        [Browsable(true)]
        [ReadOnly(true)]
        public bool BooleanValueReadonly { get; set; } = true;

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
        public CheckedList<string> CheckedListValue { get; set; } = new CheckedList<string>(new string[] { "bodong", "John", "David" }, new string[] { "bodong" });

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
}
