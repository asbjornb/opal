namespace Opal.Client;

/// <summary>
/// Marks a method as callable through the Opal client.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class CallableAttribute : Attribute
{
    /// <summary>
    /// Gets the description of the callable method.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets a value indicating whether the method is read-only.
    /// </summary>
    public bool ReadOnly { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CallableAttribute"/> class.
    /// </summary>
    /// <param name="description">The description of the callable method.</param>
    /// <param name="readOnly">A value indicating whether the method is read-only. Default is false.</param>
    public CallableAttribute(string description, bool readOnly = false)
    {
        Description = description;
        ReadOnly = readOnly;
    }
}