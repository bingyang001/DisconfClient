namespace DisconfClient
{
    /// <summary>
    /// 配置项值变更时的回调类的接口
    /// </summary>
    public interface ICallback
    {
        /// <summary>
        /// 激活回调方法
        /// </summary>
        void Invoke();
    }
}
