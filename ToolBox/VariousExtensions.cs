using System;
using System.Collections.Generic;
using System.Reflection;
using Studio;
using UnityEngine;

namespace ToolBox.Extensions {
    internal static class VariousExtensions
    {
        public static void Resize<T>(this List<T> self, int newSize)
        {
            int diff = self.Count - newSize;
            if (diff < 0)
                while (self.Count != newSize)
                    self.Add(default(T));
            else if (diff > 0)
                while (self.Count != newSize)
                    self.RemoveRange(newSize, diff);
        }

        public static int IndexOf<T>(this T[] self, T obj)
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (self[i].Equals(obj))
                    return i;
            }
            return -1;
        }

        public static void AddRange<T, T2>(this IDictionary<T, T2> self, IDictionary<T, T2> toAdd)
        {
            foreach (KeyValuePair<T, T2> pair in toAdd)
                self.Add(pair.Key, pair.Value);
        }

        public static bool IsVisible(this TreeNodeObject self)
        {
            if (self.parent != null)
                return self.visible && self.parent.IsVisible();
            return self.visible;
        }

        public static int LastIndexOf(this byte[] self, byte[] needle)
        {
            int limit = needle.Length - 1;
            for (int i = self.Length - 1; i > limit; i--)
            {
                int j;
                int i2 = i;
                for (j = needle.Length - 1; j >= 0; --j)
                {
                    if (self[i2] != needle[j])
                        break;
                    --i2;
                }
                if (j == -1)
                    return i2 + 1;
            }
            return -1;
        }

        public static Color GetContrastingColor(this Color self)
        {
            float luminance = 0.299f * self.r + 0.587f * self.g + 0.114f * self.b;
            if (luminance > 0.5f)
                return Color.black;
            return Color.white;
        }

#if KOIKATSU || AISHOUJO || HONEYSELECT2
        private static MethodInfo _initTransforms = null;
        public static void InitTransforms(this DynamicBone self)
        {
            if (_initTransforms == null)
                _initTransforms = self.GetType().GetMethod("InitTransforms", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            _initTransforms.Invoke(self, null);
        }

        private static MethodInfo _setupParticles = null;
        public static void SetupParticles(this DynamicBone self)
        {
            if (_setupParticles == null)
                _setupParticles = self.GetType().GetMethod("SetupParticles", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            _setupParticles.Invoke(self, null);
        }
#endif

        public static string RelativePath(string from, string to)
        {
            if (from.EndsWith("/") == false && from.EndsWith("\\") == false)
                from += "\\";
            Uri toUri = new Uri(to);
            Uri fromUri = new Uri(from);
            return Uri.UnescapeDataString(fromUri.MakeRelativeUri(toUri).ToString().Replace('/', '\\'));
        }

    }

    public delegate void Action<T1, T2, T3, T4, T5>(T1 arg, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    public delegate void Action<T1, T2, T3, T4, T5, T6>(T1 arg, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

    public class HashedPair<T, T2>
    {
        public readonly T key;
        public readonly T2 value;

        private readonly int _hashCode;

        public HashedPair(T key, T2 value)
        {
            this.key = key;
            this.value = value;

            unchecked
            {
                int hash = 17;
                hash = hash * 31 + (this.key != null ? this.key.GetHashCode() : 0);
                this._hashCode = hash * 31 + (this.value != null ? this.value.GetHashCode() : 0);
            }
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override string ToString()
        {
            return $"key: {this.key}, value: {this.value}";
        }
    }

    public class LambdaComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _compareFunc;

        public LambdaComparer(Func<T, T, bool> compareFunc)
        {
            this._compareFunc = compareFunc;
        }

        public bool Equals(T x, T y)
        {
            return this._compareFunc(x, y);
        }

        public int GetHashCode(T obj)
        {
            return EqualityComparer<T>.Default.GetHashCode(obj);
        }
    }
}