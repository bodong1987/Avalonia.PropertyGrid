using System.ComponentModel;
using PropertyModels.ComponentModel;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Avalonia.PropertyGrid.Samples.FeatureDemos.Models
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Vector3 : MiniReactiveObject
    {
        private double _x, _y, _z;

        public double X
        {
            get => _x;
            set => this.RaiseAndSetIfChanged(ref _x, value);
        }

        public double Y
        {
            get => _y;
            set => this.RaiseAndSetIfChanged(ref _y, value);
        }

        public double Z
        {
            get => _z;
            set => this.RaiseAndSetIfChanged(ref _z, value);
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

        public override string ToString() => $"{X},{Y},{Z}";
    }
}
