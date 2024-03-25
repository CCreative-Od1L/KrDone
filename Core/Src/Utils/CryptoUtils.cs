using System.IO;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Core.Utils {
    public static class CryptoUtils {
        static readonly Aes aesAlg;
        static readonly string AesCryptFileName = @"A1HPeuqa.dat";
        static readonly byte[] InnerAesCryptKey = [
            0x31, 0x0d, 0xba, 0x6a, 0x56, 0xf1, 0xa3, 0x59,
            0xe5, 0xd4, 0xdb, 0x26, 0xc7, 0x1f, 0x04, 0xa2,
            0x95, 0x6a, 0x52, 0xef, 0x76, 0x91, 0xd9, 0x4b,
            0x07, 0xbc, 0x0c, 0x6a, 0x2b, 0x38, 0x81, 0x50,
        ];
        static readonly byte[] InnerAesCryptIV = [
            0x11, 0x97, 0xac, 0x54, 0xa8, 0x3d, 0x56, 0x73,
            0xdf, 0x8a, 0x83, 0x60, 0xd0, 0x93, 0xc2, 0x02,
        ];
        static CryptoUtils() {
            string? cryptoDirectory = AppDomain.CurrentDomain.BaseDirectory;

            aesAlg = Aes.Create();
            AesInit(cryptoDirectory!);
        }
        static void AesInit(string cryptoDirectory) {
            string AesCryptFilePath = Path.Combine(cryptoDirectory, AesCryptFileName);
            byte[] bytesBuf;
            if (File.Exists(AesCryptFilePath)) {
                bytesBuf = InnerAesDecryptBytesToBytes(FileUtils.ReadBytes(AesCryptFilePath));
                aesAlg.Key = bytesBuf.Take(aesAlg.KeySize / 8).ToArray();
                aesAlg.IV = bytesBuf.TakeLast(bytesBuf.Length - aesAlg.KeySize / 8).ToArray();
            } else {
                bytesBuf = [.. aesAlg.Key, .. aesAlg.IV];
                FileUtils.WriteBytes(AesCryptFilePath, InnerAesEncryptBytesToBytes(bytesBuf));
            }
        }
        public static byte[] AesEncryptStringToBytes(string plainText) {
            if(string.IsNullOrEmpty(plainText)) { return []; }

            byte[] encrypted;
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            try {
                using MemoryStream msEncrypt = new();
                using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (StreamWriter swEncrypt = new(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }
                encrypted = msEncrypt.ToArray();
                return InnerAesEncryptBytesToBytes(encrypted);
            } catch (Exception) {
                return [];   
            }            
        }
        public static string AesDecryptBytesToString(byte[] cipherBytes) {
            if(cipherBytes == null || cipherBytes.Length <= 0) { return string.Empty; }
            try {
                cipherBytes = InnerAesDecryptBytesToBytes(cipherBytes);
                string plainText = string.Empty;
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new(cipherBytes)) {
                    using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                    using StreamReader srDecrypt = new(csDecrypt);
                    plainText = srDecrypt.ReadToEnd();
                }
                return plainText;
            } catch (Exception) {
                return string.Empty;
            }
        }
        static byte[] InnerAesEncryptBytesToBytes(byte[] plainByte) {
            byte[] cipherBytes;
            
            using MemoryStream ms = new();
            using CryptoStream cryptoStream = new(
                ms, 
                aesAlg.CreateEncryptor(InnerAesCryptKey, InnerAesCryptIV), 
                CryptoStreamMode.Write);
            cryptoStream.Write(plainByte, 0, plainByte.Length);
            cryptoStream.FlushFinalBlock();
            cipherBytes = ms.ToArray();
            return cipherBytes;
        }
        static byte[] InnerAesDecryptBytesToBytes(byte[] cipherBytes) {
            byte[] plainBytes;

            using MemoryStream ms = new();
            using CryptoStream cryptoStream = new(
                ms, 
                aesAlg.CreateDecryptor(InnerAesCryptKey, InnerAesCryptIV), 
                CryptoStreamMode.Write);
            cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
            cryptoStream.FlushFinalBlock();
            plainBytes = ms.ToArray();
            return plainBytes;
        }
        static public byte[] AesPemPublicEncrypt(string pemPublicKey, string content) {
            // * pem -> xml
            RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(pemPublicKey));
            string XMLKey = string.Format(
                "<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned()));

            using RSACryptoServiceProvider rsa = new();
            rsa.FromXmlString(XMLKey);
            return rsa.Encrypt(Encoding.UTF8.GetBytes(content), true);
        }
    }
}