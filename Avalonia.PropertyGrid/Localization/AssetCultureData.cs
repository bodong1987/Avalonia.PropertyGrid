using Avalonia.Platform;
using PropertyModels.Localization;
using System;
using System.IO;
using System.Text;

namespace Avalonia.PropertyGrid.Localization
{
    internal class AssetCultureData : AbstractCultureData
    {
        public AssetCultureData(Uri uri, bool autoLoad = false) : 
            base(uri)
        {
            if(autoLoad)
            {
                Reload();
            }
        }

        public override bool Reload()        
        {
            try
            {
                using (var stream = AssetLoader.Open(Path))
                {
                    using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
                    {
                        var tempDict = ReadJsonStringDictionary(sr.ReadToEnd());

                        if (tempDict != null)
                        {
                            LocalTexts = tempDict;
                        }
                    }
                }

                return LocalTexts != null;
            }
            catch
            {
                return false;
            }            
        }

    }
}
