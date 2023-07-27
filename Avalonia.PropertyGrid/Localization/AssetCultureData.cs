using Avalonia.Platform;
using Avalonia.PropertyGrid.Model.Localilzation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
                        var tempDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(sr.ReadToEnd());

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
