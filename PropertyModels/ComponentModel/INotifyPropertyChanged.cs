using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyModels.ComponentModel
{
    /// <summary>
    /// Interface INotifyPropertyChanged
    /// Extends the <see cref="System.ComponentModel.INotifyPropertyChanged" />
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public interface INotifyPropertyChanged : System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        void RaisePropertyChanged(string propertyName);

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        void RaisePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e);
    }
}
