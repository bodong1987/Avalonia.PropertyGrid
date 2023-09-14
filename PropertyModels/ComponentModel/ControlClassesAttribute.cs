using System;

namespace PropertyModels.ComponentModel;

/// <summary>
/// Defines the Classes of generated control. 
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class ControlClassesAttribute: Attribute
{
    /// <summary>
    /// The Classes to be injected to generated control. 
    /// </summary>
    public readonly string[] Classes;

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlClassesAttribute"/> class
    /// </summary>
    /// <param name="classes"></param>
    public ControlClassesAttribute(params string[] classes)
    {
        Classes = classes;
    }
}