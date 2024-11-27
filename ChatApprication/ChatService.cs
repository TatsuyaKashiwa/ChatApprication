using ChatApplication.ServiceDefinition;
using MagicOnion;
using MagicOnion.Server;

namespace ChatApprication.Service;

public record CommentClient
{
    //public required string SessionID { get; init; }
    public required int SessionID { get; init; }
    public required string Comment { get; init; }
}

public class ChatService : ServiceBase<IChatService>, IChatService
{
    private static ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();
    static List<CommentClient> comments = new();


    public async Task<ClientStreamingResult<string, bool>> SaveAndShowCommentAsync()
    {
        var streaming = this.GetClientStreamingContext<string, bool>();

        var id = streaming.GetHashCode();

        //await streaming.ForEachAsync(x =>
        //    {
        //        var idAndCommnet = new CommentClient() { SessionID = id, Comment = $"{id}さん ; {x}" };
        //        comments.Add(idAndCommnet);
        //    });

        await streaming.ForEachAsync(x =>
        {
            var idAndCommnet = new CommentClient() { SessionID = id, Comment = $"{id}さん ; {x}" };
            _locker.EnterWriteLock();
            try
            {
                comments.Add(idAndCommnet);
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        });

        try 
        { 
            return streaming.Result(false);
        }
        finally
        {
            _locker.Dispose();
        }
    }



    public async UnaryResult<List<string>> GetArchiveAsync()
    {
        _locker.EnterReadLock();
        try 
        {
            return comments
            .Select(x => x.Comment)
            .ToList();
        }
        finally
        { 
            _locker.ExitReadLock();
        }
        //List<string> returnComments;
        //_locker.EnterReadLock();
        //try
        //{
        //    _locker.EnterReadLock();
        //    returnComments = comments
        //    .Select(x => x.Comment)
        //    .ToList();
        //}
        //finally
        //{
        //    _locker.ExitReadLock();
        //}
        //return returnComments;
    }

}
