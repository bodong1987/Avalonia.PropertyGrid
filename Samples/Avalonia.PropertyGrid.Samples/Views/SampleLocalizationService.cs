using Avalonia.PropertyGrid.Localization;

namespace Avalonia.PropertyGrid.Samples.Views
{
    internal class SampleLocalizationService : AssemblyJsonAssetLocalizationService
    {
        public SampleLocalizationService()
            : base(typeof(SampleLocalizationService).Assembly)
        {
        }
    }
}
