using System;

namespace PropertyModels.ComponentModel;

/// <summary>
/// Class WatermarkAttribute.
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class InnerRightContentStringAttribute : Attribute
{
    /// <summary>
    /// The content
    /// </summary>
    public readonly string Content;

    /// <summary>
    /// Initializes a new instance of the <see cref="InnerRightContentStringAttribute"/> class.
    /// </summary>
    /// <param name="content">The content.</param>
    public InnerRightContentStringAttribute(string content)
    {
        Content = content;
    }
}