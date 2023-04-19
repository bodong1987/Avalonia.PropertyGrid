using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Implements;
using Avalonia.PropertyGrid.Model.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.ViewModels
{
    internal class PropertyGridViewModel : ReactiveObject
    {
        public IPropertyGridFilterPattern FilterPattern { get; set; } = new PropertyGridFilterPattern();

        object _SelectedObject;
        public object SelectedObject
        {
            get => _SelectedObject;
            set => this.RaiseAndSetIfChanged(ref _SelectedObject, value);
        }

        bool _ShowCategory = true;

        public bool ShowCategory
        {
            get => _ShowCategory;
            set => this.RaiseAndSetIfChanged(ref _ShowCategory, value);
        }

    }
}
