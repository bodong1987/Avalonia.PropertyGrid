using Avalonia.PropertyGrid.Model.Collections;
using Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations;
using Avalonia.PropertyGrid.Model.Extensions;
using System;
using System.ComponentModel;
using System.Linq;

namespace Avalonia.PropertyGrid.Samples.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        readonly SimpleObject _SimpleObject = new SimpleObject();

        public SimpleObject simpleObject => _SimpleObject;
    }

    public class SimpleObject
    {
        [DisplayName("Target Name")]
        public string Name { get; set; }

        [DisplayName("Target Path")]
        [PathBrowsable(Filters = "Image Files(*.jpg;*.png;*.bmp;*.tag)|*.jpg;*.png;*.bmp;*.tag")]
        public string Path { get; set; }

        [Category("Id")]
        public string UUID { get; set; } = Guid.NewGuid().ToString();

        [Category("Id")]
        public bool EncryptData { get; set; } = true;

        [Category("Id")]
        public bool SafeMode { get; set; } = false;

        [Category("Id")]
        public bool? ThreeStates { get; set; } = null;

        [Category("Id")]
        public PhoneService Service { get; set; } = PhoneService.None;

        [Category("System")]
        public PlatformID CurrentPlatform => Environment.OSVersion.Platform;

        [Category("System")]
        public PlatformID Platform { get; set; } = Environment.OSVersion.Platform;

        [Category("System")]
        [Description("Select platforms")]
        public CheckedList<PlatformID> Platforms { get; set; } = new CheckedList<PlatformID>(Enum.GetValues(typeof(PlatformID)).Cast<PlatformID>());

        [Category("System")]
        public SelectableList<string> LoginName { get; set; } = new SelectableList<string>(new string[] { "John", "David", "bodong" }, "bodong");

        [Category("System")]
        [PasswordPropertyText(true)]
        public string Password { get; set; }

        string _SourceImagePath;
        [Category("DataValidation")]
        [PathBrowsable(Filters = "Image Files(*.jpg;*.png;*.bmp;*.tag)|*.jpg;*.png;*.bmp;*.tag")]
        public string SourceImagePath
        {
            get => _SourceImagePath;
            set
            {
                if(value.IsNullOrEmpty())
                {
                    throw new ArgumentNullException(nameof(SourceImagePath));
                }

                if(!System.IO.Path.GetExtension(value).iEquals(".png"))
                {
                    throw new ArgumentException($"{nameof(SourceImagePath)} must be .png file.");
                }

                _SourceImagePath = value;
            }
        }
    }

    [Flags]
    public enum PhoneService
    {
        None = 0,
        LandLine = 1,
        Cell = 2,
        Fax = 4,
        Internet = 8,
        Other = 16
    }
}