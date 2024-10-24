using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyModels.Extensions;

namespace Avalonia.PropertyGrid.Samples.Models
{
    public class DynamicVisibilityObject : ReactiveObject
    {
        [ConditionTarget]
        public bool IsShowPath { get; set; } = true;

        [VisibilityPropertyCondition(nameof(IsShowPath), true)]
        [PathBrowsable(Filters = "Image Files(*.jpg;*.png;*.bmp;*.tag)|*.jpg;*.png;*.bmp;*.tag")]
        public string Path { get; set; } = "";

        [ConditionTarget]
        public PlatformID Platform { get; set; } = PlatformID.Win32NT;

        [VisibilityPropertyCondition(nameof(Platform), PlatformID.Unix)]
        [ConditionTarget]
        public string UnixVersion { get; set; } = "";

        // show more complex conditions...
        [Browsable(false)]
        [DependsOnProperty(nameof(IsShowPath), nameof(Platform), nameof(UnixVersion))]
        [ConditionTarget]
        public bool IsShowUnixLoginInfo => IsShowPath && Platform == PlatformID.Unix && UnixVersion.IsNotNullOrEmpty();

        [VisibilityPropertyCondition(nameof(IsShowUnixLoginInfo), true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public LoginInfo unixLogInInfo { get; set; } = new LoginInfo();
    }
}
