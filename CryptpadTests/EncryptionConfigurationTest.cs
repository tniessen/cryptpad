using Microsoft.VisualStudio.TestTools.UnitTesting;
using cryptpad;
using System.Linq;
using System.Text;

namespace CryptpadTests
{
    [TestClass]
    public class EncryptionConfigurationTest
    {
        [TestMethod]
        public void TestDefaultCipher()
        {
            Assert.AreEqual(EncryptionConfiguration.AES_CBC_PKCS7, EncryptionConfiguration.DefaultAlgorithm);
        }

        [TestMethod]
        public void TestDefaultKeyGeneration()
        {
            Assert.AreEqual(EncryptionConfiguration.SHA256, EncryptionConfiguration.DefaultKeyAlgorithm);
        }

        [TestMethod]
        public void TestAlgorithmList()
        {
            Assert.IsTrue(EncryptionConfiguration.Algorithms.Contains(EncryptionConfiguration.AES_CBC_PKCS7));
            Assert.IsTrue(EncryptionConfiguration.Algorithms.Contains(EncryptionConfiguration.AES_ECB_PKCS7));
        }

        [TestMethod]
        public void TestKeyGenerationList()
        {
            Assert.IsTrue(EncryptionConfiguration.KeyAlgorithms.Contains(EncryptionConfiguration.SHA256));
            Assert.IsTrue(EncryptionConfiguration.KeyAlgorithms.Contains(EncryptionConfiguration.SHA512));
            Assert.IsTrue(EncryptionConfiguration.KeyAlgorithms.Contains(EncryptionConfiguration.MD5));
            Assert.IsTrue(EncryptionConfiguration.KeyAlgorithms.Contains(EncryptionConfiguration.RIPEMD160));
        }

        [TestMethod]
        public void TestAesCbcPkcs7()
        {
            EncryptionConfiguration.Algorithm alg = EncryptionConfiguration.AES_CBC_PKCS7;
            Assert.AreEqual("AES/CBC/PKCS7", alg.Name);
            Assert.AreEqual(256, alg.DefaultKeySize);
        }

        [TestMethod]
        public void TestAesEcbPkcs7()
        {
            EncryptionConfiguration.Algorithm alg = EncryptionConfiguration.AES_ECB_PKCS7;
            Assert.AreEqual("AES/ECB/PKCS7", alg.Name);
            Assert.AreEqual(256, alg.DefaultKeySize);
        }

        [TestMethod]
        public void TestSha256()
        {
            EncryptionConfiguration.KeyAlgorithm alg = EncryptionConfiguration.SHA256;
            Assert.AreEqual("SHA-256", alg.Name);
            foreach (int keySize in new int[] { 8, 16, 32, 64, 128, 256 })
            {
                Assert.IsTrue(alg.CanGenerateKey(keySize));
            }
            foreach (int keySize in new int[] { 257, 320, 384, 512 })
            {
                Assert.IsFalse(alg.CanGenerateKey(keySize));
            }

            byte[] key = new byte[256 / 8];
            alg.GenerateKey(Encoding.UTF8.GetBytes("Hello world"), key);
            Assert.AreEqual("64ec88ca00b268e5ba1a35678a1b5316d212f4f366b2477232534a8aeca37f3c", BytesToHex(key));

            key = new byte[160 / 8];
            alg.GenerateKey(Encoding.UTF8.GetBytes("Hello world!"), key);
            Assert.AreEqual("c0535e4be2b79ffd93291305436bf889314e4a3f", BytesToHex(key));
        }

        [TestMethod]
        public void TestSha512()
        {
            EncryptionConfiguration.KeyAlgorithm alg = EncryptionConfiguration.SHA512;
            Assert.AreEqual("SHA-512", alg.Name);
            foreach (int keySize in new int[] { 8, 16, 32, 64, 128, 256, 512 })
            {
                Assert.IsTrue(alg.CanGenerateKey(keySize));
            }
            foreach (int keySize in new int[] { 513, 640, 1024 })
            {
                Assert.IsFalse(alg.CanGenerateKey(keySize));
            }

            byte[] key = new byte[512 / 8];
            alg.GenerateKey(Encoding.UTF8.GetBytes("Hello world"), key);
            Assert.AreEqual("b7f783baed8297f0db917462184ff4f08e69c2d5e5f79a942600f9725f58ce1f29c18139bf80b06c0fff2bdd34738452ecf40c488c22a7e3d80cdf6f9c1c0d47", BytesToHex(key));

            key = new byte[160 / 8];
            alg.GenerateKey(Encoding.UTF8.GetBytes("Hello world!"), key);
            Assert.AreEqual("f6cde2a0f819314cdde55fc227d8d7dae3d28cc5", BytesToHex(key));
        }

        [TestMethod]
        public void TestMd5()
        {
            EncryptionConfiguration.KeyAlgorithm alg = EncryptionConfiguration.MD5;
            Assert.AreEqual("MD5", alg.Name);
            foreach (int keySize in new int[] { 8, 16, 32, 64, 128 })
            {
                Assert.IsTrue(alg.CanGenerateKey(keySize));
            }
            foreach (int keySize in new int[] { 129, 192, 256 })
            {
                Assert.IsFalse(alg.CanGenerateKey(keySize));
            }

            byte[] key = new byte[128 / 8];
            alg.GenerateKey(Encoding.UTF8.GetBytes("Hello world"), key);
            Assert.AreEqual("3e25960a79dbc69b674cd4ec67a72c62", BytesToHex(key));

            key = new byte[96 / 8];
            alg.GenerateKey(Encoding.UTF8.GetBytes("Hello world!"), key);
            Assert.AreEqual("86fb269d190d2c85f6e0468c", BytesToHex(key));
        }

        [TestMethod]
        public void TestRipemd160()
        {
            EncryptionConfiguration.KeyAlgorithm alg = EncryptionConfiguration.RIPEMD160;
            Assert.AreEqual("RIPEMD-160", alg.Name);
            foreach (int keySize in new int[] { 8, 16, 32, 64, 128, 160 })
            {
                Assert.IsTrue(alg.CanGenerateKey(keySize));
            }
            foreach (int keySize in new int[] { 161, 192, 256 })
            {
                Assert.IsFalse(alg.CanGenerateKey(keySize));
            }

            byte[] key = new byte[160 / 8];
            alg.GenerateKey(Encoding.UTF8.GetBytes("Hello world"), key);
            Assert.AreEqual("dbea7bd24eef40a2e79387542e36dd408b77b21a", BytesToHex(key));

            key = new byte[128 / 8];
            alg.GenerateKey(Encoding.UTF8.GetBytes("Hello world!"), key);
            Assert.AreEqual("7f772647d88750add82d8e1a7a3e5c09", BytesToHex(key));
        }

        private static string BytesToHex(byte[] bytes)
        {
            string chars = "0123456789abcdef";
            string result = "";
            foreach (byte b in bytes)
            {
                result += chars[(b & 0xf0) >> 4];
                result += chars[b & 0x0f];
            }
            return result;
        }
    }
}
