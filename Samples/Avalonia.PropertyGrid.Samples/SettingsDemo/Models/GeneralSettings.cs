using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PropertyModels.Collections;

namespace Avalonia.PropertyGrid.Samples.SettingsDemo.Models;

public class GeneralSettings
{
    [Category("Update")]
    [DisplayName("Auto Update")]
    [Description("Enable automatic system updates")]
    public bool EnableAutoUpdate { get; set; } = true;

    [Category("Update")]
    [DisplayName("Update Interval")]
    [Range(1, 30)]
    public int UpdateCheckIntervalDays { get; set; } = 7;

    [Category("Region")]
    [DisplayName("Time Zone")]
    public SelectableList<string> TimeZone { get; set; } = new (["UTC", "UTC+08:00", "UTC-05:00", "UTC+01:00"], "UTC+08:00");
}