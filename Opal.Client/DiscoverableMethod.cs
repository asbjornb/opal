using System.Reflection;

namespace Opal.Client;

/// <summary>
/// Represents a discovered method marked with the <see cref="CallableAttribute"/>.
/// </summary>
public class DiscoveredMethod
{
    /// <summary>
    /// Gets the name of the class that declares the method.
    /// </summary>
    public string DeclaringClassName { get; }

    /// <summary>
    /// Gets the name of the method.
    /// </summary>
    public string MethodName { get; }

    /// <summary>
    /// Gets the description of the method from the <see cref="CallableAttribute"/>.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets a value indicating whether the method is read-only.
    /// </summary>
    public bool ReadOnly { get; }

    /// <summary>
    /// Gets information about the method parameters.
    /// </summary>
    public IReadOnlyList<ParameterInfo> Parameters { get; }

    /// <summary>
    /// Gets the return type of the method.
    /// </summary>
    public Type ReturnType { get; }

    /// <summary>
    /// Gets the MethodInfo object for the discovered method.
    /// </summary>
    public MethodInfo MethodInfo { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscoveredMethod"/> class.
    /// </summary>
    /// <param name="methodInfo">The method information.</param>
    /// <param name="callableAttribute">The callable attribute applied to the method.</param>
    public DiscoveredMethod(MethodInfo methodInfo, CallableAttribute callableAttribute)
    {
        MethodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));

        ArgumentNullException.ThrowIfNull(callableAttribute);

        DeclaringClassName = methodInfo.DeclaringType?.Name ?? string.Empty;
        MethodName = methodInfo.Name;
        Description = callableAttribute.Description;
        ReadOnly = callableAttribute.ReadOnly;
        Parameters = methodInfo.GetParameters();
        ReturnType = methodInfo.ReturnType;
    }

    /// <summary>
    /// Returns a string representation of the discovered method.
    /// </summary>
    /// <returns>A string representation of the discovered method.</returns>
    public override string ToString()
    {
        return $"{MethodName} [ReadOnly={ReadOnly}]: {Description}";
    }
}