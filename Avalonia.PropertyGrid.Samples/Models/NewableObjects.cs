using Avalonia.PropertyGrid.Model.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Samples.Models
{
    public class IntVector1 : MiniReactiveObject, ICloneable
    {
        public int X { get; set; }

        public virtual object Clone()
        {
            return new IntVector1() { X = X };
        }
    }

    public class IntVector2 : IntVector1
    {
        public int Y { get; set; }

        public override object Clone()
        {
            return new IntVector2()
            {
                X = X,
                Y = Y
            };
        }
    }

    public class IntVector3 : IntVector2
    {
        public int Z { get; set; }

        public override object Clone()
        {
            return new IntVector3()
            {
                X = X,
                Y = Y,
                Z = Z
            };
        }
    }

    class IntVectorElementFactory : CloneableObjectElementFactory<IntVector1>
    {
        public IntVectorElementFactory()
        {
            AutoCollect(GetType().Assembly);
        }
    }
}
