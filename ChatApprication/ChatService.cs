using ChatApplication.ServiceDefinition;
using MagicOnion;
using MagicOnion.Server;

namespace ChatApprication.Service;

public record CommentClient
{
    //public required string SessionID { get; init; }
    public required int SessionID { get; init; }
    public required string Comment { get; init; }
}

public class ChatService : ServiceBase<IChatService>, IChatService
{
    private static int _id = 0;

    private static Lazy<Object> _Locker = new Lazy<Object>();
    
    private static List<CommentClient> _comments = new();

    public async UnaryResult<int> GetId() 
    {
        return _id++;
    }
    
    public async Task<ClientStreamingResult<string, bool>> SaveCommentAsync()
    {
        var context = this.GetClientStreamingContext<string, bool>();

        var id = context.GetHashCode();

        await context.ForEachAsync(x =>
        {
            var idAndCommnet = new CommentClient() { SessionID = id, Comment = $"{id}さん ; {x}" };
            lock(_Locker.Value)
            {
                _comments.Add(idAndCommnet);
            }
    
        });

            return context.Result(false);
    }



    public async UnaryResult<List<string>> GetArchiveAsync()
    {
        lock(_Locker.Value)
        {
            return _comments
            .Select(x => x.Comment)
            .ToList();
        }
    }

}
