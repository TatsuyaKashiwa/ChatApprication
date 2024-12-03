using ChatApplication.ServiceDefinition;
using MagicOnion;
using MagicOnion.Server;

namespace ChatApprication.Service;

public class ChatService : ServiceBase<IChatService>, IChatService
{
    /// <summary>
    /// lockのためのObject
    /// </summary>
    /// <remarks>
    /// lock用ObjectのインスタンスとしてObject型の変数を用意
    /// </remarks>
    private Object _Locker = new();

    //クライアント情報を保持するディクショナリ
    //登録順は問わないのでディクショナリにした
    //{guid, handleName}
    private static Dictionary<string, string> _clientDataSet = [];

    /// <summary>
    /// クライアントから投稿されたコメントを保存するList
    /// </summary>
    /// <remarks>
    /// すべてのクライアントからのコメントを保存するためstaticなフィールドとした
    /// </remarks>
    private static List<CommentInformation> _comments = [];

    //名前(クライアント情報ディクショナリのValue)の重複を確認
    //重複していればtrueを返却
    public async UnaryResult<bool> ExistsName(string handleName)
    {
        return _clientDataSet.ContainsValue(handleName);
    }


    /// <summary>
    /// クライアント名とGUIDをセットにしたrecord型をListに登録
    /// </summary>
    /// <param name="handleName">クライアントから入力されたハンドルネーム</param>
    /// <param name="guid">クライアントが保持するGUID</param>
    /// <returns>
    /// true : クライアントでループを継続
    /// false : クライアントでループを抜けてClientStream接続へ進む
    /// </returns>
    /// <remarks>
    /// LINQの結果がtrue(すでに登録されている名前)であればtrueを返し、
    /// falseであれば引数 guid をサーバ側のstaticな_id変数に保持し、
    /// 引数 handleName とともにrecord型にして
    /// Listに追加してfalseを返す。
    /// falseの分岐の処理はstaticな変数へのアクセス・変更となるのでlockを掛けた
    /// </remarks>
    public async UnaryResult<string> RegisterClientData(string handleName)
    {
        var guid = Guid.NewGuid().ToString();
        lock (this._Locker)
        {
            // TODO: 重複に対応する仕組みを検討すること、dictionary で管理するなど
            // TODO: _guid が引数で渡せないとあるが、カスタム構造体か文字列の前置詞として追記を試してみては？
            //       引数を string にしていることが要因なのでは？
            //CHECKED: ディクショナリでの管理に変更した
            //CHECKED: カスタム構造体を利用しstaticなGUID保持変数は削除した。
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
    public async UnaryResult<List<CommentInformation>> GetArchiveAsync()
    {
        lock (this._Locker)
        {
            return _comments;
        }
    }

    public async UnaryResult<List<string>> GetMyCommnetAsync(string guid) 
    {
        return _comments
            .Where(x => x.Guid == guid)
            .Select(x => x.Comment)
            .ToList();
    }

}
