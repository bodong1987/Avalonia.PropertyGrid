using Avalonia.Media;

namespace Avalonia.PropertyGrid.Utils
{
    /// <summary>
    /// Class FontUtils.
    /// </summary>
    public static class FontUtils
    {
        /// <summary>
        /// Gets the default font family.
        /// Change this property can change the default TextBox's Font
        /// so it can display complex characters correctly.
        /// </summary>
        /// <value>The default font family.</value>
        // ReSharper disable once StringLiteralTypo
        public static FontFamily DefaultFontFamily => new("Microsoft YaHei,Simsun,苹方-简,宋体-简");
    }
}
