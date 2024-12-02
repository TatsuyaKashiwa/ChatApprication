using MagicOnion;

namespace ChatApplication.ServiceDefinition;

public struct CommentInfomation 
{
    public string Name { get; set; }
    public string Guid { get; set; }
    public string Comment { get; set; 
}

    public interface IChatService : IService<IChatService>
    {
     public CommentInfomation commentInfomation { get; set; }
    //GUIDを発行
    public UnaryResult<string> GetMyGuid();

    //クライアント情報を設定
    public UnaryResult<bool> RegisterClientData(string handlename, string guid);

    //ClientStreamingならびにコメント保管
    public Task<ClientStreamingResult<string, bool>> SaveCommentAsync();

    //コメント履歴表示
    public UnaryResult<List<string>> GetArchiveAsync();
}
