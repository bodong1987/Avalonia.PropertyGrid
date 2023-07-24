using Avalonia.PropertyGrid.Model.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Samples.Models
{
    public class LoginInfo : MiniReactiveObject
    {
        [Watermark("Your Login Name")]
        public string UserName { get; set; }

        [PasswordPropertyText(true)]
        [Watermark("Your Password")]
        public string Password { get; set; }

        public PlatformID ServerType { get; set; } = PlatformID.Unix;

        public EncryptData EncryptPolicy { get; set; } = new EncryptData();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class EncryptData : MiniReactiveObject
    {
        public EncryptionPolicy Policy { get; set; } = EncryptionPolicy.RequireEncryption;

        public RSAEncryptionPaddingMode PaddingMode { get; set; } = RSAEncryptionPaddingMode.Pkcs1;
    }
}
