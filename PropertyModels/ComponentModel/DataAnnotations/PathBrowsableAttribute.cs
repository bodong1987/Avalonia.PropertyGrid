using System;

namespace PropertyModels.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Enum PathBrowsableType
    /// </summary>
    public enum PathBrowsableType
    {
        /// <summary>
        /// The file
        /// </summary>
        File,

        /// <summary>
        /// The multiple files
        /// </summary>
        MultipleFiles,

        /// <summary>
        /// The directory
        /// </summary>
        Directory,

        /// <summary>
        /// The multiple directories
        /// </summary>
        MultipleDirectories
    };

    /// <summary>
    /// Class PathBrowsableAttribute.
    /// Implements the <see cref="Attribute" />
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class PathBrowsableAttribute : Attribute
    {
        /// <summary>
        /// The type
        /// </summary>
        public readonly PathBrowsableType Type;

        /// <summary>
        /// Gets or sets a value indicating whether [save mode].
        /// </summary>
        /// <value><c>true</c> if [save mode]; otherwise, <c>false</c>.</value>
        public bool SaveMode { get; set; }

        /// <summary>
        /// Gets or sets the initial name of the file or directory (When PathBrowsableType is Directory).
        /// </summary>
        /// <value>The initial name of the file.</value>
        public string? InitialFileName { get; set; }

        /// <summary>
        /// Gets or sets the filters.
        /// Use traditional filter formats, for example:
        ///     "txt files (*.txt)|*.txt|All files (*.*)|*.*"
        /// </summary>
        /// <value>The filters.</value>
        public string? Filters { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string? Title { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is file selection.
        /// </summary>
        /// <value><c>true</c> if this instance is file selection; otherwise, <c>false</c>.</value>
        public bool IsFileSelection => Type == PathBrowsableType.File || Type == PathBrowsableType.MultipleFiles;

        /// <summary>
        /// Gets a value indicating whether this instance is directory selection.
        /// </summary>
        /// <value><c>true</c> if this instance is directory selection; otherwise, <c>false</c>.</value>
        public bool IsDirectorySelection => Type == PathBrowsableType.Directory;

        /// <summary>
        /// Gets a value indicating whether this instance is multiple selection.
        /// </summary>
        /// <value><c>true</c> if this instance is multiple selection; otherwise, <c>false</c>.</value>
        public bool IsMultipleSelection => !SaveMode && Type == PathBrowsableType.MultipleFiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathBrowsableAttribute" /> class.
        /// </summary>
        /// <param name="pathSelection">The path selection.</param>
        /// <param name="saveMode">if set to <c>true</c> [save mode].</param>
        public PathBrowsableAttribute(PathBrowsableType pathSelection = PathBrowsableType.File, bool saveMode = false)
        {
            Type = pathSelection;
            SaveMode = saveMode;
        }
    }
}