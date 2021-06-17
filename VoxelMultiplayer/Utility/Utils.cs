using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using VoxelTycoon.Buildings;

namespace VoxelMultiplayer.Utility
{
    class Utils
    {
        public const BindingFlags AllBindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

        public static object Invoke(object classObj, string fieldName, params object[] methodParams)
        {
            Debug.LogError("Invoke: " + classObj.ToString() + "." + fieldName);

            var methodParamTypes = methodParams?.Select(p => p.GetType()).ToArray() ?? new Type[] { };
            MethodInfo method = null;
            var type = classObj.GetType();
            while (method == null && type != null)
            {
                method = type.GetMethod(fieldName, AllBindingFlags, Type.DefaultBinder, methodParamTypes, null);
                type = type.BaseType;
            }
            return method?.Invoke(classObj, methodParams);
        }

        public static object GetField(object classObj, string fieldName, BindingFlags bindingFlags = AllBindingFlags)
        {
            Debug.LogError("GetField: " + classObj.ToString() + "." + fieldName);
            return classObj.GetType().GetField(fieldName, bindingFlags)?.GetValue(classObj);
        }

        public static T GetField<T>(object classObj, string fieldName, BindingFlags bindingFlags = AllBindingFlags)
        {
            return (T)GetField(classObj, fieldName, bindingFlags);
        }

        public static void SetField(object classObj, string fieldName, object value, BindingFlags bindingFlags = AllBindingFlags)
        {
            Debug.LogError("SetField: " + classObj.ToString() + "." + fieldName);
            classObj.GetType().GetField(fieldName, bindingFlags)?.SetValue(classObj, value);
        }

        public static object GetProperty(object classObj, string fieldName, BindingFlags bindingFlags = AllBindingFlags)
        {
            Debug.LogError("GetProperty: " + classObj.ToString() + "." + fieldName);
            return classObj.GetType().GetProperty(fieldName, bindingFlags)?.GetValue(classObj);
        }

        public static T GetProperty<T>(object classObj, string fieldName, BindingFlags bindingFlags = AllBindingFlags)
        {
            return (T)GetProperty(classObj, fieldName, bindingFlags);
        }

        public static object GetProperty2(object classObj, string fieldName, Type returnType)
        {
            Debug.LogError("GetProperty: " + classObj.ToString() + "." + fieldName);
            return classObj.GetType().GetProperty(fieldName, returnType)?.GetValue(classObj);
        }

        public static T GetProperty2<T>(object classObj, string fieldName, Type returnType)
        {
            return (T)GetProperty2(classObj, fieldName, returnType);
        }

        public static bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                fs.Write(byteArray, 0, byteArray.Length);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception caught in process: " + ex);
                return false;
            }
        }
    }
}
