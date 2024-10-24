namespace Avalonia.PropertyGrid.Controls;

/// <summary>
/// view type 
/// </summary>
public enum PropertyGridViewType
{
    /// <summary>
    /// tiled view
    /// </summary>
    TiledView,

    /// <summary>
    /// tree view
    /// </summary>
    TreeView
}

/// <summary>
/// property grid view interface
/// </summary>
public interface IPropertyGridView
{
    /// <summary>
    /// get the view type of this instance
    /// </summary>
    PropertyGridViewType ViewType { get; }
    
    /// <summary>
    /// is show title now
    /// </summary>
    bool ShowTitle { get; set; }

    /// <summary>
    /// Gets or sets the width of the name.
    /// </summary>
    /// <value>The width of the name.</value>
    double NameWidth { get; set; }

    /// <summary>
    /// call on enter this view state
    /// </summary>
    void OnEnterState();
    
    /// <summary>
    /// call on leave this view state
    /// </summary>
    void OnLeaveState();

    /// <summary>
    /// Refreshes this instance.
    /// </summary>
    void Refresh();

    /// <summary>
    /// Gets the expandable object cache.
    /// </summary>
    /// <returns>IExpandableObjectCache.</returns>
    IExpandableObjectCache GetExpandableObjectCache();

    /// <summary>
    /// Gets the cell information cache.
    /// </summary>
    /// <returns>IPropertyGridCellInfoCache.</returns>
    IPropertyGridCellInfoCache GetCellInfoCache();
}