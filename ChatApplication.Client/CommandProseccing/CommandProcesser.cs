using ChatApplication.ServiceDefinition;
using MagicOnion;

namespace ChatApplication.Client.CommandProseccing;
public static class CommandProcesser
{
    //全体のコメント履歴を表示
    //-a or --archiveに対応
    public static async void ShowArchiveAsync(this IChatService client)
    {
        var comments = await client.GetArchiveAsync();
        foreach (var nameAndComment in comments)
        {
            Console.WriteLine($"{nameAndComment.Name}さん : {nameAndComment.Comment}");
        }
    }

    //ClientStreamを終了させる。返却値(false)によりクライアント側の入力ループを終了させる。
    //-f or --finishに対応
    public static async Task<bool> FinishClientStreamAsync(this ClientStreamingResult<CommentInformation, bool> streaming)
    {
        await streaming.RequestStream.CompleteAsync();
        return await streaming.ResponseAsync;
    }

    //helpを表示
    //-h or --helpに対応
    public static void ShowHelp()
    {
        Console.WriteLine("""
                            -a | --archive  : 履歴を表示させたい場合
                            -f | --finish   : 終了したい場合
                            -h | --help     : helpを表示
                            -y | --yourcomment: 自分で投稿したコメントを表示
                          """);
    }

    //投稿するコメント情報の構造体をnewする
    public static CommentInformation SetCommentInformation(this string comment, string handleName, string guid)
    {
        return new CommentInformation
        {
            Comment = comment,
            Name = handleName,
            Guid = guid,
        };
    }

    //自身のコメントを表示(GUIDで識別)
    //-y or --yourcommentに対応
    public static async void ShowYourCommentAsync(this IChatService client, string guid)
    {
        var comments = await client.GetYourCommentAsync(guid);
        Console.WriteLine("あなたの入力したコメントを以下に表示します。");
        foreach (var yourcomment in comments)
        {
            Console.WriteLine(yourcomment);
        }
    }
}
