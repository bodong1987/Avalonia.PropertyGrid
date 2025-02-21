using System;

namespace PropertyModels.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Specifies how the content is resized to fill its allocated space.
    /// </summary>
    public enum StretchType
    {
        /// <summary>
        /// The content preserves its original size.
        /// </summary>
        None,

        /// <summary>
        /// The content is resized to fill the destination dimensions. The aspect ratio is not preserved.
        /// </summary>
        Fill,

        /// <summary>
        /// The content is resized to fit in the destination dimensions while preserving its native aspect ratio.
        /// </summary>
        Uniform,

        /// <summary>
        /// The content is resized to completely fill the destination rectangle while preserving its native aspect ratio.
        /// A portion of the content may not be visible if the aspect ratio of the content does not match the aspect ratio of the allocated space.
        /// </summary>
        UniformToFill
    }

    /// <summary>
    /// Specifies the direction in which content is scaled.
    /// </summary>
    public enum StretchDirectionType
    {
        /// <summary>
        /// Only scales the content upwards when the content is smaller than the available space.
        /// If the content is larger, no scaling downwards is done.
        /// </summary>
        UpOnly,

        /// <summary>
        /// Only scales the content downwards when the content is larger than the available space.
        /// If the content is smaller, no scaling upwards is done.
        /// </summary>
        DownOnly,

        /// <summary>
        /// Always stretches to fit the available space according to the stretch mode.
        /// </summary>
        Both
    }

    /// <summary>
    /// Attribute to specify the image preview mode, including stretch and stretch direction.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class ImagePreviewModeAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the stretch mode.
        /// </summary>
        public StretchType Stretch { get; set; }

        /// <summary>
        /// Gets or sets the stretch direction.
        /// </summary>
        public StretchDirectionType StretchDirection { get; set; }
    }
}