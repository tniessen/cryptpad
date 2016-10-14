
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
