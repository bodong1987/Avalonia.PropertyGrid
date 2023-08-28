using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyModels.ComponentModel
{
    /// <summary>
    /// Interface INotifyPropertyChanging
    /// Implements the <see cref="System.ComponentModel.INotifyPropertyChanging" />
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanging" />
    public interface INotifyPropertyChanging : System.ComponentModel.INotifyPropertyChanging
    {
        /// <summary>
        /// Raises the property changing.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        void RaisePropertyChanging(string propertyName);

        /// <summary>
        /// Raises the property changing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangingEventArgs"/> instance containing the event data.</param>
        void RaisePropertyChanging(object sender, PropertyChangingEventArgs e);
    }
}
