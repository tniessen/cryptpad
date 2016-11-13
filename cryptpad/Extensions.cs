
using System.Collections.Generic;

namespace cryptpad
{
    static class Extensions
    {
        /// <summary>
        /// Fills a section of an array with a value.
        /// </summary>
        public static void Fill<T>(this T[] array, T value, int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                array[i] = value;
            }
        }

        /// <summary>
        /// Compares two arrays for equality, considering their contents.
        /// </summary>
        public static bool DeepEquals<T>(this T[] a, T[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a.Length; i++)
            {
                if (!comparer.Equals(a[i], b[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Copies bytes from an unmanaged array to a managed byte array.
        /// </summary>
        public static unsafe void CopyFromPointer(this byte[] array, byte* pointer, int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                array[i] = pointer[i];
            }
        }
    }
}
