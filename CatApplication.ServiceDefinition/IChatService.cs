using MagicOnion;
using MessagePack;

namespace ChatApplication.ServiceDefinition;

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
    public UnaryResult<List<string>> GetMyCommentAsync(string guid);
}
