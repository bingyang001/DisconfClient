using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DisconfClient
{
    public static class ActionRetryHelper
    {
        /// <summary>
        /// 方法重试。说明：方法至少执行一次，如果行异常时，则进入重试逻辑。
        /// </summary>
        /// <param name="action">action</param>
        /// <param name="retryCount">重试次数</param>
        /// <param name="retryTime">重试间隔时间</param>
        /// <param name="exceptionAction">发生异常时操作</param>
        /// <param name="errorHandle">异常消息处理</param>
        /// <param name="retryExceptionAction">超过重试次数后且任然失败的操作</param>
        public static void Retry(Action action, uint retryCount, TimeSpan retryTime, Action exceptionAction = null, Action<Exception> errorHandle = null, Action retryExceptionAction = null)
        {
            bool isException = false;
            int count = 0;
            do
            {
                try
                {
                    isException = false;
                    action();
                }
                catch (Exception ex)
                {
                    isException = true;
                    if (exceptionAction != null)
                        exceptionAction();
                    if (errorHandle != null)
                        errorHandle(ex);
                    Thread.Sleep(retryTime);
                }
            } while (isException && Interlocked.Increment(ref count) < retryCount);
            if (isException && count >= retryCount && retryExceptionAction != null)
                retryExceptionAction();
        }
        /// <summary>
        /// 方法重试。说明：方法至少执行一次，如果行异常时，则进入重试逻辑。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">action</param>
        /// <param name="retryCount">重试次数</param>
        /// <param name="retryTime">重试间隔时间</param>
        /// <param name="exceptionAction">发生异常时操作</param>
        /// <param name="errorHandle">异常消息处理</param>
        /// <param name="defaultReturnValue">默认返回值</param>
        /// <returns></returns>
        public static T Retry<T>(Func<T> action, uint retryCount, TimeSpan retryTime, Action exceptionAction = null, Action<Exception> errorHandle = null, T defaultReturnValue = default (T))
        {
            int count = 0;
            do
            {
                try
                {
                    return action();
                }
                catch (Exception ex)
                {
                    if (exceptionAction != null)
                        exceptionAction();
                    if (errorHandle != null)
                        errorHandle(ex);
                    Thread.Sleep(retryTime);
                }
            } while (Interlocked.Increment(ref count) < retryCount);
            return defaultReturnValue;
        }
    }
}
