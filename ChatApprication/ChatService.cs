using ChatApplication.ServiceDefinition;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;

namespace ChatApprication.Service;

public record CommentClient 
{
    //public required string SessionID { get; init; }
    public required int SessionID { get; init; }
    public required string Comment { get; init; }
}

public class ChatService : ServiceBase<IChatService>, IChatService
{
    //Client(あるいはDuplex)Streamingではメソッド内に結果を受ける変数,Collectionを入れるのでコメント化
    //private static List<CommentClient> _commentList = new();
    //IDを固定してテスト
    public async Task<ClientStreamingResult<string, List<string>>> SaveAndShowComment()
    {
        var context = this.GetClientStreamingContext<string, List<string>>();
        List<CommentClient> commentList = new();
        //以下五行Unaryの書式なのでコメントアウト
        //var httpContext = clientContext.GetHttpContext();
        //var sessionID = httpContext.Session.ToString();
        //var commentClient = new CommentClient() { SessionID = 1, Comment = comment };
        //_commentList.Add(commentClient);
        //return 's';
        await context.ForEachAsync(x =>
        {
            var commentClient = new CommentClient() { SessionID = 1, Comment = x };
            commentList.Add(commentClient);
        });

        var returnComment = commentList
            .Select(x => x.Comment)
            .ToList();

        return context.Result(returnComment);
    }

    public async UnaryResult<List<string>> ShowCommentArchive() 
    {
        return _commentList
            .Select(x=>x.Comment)
            .ToList();
    }
}
