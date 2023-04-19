using Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel;

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
        public PlatformID Platform { get; set; } = PlatformID.Win32NT;
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