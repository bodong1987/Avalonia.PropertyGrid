using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Avalonia.PropertyGrid.Samples.Models;
using Avalonia.PropertyGrid.Services;
using Avalonia.PropertyGrid.ViewModels;
using PropertyModels.ComponentModel;
using PropertyModels.Localization;

namespace Avalonia.PropertyGrid.Samples.ViewModels
{
    public enum ThemeType
    {
        Fluent,
        Simple
    }

    public partial class MainViewModel : ViewModelBase
    {
        public SimpleObject simpleObject { get; } = new SimpleObject("SimpleTests");

        public SimpleObject syncObject { get; } = new SimpleObject("SyncTests");

        public SimpleObject multiObject0 { get; } = new SimpleObject("MultiObject0");
        public SimpleObject multiObject1 { get; } = new SimpleObject("MultiObject1");

        public IEnumerable<SimpleObject> multiObjects => [multiObject0, multiObject1];

        public ScriptableOptions customOptions { get; } = new ScriptableOptions();

        public DynamicVisibilityObject dynamicVisiblity { get; } = new DynamicVisibilityObject();

        public TestExtendsObject extendsObject { get; } = new TestExtendsObject();

        public CancelableObject cancelableObject { get; } = new CancelableObject("Cancelable");

        private readonly List<ICultureData> _AllCultures = [];

        public ICultureData[] AllCultures => [.. _AllCultures];

        public static string Version => $"v{typeof(Utils.FontUtils).Assembly.GetName().Version?.ToString() ?? "Unknown Version"}";

        public ICultureData CurrentCulture
        {
            get => LocalizationService.Default.CultureData;
            set
            {
                if (value != null)
                {
                    LocalizationService.Default.SelectCulture(value.Culture.Name);

                    this.RaisePropertyChanged(nameof(CurrentCulture));
                }
            }
        }

        private TestCustomObject _CustomObject2 = new();
        public TestCustomObject customObject
        {
            get => _CustomObject2;
            set => this.RaiseAndSetIfChanged(ref _CustomObject2, value);
        }

        #region View
        private PropertyGridShowStyle _ShowStyle = PropertyGridShowStyle.Category;

        public PropertyGridShowStyle ShowStyle
        {
            get => _ShowStyle;
            set
            {
                if (_ShowStyle != value)
                {
                    this.RaiseAndSetIfChanged(ref _ShowStyle, value);
                }
            }
        }

        private bool _AllowFilter = true;
        public bool AllowFilter
        {
            get => _AllowFilter;
            set => this.RaiseAndSetIfChanged(ref _AllowFilter, value);
        }

        private bool _AllowQuickFilter = true;
        public bool AllowQuickFilter
        {
            get => _AllowQuickFilter;
            set => this.RaiseAndSetIfChanged(ref _AllowQuickFilter, value);
        }

        private bool _IsShowTitle = true;
        public bool IsShowTitle
        {
            get => _IsShowTitle;
            set => this.RaiseAndSetIfChanged(ref _IsShowTitle, value);
        }

        private bool _IsReadOnly = false;
        public bool IsReadOnly
        {
            get => _IsReadOnly;
            set => this.RaiseAndSetIfChanged(ref _IsReadOnly, value);
        }

        private double _DefaultNameWidth = 280;
        public double DefaultNameWidth
        {
            get => _DefaultNameWidth;
            set => this.RaiseAndSetIfChanged(ref _DefaultNameWidth, value);
        }
        #endregion

        public MainViewModel()
        {
            LocalizationService.Default.AddExtraService(new SampleLocalizationService());

            GenOptions();

            _AllCultures.AddRange(LocalizationService.Default.GetCultures());

            var propertyManager = new DynamicPropertyManager<TestCustomObject>();
            propertyManager.Properties.Add(
                DynamicPropertyManager<TestCustomObject>.CreateProperty<TestCustomObject, string>(
                    "StringArray0",
                    x => x!.StringArray[0],
                    (x, v) => x!.StringArray[0] = v!,
                    []
                    )
                );

            propertyManager.Properties.Add(
                DynamicPropertyManager<TestCustomObject>.CreateProperty<TestCustomObject, string>(
                    "StringArray1",
                    x => x!.StringArray[1],
                    (x, v) => x!.StringArray[1] = v!,
                    []
                    )
                );

            propertyManager.Properties.Add(
                DynamicPropertyManager<TestCustomObject>.CreateProperty<TestCustomObject, string>(
                    "StringArray2",
                    x => x!.StringArray[2],
                    (x, v) => x!.StringArray[2] = v!,
                    [
                        new ReadOnlyAttribute(true)
                    ]
                    )
                );
        }

        #region Gen Custom Object Properties
        private void GenOptions()
        {
            customOptions.AddProperty(new ScriptableObject()
            {
                Name = "IntValue",
                DisplayName = "Int Value",
                Description = "Custom type = int",
                Value = 1024
            });

            customOptions.AddProperty(new ScriptableObject()
            {
                Name = "BooleanValue",
                DisplayName = "Boolean Value",
                Description = "Custom type = boolean",
                Value = true
            });

            customOptions.AddProperty(new ScriptableObject()
            {
                Name = "ThreeBooleanValue",
                DisplayName = "Three Boolean Value",
                Description = "Custom type = boolean?",
                Value = null,
                ValueType = typeof(bool?)
            });

            customOptions.AddProperty(new ScriptableObject()
            {
                Name = "StringValue",
                DisplayName = "String Value",
                Description = "Custom type = string",
                Value = "bodong"
            });

            customOptions.AddProperty(new ScriptableObject()
            {
                Name = "EnumValue",
                DisplayName = "Enum Value",
                Description = "Custom type = Enum",
                Value = Environment.OSVersion.Platform,
                ExtraAttributes = [new ValidatePlatformAttribute()]
            });

            customOptions.AddProperty(new ScriptableObject()
            {
                Name = "EnumValueR",
                DisplayName = "Enum Value(Readonly)",
                Description = "Custom type = Enum",
                Value = Environment.OSVersion.Platform,
                ExtraAttributes = [new ReadOnlyAttribute(true)]
            });

            customOptions.AddProperty(new ScriptableObject()
            {
                Name = "BindingList",
                DisplayName = "BindingList Value",
                Description = "Custom type = BindingList",
                Value = new BindingList<int>() { 1024, 2048, 4096 },
                ExtraAttributes = [new CategoryAttribute("Array")]
            });

            customOptions.AddProperty(new ScriptableObject()
            {
                Name = "BindingListNotEditable",
                DisplayName = "Not Editable List",
                Description = "Custom type = BindingList(Not Editable)",
                Value = new BindingList<int>() { 1024, 2048, 4096 },
                ExtraAttributes = [new CategoryAttribute("Array"), new EditableAttribute(false)]
            });

            customOptions.AddProperty(new ScriptableObject()
            {
                Name = "BindingListReadOnly",
                DisplayName = "ReadOnly List",
                Description = "Custom type = BindingList(Readonly)",
                Value = new BindingList<int>() { 1024, 2048, 4096 },
                ExtraAttributes = [new CategoryAttribute("Array"), new ReadOnlyAttribute(true)]
            });
        }
        #endregion
    }
}
