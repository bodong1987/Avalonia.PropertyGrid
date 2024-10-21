using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Implements;
using PropertyModels.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.PropertyGrid.Services
{
    /// <summary>
    /// Class CellEditFactoryService.
    /// </summary>
    public static class CellEditFactoryService
    {
        /// <summary>
        /// The default
        /// </summary>
        public readonly static ICellEditFactoryCollection Default = new CellEditFactoryCollection();

        /// <summary>
        /// Initializes static members of the <see cref="CellEditFactoryService"/> class.
        /// </summary>
        static CellEditFactoryService()
        {
            foreach (var type in typeof(CellEditFactoryService).Assembly.GetTypes())
            {
                if (type.IsClass && !type.IsAbstract && type.IsImplementFrom<ICellEditFactory>())
                {
                    Default.AddFactory((Activator.CreateInstance(type) as ICellEditFactory)!);
                }
            }
        }
    }
}
