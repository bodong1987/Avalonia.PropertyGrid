using Avalonia.Controls;
using Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static async Task<string[]> ShowOpenFileDialogAsync(Window parent, string filters, bool multipleSelection = false, string title = null, string filename = null)
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
        public static async Task<string> ShowOpenSingleFileDialogAsync(Window parent, string filters, string title = null, string filename = null)
        {
            var files = await ShowDialogAsync(
                parent,
                PathBrowsableType.File,
                false,
                title,
                filters,
                filename
                );

            return files.FirstOrDefault();
        }

        /// <summary>
        /// Show save file dialog as an asynchronous operation.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="title">The title.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>A Task&lt;System.String&gt; representing the asynchronous operation.</returns>
        public static async Task<string> ShowSaveFileDialogAsync(Window parent, string filters, string title = null, string filename = null)
        {
            var files = await ShowDialogAsync(
                parent,
                PathBrowsableType.File,
                true,
                title,
                filters,
                filename
                );

            return files.FirstOrDefault();
        }

        /// <summary>
        /// Show open folder dialog as an asynchronous operation.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="title">The title.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>A Task&lt;System.String&gt; representing the asynchronous operation.</returns>
        public static async Task<string> ShowOpenFolderDialogAsync(Window parent, string title = null, string filename = null)
        {
            var files = await ShowDialogAsync(
                parent,
                PathBrowsableType.Directory,
                false,
                title,
                null,
                filename
                );

            return files.FirstOrDefault();
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
        public static async Task<string[]> ShowDialogAsync(Window parentWindow, PathBrowsableType type, bool saveMode, string title, string filters, string initFileName)
        {
            if (saveMode)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = title ?? "Please select a file";
                saveFileDialog.Filters = ConvertToFilterList(filters);
                saveFileDialog.InitialFileName = initFileName;

                var path = await saveFileDialog.ShowAsync(parentWindow);

                return new string[] { path };
            }
            else if (type == PathBrowsableType.Directory)
            {
                OpenFolderDialog openFolderDialog = new OpenFolderDialog();
                openFolderDialog.Title = title ?? "Please select a Folder";
                openFolderDialog.Directory = initFileName;

                var path = await openFolderDialog.ShowAsync(parentWindow);
                return new string[] { path };
            }
            else if (type == PathBrowsableType.File || type == PathBrowsableType.MultipleFiles)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = title ?? (type == PathBrowsableType.MultipleFiles ? "Please select files" : "Please slect a file");
                openFileDialog.InitialFileName = initFileName;
                openFileDialog.Filters = ConvertToFilterList(filters);

                var paths = await openFileDialog.ShowAsync(parentWindow);

                return paths;
            }

            return null;
        }

        /// <summary>
        /// Converts to filter list.
        /// </summary>
        /// <param name="filters">The filters.</param>
        /// <returns>List&lt;FileDialogFilter&gt;.</returns>
        public static List<FileDialogFilter> ConvertToFilterList(string filters)
        {
            List<FileDialogFilter> list = new List<FileDialogFilter>();

            var results = filters.Split('|');

            for (int i = 0; i < results.Length / 2; ++i)
            {
                string name = results[i * 2 + 0];
                string exts = results[i * 2 + 1];

                FileDialogFilter filter = new FileDialogFilter();
                filter.Name = name;
                filter.Extensions = exts.Split(';').Select(x =>
                {
                    var y = x.Trim();
                    if (y.StartsWith("*."))
                    {
                        return y.Substring("*.".Length);
                    }
                    return y;
                }).ToList();

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
        public static async Task<string[]> ShowPathBrowserAsync(Window parent, PathBrowsableAttribute attribute)
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
