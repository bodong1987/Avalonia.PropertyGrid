using Avalonia.PropertyGrid.Model.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Samples.Models
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Vector3 : ReactiveObject
    {
        public double _X,_Y, _Z;

        public double X
        {
            get => _X;
            set => this.RaiseAndSetIfChanged(ref _X, value);
        }

        public double Y
        {
            get => _Y;
            set => this.RaiseAndSetIfChanged( ref _Y, value);
        }

        public double Z
        {
            get => _Z;
            set => this.RaiseAndSetIfChanged(ref _Z, value);
        }

        public Vector3()
        {
        }

        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
