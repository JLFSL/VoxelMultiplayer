using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace VoxelMultiplayer.Utility
{
    class Utils
    {
        public static object InvokeMethod(object obj, string methodName, params object[] methodParams)
        {
            var methodParamTypes = methodParams?.Select(p => p.GetType()).ToArray() ?? new Type[] { };
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
            MethodInfo method = null;
            var type = obj.GetType();
            while (method == null && type != null)
            {
                method = type.GetMethod(methodName, bindingFlags, Type.DefaultBinder, methodParamTypes, null);
                type = type.BaseType;
            }

            return method?.Invoke(obj, methodParams);
        }

        public static object GetValueForField(object classObj, string fieldName, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        {
            return classObj.GetType().GetField(fieldName, bindingFlags)?.GetValue(classObj);
        }

        public static void SetValueForField(object classObj, string fieldName, object value, BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        {
            classObj.GetType().GetField(fieldName, bindingFlags)?.SetValue(classObj, value);
        }

        public static bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception caught in process: " + ex);
                return false;
            }
        }

        public static byte[] ObjectToByteArray(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }
    }
}
