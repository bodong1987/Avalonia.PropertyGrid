using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Model.ComponentModel
{
    public class CheckedMaskModel
    {
        private HashSet<string> _Masks = new HashSet<string>();

        public string[] Masks => _Masks.OrderBy(x=>x).ToArray();

        public string All { get; private set; }

        public bool IsAllChecked { get; private set; } = true;

        public HashSet<string> CheckedValues = new HashSet<string>();

        public event EventHandler CheckChanged;
                
        public CheckedMaskModel(IEnumerable<string> masks, string all)
        {
            All = all;
            
            foreach(string mask in masks)
            {
                if(!_Masks.Contains(mask))
                {
                    _Masks.Add(mask);
                }
            }
        }

        public bool IsChecked(string mask)
        {
            if(IsAllChecked)
            {
                return true;
            }

            return CheckedValues.Contains(mask);
        }

        public void Check(string mask)
        {
            if(mask == All)
            {
                IsAllChecked = true;

                CheckedValues.Clear();

                CheckChanged?.Invoke(this, EventArgs.Empty);

                return;
            }

            if (!CheckedValues.Contains(mask))
            {
                CheckedValues.Add(mask);

                CheckChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void UnCheck(string mask)
        {
            if(mask == All)
            {
                IsAllChecked = false;

                CheckChanged?.Invoke(this, EventArgs.Empty);
                return;
            }

            if(CheckedValues.Contains(mask))
            {
                CheckedValues.Remove(mask);
                CheckChanged?.Invoke(this, EventArgs.Empty);
            }            
        }
    }
}
