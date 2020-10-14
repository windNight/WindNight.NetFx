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
        public static string Md5Encrypt(this string text, Encoding encoding = null)
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




    }
}