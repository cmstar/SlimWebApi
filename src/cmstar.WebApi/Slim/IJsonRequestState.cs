namespace cmstar.WebApi.Slim
{
    /// <summary>
    /// 表示<see cref="IRequestState"/>，使用JSON作为请求数据。
    /// </summary>
    public interface IJsonRequestState : IRequestState
    {
        string RequestJson { get; }
    }
}
