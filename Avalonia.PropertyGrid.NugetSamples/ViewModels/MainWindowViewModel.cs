using Avalonia.PropertyGrid.Model.Collections;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.ViewModels;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
    }

}