using ChatApplication.ServiceDefinition;
using Grpc.Net.Client;
using MagicOnion;
using MagicOnion.Client;

namespace ChatApplication.Client;

internal class Program
{
    internal static async Task Main(string[] args)
    {
        //チャネル作成
        var channel = GrpcChannel.ForAddress("https://localhost:7101");

        //クライアントインスタンス作成
        var client = MagicOnionClient.Create<IChatService>(channel);

        //GUID設定
        

        //ストリーム(ClientStreamingResult)作成
        var streaming = await client.SaveCommentAsync();

        //ループの継続を制御する変数
        //サーバからClientStreamからのresponseが返ってくるとfalseになりループが終了する
        var canContinue = true;

        Console.WriteLine("""
                              コメントを入力してください。
                              archive : 履歴を表示させたい場合
                              finish  : 終了したい場合
                          """);

        while (canContinue)
        {
            //コメントと履歴表示・終了指示を標準入力から受ける
            var description = Console.ReadLine();

            //"archive"が入力されると履歴表示
            if (description.Equals("archive"))
            {
                var comments = await client.GetArchiveAsync();
                foreach (var comment in comments)
                {
                    Console.WriteLine(comment);
                }
            }
            //"finish"が入力されると終了処理
            //サーバのForEachAsyncを止めて、返り値(false)をcanContinueに受け取りTCPコネクションを切断
            else if (description.Equals("finish"))
            {
                await streaming.RequestStream.CompleteAsync();
                canContinue = await streaming.ResponseAsync;
                Console.WriteLine("終了しました");
            }
            //それ以外はコメントとしてサーバで保管
            else
            {
                await streaming.RequestStream.WriteAsync(description);
            }
        }
    }
}
