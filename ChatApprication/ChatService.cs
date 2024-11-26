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

    public async Task<ClientStreamingResult<string, bool>> SaveAndShowCommentAsync()
    {
        var streaming = this.GetClientStreamingContext<string, List<string>>();
        var id = streaming.GetHashCode();
        List<CommentClient> comments = new();
        await streaming.ForEachAsync(x =>
        {
            var idAndCommnet = new CommentClient() { SessionID = id, Comment = $"{id}さん ; {x}" };
            comments.Add(idAndCommnet);
        });

        //var returnComments = comments
        //    .Select(x => x.Comment)
        //    .ToList();

        //boolを返すには？
        return streaming.Result(true);
    }

    //streaming.Result(returnComments)

}
