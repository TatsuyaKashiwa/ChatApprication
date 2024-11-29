using MagicOnion;

namespace ChatApplication.ServiceDefinition;
public interface IChatService : IService<IChatService>
{
    //GUIDを発行
    public UnaryResult<string> GetMyGuid();

    //クライアント情報を設定
    public UnaryResult<bool> RegisterClientData(string handlename);

    //ClientStreamingならびにコメント保管
    public Task<ClientStreamingResult<string, bool>> SaveCommentAsync();

    //コメント履歴表示
    public UnaryResult<List<string>> GetArchiveAsync();
}
