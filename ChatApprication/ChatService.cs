using ChatApplication.ServiceDefinition;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace ChatApprication.Service;

public record CommentClient 
{
    //public required string SessionID { get; init; }
    public required int SessionID { get; init; }
    public required string Comment { get; init; }
}

public class ChatService : ServiceBase<IChatService>, IChatService
{

    //HttpContextを使うなら別なstaticな変数に保持すること
    //Listであればpairを用いる or ディクショナリにタイムスタンプを含める。
    private static List<CommentClient> _commentList = new();
    //IDを固定してテストするためコメント化
    //public async UnaryResult<char> PostComment(ServerCallContext clientContext, string comment)
    public async UnaryResult<char> PostComment(int id, string comment)
    {
        //下2行IDをCLからの定数でテストするためコメント化
        //var httpContext = clientContext.GetHttpContext();
        //var sessionID = httpContext.Session.ToString();
        var commentClient = new CommentClient() { SessionID = id, Comment = comment };
        _commentList.Add(commentClient);
        return 's';
    }

    public async UnaryResult<List<string>> ShowCommentArchive() 
    {
        //LINQを用いなかったもの
        List<string> comments = new();   
        foreach (var comment in _commentList) 
        {
            comments.Add(comment.Comment);
        }
        return comments;
        //return _commentList
        //    .Select(x=>x.Comment)
        //    .ToList();
    }
}
