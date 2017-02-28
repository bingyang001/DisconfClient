using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DisconfClient
{
    public class ConsoleLogAdapter : ILogger
    {
        /// <summary>
        /// 同样是记录信息，不过出现的频率要比Trace少一些，一般用来调试程序
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Debug(string message, Exception exception = null)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Debug#{0} ThreadId: {1} Message: {2} {3}", DateTime.Now, Thread.CurrentThread.ManagedThreadId, message, exception == null ? "" : "Exception:" + exception.ToString());
            Console.ResetColor();
        }

        /// <summary>
        /// 信息类型的消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Info(string message, Exception exception = null)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Info#{0} ThreadId: {1} Message: {2} {3}", DateTime.Now, Thread.CurrentThread.ManagedThreadId, message, exception == null ? "" : "Exception:" + exception.ToString());
            Console.ResetColor();
        }

        /// <summary>
        /// 警告信息，一般用于比较重要的场合
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Warn(string message, Exception exception = null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Warn#{0} ThreadId: {1} Message: {2} {3}", DateTime.Now, Thread.CurrentThread.ManagedThreadId, message, exception == null ? "" : "Exception:" + exception.ToString());
            Console.ResetColor();
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Error(string message, Exception exception = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error#{0} ThreadId: {1} Message: {2} {3}", DateTime.Now, Thread.CurrentThread.ManagedThreadId, message, exception == null ? "" : "Exception:" + exception.ToString());
            Console.ResetColor();
        }

        /// <summary>
        /// 致命异常信息。一般来讲，发生致命异常之后程序将无法继续执行。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Fatal(string message, Exception exception = null)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Fatal#{0} ThreadId: {1} Message: {2} {3}", DateTime.Now, Thread.CurrentThread.ManagedThreadId, message, exception == null ? "" : "Exception:" + exception.ToString());
            Console.ResetColor();
        }
    }
}
