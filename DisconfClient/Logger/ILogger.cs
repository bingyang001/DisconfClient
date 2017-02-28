using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DisconfClient
{
    public interface ILogger
    {
        /// <summary>
        /// 同样是记录信息，不过出现的频率要比Trace少一些，一般用来调试程序
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Debug(string message, Exception exception = null);

        /// <summary>
        /// 信息类型的消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Info(string message, Exception exception = null);

        /// <summary>
        /// 警告信息，一般用于比较重要的场合
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Warn(string message, Exception exception = null);

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Error(string message, Exception exception = null);

        /// <summary>
        /// 致命异常信息。一般来讲，发生致命异常之后程序将无法继续执行。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Fatal(string message, Exception exception = null);

    }
}
