using System;
using System.ComponentModel;
using PropertyModels.ComponentModel;

namespace Avalonia.PropertyGrid.Samples.SettingsDemo.Models;

public class AccountSettings : ReactiveObject
{
    [Category("Credentials")]
    [DisplayName("Username")]
    [ReadOnly(true)]
    public string Username { get; set; } = "Admin";

    [Category("Credentials")]
    [DisplayName("Password")]
    [PasswordPropertyText(true)]
    public string Password { get; set; } = "********";

    [Category("Security")]
    [DisplayName("2FA Enabled")]
    public bool TwoFactorEnabled { get; set; }

    [Category("Security")]
    [DisplayName("Last Login")]
    public DateTime LastLogin { get; set; } = DateTime.Now.AddDays(-2);
}