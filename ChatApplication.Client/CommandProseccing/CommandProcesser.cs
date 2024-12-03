using ChatApplication.ServiceDefinition;
using MagicOnion;

namespace ChatApplication.Client.CommandProseccing;
public static class CommandProcesser
{
    public static async void ShowArchiveAsync(this IChatService client)
    {
        var comments = await client.GetArchiveAsync();
        foreach (var nameAndComment in comments)
        {
            Console.WriteLine($"{nameAndComment.Name}さん : {nameAndComment.Comment}");
        }
    }

    public static async Task<bool> FinishClientStreamAsync(this ClientStreamingResult<CommentInformation, bool> streaming)
    {
        await streaming.RequestStream.CompleteAsync();
        return await streaming.ResponseAsync;
    }

    public static void ShowHelp()
    {
        Console.WriteLine("""
                            -a | --archive  : 履歴を表示させたい場合
                            -f | --finish   : 終了したい場合
                            -h | --help     : helpを表示
                            -m | --mycomment: 自分で投稿したコメントを表示
                          """);
    }

    public static CommentInformation SetCommentInformation(this string comment, string handleName, string guid)
    {
        return new CommentInformation
        {
            Comment = comment,
            Name = handleName,
            Guid = guid,
        };
    }

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
