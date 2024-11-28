using ChatApplication.ServiceDefinition;
using MagicOnion;
using MagicOnion.Server;

namespace ChatApprication.Service;

public record CommentClient
{
    public required int SessionID { get; init; }
    public required string Comment { get; init; }
}

public class ChatService : ServiceBase<IChatService>, IChatService
{
    private static int _id = 0;

    private static Lazy<Object> _Locker = new();

    /// <summary>
    /// aaaa
    /// </summary>
    private static List<CommentClient> _comments = new();

    public async Task<ClientStreamingResult<string, bool>> SaveCommentAsync()
    {
        //stream確立時に
        var context = this.GetClientStreamingContext<string, bool>();

        var id = _id;
        _id++;

        //コメント追加時はラムダ式の式部分のみが実行される
        await context.ForEachAsync(x =>
        {
            var idAndCommnet = new CommentClient() { SessionID = id, Comment = $"{id}さん ; {x}" };
            lock (_Locker.Value)
            {
                _comments.Add(idAndCommnet);
            }

        });

        //クライアントからResponseAsyncが呼ばれたとき(finishが入力されたとき)実行
        //この式が呼ばれることでClientStreamのResponseが返されTCPコネクションが切断される。
        return context.Result(false);
    }



    public async UnaryResult<List<string>> GetArchiveAsync()
    {
        lock (_Locker.Value)
        {
            return _comments
            .Select(x => x.Comment)
            .ToList();
        }
    }

}
