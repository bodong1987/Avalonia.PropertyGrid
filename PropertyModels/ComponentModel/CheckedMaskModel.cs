using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PropertyModels.ComponentModel
{
    /// <summary>
    /// Class CheckedMaskModel.
    /// </summary>
    public class CheckedMaskModel : MiniReactiveObject, ICheckedMaskModel
    {
        /// <summary>
        /// The masks
        /// </summary>
        private HashSet<string> _Masks = new HashSet<string>();

        /// <summary>
        /// Gets the masks.
        /// </summary>
        /// <value>The masks.</value>
        public string[] Masks => _Masks.OrderBy(x=>x).ToArray();

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <value>All.</value>
        public string All { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is all checked.
        /// </summary>
        /// <value><c>true</c> if this instance is all checked; otherwise, <c>false</c>.</value>
        public bool IsAllChecked { get; private set; } = true;

        /// <summary>
        /// The checked values
        /// </summary>
        public HashSet<string> CheckedValues = new HashSet<string>();

        /// <summary>
        /// Occurs when [check changed].
        /// </summary>
        public event EventHandler CheckChanged;

        bool _IsUpdating = false;
        bool _IsDirty = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckedMaskModel"/> class.
        /// </summary>
        /// <param name="masks">The masks.</param>
        /// <param name="all">All.</param>
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

        /// <summary>
        /// Determines whether the specified mask is checked.
        /// </summary>
        /// <param name="mask">The mask.</param>
        /// <returns><c>true</c> if the specified mask is checked; otherwise, <c>false</c>.</returns>
        public bool IsChecked(string mask)
        {
            if(IsAllChecked)
            {
                return true;
            }

            return CheckedValues.Contains(mask);
        }

        /// <summary>
        /// Begins the update.
        /// </summary>
        public void BeginUpdate()
        {
            Debug.Assert(!_IsUpdating);

            _IsUpdating = true;
            _IsDirty = false;
        }

        /// <summary>
        /// Ends the update.
        /// </summary>
        public void EndUpdate()
        {
            Debug.Assert(_IsUpdating);

            _IsUpdating = false;

            if(_IsDirty)
            {
                CheckChanged?.Invoke(this, EventArgs.Empty);
                _IsDirty = false;
            }
        }

        private void RaiseChangedEvent()
        {
            if(!_IsUpdating)
            {
                CheckChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                _IsDirty = true;
            }
        }

        /// <summary>
        /// Checks the specified mask.
        /// </summary>
        /// <param name="mask">The mask.</param>
        public void Check(string mask)
        {
            if(mask == All)
            {
                IsAllChecked = true;

                CheckedValues.Clear();

                RaiseChangedEvent();

                return;
            }

            if (!CheckedValues.Contains(mask))
            {
                CheckedValues.Add(mask);

                RaiseChangedEvent();
            }
        }

        /// <summary>
        /// Uns the check.
        /// </summary>
        /// <param name="mask">The mask.</param>
        public void UnCheck(string mask)
        {
            if(mask == All)
            {
                IsAllChecked = false;

                RaiseChangedEvent();
                return;
            }

            if(CheckedValues.Contains(mask))
            {
                CheckedValues.Remove(mask);

                RaiseChangedEvent();
            }            
        }
    }
}
