using ChatApplication.ServiceDefinition;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using System.Linq;

namespace ChatApprication.Service;

public record CommentClient 
{
    public required string SessionID { get; init; }
    public required string Comment { get; init; }
}

public class ChatService : ServiceBase<IChatService>, IChatService
{
    //HttpContextを使うなら別なstaticな変数に保持すること
    //Listであればpairを用いる or ディクショナリにタイムスタンプを含める。
    private List<CommentClient> _commentList = new();
    public void PostComment(ServerCallContext clientContext, string comment)
    {
        var httpContext = clientContext.GetHttpContext();
        var sessionID = httpContext.Session.ToString();
        var commentClient = new CommentClient() { SessionID = sessionID, Comment = comment };
        _commentList.Add(commentClient);
    }

    public async UnaryResult<List<string>> ShowCommentArchive() 
    {
        //LINQを用いなかったもの
        //List<string> comments = new();   
        //foreach (var comment in _commentList) 
        //{
        //    comments.Add(comment.Comment);
        //}
        return _commentList
            .Select(x=>x.Comment)
            .ToList();
    }
}
