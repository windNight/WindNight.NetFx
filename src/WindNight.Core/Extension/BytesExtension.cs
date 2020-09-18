using System.IO;
using System.IO.Compression;

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
        public static string ToGetString(this byte[] bytes, Encoding encoding = null)
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
        public static byte[] ToBytes(this string text, Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            return encoding.GetBytes(text);
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
            var compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true);
            compressedzipStream.Write(rawData, 0, rawData.Length);
            compressedzipStream.Close();
            return ms.ToArray();
        }

        /// <summary>
        ///     将传入的二进制字符串资料以GZip算法解压缩
        /// </summary>
        /// <param name="zippedString">经GZip压缩后的二进制字符串</param>
        /// <param name="encoding"> </param>
        /// <returns>原始未压缩字符串</returns>
        public static string GZipDecompressString(this string zippedString, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(zippedString) || zippedString.Length == 0) return "";

            var zippedData = Convert.FromBase64String(zippedString);
            if (encoding == null) encoding = Encoding.UTF8;
            return DoDecompress(zippedData).ToGetString(encoding);
        }
    }
}