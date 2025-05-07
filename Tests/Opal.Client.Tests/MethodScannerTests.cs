namespace Opal.Client.Tests;

public class MethodScannerTests
{
    #region Test Classes

    private class TestClass
    {
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable CA1822 // Mark members as static
        [Callable("Public callable method", true)]
        public void PublicCallableMethod() { }

        [Callable("Public callable method with parameters", false)]
        public string PublicCallableWithParams(int id, string name) => $"{id}: {name}";

        public void PublicNonCallableMethod() { }

        [Callable("Private callable method", true)]
        private void PrivateCallableMethod() { }

        [Callable("Static callable method", true)]
        public static void StaticCallableMethod() { }

        public static void StaticNonCallableMethod() { }

        [Callable("Private static callable method", true)]
        private static void PrivateStaticCallableMethod() { }
    }

    private class TestClassWithComplexParams
    {
        [Callable("Method with complex parameters")]
        public void ComplexParams(DateTime date, TimeSpan duration, Uri uri) { }
    }

    private class TestClassWithOverloads
    {
        [Callable("First overload")]
        public void OverloadedMethod() { }

        [Callable("Second overload")]
        public void OverloadedMethod(int param) { }
    }
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1822 // Mark members as static
#pragma warning restore IDE0079 // Remove unnecessary suppression
    #endregion

    [Fact]
    public void ScanType_FindsPublicCallableMethods()
    {
        // Arrange
        var type = typeof(TestClass);

        // Act
        var methods = MethodScanner.ScanType(type).ToList();

        // Assert
        Assert.Equal(3, methods.Count);
        Assert.Contains(methods, m => m.MethodName == "PublicCallableMethod");
        Assert.Contains(methods, m => m.MethodName == "PublicCallableWithParams");
        Assert.Contains(methods, m => m.MethodName == "StaticCallableMethod");
    }

    [Fact]
    public void ScanType_IncludesStaticMethods()
    {
        // Arrange
        var type = typeof(TestClass);

        // Act
        var methods = MethodScanner.ScanType(type).ToList();
        var staticMethod = methods.FirstOrDefault(m => m.MethodName == "StaticCallableMethod");

        // Assert
        Assert.NotNull(staticMethod);
        Assert.Equal("Static callable method", staticMethod.Description);
        Assert.True(staticMethod.ReadOnly);
    }

    [Fact]
    public void ScanType_ExcludesPrivateMethods()
    {
        // Arrange
        var type = typeof(TestClass);

        // Act
        var methods = MethodScanner.ScanType(type).ToList();

        // Assert
        Assert.DoesNotContain(methods, m => m.MethodName == "PrivateCallableMethod");
        Assert.DoesNotContain(methods, m => m.MethodName == "PrivateStaticCallableMethod");
    }

    [Fact]
    public void ScanType_ExcludesNonCallableMethods()
    {
        // Arrange
        var type = typeof(TestClass);

        // Act
        var methods = MethodScanner.ScanType(type).ToList();

        // Assert
        Assert.DoesNotContain(methods, m => m.MethodName == "PublicNonCallableMethod");
        Assert.DoesNotContain(methods, m => m.MethodName == "StaticNonCallableMethod");
    }

    [Fact]
    public void ScanType_CorrectlyCapturesMethodInfo()
    {
        // Arrange
        var type = typeof(TestClass);

        // Act
        var method = MethodScanner.ScanType(type)
            .FirstOrDefault(m => m.MethodName == "PublicCallableMethod");

        // Assert
        Assert.NotNull(method);
        Assert.Equal("TestClass", method.DeclaringClassName);
        Assert.Equal("Public callable method", method.Description);
        Assert.True(method.ReadOnly);
        Assert.Empty(method.Parameters);
        Assert.Equal(typeof(void), method.ReturnType);
    }

    [Fact]
    public void ScanType_CorrectlyCapturesParameters()
    {
        // Arrange
        var type = typeof(TestClass);

        // Act
        var method = MethodScanner.ScanType(type)
            .FirstOrDefault(m => m.MethodName == "PublicCallableWithParams");

        // Assert
        Assert.NotNull(method);
        Assert.Equal(2, method.Parameters.Count);
        Assert.Equal("id", method.Parameters[0].Name);
        Assert.Equal(typeof(int), method.Parameters[0].ParameterType);
        Assert.Equal("name", method.Parameters[1].Name);
        Assert.Equal(typeof(string), method.Parameters[1].ParameterType);
        Assert.Equal(typeof(string), method.ReturnType);
        Assert.False(method.ReadOnly);
    }

    [Fact]
    public void ScanType_HandlesComplexParameterTypes()
    {
        // Arrange
        var type = typeof(TestClassWithComplexParams);

        // Act
        var method = MethodScanner.ScanType(type).FirstOrDefault();

        // Assert
        Assert.NotNull(method);
        Assert.Equal(3, method.Parameters.Count);
        Assert.Equal(typeof(DateTime), method.Parameters[0].ParameterType);
        Assert.Equal(typeof(TimeSpan), method.Parameters[1].ParameterType);
        Assert.Equal(typeof(Uri), method.Parameters[2].ParameterType);
    }

    [Fact]
    public void ScanType_ThrowsOnOverloadedMethods()
    {
        // Arrange
        var type = typeof(TestClassWithOverloads);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => MethodScanner.ScanType(type).ToList());
        Assert.Contains("Method overloading is not supported", exception.Message);
    }
}