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
    //GUIDを発行
    public UnaryResult<string> GetMyGuid();

    //クライアント情報を設定
    public UnaryResult<bool> RegisterClientData(string handlename, string guid);

    //ClientStreamingならびにコメント保管
    public Task<ClientStreamingResult<CommentInformation, bool>> SaveCommentAsync();

    //コメント履歴表示
    public UnaryResult<List<string>> GetArchiveAsync();
}
