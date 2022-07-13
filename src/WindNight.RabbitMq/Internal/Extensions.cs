using System.Security.Cryptography;
using System.Text;

namespace WindNight.RabbitMq.Internal;

internal static class Extensions
{
    //    /// <summary>
    //    ///     Base64加密
    //    /// </summary>
    //    /// <param name="str">待加密字符串</param>
    //    /// <param name="encode">字符编码，默认UTD-8</param>
    //    /// <returns>Base64后的字符串</returns>
    //    public static string ToBase64String(this string str, string encode = "UTF-8")
    //    {
    //        if (string.IsNullOrEmpty(str)) return str;
    //        return Convert.ToBase64String(Encoding.GetEncoding(encode).GetBytes(str));
    //    }

    //    /// <summary>
    //    ///     Base64解密
    //    /// </summary>
    //    /// <param name="str">待解密字符串</param>
    //    /// <param name="encode">字符编码，默认UTD-8</param>
    //    /// <returns>Base64解密后的字符串</returns>
    //    public static string FromBase64String(this string str, string encode = "UTF-8")
    //    {
    //        if (string.IsNullOrEmpty(str)) return str;
    //        try
    //        {
    //            return Encoding.GetEncoding(encode).GetString(Convert.FromBase64String(str));
    //        }
    //        catch
    //        {
    //            return null;
    //        }
    //    }

    /// <summary>
    ///     AES加密
    /// </summary>
    /// <param name="str">待加密字符串</param>
    /// <param name="key">密钥(长度16位)</param>
    /// <returns>加密后的字符串 (BASE64)</returns>
    public static string ToAesEncrypt(this string str, string key)
    {
        using (var aes = Aes.Create())
        {
            var bytes = str.ToBytes(); // Encoding.UTF8.GetBytes(str);
            aes.Key = key.ToBytes(); // Encoding.UTF8.GetBytes(key);
            aes.IV = key.ToBytes(); // Encoding.UTF8.GetBytes(key);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (var cryptoTransform = aes.CreateEncryptor())
            {
                var bResult = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);

                return bResult.ToBase64String(); //返回base64加密; 
            }
        }
    }

    /// <summary>
    ///     AES解密
    /// </summary>
    /// <param name="str">待解密字符串 Base64 （UTF-8）</param>
    /// <param name="key">密钥(长度16位)</param>
    /// <returns>解密后的字符串</returns>
    public static string ToAesDecrypt(this string str, string key)
    {
        using (var aes = Aes.Create())
        {
            var bytes = str.FromBase64String(); // Convert.FromBase64String(str); //解密base64;   
            aes.Key = key.ToBytes(); // Encoding.UTF8.GetBytes(key);
            aes.IV = key.ToBytes(); // Encoding.UTF8.GetBytes(key);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (var cryptoTransform = aes.CreateDecryptor())
            {
                var bResult = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
                return bResult.ToGetString(); // Encoding.UTF8.GetString(bResult);
            }
        }
    }
}