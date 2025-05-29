using System.ComponentModel;
using Avalonia.Media.Imaging;
using PropertyModels.ComponentModel.DataAnnotations;

namespace Avalonia.PropertyGrid.Samples.SettingsDemo.Models;

public class AboutSettings
{
    [Category("System Info")]
    [DisplayName("OS Version")]
    [ReadOnly(true)]
    public string OsVersion { get; } = "Windows 10 Build 19044";

    [Category("System Info")]
    [DisplayName("Manufacturer")]
    [ReadOnly(true)]
    public string Manufacturer { get; } = "Contoso Electronics";

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