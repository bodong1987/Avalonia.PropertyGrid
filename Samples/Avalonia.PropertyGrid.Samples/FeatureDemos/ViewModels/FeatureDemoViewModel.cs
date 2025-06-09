using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Avalonia.PropertyGrid.Samples.FeatureDemos.Models;
using PropertyModels.ComponentModel;

namespace Avalonia.PropertyGrid.Samples.FeatureDemos.ViewModels
{
    public class FeatureDemoViewModel : MiniReactiveObject
    {
        public SimpleObject SimpleObject { get; } = new("SimpleTests");
        
        public CommandHistoryViewModel CommandHistory { get; } = new();

        public SimpleObject SyncObject { get; } = new("SyncTests");

        public SimpleObject MultiObject0 { get; } = new("MultiObject0");
        public SimpleObject MultiObject1 { get; } = new("MultiObject1");

        // must use SimpleObject[]
        // ReSharper disable once UseCollectionExpression
        // ReSharper disable once RedundantExplicitArrayCreation
        public IEnumerable<SimpleObject> MultiObjects => new SimpleObject[]{MultiObject0, MultiObject1};

        public ScriptableOptions CustomOptions { get; } = new();

        public DynamicVisibilityObject DynamicVisibility { get; } = new();

        public TestExtendsObject ExtendsObject { get; } = new();

        private TestCustomObject _customObject2 = new();
        public TestCustomObject CustomObject
        {
            get => _customObject2;
            set => this.RaiseAndSetIfChanged(ref _customObject2, value);
        }

        public FeatureDemoViewModel()
        {
            GenOptions();

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
                ExtraAttributes = [new CategoryAttribute("BindingList")]
            });

            CustomOptions.AddProperty(new ScriptableObject
            {
                Name = "BindingListNotEditable",
                DisplayName = "Not Editable List",
                Description = "Custom type = BindingList(Not Editable)",
                Value = new BindingList<int> { 1024, 2048, 4096 },
                ExtraAttributes = [new CategoryAttribute("BindingList"), new EditableAttribute(false)]
            });

            CustomOptions.AddProperty(new ScriptableObject
            {
                Name = "BindingListReadOnly",
                DisplayName = "ReadOnly List",
                Description = "Custom type = BindingList(Readonly)",
                Value = new BindingList<int> { 1024, 2048, 4096 },
                ExtraAttributes = [new CategoryAttribute("BindingList"), new ReadOnlyAttribute(true)]
            });
        }
        #endregion
    }
}
