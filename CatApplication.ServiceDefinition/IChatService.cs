using MagicOnion;

namespace ChatApplication.ServiceDefinition;
public interface IChatService : IService<IChatService>
{
    public Task<ClientStreamingResult<string, bool>> SaveCommentAsync();

    public UnaryResult<List<string>> GetArchiveAsync();
}
