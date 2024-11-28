using MagicOnion;

namespace ChatApplication.ServiceDefinition;
public interface IChatService : IService<IChatService>
{
    //ClientStreamingならびにコメント保管
    public Task<ClientStreamingResult<string, bool>> SaveCommentAsync();

    //コメント履歴表示
    public UnaryResult<List<string>> GetArchiveAsync();
}
