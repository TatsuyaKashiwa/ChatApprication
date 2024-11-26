using MagicOnion;

namespace ChatApplication.ServiceDefinition;
public interface IChatService : IService<IChatService>
{
    public Task<ClientStreamingResult<string, List<string>>> SaveAndShowCommentAsync();
}
