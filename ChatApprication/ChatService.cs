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
    private static ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();
    static List<CommentClient> comments = new();

    public async Task<ClientStreamingResult<string, bool>> SaveAndShowCommentAsync()
    {
        //[TODO]型を確認すること！
        var streaming = this.GetClientStreamingContext<string, bool>();
        var id = streaming.GetHashCode();
        await streaming.ForEachAsync(x =>
        {
            var idAndCommnet = new CommentClient() { SessionID = id, Comment = $"{id}さん ; {x}" };
            comments.Add(idAndCommnet);
        });

        return streaming.Result(false);
    }

    //streaming.Result(returnComments)

    public async UnaryResult<List<string>> GetArchiveAsync() 
    {
        return comments
            .Select(x => x.Comment)
            .ToList();
    }

}
