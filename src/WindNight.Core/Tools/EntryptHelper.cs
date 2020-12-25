using System.IO;
using System.Text;

namespace System.Security.Cryptography.Extensions
{
    /// <summary>  </summary>
    public static class EncryptHelper
    {
        /// <summary>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoding"> default is  Encoding.UTF8</param>
        /// <returns></returns>
        public static string Md5Encrypt(this string text, Encoding? encoding = null)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            var bs = encoding.GetBytes(text);
            return bs.Md5Encrypt();
        }

        /// <summary>
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        public static string Md5Encrypt(this byte[] bs)
        {
            string result;

            using (var md5 = new MD5CryptoServiceProvider())
            {
                bs = md5.ComputeHash(bs);
                var s = new StringBuilder();

                foreach (var b in bs) s.Append(b.ToString("X2"));

                result = s.ToString();
            }

            return result.ToLower();
        }

        public static string DoHmacSha1Sign(this string text, string signKey, Encoding? encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            var hmacsha1 = new HMACSHA1(encoding.GetBytes(signKey));
            var dataBuffer = encoding.GetBytes(text);
            var hashBytes = hmacsha1.ComputeHash(dataBuffer);
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="encryptString">待加密字符串</param>
        /// <param name="key">16位密钥</param>
        /// <param name="iv">16位偏移量</param>
        /// <param name="paddingMode"> </param>
        /// <param name="cipherMode"> </param>
        /// <param name="keyLength"> </param>
        /// <param name="ivLength"> </param>
        /// <param name="encoding"> </param>
        /// <returns></returns>
        public static string AesEncrypt(this string encryptString, string key, string iv,
            PaddingMode paddingMode = PaddingMode.Zeros, CipherMode cipherMode = CipherMode.CBC,
            int keyLength = 16, int ivLength = 16, Encoding? encoding = null
            )
        {
            try
            {
                var inputByteArray = encryptString.ToBytes(encoding);// Encoding.UTF8.GetBytes(encryptString);
                SymmetricAlgorithm des = Rijndael.Create();
                des.Key = Encoding.ASCII.GetBytes(keyLength > 0 ? key.Substring(0, keyLength) : key);
                des.IV = Encoding.ASCII.GetBytes(ivLength > 0 ? iv.Substring(0, ivLength) : iv);
                des.Mode = cipherMode;//CipherMode.CBC;
                des.Padding = paddingMode;// PaddingMode.Zeros;

                var mStream = new MemoryStream();
                var cStream = new CryptoStream(mStream, des.CreateEncryptor(), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();

                var desBytes = mStream.ToArray();
                // if (handler != null) return handler.Invoke(desBytes);
                // if (encoding == null) encoding = Encoding.UTF8;
                // return encoding.GetString(desBytes);
                var sb = new StringBuilder();
                foreach (var t in desBytes)
                {
                    sb.Append(t.ToString("x2"));
                }
                mStream.Close();
                cStream.Close();
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return encryptString;
            }

        }


        /// <summary>
        /// AES解密 
        /// </summary>
        /// <param name="decryptString">解密字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">偏移量</param>
        /// <param name="paddingMode"> </param>
        /// <param name="cipherMode"> </param>
        /// <param name="keyLength"> </param>
        /// <param name="ivLength"> </param>
        /// <param name="encoding"> </param>
        /// <returns></returns>
        public static string AesDecrypt(this string decryptString, string key, string iv,
            PaddingMode paddingMode = PaddingMode.Zeros, CipherMode cipherMode = CipherMode.CBC,
            int keyLength = -1, int ivLength = -1, Encoding? encoding = null)
        {
            try
            {
                var inputByteArray = decryptString.ToToHexByte();
                SymmetricAlgorithm des = Rijndael.Create();
                des.Key = Encoding.ASCII.GetBytes(keyLength > 0 ? key.Substring(0, keyLength) : key);
                des.IV = Encoding.ASCII.GetBytes(ivLength > 0 ? iv.Substring(0, ivLength) : iv);
                des.Padding = paddingMode; // PaddingMode.Zeros
                des.Mode = cipherMode;     // CipherMode.CBC;
                var mStream = new MemoryStream();
                var cStream = new CryptoStream(mStream, des.CreateDecryptor(), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                var desDecryBytes = mStream.ToArray();
                if (encoding == null) encoding = Encoding.UTF8;
                return encoding.GetString(desDecryBytes);
            }
            catch (Exception ex)
            {
                return decryptString;
            }
        }

        /// <summary>
        /// 转16进制字符串
        /// </summary>
        /// <param name="hexString">待转换字符串</param>
        /// <returns></returns>
        public static byte[] ToToHexByte(this string hexString)
        {
            try
            {
                hexString = hexString.Replace(" ", "");
                if ((hexString.Length % 2) != 0)
                    hexString += " ";
                byte[] returnBytes = new byte[hexString.Length / 2];
                for (int i = 0; i < returnBytes.Length; i++)
                    returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                return returnBytes;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}