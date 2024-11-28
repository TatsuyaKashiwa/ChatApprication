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

    private Lazy<Object> _Locker = new Lazy<Object>(() =>new Object());

    /// <summary>
    /// クライアントから投稿されたコメントを保存するList
    /// </summary>
    /// <remarks>
    /// すべてのクライアントからのコメントを保存するためstaticなフィールドとした
    /// 
    /// 初期化は一回のみとしたいため,LazyThreadSafetyModeをExecutionAndPublicationとした
    /// </remarks>
    private static List<CommentClient> _comments = new List<CommentClient>();

    public async Task<ClientStreamingResult<string, bool>> SaveCommentAsync()
    {
        //context取得からForEachAsyncの上までの領域は最初にSaveCommentAsyncがクライアントから呼ばれたときに一度だけ呼ばれる
        var context = this.GetClientStreamingContext<string, bool>();

        //この領域で_idをローカル変数に取り込むことでClientStream通信に固有のidを取得することができる
        var id = _id;
        _id++;

        //コメント追加時に(はラムダ式の式部分のみが)実行される
        //xにはクライアントからの投稿の文字列が入る
        //staticなListへアクセスする際にはlockで排他制御を行う
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
