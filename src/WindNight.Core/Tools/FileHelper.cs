using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindNight.Core.@internal;

namespace WindNight.Core.IO
{
    public static class FileHelper
    {
        static Encoding FixEncoding(Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            return encoding;
        }

        /// <summary>
        /// 检测文件的编码
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件编码</returns>
        public static Encoding DetectEncoding(string filePath)
        {
            using (var reader = new StreamReader(filePath, Encoding.Default, true))
            {
                reader.Peek(); // 触发编码检测
                return reader.CurrentEncoding;
            }
        }

        // 读取文本文件的所有内容
        public static string ReadAllText(string filePath, Encoding encoding = null)
        {
            try
            {

                encoding = FixEncoding(encoding);
                return File.ReadAllText(filePath, encoding);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"ReadAllText({filePath}) Handler Error {ex.Message}", ex);
                return null;
            }

        }

        // 写入文本到文件，如果文件存在则覆盖
        public static void WriteAllText(string filePath, string content, Encoding encoding = null)
        {
            try
            {
                Monitor.Enter(filePath);
                encoding = FixEncoding(encoding);

                File.WriteAllText(filePath, content, encoding);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"WriteAllText({filePath}) Handler Error {ex.Message}", ex);
            }
            finally
            {
                Monitor.Exit(filePath);
            }
        }

        // 追加文本到文件
        public static void AppendAllText(string filePath, string content, Encoding encoding = null)
        {
            try
            {
                encoding = FixEncoding(encoding);

                File.AppendAllText(filePath, content, encoding);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"AppendAllText({filePath}) Handler Error {ex.Message}", ex);

            }
        }


        // 读取所有行到字符串数组
        public static string[] ReadAllLines(string filePath, Encoding encoding = null)
        {
            try
            {
                encoding = FixEncoding(encoding);

                return File.ReadAllLines(filePath, encoding);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"ReadAllLines({filePath}) Handler Error {ex.Message}", ex);
                return null;
            }
        }


        // 写入字符串数组到文件的每一行
        public static void WriteAllLines(string filePath, string[] content, Encoding encoding = null)
        {
            try
            {

                encoding = FixEncoding(encoding);
                File.WriteAllLines(filePath, content, encoding);

            }
            catch (Exception ex)
            {
                LogHelper.Error($"WriteAllLines({filePath}) Handler Error {ex.Message}", ex);
            }
        }

        // 检查文件是否存在
        public static bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }

        // 创建或覆盖文件
        public static void CreateFile(string filePath)
        {
            try
            {
                using (var stream = File.Create(filePath))
                {
                    // 文件被创建后立即关闭流
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error($"CreateFile({filePath}) Handler Error {ex.Message}", ex);
            }
        }


        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="sourceFilePath">源文件路径</param>
        /// <param name="destinationFilePath">目标文件路径</param>
        /// <param name="overwrite">是否覆盖</param>
        public static void MoveFile(string sourceFilePath, string destinationFilePath, bool overwrite = false)
        {
            try
            {
                if (!File.Exists(sourceFilePath))
                {
                    throw new FileNotFoundException("源文件未找到", sourceFilePath);
                }

                if (overwrite && File.Exists(destinationFilePath))
                {
                    File.Delete(destinationFilePath);
                }

                File.Move(sourceFilePath, destinationFilePath);
            }
            catch (Exception ex)
            {

                LogHelper.Error($"MoveFile(source:{sourceFilePath},dest:{destinationFilePath}) Handler Error {ex.Message}", ex);

            }


        }



#if NETSTANDARD2_1_OR_GREATER

        // 读取文本文件的所有内容
        public static async Task<string> ReadAllTextAsync(string filePath, Encoding encoding = null)
        {
            try
            {

                encoding = FixEncoding(encoding);
                return await File.ReadAllTextAsync(filePath, encoding);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"ReadAllText({filePath}) Handler Error {ex.Message}", ex);
                return null;
            }

        }

        // 写入文本到文件，如果文件存在则覆盖
        public static async Task WriteAllTextAsync(string filePath, string content, Encoding encoding = null)
        {
            try
            {
                Monitor.Enter(filePath);
                encoding = FixEncoding(encoding);

                await File.WriteAllTextAsync(filePath, content, encoding);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"WriteAllText({filePath}) Handler Error {ex.Message}", ex);
            }
            finally
            {
                Monitor.Exit(filePath);
            }
        }

        // 追加文本到文件
        public static async Task AppendAllTextAsync(string filePath, string content, Encoding encoding = null)
        {
            try
            {
                encoding = FixEncoding(encoding);

                await File.AppendAllTextAsync(filePath, content, encoding);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"AppendAllText({filePath}) Handler Error {ex.Message}", ex);

            }
        }


        // 读取所有行到字符串数组
        public static async Task<string[]> ReadAllLinesAsync(string filePath, Encoding encoding = null)
        {
            try
            {
                encoding = FixEncoding(encoding);

                return await File.ReadAllLinesAsync(filePath, encoding);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"ReadAllLines({filePath}) Handler Error {ex.Message}", ex);
                return null;
            }
        }


        // 写入字符串数组到文件的每一行
        public static async Task WriteAllLinesAsync(string filePath, string[] content, Encoding encoding = null)
        {
            try
            {

                encoding = FixEncoding(encoding);
                await File.WriteAllLinesAsync(filePath, content, encoding);

            }
            catch (Exception ex)
            {
                LogHelper.Error($"WriteAllLines({filePath}) Handler Error {ex.Message}", ex);
            }
        }




#endif


    }
}
