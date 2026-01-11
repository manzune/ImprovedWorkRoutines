using System;
using System.Reflection;

namespace ImprovedWorkRoutines.Utils
{
    public static class Reflection
    {
        public static T InvokeMethod<T>(Type type, string methodName, object instance, object[] parameters = null)
        {
            parameters ??= [];
            return (T)type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Invoke(instance, parameters);
        }

        public static T InvokeMethodWithOut<T>(Type type, string methodName, object instance, object[] parameters, out object[] outParameters)
        {
            T result = (T)type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Invoke(instance, parameters);
            outParameters = parameters;

            return result;
        }

        public static void InvokeMethod(Type type, string methodName, object instance, object[] parameters = null)
        {
            parameters ??= [];
            type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Invoke(instance, parameters);
        }

        public static T GetFieldValue<T>(Type type, string fieldName, object instance)
        {
            return (T)type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(instance);
        }

        public static T GetPropertyValue<T>(Type type, string propertyName, object instance)
        {
            return (T)type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(instance);
        }

        public static void SetFieldValue<T>(Type type, string fieldName, object instance, T value)
        {
            type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(instance, value);
        }

        public static void SetPropertyValue<T>(Type type, string propertyName, object instance, T value)
        {
            type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(instance, value);
        }
    }
}
