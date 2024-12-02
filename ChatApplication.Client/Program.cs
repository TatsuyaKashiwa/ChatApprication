using ChatApplication.Client.Distinguishers;
using ChatApplication.ServiceDefinition;
using Grpc.Net.Client;
using MagicOnion;
using MagicOnion.Client;
using System.Xml.Linq;

namespace ChatApplication.Client;

public enum Directions
{
    Archive,
    Finish,
    Help,
    Comment
};


internal class Program
{
    internal static async Task Main(string[] args)
    {
        //チャネル作成
        var channel = GrpcChannel.ForAddress("https://localhost:7101");

        //クライアントインスタンス作成
        var client = MagicOnionClient.Create<IChatService>(channel);

        //GUID取得
        var guid = await client.GetMyGuid();

        //GUID設定
        var IsNameExists = true;
        while (IsNameExists) 
        {
            Console.WriteLine("ハンドルネームを入力してください");
            var handlename = Console.ReadLine();
            IsNameExists = await client.RegisterClientData(handlename, guid);
        }

        //ストリーム(ClientStreamingResult)作成
        var streaming = await client.SaveCommentAsync();

        Console.WriteLine("""
                              コメントを入力してください。
                              -a | --archive : 履歴を表示させたい場合
                              -f | --finish  : 終了したい場合
                              -h | --help    : helpを表示 
                          """);

        //ループの継続を制御する変数
        //サーバからClientStreamからのresponseが返ってくるとfalseになりループが終了する
        var canContinue = true;

        while (canContinue)
        {
            //コメントと履歴表示・終了指示を標準入力から受ける
            var description = Console.ReadLine();

            var direction = description.DistinguishEntry();

            //"archive"が入力されると履歴表示
            if (direction is Directions.Archive)
            {
                var comments = await client.GetArchiveAsync();
                foreach (var comment in comments)
                {
                    Console.WriteLine(comment);
                }
            }
            //"finish"が入力されると終了処理
            //サーバのForEachAsyncを止めて、返り値(false)をcanContinueに受け取りTCPコネクションを切断
            else if (direction is Directions.Finish)
            {
                await streaming.RequestStream.CompleteAsync();
                canContinue = await streaming.ResponseAsync;
                Console.WriteLine("終了しました");
            }
            else if (direction is Directions.Help) 
            {
                Console.WriteLine("""
                                    -a | --archive : 履歴を表示させたい場合
                                    -f | --finish  : 終了したい場合
                                    -h | --help    : helpを表示 
                                  """);
            }
            //それ以外はコメントとしてサーバで保管
            else
            {
                await streaming.RequestStream.WriteAsync(description);
            }
        }
    }
}
