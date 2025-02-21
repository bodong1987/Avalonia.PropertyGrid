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
    }
}
