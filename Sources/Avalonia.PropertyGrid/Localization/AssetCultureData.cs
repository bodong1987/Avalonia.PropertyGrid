using System;
using System.IO;
using System.Text;
using Avalonia.Platform;
using PropertyModels.Localization;

namespace Avalonia.PropertyGrid.Localization;

internal class AssetCultureData : AbstractCultureData
{
    public AssetCultureData(Uri uri, bool autoLoad = false)
        : base(uri)
    {
        if (autoLoad)
        {
            _ = Reload();
        }
    }

    public sealed override bool Reload()
    {
        try
        {
            using (var stream = AssetLoader.Open(Path))
            {
                using var sr = new StreamReader(stream, Encoding.UTF8);
                var tempDict = ReadJsonStringDictionary(sr.ReadToEnd());

                LocalTexts = tempDict;
            }

            return LocalTexts != null;
        }
        catch
        {
            return false;
        }
    }
}