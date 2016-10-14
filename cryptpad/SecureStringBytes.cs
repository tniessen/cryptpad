using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace cryptpad
{
    class SecureStringBytes : IDisposable
    {
        public readonly byte[] Bytes;

        public SecureStringBytes(SecureString str, Encoding encoding)
        {
            Bytes = SecureStringToBytes(str, encoding);
        }

        public void Dispose()
        {
            // Overwrite with zeroes so we do not depend on the GC
            Bytes.Fill<byte>(0, 0, Bytes.Length);
        }

        /// <summary>
        /// Converts a SecureString to a byte array, making sure that no copies
        /// of the string remain in memory (except from the SecureString itself).
        /// </summary>
        private static unsafe byte[] SecureStringToBytes(SecureString str, Encoding encoding)
        {
            IntPtr bytes = IntPtr.Zero;
            IntPtr ustr = IntPtr.Zero;

            try
            {
                int allocSize = encoding.GetMaxByteCount(str.Length);

                bytes = Marshal.AllocHGlobal(allocSize);
                ustr = Marshal.SecureStringToBSTR(str);

                byte* pBytes = (byte*)bytes.ToPointer();
                char* pStr   = (char*)ustr.ToPointer();

                int size = encoding.GetBytes(pStr, str.Length, pBytes, allocSize);

                byte[] result = new byte[size];
                result.CopyFromPointer(pBytes, 0, size);

                return result;
            }
            finally
            {
                if (bytes != IntPtr.Zero) Marshal.FreeHGlobal(bytes);
                if (ustr != IntPtr.Zero) Marshal.ZeroFreeBSTR(ustr);
            }
        }
    }
}
