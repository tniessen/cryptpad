using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace cryptpad
{
    public class EncryptionConfiguration
    {
        public static readonly Algorithm AES_CBC_PKCS7 = new AesAlgorithm() { Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 };
        public static readonly Algorithm AES_ECB_PKCS7 = new AesAlgorithm() { Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };

        public static readonly Algorithm DefaultAlgorithm = AES_CBC_PKCS7;

        public static Algorithm[] Algorithms =
        {
            AES_CBC_PKCS7,
            AES_ECB_PKCS7
        };

        public static readonly KeyAlgorithm SHA256 = new HashKeyAlgorithm(typeof(SHA256), "SHA-256", 256);
        public static readonly KeyAlgorithm SHA512 = new HashKeyAlgorithm(typeof(SHA512), "SHA-512", 512);
        public static readonly KeyAlgorithm RIPEMD160 = new HashKeyAlgorithm(typeof(RIPEMD160), "RIPEMD-160", 160);
        public static readonly KeyAlgorithm MD5 = new HashKeyAlgorithm(typeof(MD5), "MD5", 128);

        public static KeyAlgorithm[] KeyAlgorithms =
        {
            SHA256,
            SHA512,
            RIPEMD160,
            MD5
        };

        public static readonly KeyAlgorithm DefaultKeyAlgorithm = SHA256;

        public Algorithm Algo = DefaultAlgorithm;
        public int KeySize = DefaultAlgorithm.DefaultKeySize;
        public KeyAlgorithm KeyAlgo = DefaultKeyAlgorithm;

        public abstract class Algorithm
        {
            public abstract int[] LegalKeySizes
            {
                get;
            }

            public abstract int DefaultKeySize
            {
                get;
            }

            public abstract string Name
            {
                get;
            }

            public override string ToString()
            {
                return Name;
            }

            public abstract SymmetricAlgorithm GetImplementation();
        }

        public abstract class KeyAlgorithm
        {
            public abstract bool CanGenerateKey(int size);

            public abstract void GenerateKey(byte[] password, byte[] key);

            public abstract string Name
            {
                get;
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }

    class AesAlgorithm : EncryptionConfiguration.Algorithm
    {
        private Aes aes = Aes.Create();

        public CipherMode Mode
        {
            set { aes.Mode = value; }
            get { return aes.Mode; }
        }

        public PaddingMode Padding
        {
            set { aes.Padding = value; }
            get { return aes.Padding; }
        }

        public override string Name
        {
            get { return "AES/" + aes.Mode + "/" + aes.Padding; }
        }

        public override int DefaultKeySize
        {
            get { return 256; }
        }

        public override int[] LegalKeySizes
        {
            get
            {
                List<int> keySizes = new List<int>();
                foreach (KeySizes ks in Aes.Create().LegalKeySizes)
                {
                    for (int i = ks.MinSize; i <= ks.MaxSize; i += ks.SkipSize)
                    {
                        keySizes.Add(i);
                    }
                }
                return keySizes.ToArray();
            }
        }

        public override SymmetricAlgorithm GetImplementation()
        {
            Aes aes = Aes.Create();
            aes.Mode = this.aes.Mode;
            aes.Padding = this.aes.Padding;
            return aes;
        }
    }

    class HashKeyAlgorithm : EncryptionConfiguration.KeyAlgorithm
    {
        private Type type;
        private string name;
        private int maxSize;

        public HashKeyAlgorithm(Type type, string name, int maxSize)
        {
            this.type = type;
            this.name = name;
            this.maxSize = maxSize;
        }

        public override string Name
        {
            get { return name; }
        }

        public override bool CanGenerateKey(int size)
        {
            return size <= maxSize;
        }

        public override void GenerateKey(byte[] password, byte[] key)
        {
            // Create instance
            System.Reflection.MethodInfo create = type.GetMethod("Create", new Type[0]);
            HashAlgorithm alg = (HashAlgorithm)create.Invoke(null, new object[0]);

            // Compute hash
            byte[] hash = alg.ComputeHash(password);

            // Copy hash to key, skipping bytes if necessary
            Array.Copy(hash, key, key.Length);

            // Overwrite hash so we don't have to rely on the GC
            hash.Fill<byte>(0, 0, hash.Length);
        }
    }
}
