using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Samples.Models
{
    public class DynamicVisibilityObject : MiniReactiveObject
    {
        [ConditionTarget]
        public bool IsShowPath { get; set; } = true;

        [VisibilityPropertyCondition(nameof(IsShowPath), true)]
        [PathBrowsable(Filters = "Image Files(*.jpg;*.png;*.bmp;*.tag)|*.jpg;*.png;*.bmp;*.tag")]
        public string Path { get; set; } = "";

        [ConditionTarget]
        public PlatformID Platform { get; set; } = PlatformID.Win32NT;

        [VisibilityPropertyCondition(nameof(Platform), PlatformID.Unix)]
        public string UnixVersion { get; set; } = "";
    }
}
