using Avalonia.PropertyGrid.Samples.Models;
using Avalonia.PropertyGrid.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using System.Globalization;

namespace Avalonia.PropertyGrid.Samples.ViewModels
{
    public class MainDemoViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        readonly SimpleObject _SimpleObject = new SimpleObject("SimpleTests");

        public SimpleObject simpleObject => _SimpleObject;

        readonly SimpleObject _SyncObject = new SimpleObject("SyncTests");
        public SimpleObject syncObject => _SyncObject;

        readonly SimpleObject _MultiObj0 = new SimpleObject("MultiObject0");
        readonly SimpleObject _MultiObj1 = new SimpleObject("MultiObject1");

        public SimpleObject multiObject0 => _MultiObj0;
        public SimpleObject multiObject1 => _MultiObj1;

        public IEnumerable<SimpleObject> multiObjects => new SimpleObject[] { multiObject0, multiObject1 };

        readonly ScriptableOptions _Options = new ScriptableOptions();
        public ScriptableOptions customOptions => _Options;

        readonly SimpleExtendsObject _SimpleExtends = new SimpleExtendsObject();

        public SimpleExtendsObject simpleExtends => _SimpleExtends;

        readonly DynamicVisibilityObject _DynamicVisiblityObject = new DynamicVisibilityObject();
        public DynamicVisibilityObject dynamicVisiblity => _DynamicVisiblityObject;

        readonly TestExtendsObject _extendsObject = new TestExtendsObject();
        public TestExtendsObject extendsObject => _extendsObject;

        readonly CancelableObject _cancelableObject = new CancelableObject("Cancelable");

        public CancelableObject cancelableObject => _cancelableObject;

        #region View
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

        bool _AllowQuickFilter = true;
        public bool AllowQuickFilter
        {
            get => _AllowQuickFilter;
            set => this.RaiseAndSetIfChanged(ref _AllowQuickFilter, value);
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

        bool _IsShowTitle = true;
        public bool IsShowTitle
        {
            get => _IsShowTitle;
            set => this.RaiseAndSetIfChanged(ref _IsShowTitle, value);
        }

        double _DefaultNameWidth = 180;
        public double DefaultNameWidth
        {
            get => _DefaultNameWidth;
            set => this.RaiseAndSetIfChanged(ref _DefaultNameWidth, value);
        }
        #endregion

        public MainDemoViewModel()
        {
            Controls.PropertyGrid.LocalizationService.AddExtraService(new SampleLocalizationService());

            GenOptions();
        }

        #region Gen Custom Object Properties
        private void GenOptions()
        {
            _Options.AddProperty(new ScriptableObject()
            {
                Name = "IntValue",
                DisplayName = "Int Value",
                Description = "Custom type = int",
                Value = 1024
            });

            _Options.AddProperty(new ScriptableObject()
            {
                Name = "BooleanValue",
                DisplayName = "Boolean Value",
                Description = "Custom type = boolean",
                Value = true
            });

            _Options.AddProperty(new ScriptableObject()
            {
                Name = "ThreeBooleanValue",
                DisplayName = "Three Boolean Value",
                Description = "Custom type = boolean?",
                Value = null,
                ValueType = typeof(bool?)
            });

            _Options.AddProperty(new ScriptableObject()
            {
                Name = "StringValue",
                DisplayName = "String Value",
                Description = "Custom type = string",
                Value = "bodong"
            });

            _Options.AddProperty(new ScriptableObject()
            {
                Name = "EnumValue",
                DisplayName = "Enum Value",
                Description = "Custom type = Enum",
                Value = Environment.OSVersion.Platform,
                ExtraAttributes = new Attribute[] { new ValidatePlatformAttribute() }
            });

            _Options.AddProperty(new ScriptableObject()
            {
                Name = "EnumValueR",
                DisplayName = "Enum Value(Readonly)",
                Description = "Custom type = Enum",
                Value = Environment.OSVersion.Platform,
                ExtraAttributes = new Attribute[] { new ReadOnlyAttribute(true) }
            });

            _Options.AddProperty(new ScriptableObject()
            {
                Name = "BindingList",
                DisplayName = "BindingList Value",
                Description = "Custom type = BindingList",
                Value = new BindingList<int>() { 1024, 2048, 4096 },
                ExtraAttributes = new Attribute[] { new CategoryAttribute("Array") }
            });

            _Options.AddProperty(new ScriptableObject()
            {
                Name = "BindingListNotEditable",
                DisplayName = "Not Editable List",
                Description = "Custom type = BindingList(Not Editable)",
                Value = new BindingList<int>() { 1024, 2048, 4096 },
                ExtraAttributes = new Attribute[] { new CategoryAttribute("Array"), new EditableAttribute(false) }
            });

            _Options.AddProperty(new ScriptableObject()
            {
                Name = "BindingListReadOnly",
                DisplayName = "ReadOnly List",
                Description = "Custom type = BindingList(Readonly)",
                Value = new BindingList<int>() { 1024, 2048, 4096 },
                ExtraAttributes = new Attribute[] { new CategoryAttribute("Array"), new ReadOnlyAttribute(true) }
            });
        }
        #endregion
    }
}
