using ChatApplication.ServiceDefinition;
using MagicOnion;
using MagicOnion.Server;

namespace ChatApprication.Service;

/// <summary>
/// クライアント名とGUIDを紐づけるrecord型
/// </summary>
public record ClientData
{
    public required string ClientName { get; init; }
    public required string ClientGuid { get; init; }
}

/// <summary>
/// 名前とコメントを紐づけるrecord型
/// </summary>
/// <remarks>
/// PostedClientName : クライアント名
/// Comment ： 投稿コメント
/// </remarks>
public record CommentInformations
{
    public required string PostedClientName { get; init; }
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
    private static string _guid = "";

    /// <summary>
    /// lockのためのObject
    /// </summary>
    /// <remarks>
    /// lock用ObjectのインスタンスとしてObject型の変数を用意
    /// </remarks>
    private Object _Locker = new();

    //クライアント情報を保持するクラス
    private static List<ClientData> _clientDataSet = [];

    /// <summary>
    /// クライアントから投稿されたコメントを保存するList
    /// </summary>
    /// <remarks>
    /// すべてのクライアントからのコメントを保存するためstaticなフィールドとした
    /// </remarks>
    private static List<CommentInformation> _comments = [];

    //GUIDを取得してクライアントへ返却
    public async UnaryResult<string> GetMyGuid()
    {
        return Guid.NewGuid().ToString();
    }


    /// <summary>
    /// クライアント名とGUIDをセットにしたrecord型をListに登録
    /// </summary>
    /// <param name="handlename">クライアントから入力されたハンドルネーム</param>
    /// <param name="guid">クライアントが保持するGUID</param>
    /// <returns>
    /// true : クライアントでループを継続
    /// false : クライアントでループを抜けてClientStream接続へ進む
    /// </returns>
    /// <remarks>
    /// LINQの結果がtrue(すでに登録されている名前)であればtrueを返し、
    /// falseであれば引数 guid をサーバ側のstaticな_id変数に保持し、
    /// 引数 handlename とともにrecord型にして
    /// Listに追加してfalseを返す。
    /// falseの分岐の処理はstaticな変数へのアクセス・変更となるのでlockを掛けた
    /// </remarks>
    public async UnaryResult<bool> RegisterClientData(string handlename, string guid)
    {
        var isNameExists = _clientDataSet
            .Select(x => x.ClientName)
            .Any(x => x == handlename);

        if (isNameExists)
        {
            return true;
        }
        else
        {
            lock (this._Locker)
            {
                // TODO: 重複に対応する仕組みを検討すること、dictionary で管理するなど
                // TODO: _guid が引数で渡せないとあるが、カスタム構造体か文字列の前置詞として追記を試してみては？
                //       引数を string にしていることが要因なのでは？
                _guid = guid;
                var clientData = new ClientData { ClientName = handlename, ClientGuid = _guid };
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
    public async Task<ClientStreamingResult<CommentInformation, bool>> SaveCommentAsync()
    {
        //context取得からForEachAsyncの上までの領域は最初にSaveCommentAsyncがクライアントから呼ばれたときに一度だけ呼ばれる
        var context = this.GetClientStreamingContext<CommentInformation, bool>();

        //GUIDに合致するクライアントのハンドルネームを取得する
        var name = _clientDataSet
            .Where(x => x.ClientGuid == _guid)
            .Select(x => x.ClientName)
            .Single();

        //コメント追加時に(はラムダ式の式部分のみが)実行される
        //xにはクライアントからの投稿の文字列が入る
        //staticなListへアクセスする際にはlockで排他制御を行う
        await context.ForEachAsync(x =>
        {
            // TODO: typo
            //CHECKED: 修正しました
            //var idAndComment = new CommentInformations() { PostedClientName = name, Comment = $"{name}さん ; {x}" };
            lock (this._Locker)
            {
                _comments.Add(x);
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
