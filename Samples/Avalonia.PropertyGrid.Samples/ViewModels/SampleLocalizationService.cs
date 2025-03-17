using Avalonia.PropertyGrid.Localization;

namespace Avalonia.PropertyGrid.Samples.ViewModels
{
    internal class SampleLocalizationService : AssemblyJsonAssetLocalizationService
    {
        public SampleLocalizationService()
            : base(typeof(SampleLocalizationService).Assembly)
        {
        }
    }
}
