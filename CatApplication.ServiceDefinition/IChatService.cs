using MagicOnion;

namespace ChatApplication.ServiceDefinition;
public interface IChatService : IService<IChatService>
{
    public Task<ClientStreamingResult<string, bool>> SaveAndShowCommentAsync();

    public UnaryResult<List<string>> GetArchiveAsync();
}
