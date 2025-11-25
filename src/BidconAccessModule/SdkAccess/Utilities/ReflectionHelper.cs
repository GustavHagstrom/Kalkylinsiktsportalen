using System.Reflection;
using System.Reflection.Emit;

namespace BidconAccessModule.SdkAccess.Utilities;
internal static class ReflectionHelper
{
    /// <summary>
    /// Creates an instance of a class with an internal, private, or protected constructor.
    /// </summary>
    /// <typeparam name="T">The type of object to create.</typeparam>
    /// <param name="constructorArgs">Arguments for the constructor.</param>
    /// <returns>An instance of the specified type.</returns>
    public static T CreateInstance<T>(params object[] constructorArgs)
    {
        var type = typeof(T);
        return (T)CreateInstance(type, constructorArgs);
    }
    public static object CreateInstance(Type type, params object[] constructorArgs)
    {
        var constructor = type.GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
            null,
            Array.ConvertAll(constructorArgs, arg => arg.GetType()),
            null);

        if (constructor == null)
        {
            throw new MissingMethodException($"No matching constructor found for type {type.FullName}.");
        }

        return constructor.Invoke(constructorArgs);
    }


    /// <summary>
    /// Gets the value of a private, internal, or protected field or property.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="instance">The object instance containing the field or property.</param>
    /// <param name="memberName">The name of the field or property.</param>
    /// <returns>The value of the field or property.</returns>
    public static T GetFieldOrPropertyValue<T>(object instance, string memberName)
    {
        var type = instance.GetType();

        // Try to get the field
        var field = type.GetField(memberName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (field != null)
        {
            return (T)field.GetValue(instance);
        }

        // Try to get the property
        var property = type.GetProperty(memberName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (property != null)
        {
            return (T)property.GetValue(instance);
        }

        throw new MissingMemberException($"No field or property named '{memberName}' found on type {type.FullName}.");
    }
    /// <summary>
    /// Sets the value of a private, internal, or protected field.
    /// </summary>
    /// <param name="instance">The object instance containing the field.</param>
    /// <param name="fieldName">The name of the field.</param>
    /// <param name="value">The new value for the field.</param>
    public static void SetFieldValue(object instance, string fieldName, object value)
    {
        var type = instance.GetType();
        var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        if (field == null)
        {
            throw new MissingFieldException($"No field named '{fieldName}' found on type {type.FullName}.");
        }

        field.SetValue(instance, value);
    }

    /// <summary>
    /// Invokes a private, internal, or protected method on an object instance.
    /// </summary>
    /// <typeparam name="T">The return type of the method.</typeparam>
    /// <param name="instance">The object instance containing the method.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="methodArgs">Arguments for the method.</param>
    /// <returns>The result of the method invocation.</returns>
    public static T InvokeMethod<T>(object instance, string methodName, params object[] methodArgs)
    {
        var type = instance.GetType();
        var method = type.GetMethod(
            methodName,
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        if (method == null)
        {
            throw new MissingMethodException($"No method named '{methodName}' found on type {type.FullName}.");
        }

        return (T)method.Invoke(instance, methodArgs);
    }
    public static void InvokeMethod(object instance, string methodName, params object[] methodArgs)
    {
        var type = instance.GetType();
        var method = type.GetMethod(
            methodName,
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        if (method == null)
        {
            throw new MissingMethodException($"No method named '{methodName}' found on type {type.FullName}.");
        }

        method.Invoke(instance, methodArgs);
    }
    /// <summary>
    /// Invokes a private, internal, or protected static method on a specified class.
    /// </summary>
    /// <typeparam name="TClass">The type of the class containing the static method.</typeparam>
    /// <typeparam name="TReturn">The expected return type of the method.</typeparam>
    /// <param name="methodName">The name of the static method to invoke.</param>
    /// <param name="methodArgs">Arguments for the static method.</param>
    /// <returns>The result of the method invocation, or default(TReturn) if the method returns void.</returns>
    public static TReturn? InvokeStaticMethod<TClass, TReturn>(string methodName, params object[] methodArgs)
    {
        Type type = typeof(TClass);

        // Look for the method in the specified class
        var method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        if (method == null)
        {
            throw new MissingMethodException($"The static method '{methodName}' was not found in type '{type.FullName}'.");
        }

        // Invoke the method
        var result = method.Invoke(null, methodArgs);

        // Return the result, or default(TReturn) if the method is void
        return (TReturn?)result;
    }
    /// <summary>
    /// Invokes a private, internal, or protected static method on a specified class.
    /// </summary>
    /// <typeparam name="TClass">The type of the class containing the static method.</typeparam>
    /// <typeparam name="TReturn">The expected return type of the method.</typeparam>
    /// <param name="methodName">The name of the static method to invoke.</param>
    /// <param name="methodArgs">Arguments for the static method.</param>
    /// <returns>The result of the method invocation, or default(TReturn) if the method returns void.</returns>
    public static void InvokeStaticMethod<T>(string methodName, params object[] methodArgs)
    {
        Type type = typeof(T);

        // Look for the method in the specified class
        var method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        if (method == null)
        {
            throw new MissingMethodException($"The static method '{methodName}' was not found in type '{type.FullName}'.");
        }

        // Invoke the method
        method.Invoke(null, methodArgs);
    }
    /// <summary>
    /// Creates a subclass of an internal class with a specified name.
    /// </summary>
    /// <param name="internalBaseType"></param>
    /// <param name="newClassName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static Type CreateSubclass(Type internalBaseType, string newClassName)
    {
        if (!internalBaseType.IsClass || internalBaseType.IsNotPublic)
        {
            throw new ArgumentException("The provided type must be an internal class.", nameof(internalBaseType));
        }

        // Define a dynamic assembly and module
        var assemblyName = new AssemblyName("DynamicAssembly");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

        // Define a new public class that extends the internal base class
        var typeBuilder = moduleBuilder.DefineType(
            newClassName,
            TypeAttributes.Public | TypeAttributes.Class,
            internalBaseType);

        // Create the type
        return typeBuilder.CreateType();
    }
}
