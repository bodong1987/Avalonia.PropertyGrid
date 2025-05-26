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

    public class MainViewModel : ViewModelBase
    {
        public SimpleObject SimpleObject { get; } = new("SimpleTests");

        public SimpleObject SyncObject { get; } = new("SyncTests");

        public SimpleObject MultiObject0 { get; } = new("MultiObject0");
        public SimpleObject MultiObject1 { get; } = new("MultiObject1");

        // ReSharper disable once UseCollectionExpression
        public IEnumerable<SimpleObject> MultiObjects => new SimpleObject[]{MultiObject0, MultiObject1};

        public ScriptableOptions CustomOptions { get; } = new();

        public DynamicVisibilityObject DynamicVisibility { get; } = new();

        public TestExtendsObject ExtendsObject { get; } = new();

        public CancelableObject CancelableObject { get; } = new("Cancelable");

        private readonly List<ICultureData> _allCultures = [];

        public ICultureData[] AllCultures => [.. _allCultures];

        public static string Version => $"v{typeof(Utils.FontUtils).Assembly.GetName().Version?.ToString() ?? "Unknown Version"}";

        public ICultureData CurrentCulture
        {
            get => LocalizationService.Default.CultureData;
            set
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (value != null)
                {
                    LocalizationService.Default.SelectCulture(value.Culture.Name);

                    RaisePropertyChanged(nameof(CurrentCulture));
                }
            }
        }

        private TestCustomObject _customObject2 = new();
        public TestCustomObject CustomObject
        {
            get => _customObject2;
            set => this.RaiseAndSetIfChanged(ref _customObject2, value);
        }

        #region View
        private PropertyGridDisplayMode _displayMode = PropertyGridDisplayMode.Tree;
        public PropertyGridDisplayMode DisplayMode
        {
            get => _displayMode;
            set
            {
                if (_displayMode != value)
                {
                    this.RaiseAndSetIfChanged(ref _displayMode, value);
                }
            }
        }
        
        private PropertyGridShowStyle _showStyle = PropertyGridShowStyle.Category;
        public PropertyGridShowStyle ShowStyle
        {
            get => _showStyle;
            set
            {
                if (_showStyle != value)
                {
                    this.RaiseAndSetIfChanged(ref _showStyle, value);
                }
            }
        }

        private bool _allowFilter = true;
        public bool AllowFilter
        {
            get => _allowFilter;
            set => this.RaiseAndSetIfChanged(ref _allowFilter, value);
        }

        private bool _allowQuickFilter = true;
        public bool AllowQuickFilter
        {
            get => _allowQuickFilter;
            set => this.RaiseAndSetIfChanged(ref _allowQuickFilter, value);
        }

        private bool _isShowTitle = true;
        public bool IsShowTitle
        {
            get => _isShowTitle;
            set => this.RaiseAndSetIfChanged(ref _isShowTitle, value);
        }

        private bool _isAllExpanded = true;

        public bool IsAllExpanded
        {
            get => _isAllExpanded;
            set => this.RaiseAndSetIfChanged(ref _isAllExpanded, value);
        }

        private bool _isReadOnly;
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => this.RaiseAndSetIfChanged(ref _isReadOnly, value);
        }

        private double _defaultNameWidth = 280;
        public double DefaultNameWidth
        {
            get => _defaultNameWidth;
            set => this.RaiseAndSetIfChanged(ref _defaultNameWidth, value);
        }
        
        private PropertyOperationVisibility _showPropertyOperation = PropertyOperationVisibility.Default;
        public PropertyOperationVisibility ShowPropertyOperation
        {
            get => _showPropertyOperation;
            set => this.RaiseAndSetIfChanged(ref _showPropertyOperation, value);
        }
        #endregion

        public MainViewModel()
        {
            LocalizationService.Default.AddExtraService(new SampleLocalizationService());

            GenOptions();

            _allCultures.AddRange(LocalizationService.Default.GetCultures());

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
            CustomOptions.AddProperty(new ScriptableObject
            {
                Name = "IntValue",
                DisplayName = "Int Value",
                Description = "Custom type = int",
                Value = 1024
            });

            CustomOptions.AddProperty(new ScriptableObject
            {
                Name = "BooleanValue",
                DisplayName = "Boolean Value",
                Description = "Custom type = boolean",
                Value = true
            });

            CustomOptions.AddProperty(new ScriptableObject
            {
                Name = "ThreeBooleanValue",
                DisplayName = "Three Boolean Value",
                Description = "Custom type = boolean?",
                Value = null,
                ValueType = typeof(bool?)
            });

            CustomOptions.AddProperty(new ScriptableObject
            {
                Name = "StringValue",
                DisplayName = "String Value",
                Description = "Custom type = string",
                Value = "bodong"
            });

            CustomOptions.AddProperty(new ScriptableObject
            {
                Name = "EnumValue",
                DisplayName = "Enum Value",
                Description = "Custom type = Enum",
                Value = Environment.OSVersion.Platform,
                ExtraAttributes = [new ValidatePlatformAttribute()]
            });

            CustomOptions.AddProperty(new ScriptableObject
            {
                Name = "EnumValueR",
                DisplayName = "Enum Value(Readonly)",
                Description = "Custom type = Enum",
                Value = Environment.OSVersion.Platform,
                ExtraAttributes = [new ReadOnlyAttribute(true)]
            });

            CustomOptions.AddProperty(new ScriptableObject
            {
                Name = "BindingList",
                DisplayName = "BindingList Value",
                Description = "Custom type = BindingList",
                Value = new BindingList<int> { 1024, 2048, 4096 },
                ExtraAttributes = [new CategoryAttribute("Array")]
            });

            CustomOptions.AddProperty(new ScriptableObject
            {
                Name = "BindingListNotEditable",
                DisplayName = "Not Editable List",
                Description = "Custom type = BindingList(Not Editable)",
                Value = new BindingList<int> { 1024, 2048, 4096 },
                ExtraAttributes = [new CategoryAttribute("Array"), new EditableAttribute(false)]
            });

            CustomOptions.AddProperty(new ScriptableObject
            {
                Name = "BindingListReadOnly",
                DisplayName = "ReadOnly List",
                Description = "Custom type = BindingList(Readonly)",
                Value = new BindingList<int> { 1024, 2048, 4096 },
                ExtraAttributes = [new CategoryAttribute("Array"), new ReadOnlyAttribute(true)]
            });
        }
        #endregion
    }
}
