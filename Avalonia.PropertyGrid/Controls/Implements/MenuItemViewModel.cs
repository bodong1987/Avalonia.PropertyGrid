using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Avalonia.PropertyGrid.Controls.Implements
{
    /// <summary>
    /// Class MenuItemViewModel.
    /// </summary>
    public class MenuItemViewModel
    {
        string _Header;

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public string Header
        {
            get => LocalizationService.Default[_Header];
            set => _Header = value;
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>The command.</value>
        public ICommand Command { get; set; }

        /// <summary>
        /// Gets or sets the command parameter.
        /// </summary>
        /// <value>The command parameter.</value>
        public object CommandParameter { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public IList<MenuItemViewModel> Items{get;set;} = new List<MenuItemViewModel>();
    }

    internal static class BindingListMenuItemBuilder
    {
        public static MenuItemViewModel[] BuildListMenu(IObjectElementFactory factory, Action onAction)
        {
            if(factory == null || factory.GetSupportedTypes().Length <= 0)
            {
                return null;
            }

            List<MenuItemViewModel> list = new List<MenuItemViewModel>();

            MenuItemViewModel NewItem = new MenuItemViewModel()
            {
                Header = "New"
            };

            list.Add(NewItem);

            foreach(var type in factory.GetSupportedTypes())
            {
                MenuItemViewModel item = new MenuItemViewModel()
                {
                    Header = type.Name
                };

                item.Command = ReactiveCommand.Create(onAction);

                NewItem.Items.Add(item);
            }

            return list.ToArray();
        }
    }

}
