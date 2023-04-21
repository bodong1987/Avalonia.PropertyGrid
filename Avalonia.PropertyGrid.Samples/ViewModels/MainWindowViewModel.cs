using Avalonia.PropertyGrid.Samples.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Avalonia.PropertyGrid.Samples.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
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

        readonly BooleanExtensionObject _BooleanExtension = new BooleanExtensionObject();

        public BooleanExtensionObject booleanExtension => _BooleanExtension;

        public MainWindowViewModel()
        {
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
                ExtraAttributes = new Attribute[] { new CategoryAttribute("BindingList") }
            });

            _Options.AddProperty(new ScriptableObject()
            {
                Name = "BindingListNotEditable",
                DisplayName = "Not Editable List",
                Description = "Custom type = BindingList(Not Editable)",
                Value = new BindingList<int>() { 1024, 2048, 4096 },
                ExtraAttributes = new Attribute[] { new CategoryAttribute("BindingList"), new EditableAttribute(false) }
            });

            _Options.AddProperty(new ScriptableObject()
            {
                Name = "BindingListReadOnly",
                DisplayName = "ReadOnly List",
                Description = "Custom type = BindingList(Readonly)",
                Value = new BindingList<int>() { 1024, 2048, 4096 },
                ExtraAttributes = new Attribute[] { new CategoryAttribute("BindingList"), new ReadOnlyAttribute(true) }
            });
        }
        #endregion
    }
}