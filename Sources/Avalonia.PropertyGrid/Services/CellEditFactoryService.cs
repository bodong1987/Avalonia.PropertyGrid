using System;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Implements;
using PropertyModels.Extensions;

namespace Avalonia.PropertyGrid.Services;

/// <summary>
/// Class CellEditFactoryService.
/// </summary>
public static class CellEditFactoryService
{
    /// <summary>
    /// The default
    /// </summary>
    public static readonly ICellEditFactoryCollection Default = new CellEditFactoryCollection();

    /// <summary>
    /// Initializes static members of the <see cref="CellEditFactoryService"/> class.
    /// </summary>
    static CellEditFactoryService()
    {
        foreach (var type in typeof(CellEditFactoryService).Assembly.GetTypes())
        {
            if (type is { IsClass: true, IsAbstract: false } && type.IsImplementFrom<ICellEditFactory>())
            {
                Default.AddFactory((Activator.CreateInstance(type) as ICellEditFactory)!);
            }
        }
    }
}