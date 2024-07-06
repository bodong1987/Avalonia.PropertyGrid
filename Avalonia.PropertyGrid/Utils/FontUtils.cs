namespace Avalonia.PropertyGrid.Utils
{
    /// <summary>
    /// Class FontUtils.
    /// </summary>
    public static class FontUtils
    {
        /// <summary>
        /// Gets or sets the default font family.
        /// Change this property can change the default TextBox's Font
        /// it can display complex characters correctly.
        /// </summary>
        /// <value>The default font family.</value>
        // ReSharper disable once StringLiteralTypo
        public static string DefaultFontFamily { get; set; } = "Microsoft YaHei,Simsun,苹方-简,宋体-简";
    }
}
