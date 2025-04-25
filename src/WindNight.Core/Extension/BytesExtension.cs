using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using WindNight.Linq.Extensions.Expressions;

namespace System.Text
{
    public static class BytesExtension
    {
        /// <summary>
        ///     When overridden in a derived class, decodes all the bytes in the specified byte
        ///     array into a string.
        /// </summary>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
        /// <param name="encoding">default is UTF8 .</param>
        /// <exception cref="ArgumentException">The byte array contains invalid Unicode code points.</exception>
        /// <exception cref="ArgumentNullException">bytes is null.</exception>
        /// <exception cref="DecoderFallbackException">
        ///     A fallback occurred (see Character Encoding in .NET for complete explanation)
        ///     -and- System.Text.Encoding.DecoderFallback is set to System.Text.DecoderExceptionFallback.
        /// </exception>
        /// <returns>
        ///     A string that contains the results of decoding the specified sequence of bytes.
        /// </returns>
        public static string ToGetString(this byte[] bytes, Encoding? encoding = null)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            return encoding.GetString(bytes);
        }

        /// <summary>
        ///     When overridden in a derived class, encodes all the characters in the specified
        ///     string into a sequence of bytes.
        /// </summary>
        /// <param name="text">The string containing the characters to encode. </param>
        /// <param name="encoding">default is UTF8 .</param>
        /// <exception cref="ArgumentNullException">text is null.</exception>
        /// <exception cref="EncoderFallbackException">
        ///     A fallback occurred (see Character Encoding in .NET for complete explanation)
        ///     -and- System.Text.Encoding.EncoderFallback is set to System.Text.EncoderExceptionFallback.
        /// </exception>
        /// <returns>
        ///     A byte array containing the results of encoding the specified set of characters.
        /// </returns>
        public static byte[] ToBytes(this string text, Encoding? encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return encoding.GetBytes(text);
        }

        /// <summary>
        ///     Converts the specified string, which encodes binary data as base-64 digits, to an equivalent 8-bit unsigned integer
        ///     array.
        /// </summary>
        /// <param name="base64Str">The string to convert. </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="base64Str" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="T:System.FormatException">
        ///     The length of <paramref name="base64Str" />, ignoring white-space characters, is not zero or a multiple of 4.
        ///     -or-
        ///     The format of <paramref name="base64Str" /> is invalid. <paramref name="base64Str" /> contains a non-base-64
        ///     character, more than two padding characters, or a non-white space-character among the padding characters.
        /// </exception>
        /// <returns>
        ///     An array of 8-bit unsigned integers that is equivalent to <paramref name="base64Str" />.
        /// </returns>
        public static byte[] FromBase64String(this string base64Str)
        {
            return Convert.FromBase64String(base64Str);
        }

        /// <summary>
        ///     Base64加密
        /// </summary>
        /// <param name="bytes"> </param>
        /// <returns>Base64后的字符串</returns>
        public static string ToBase64String(this byte[] bytes)
        {
            if (bytes.IsNullOrEmpty())
            {
                return string.Empty;
            }
            try
            {
                return Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }


        /// <summary>
        ///     ZIP解压
        /// </summary>
        /// <param name="zippedData"></param>
        /// <returns></returns>
        public static byte[] DoDecompress(this byte[] zippedData)
        {
            var ms = new MemoryStream(zippedData);
            var compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);
            var outBuffer = new MemoryStream();
            var block = new byte[1024];
            while (true)
            {
                var bytesRead = compressedzipStream.Read(block, 0, block.Length);
                if (bytesRead <= 0)
                    break;
                outBuffer.Write(block, 0, bytesRead);
            }

            compressedzipStream.Close();
            return outBuffer.ToArray();
        }

        /// <summary>
        ///     GZip压缩
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        public static byte[] DoCompress(this byte[] rawData)
        {
            var ms = new MemoryStream();
            var gZipStream = new GZipStream(ms, CompressionMode.Compress, true);
            gZipStream.Write(rawData, 0, rawData.Length);
            gZipStream.Close();
            return ms.ToArray();
        }

        /// <summary>
        ///     将传入的二进制字符串资料以GZip算法解压缩
        /// </summary>
        /// <param name="zippedString">经GZip压缩后的二进制字符串</param>
        /// <param name="encoding"> </param>
        /// <returns>原始未压缩字符串</returns>
        public static string GZipDecompressString(this string zippedString, Encoding? encoding = null)
        {
            if (zippedString.IsNullOrEmpty() || zippedString.Length == 0) return "";

            var zippedData = Convert.FromBase64String(zippedString);
            if (encoding == null) encoding = Encoding.UTF8;
            return DoDecompress(zippedData).ToGetString(encoding);
        }

        public static byte[] Combine(params byte[][] arrays)
        {
            var ret = new byte[arrays.Sum(x => x.Length)];
            var offset = 0;
            foreach (var data in arrays)
            {
                Buffer.BlockCopy(data, 0, ret, offset, data.Length);
                offset += data.Length;
            }

            return ret;
        }

        public static string ToHexString(this byte[] bytes, string splitStr = " ")
        {
            return bytes.ToHexString(0, bytes.Length, splitStr);
        }

        public static string ToHexString(this byte[] bytes, int startIndex, int length, string splitStr)
        {
            if (bytes == null || bytes.Length == 0 || length == 0) return string.Empty;
            var sb = new StringBuilder();
            for (var i = startIndex; i < bytes.Length && i < startIndex + length; i++)
            {
                var sh = bytes[i].ToString("X").PadLeft(2, '0');
                sb.Append($"{sh}{splitStr}");
            }

            return sb.ToString();
        }

        /// <summary>
        ///     带空格等分隔符的Hex字符串
        /// </summary>
        /// <param name="hexStr"></param>
        /// <returns></returns>
        public static byte[] ToBytesWithSplit(this string hexStr)
        {
            try
            {
                hexStr = hexStr.Trim();
                var arr = hexStr.Split(new[] { ' ', ',', ';', '\r', '\n' },
                    StringSplitOptions.RemoveEmptyEntries);
                var bytes = new byte[arr.Length];
                for (var i = 0; i < arr.Length; i++)
                {
                    bytes[i] = byte.Parse(arr[i], NumberStyles.HexNumber);
                }

                return bytes;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     将没有分隔符的字符串转成16进制，如果字符串格式是奇数，则最高位补0
        /// </summary>
        /// <param name="hexStr"></param>
        /// <returns></returns>
        public static byte[] ToBytesWithoutSplit(this string hexStr)
        {
            try
            {
                if (hexStr.IsNullOrEmpty()) return null;
                var len = hexStr.Length;
                var bytes = new byte[len / 2];
                if (len / 2 != 0)
                {
                    hexStr = hexStr.PadLeft(1, '0');
                }

                len = hexStr.Length;
                for (var i = 0; i < len / 2; i++)
                {
                    bytes[i] = byte.Parse(hexStr.Substring(2 * i, 2), NumberStyles.HexNumber);
                }

                return bytes;
            }
            catch
            {
                return null;
            }
        }
    }
}
