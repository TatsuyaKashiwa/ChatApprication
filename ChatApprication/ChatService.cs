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

    public async Task<ClientStreamingResult<string, List<string>>> SaveAndShowCommentAsync()
    {
        var streaming = this.GetClientStreamingContext<string, List<string>>();
        var id = streaming.GetHashCode();
        List<CommentClient> commentList = new();
        await streaming.ForEachAsync(x =>
        {
            var commentClient = new CommentClient() { SessionID = id, Comment = $"{id}さん ; {x}" };
            commentList.Add(commentClient);
        });

        var returnComment = commentList
            .Select(x => x.Comment)
            .ToList();

        return streaming.Result(returnComment);
    }

}
