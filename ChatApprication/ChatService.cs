using ChatApplication.ServiceDefinition;
using Grpc.Core;
using MagicOnion.Server;

namespace ChatApprication.Service;

public class ChatService : ServiceBase<IChatService>, IChatService
{
    private List<HttpContext> _commentList = new();
    public void PostComment(ServerCallContext sessionId, string comment)
    {
        var httpContext = sessionId.GetHttpContext();
        httpContext.Session.SetString($"{sessionId.ToString}", comment);
        _commentList.Add(httpContext);
    }
}
