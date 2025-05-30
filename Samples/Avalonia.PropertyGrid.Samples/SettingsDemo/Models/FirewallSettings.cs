using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PropertyModels.Collections;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;

namespace Avalonia.PropertyGrid.Samples.SettingsDemo.Models;

public class FirewallSettings : ReactiveObject
{
    [Category("General")]
    [DisplayName("Firewall Status")]
    public bool IsEnabled { get; set; } = true;

    [Category("Notifications")]
    [DisplayName("Block Notifications")]
    public bool BlockAllNotifications { get; set; }

    [Category("Rules")]
    [DisplayName("Allowed Ports")]
    public CheckedList<int> AllowedPorts { get; set; } = new( [80, 443] );

    [Category("Rules")]
    [DisplayName("Custom Port Range")]
    [Range(1, 65535)]
    public int CustomPortStart { get; set; } = 8000;
    
    [Range(1, 65535)]
    public int CustomPortEnd { get; set; } = 9000;

    [Category("Advanced")]
    [DisplayName("Allowed IPs")]
    [Description("CIDR format (e.g. 192.168.1.0/24)")]
    [CIDRValidation]
    public BindingList<string> AllowedIPs { get; set; } = ["192.168.1.0/24"];

    [Category("Logging")]
    [DisplayName("Enable Logging")]
    public bool EnableLogging { get; set; } = true;

    [Category("Logging")]
    [DisplayName("Log Retention (days)")]
    [Range(1, 365)]
    [VisibilityPropertyCondition(nameof(EnableLogging), true)]
    public int LogRetentionDays { get; set; } = 30;
}

// ReSharper disable once InconsistentNaming
public class CIDRValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is string cidr)
        {
#pragma warning disable SYSLIB1045
            if (!System.Text.RegularExpressions.Regex.IsMatch(cidr, @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}/\d{1,2}$"))
#pragma warning restore SYSLIB1045
            {
                return new ValidationResult("Invalid CIDR format");
            }
        }
        return ValidationResult.Success;
    }
}