using ChatApplication.ServiceDefinition;
using MagicOnion;
using MagicOnion.Server;

namespace ChatApprication.Service;

public record ClientId
{
    public required string ClientName { get; init; }
    public required string ClientGuid { get; init; }
}

/// <summary>
/// idとコメントを紐づけるrecord型
/// </summary>
/// <remarks>
/// ClientName : クライアントID
/// Comment ： 投稿コメント
/// </remarks>
public record CommentClient
{
    public required string ClientName { get; init; }
    public required string Comment { get; init; }
}

public class ChatService : ServiceBase<IChatService>, IChatService
{
    /// <summary>
    /// ユーザID
    /// </summary>
    /// <remarks>
    /// ClientStreamの接続ごとに各クライアントでローカル変数として保持する
    /// </remarks>
    private static int _id = 0;

    // TODO 通常のObjectへ変更
    /// <summary>
    /// lockのためのObject
    /// </summary>
    /// <remarks>
    /// lock用Objectのインスタンスはlockが必要になったときに用いられるため
    /// Lazy Objectとした。
    /// </remarks>
    private Object _Locker = new();

    private static List<ClientId> _clientDataSet = new();

    /// <summary>
    /// クライアントから投稿されたコメントを保存するList
    /// </summary>
    /// <remarks>
    /// すべてのクライアントからのコメントを保存するためstaticなフィールドとした
    /// </remarks>
    private static List<CommentClient> _comments = new();

    public async UnaryResult<bool> SetYourGuid(string handlename) 
    {
        var query = _clientDataSet
            .Select(x => x.ClientName)
            .Any(x => x.Equals(handlename));

        if (query)
        {
            return true;
        }
        else 
        {
            string guid = Guid.NewGuid().ToString();
            var clientData = new ClientId { ClientName = handlename, ClientGuid = guid };
            _clientDataSet.Add(clientData);
            return false; 
        }
    }

    /// <summary>
    /// ClientStream通信を行うクラス
    /// </summary>
    /// <returns>
    /// false(クライアント側の無限ループを止める)
    /// </returns>
    /// <remarks>
    /// ClientStream通信を行うクラス
    /// ClientStreamで接続されている間クライアントからの呼び出しで
    /// コメントの保存を行う
    /// クライアントから終了の指示が送られると returnが実行されて接続を切断する。
    /// </remarks>
    public async Task<ClientStreamingResult<string, bool>> SaveCommentAsync()
    {
        //context取得からForEachAsyncの上までの領域は最初にSaveCommentAsyncがクライアントから呼ばれたときに一度だけ呼ばれる
        var context = this.GetClientStreamingContext<string, bool>();

        // TODO UIDやOIDを用いたものへ変更
        //この領域で_idをローカル変数に取り込むことでアクセスしたクライアントのClientStream通信のidを取得する
        //取得後にインクリメントすることで各クライアントで固有の値とする
        var id = _id.ToString();
        

        //コメント追加時に(はラムダ式の式部分のみが)実行される
        //xにはクライアントからの投稿の文字列が入る
        //staticなListへアクセスする際にはlockで排他制御を行う
        await context.ForEachAsync(x =>
        {
            var idAndCommnet = new CommentClient() { ClientName = id, Comment = $"{id}さん ; {x}" };
            lock (this._Locker)
            {
                _comments
                .Add(idAndCommnet);
            }
        });

        //クライアントからResponseAsyncが呼ばれたとき(finishが入力されたとき)実行
        //この式が呼ばれることでClientStreamのResponseが返されTCPコネクションが切断される。
        return context.Result(false);
    }

    /// <summary>
    /// コメント履歴返却
    /// </summary>
    /// <returns>
    /// List<string>型のコメント履歴
    /// </returns>
    ///<remarks>
    ///id・コメントが保存されているListからコメントのみを取り出しクライアントへ返却する
    ///</remarks>
    public async UnaryResult<List<string>> GetArchiveAsync()
    {
        lock (this._Locker)
        {
            return _comments
            .Select(x => x.Comment)
            .ToList();
        }
    }

}
