using System.ComponentModel;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;

namespace Avalonia.PropertyGrid.Samples.SettingsDemo.Models;

public class AboutSettings : ReactiveObject
{
    [Category("System Info")]
    [DisplayName("OS Version")]
    [ReadOnly(true)]
#pragma warning disable CA1822
    public string OsVersion => "Windows 10 Build 19044";
#pragma warning restore CA1822

    [Category("System Info")]
    [DisplayName("Manufacturer")]
    [ReadOnly(true)]
#pragma warning disable CA1822
    public string Manufacturer => "Contoso Electronics";
#pragma warning restore CA1822

    [Category("License")]
    [DisplayName("Activation Status")]
    [Description("Check license status online")]
    [PathBrowsable(Filters = "Web Link|*.url")]
    public string LicenseUrl { get; set; } = "https://contoso.com/activation";

    [Category("Support")]
    [DisplayName("Technical Support")]
    [PathBrowsable(Filters = "Web Link|*.url")]
    public string SupportUrl { get; set; } = "https://support.contoso.com";

}