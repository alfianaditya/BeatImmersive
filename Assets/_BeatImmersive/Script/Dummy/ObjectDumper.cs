using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public static class ObjectDumper
{
    public static void Dump(object obj, int depth = 0)
    {
        try
        {
            if (obj == null)
            {
                Debug.Log($"{Indent(depth)}NULL");
                return;
            }

            Type type = obj.GetType();

            Debug.Log($"{Indent(depth)}================================");
            Debug.Log($"{Indent(depth)}TYPE : {type.FullName}");
            Debug.Log($"{Indent(depth)}================================");

            //----------------------------------
            // FIELDS
            //----------------------------------

            Debug.Log($"{Indent(depth)}FIELDS");

            foreach (var field in type.GetFields(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance))
            {
                try
                {
                    object value = field.GetValue(obj);

                    Debug.Log($"{Indent(depth)}FIELD : {field.Name}");
                    DumpValue(value, depth + 1);
                }
                catch (Exception e)
                {
                    Debug.LogError($"FIELD ERROR : {field.Name}");
                    Debug.LogError(e);
                }
            }

            //----------------------------------
            // PROPERTIES
            //----------------------------------

            Debug.Log($"{Indent(depth)}PROPERTIES");

            foreach (var property in type.GetProperties(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance))
            {
                try
                {
                    if (!property.CanRead)
                        continue;

                    object value = property.GetValue(obj);

                    Debug.Log($"{Indent(depth)}PROPERTY : {property.Name}");

                    DumpValue(value, depth + 1);
                }
                catch (Exception e)
                {
                    Debug.LogError($"PROPERTY ERROR : {property.Name}");
                    Debug.LogError(e);
                }
            }

            //----------------------------------
            // METHODS
            //----------------------------------

            Debug.Log($"{Indent(depth)}METHODS");

            foreach (var method in type.GetMethods(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance))
            {
                try
                {
                    Debug.Log($"{Indent(depth)}{method.Name}");
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            //----------------------------------
            // INTERFACES
            //----------------------------------

            Debug.Log($"{Indent(depth)}INTERFACES");

            foreach (var inter in type.GetInterfaces())
            {
                Debug.Log($"{Indent(depth)}{inter.FullName}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("OBJECT DUMP FAILED");
            Debug.LogError(e);
        }
    }

    private static void DumpValue(object value, int depth)
    {
        try
        {
            if (value == null)
            {
                Debug.Log($"{Indent(depth)}NULL");
                return;
            }

            Type t = value.GetType();

            //-----------------------------
            // Primitive
            //-----------------------------

            if (t.IsPrimitive || value is string || value is decimal)
            {
                Debug.Log($"{Indent(depth)}{value}");
                return;
            }

            //-----------------------------
            // IEnumerable
            //-----------------------------

            if (value is IEnumerable enumerable)
            {
                Debug.Log($"{Indent(depth)}IEnumerable");

                int index = 0;

                foreach (var item in enumerable)
                {
                    Debug.Log($"{Indent(depth)}[{index}]");

                    try
                    {
                        Dump(item, depth + 1);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }

                    index++;
                }

                return;
            }

            //-----------------------------
            // Object
            //-----------------------------

            Dump(value, depth + 1);
        }
        catch (Exception e)
        {
            Debug.LogError("DumpValue Error");
            Debug.LogError(e);
        }
    }

    private static string Indent(int depth)
    {
        return new string(' ', depth * 2);
    }
}