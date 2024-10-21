using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.PropertyGrid.Services;
using PropertyModels.ComponentModel.DataAnnotations;

namespace Avalonia.PropertyGrid.Utils
{
    /// <summary>
    /// Class PathBrowserUtils.
    /// </summary>
    public static class PathBrowserUtils
    {
        /// <summary>
        /// Show open file dialog as an asynchronous operation.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="multipleSelection">if set to <c>true</c> [multiple selection].</param>
        /// <param name="title">The title.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>A Task&lt;System.String&gt; representing the asynchronous operation.</returns>
        public static async Task<string[]?> ShowOpenFileDialogAsync(Window parent, string filters, bool multipleSelection = false, string? title = null, string? filename = null)
        {
            var files = await ShowDialogAsync(
                parent,
                multipleSelection ? PathBrowsableType.MultipleFiles : PathBrowsableType.File,
                false,
                title,
                filters,
                filename
                );

            return files;
        }

        /// <summary>
        /// Show open single file dialog as an asynchronous operation.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="title">The title.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>A Task&lt;System.String&gt; representing the asynchronous operation.</returns>
        public static async Task<string?> ShowOpenSingleFileDialogAsync(Window parent, string filters, string? title = null, string? filename = null)
        {
            var files = await ShowDialogAsync(
                parent,
                PathBrowsableType.File,
                false,
                title,
                filters,
                filename
                );

            return files?.FirstOrDefault();
        }

        /// <summary>
        /// Show save file dialog as an asynchronous operation.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="title">The title.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>A Task&lt;System.String&gt; representing the asynchronous operation.</returns>
        public static async Task<string?> ShowSaveFileDialogAsync(Window parent, string filters, string? title = null, string? filename = null)
        {
            var files = await ShowDialogAsync(
                parent,
                PathBrowsableType.File,
                true,
                title,
                filters,
                filename
                );

            return files?.FirstOrDefault();
        }

        /// <summary>
        /// Show open folder dialog as an asynchronous operation.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="title">The title.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>A Task&lt;System.String&gt; representing the asynchronous operation.</returns>
        public static async Task<string?> ShowOpenFolderDialogAsync(Window parent, string? title = null, string? filename = null)
        {
            var files = await ShowDialogAsync(
                parent,
                PathBrowsableType.Directory,
                false,
                title,
                null,
                filename
                );

            return files?.FirstOrDefault();
        }

        private static IStorageProvider? GetStorageProvider(Window parentWindow)
        {
            return TopLevel.GetTopLevel(parentWindow)?.StorageProvider;
        }

        /// <summary>
        /// Show dialog as an asynchronous operation.
        /// </summary>
        /// <param name="parentWindow">The parent window.</param>
        /// <param name="type">The type.</param>
        /// <param name="saveMode">if set to <c>true</c> [save mode].</param>
        /// <param name="title">The title.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="initFileName">Name of the initialize file.</param>
        /// <returns>A Task&lt;System.String[]&gt; representing the asynchronous operation.</returns>
        public static async Task<string[]?> ShowDialogAsync(Window parentWindow, PathBrowsableType type, bool saveMode, string? title, string? filters, string? initFileName)
        {
            var storageProvider = GetStorageProvider(parentWindow);

            if (storageProvider == null)
            {
                return null;
            }

            if (saveMode)
            {
                var options = new FilePickerSaveOptions
                {
                    Title = title ?? LocalizationService.Default["Please select a file"],
                    SuggestedFileName = initFileName,
                    FileTypeChoices = ConvertToFilterList(filters)
                };
                var storage = await storageProvider.SaveFilePickerAsync(options);

                return storage != null ? [storage.Path.LocalPath] : null;
            }

            if (type is PathBrowsableType.Directory or PathBrowsableType.MultipleDirectories)
            {
                var options = new FolderPickerOpenOptions();

                // If an InitFileName is specified, make that location available for opening.
                if (!string.IsNullOrEmpty(initFileName))
                {
                    var startFolder = await storageProvider.TryGetFolderFromPathAsync(initFileName);
                    options.SuggestedStartLocation = startFolder;
                }

                if (type == PathBrowsableType.MultipleDirectories)
                {
                    options.Title = title ?? LocalizationService.Default["Please select some folders"];
                    options.AllowMultiple = true;
                }
                else
                {
                    options.Title = title ?? LocalizationService.Default["Please select a folder"];
                    options.AllowMultiple = false;
                }

                var storage = await storageProvider.OpenFolderPickerAsync(options);
                return storage.Select(x => x.Path.LocalPath).ToArray();
            }
            if (type is PathBrowsableType.File or PathBrowsableType.MultipleFiles)
            {
                var options = new FilePickerOpenOptions();

                if (type == PathBrowsableType.MultipleFiles)
                {
                    options.Title = title ?? LocalizationService.Default["Please select some files"];
                    options.AllowMultiple = true;
                }
                else
                {
                    options.Title = title ?? LocalizationService.Default["Please select a file"];
                    options.AllowMultiple = false;
                }

                options.FileTypeFilter = ConvertToFilterList(filters);

                var storage = await storageProvider.OpenFilePickerAsync(options);

                return storage.Select(x => x.Path.LocalPath).ToArray();
            }

            return null;
        }

        /// <summary>
        /// Converts to filter list.
        /// </summary>
        /// <param name="filters">The filters.</param>
        /// <returns>List&lt;FileDialogFilter&gt;.</returns>
        public static List<FilePickerFileType> ConvertToFilterList(string? filters)
        {
            if(filters == null)
            {
                return [];
            }

            var list = new List<FilePickerFileType>();

            var results = filters.Split('|');

            for (var i = 0; i < results.Length / 2; ++i)
            {
                var name = results[i * 2 + 0];
                var extensions = results[i * 2 + 1];

                var filter = new FilePickerFileType(name.Trim())
                {
                    Patterns = extensions.Split(';').Select(x =>
                    {
                        var y = x.Trim();
                        return y;
                    }).ToList()
                };

                list.Add(filter);
            }

            return list;
        }

        /// <summary>
        /// Show path browser as an asynchronous operation.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="attribute">The attribute.</param>
        /// <returns>A Task&lt;System.String[]&gt; representing the asynchronous operation.</returns>
        public static async Task<string[]?> ShowPathBrowserAsync(Window parent, PathBrowsableAttribute attribute)
        {
            return await ShowDialogAsync(
                parent,
                attribute.Type,
                attribute.SaveMode,
                attribute.Title,
                attribute.Filters,
                attribute.InitialFileName
                );
        }
    }
}