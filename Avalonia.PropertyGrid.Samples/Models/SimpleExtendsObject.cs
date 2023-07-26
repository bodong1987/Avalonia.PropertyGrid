using Avalonia.Platform;
using Avalonia.PropertyGrid.Model.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Samples.Models
{
    public class SimpleExtendsObject : MiniReactiveObject
    {
        public bool toggleAble { get; set; } = true;

        public bool disableAble { get; set; } = false;

        public bool? threeState { get; set; }

        [ReadOnly(true)]
        public bool readonlyBoolean { get; set; }

        public bool readonlyBoolean2 => true;
    }
}
