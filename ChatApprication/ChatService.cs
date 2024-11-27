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
    private static Object _Locker = new Object();
    static List<CommentClient> comments = new();


    public async Task<ClientStreamingResult<string, bool>> SaveAndShowCommentAsync()
    {
        var context = this.GetClientStreamingContext<string, bool>();

        var id = context.GetHashCode();

        await context.ForEachAsync(x =>
        {
            var idAndCommnet = new CommentClient() { SessionID = id, Comment = $"{id}さん ; {x}" };
            lock(_Locker)
            {
                comments.Add(idAndCommnet);
            }
    
        });

            return context.Result(false);
    }



    public async UnaryResult<List<string>> GetArchiveAsync()
    {
        lock(_Locker)
        {
            return comments
            .Select(x => x.Comment)
            .ToList();
        }
    }

}
