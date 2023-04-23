using Avalonia.PropertyGrid.Model.Collections;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations;
using Avalonia.PropertyGrid.Model.Extensions;
using Avalonia.PropertyGrid.Samples.Models;
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
}