using MagicOnion;
using MessagePack;

namespace ChatApplication.ServiceDefinition;

//コメント情報（コメント・投稿者・GUID）をまとめて通信するための構造体
//CL-SV間でやり取りする独自型はここで定義する
[MessagePackObject]
public struct CommentInformation
{
    [Key(0)]
    public string Name { get; set; }

    [Key(1)]
    public string Guid { get; set; }

    [Key(2)]
    public string Comment { get; set; }
}

public interface IChatService : IService<IChatService>
{
    //名前の重複を調べる
    public UnaryResult<bool> ExistsName(string handlename);

    //クライアント情報を設定
    public UnaryResult<string> RegisterClientData(string handlename);

    //ClientStreamingならびにコメント保管
    public Task<ClientStreamingResult<CommentInformation, bool>> SaveCommentAsync();

    //コメント履歴取得
    public UnaryResult<List<CommentInformation>> GetArchiveAsync();

    //自身のコメント履歴取得
    public UnaryResult<List<string>> GetYourCommentAsync(string guid);
}
