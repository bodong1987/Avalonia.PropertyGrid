using Avalonia.PropertyGrid.Model.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Samples.Models
{
    public struct SVector3
    {
        public float x, y, z;

        public override string ToString()
        {
            return string.Format("{0:0.0}, {1:0.0}, {2:0.0}", x, y, z);
        }
    }

    public class TestExtendsObject : MiniReactiveObject
    {
        public Vector3 vec3Object { get; set; } = new Vector3();

        public SVector3 vec3Struct { get; set; }

        public BindingList<SVector3> vec3BindingList { get; set; } = new BindingList<SVector3>()
        {
            new SVector3(){ x = 7.8f, y = 3.14f, z = 0.0f }
        };
    }
}
