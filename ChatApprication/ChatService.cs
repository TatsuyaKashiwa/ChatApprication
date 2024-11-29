using ChatApplication.ServiceDefinition;
using MagicOnion;
using MagicOnion.Server;

namespace ChatApprication.Service;

/// <summary>
/// クライアント名とGUIDを紐づけるrecord型
/// </summary>
public record ClientId
{
    public required string ClientName { get; init; }
    public required string ClientGuid { get; init; }
}

/// <summary>
/// 名前とコメントを紐づけるrecord型
/// </summary>
/// <remarks>
/// ClientName : クライアント名
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
    /// Client(or Duplex)Streamの仕様により引数を渡せないためstaticな変数として保持
    /// </remarks>
    private static string _id = "";

    // TODO 通常のObjectへ変更
    /// <summary>
    /// lockのためのObject
    /// </summary>
    /// <remarks>
    /// lock用ObjectのインスタンスとしてObject型の変数を用意
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

    public async UnaryResult<string> GetMyGuid() => Guid.NewGuid().ToString();

    public async UnaryResult<bool> RegisterClientData(string handlename, string guid) 
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
            lock (this._Locker)
            {
                _id = guid;
                var clientData = new ClientId { ClientName = handlename, ClientGuid = guid };
                _clientDataSet.Add(clientData);
            }
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
        //CHANGED GUIDによるものへ変更
        //var id = _id.ToString();
        var name = _clientDataSet
            .Where(x => x.ClientGuid == _id)
            .Select(x => x.ClientName)
            .Single();

        //コメント追加時に(はラムダ式の式部分のみが)実行される
        //xにはクライアントからの投稿の文字列が入る
        //staticなListへアクセスする際にはlockで排他制御を行う
        await context.ForEachAsync(x =>
        {
            var idAndCommnet = new CommentClient() { ClientName = name, Comment = $"{name}さん ; {x}" };
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
