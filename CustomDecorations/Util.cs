﻿using ObjUnity3D;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomDecorations
{
    public class Util
    {
        internal static Mesh LoadMesh(string fullPath)
        {
            if (!File.Exists(fullPath)) return null;
            var mesh = new Mesh();
            using (var fileStream = File.Open(fullPath, FileMode.Open))
            {
                try
                {
                    mesh.LoadOBJ(OBJLoader.LoadOBJ(fileStream));
                }
                catch (Exception x)
                {
                    Debug.Log($"Failed to load mesh. {x.Message} {x.StackTrace}");
                }
                
            }
            mesh.name = Path.GetFileNameWithoutExtension(fullPath);
            return mesh;
        }

        internal static Texture2D LoadTexture(string filePath, TextureFormat format = TextureFormat.ARGB32, bool mips = true)
        {
            if (!File.Exists(filePath)) return null;            
            try
            {
                var texture = new Texture2D(1, 1, format, mips)
                {
                    anisoLevel = 7,
                    filterMode = FilterMode.Trilinear,
                    wrapMode = TextureWrapMode.Clamp
                };
                texture.LoadImage(File.ReadAllBytes(filePath));
                return texture;
            }
            catch(Exception x)
            {
                Debug.Log($"Failed to load texture. {x.Message} {x.StackTrace}");
                return null;
            }
            
        }

        //internal static IEnumerator<Mesh> LoadMesh(string fullPath)
        //{
        //    if (!File.Exists(fullPath)) yield return null;
        //    var mesh = new Mesh();
        //    using (var fileStream = File.Open(fullPath, FileMode.Open))
        //    {
        //        mesh.LoadOBJ(OBJLoader.LoadOBJ(fileStream));
        //    }
        //    mesh.name = Path.GetFileNameWithoutExtension(fullPath);
        //    yield return mesh;
        //}

        //internal static IEnumerator<Texture2D> LoadTexture(string filePath, TextureFormat format = TextureFormat.ARGB32, bool mips = true)
        //{
        //    if (!File.Exists(filePath)) yield return null;
            
        //    var texture = new Texture2D(1, 1, format, mips)
        //    {
        //        anisoLevel = 7,
        //        filterMode = FilterMode.Trilinear,
        //        wrapMode = TextureWrapMode.Clamp
        //    };
        //    texture.LoadImage(File.ReadAllBytes(filePath));
        //    yield return texture;
        //}
    }

    public class ReflectionUtil
    {
        #region Invoke
        /// <summary>
        /// Invokes a static method on the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T InvokeMethod<T>(Type type, string methodName, params object[] args)
        {
            // Try and find the method via the arguments passed in.
            var methodArgumentTypes = args.Select(a => a.GetType()).ToArray();

            // Pass a null array to GetMethod as it shortcuts early instead of doing some sanity checks inside GetMethod itself.
            if (methodArgumentTypes.Length == 0)
            {
                methodArgumentTypes = null;
            }

            var methodInfo = type.GetMethod(methodName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
                null,
                methodArgumentTypes,
                null);

            if (methodInfo == null)
            {
                throw new ArgumentException(
                    string.Format("Method '{0}({1})' could not be found on object of type {2}",
                        methodName,
                        methodArgumentTypes != null ? string.Join(", ", methodArgumentTypes.Select(t => t.Name).ToArray()) : string.Empty,
                        type.FullName),
                    "methodName");
            }

            // Note: The invokes here are specifically not in a try/catch. The exception will bubble up to the caller so it can be handled there properly,
            // rather than suppressing anything we'd do here.
            if (methodInfo.ReturnType == typeof(void))
            {
                methodInfo.Invoke(null, args);
                return default(T);
            }
            return (T)methodInfo.Invoke(null, args);
        }

        /// <summary>
        /// Invokes an instance method on the specified object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T InvokeMethod<T>(object instance, string methodName, params object[] args)
        {
            // Try and find the method via the arguments passed in.
            var methodArgumentTypes = args.Select(a => a.GetType()).ToArray();

            // Pass a null array to GetMethod as it shortcuts early instead of doing some sanity checks inside GetMethod itself.
            if (methodArgumentTypes.Length == 0)
            {
                methodArgumentTypes = null;
            }

            var methodInfo = instance.GetType().GetMethod(methodName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
                null,
                methodArgumentTypes,
                null);

            if (methodInfo == null)
            {
                throw new ArgumentException(
                    string.Format("Method '{0}({1})' could not be found on object of type {2}",
                        methodName,
                        methodArgumentTypes != null ? string.Join(", ", methodArgumentTypes.Select(t => t.Name).ToArray()) : string.Empty,
                        instance.GetType().FullName),
                    "methodName");
            }

            // Note: The invokes here are specifically not in a try/catch. The exception will bubble up to the caller so it can be handled there properly,
            // rather than suppressing anything we'd do here.
            if (methodInfo.ReturnType == typeof(void))
            {
                methodInfo.Invoke(instance, args);
                return default(T);
            }
            return (T)methodInfo.Invoke(instance, args);
        }

        public static void InvokeMethod(object instance, string methodName, params object[] args)
        {
            InvokeMethod<bool>(instance, methodName, args);
        }
        #endregion

        #region Get/SetField

        public static T GetField<T>(Type type, string fieldName)
        {
            var field = type.GetField(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (field == null)
            {
                throw new ArgumentException("Field '" + fieldName + "' could not be found on object of type " + type.FullName, "fieldName");
            }
            return (T)field.GetValue(null);
        }

        public static void SetField(Type type, string fieldName, object value)
        {
            var field = type.GetField(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (field == null)
            {
                throw new ArgumentException("Field '" + fieldName + "' could not be found on object of type " + type.FullName, "fieldName");
            }
            field.SetValue(null, value);
        }
        public static T GetField<T>(object instance, string fieldName)
        {
            var field = instance.GetType().GetField(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (field == null)
            {
                throw new ArgumentException("Field '" + fieldName + "' could not be found on object of type " + instance.GetType().FullName, "fieldName");
            }
            return (T)field.GetValue(instance);
        }

        public static void SetField(object instance, string fieldName, object value)
        {
            var field = instance.GetType().GetField(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (field == null)
            {
                throw new ArgumentException("Field '" + fieldName + "' could not be found on object of type " + instance.GetType().FullName, "fieldName");
            }
            field.SetValue(instance, value);
        }
        #endregion
    }
}
