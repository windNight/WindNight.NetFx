using System;
using System.IO;
using WindNight.Core.Abstractions;

namespace WindNight.RabbitMq.Internal;

internal static class RecordLog
{
    public static void Debug(string msg)
    {
#if !DEBUG
            return;
#endif
        Log(LogLevels.Debug, msg);
    }

    public static void Info(string msg)
    {
        Log(LogLevels.Information, msg);
    }


    public static void Err(string msg, Exception ex)
    {
        Log(LogLevels.Error, msg, ex);
    }

    /// <summary>
    ///     写日志
    /// </summary>
    /// <param name="level"></param>
    /// <param name="msg"></param>
    /// <param name="ex"></param>
    public static void Log(LogLevels level, string msg, Exception ex = null)
    {
        try
        {
            //if (CtRabbitMQLogger.LogCustom != null)
            //    CtRabbitMQLogger.LogCustom.Log(level, msg, ex);
            //else
            //{
            var dt = HardInfo.Now;
            var path = AppDomain.CurrentDomain.BaseDirectory + "Logs/RabbitMQLog";
            var filename = path + "/" + (dt.Year * 10000 + dt.Month * 100 + dt.Day) + ".txt";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!File.Exists(filename))
                File.Create(filename).Close();

            using (var sw = new StreamWriter(filename, true))
            {
                var txt =
                    $"{dt:yyyy-MM-dd HH:mm:ss:fff}：【{level}】：{msg} {(ex == null ? "" : $"：Error->{ex.Message}\r\n{ex.StackTrace}")}";
                //dt.ToString("yyyy-MM-dd HH:mm:ss:fff") + "：" + msg + " Error :" +
                //         (ex == null ? "" : ex.Message);
                sw.WriteLine(txt);
            }

            // }
        }
        catch
        {
        }
    }
}