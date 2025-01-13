using ChatApplication.ServiceDefinition;
using MagicOnion;
using MagicOnion.Server;

namespace ChatApprication.Service;

public class ChatService : ServiceBase<IChatService>, IChatService
{
    // lock用Object
    private Object _Locker = new();

    //クライアント情報を保持するディクショナリ
    //登録順は問わないのでディクショナリにした
    //{guid, handleName}
    private static Dictionary<string, string> _clientDataSet = [];

    // すべてのクライアントからのコメントを保存するためstaticなフィールドとした
    private static List<CommentInformation> _comments = [];

    //名前(クライアント情報ディクショナリのValue)の重複を確認
    //重複していればtrueを返却
    public async UnaryResult<bool> ExistsName(string handleName)
    {
        return _clientDataSet.ContainsValue(handleName);
    }


    //guidとクライアントから送られたハンドルネームを対にしてディクショナリに保存
    public async UnaryResult<string> RegisterClientData(string handleName)
    {
        var guid = Guid.NewGuid().ToString();
        lock (this._Locker)
        {
            _clientDataSet.Add(guid, handleName);
        }
        return guid;
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

        //コメント追加時に(はラムダ式の式部分のみが)実行される
        //xにはコメント情報の構造体が入る
        //staticなListへアクセスする際にはlockで排他制御を行う
        await context.ForEachAsync(x =>
        {
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
    public async UnaryResult<List<CommentInformation>> GetArchiveAsync()
    {
        lock (this._Locker)
        {
            return _comments;
        }
    }

    //実行したクライアント自身のコメントを表示する
    public async UnaryResult<List<string>> GetYourCommentAsync(string guid)
    {
        lock (this._Locker)
        {
            return _comments
                .Where(x => x.Guid == guid)
                .Select(x => x.Comment)
                .ToList();
        }
    }

}
