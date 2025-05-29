using System;
using System.ComponentModel;
using System.Net.Security;
using System.Security.Cryptography;
using PropertyModels.ComponentModel;

namespace Avalonia.PropertyGrid.Samples.Models
{
    public class LoginInfo : MiniReactiveObject
    {
        [Watermark("Your Login Name")]
        public string? UserName { get; set; }

        [PasswordPropertyText(true)]
        [Watermark("Your Password")]
        public string? Password { get; set; }

        [MultilineText]
        public string? HelpText { get; set; } = $"This is multiline Text{Environment.NewLine}Try edit me.";

        public PlatformID ServerType { get; set; } = PlatformID.Unix;

        public EncryptData EncryptPolicy { get; set; } = new();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class EncryptData : MiniReactiveObject
    {
        public EncryptionPolicy Policy { get; set; } = EncryptionPolicy.RequireEncryption;

        public RSAEncryptionPaddingMode PaddingMode { get; set; } = RSAEncryptionPaddingMode.Pkcs1;
    }
}
