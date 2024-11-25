using ChatApplication.ServiceDefinition;
using Grpc.Core;
using MagicOnion.Server;

namespace ChatApprication.Service;

public class ChatService : ServiceBase<IChatService>, IChatService
{
    //HttpContextを使うなら別なstaticな変数に保持すること
    //Listであればpairを用いる or ディクショナリにタイムスタンプを含める。
    private List<HttpContext> _commentList = new();
    public void PostComment(ServerCallContext clientContext, string comment)
    {
        var httpContext = clientContext.GetHttpContext();
        httpContext.Session.SetString($"{clientContext.ToString}", comment);
        _commentList.Add(httpContext);
    }

    public List<string> ShowCommentArchive() 
    {
        
    }
}
