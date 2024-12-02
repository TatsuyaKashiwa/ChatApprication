using ChatApplication.Client.Distinguishers;
using ChatApplication.ServiceDefinition;
using Grpc.Net.Client;
using MagicOnion.Client;

namespace ChatApplication.Client;

/// <summary>
/// 入力コマンドによる指示の分岐を表すための列挙型
/// </summary>
/// <remarks>
/// DistinguishEntry()拡張メソッドから返却される入力コマンドによる指示を表すための列挙型
/// 使用されるのはクライアント内(サーバへのアクセスは分岐後)のため
/// クライアント名前空間内で宣言
/// </remarks>
public enum Direction
{
    Archive,
    Finish,
    Help,
    Comment
};


public class Program
{
    public static async Task Main(string[] args)
    {
        // TODO: json ファイルから設定を読み込むように変更する
        //チャネル作成
        var channel = GrpcChannel.ForAddress("https://localhost:7101");

        //クライアントインスタンス作成
        var client = MagicOnionClient.Create<IChatService>(channel);

        // TODO: guid はあくまでサービスが各クライアントを識別するためのモノで
        //       ログイン等の返り値として付与したほうが良いのでは？
        //       生成はサービスであるメリットがない
        //GUID取得
        var guid = await client.GetMyGuid();

        //GUID設定
        // TODO: 命名規則に従って変数名を変更する
        // CHECKED: キャメルケースへ変更を行った
        var isNameExists = true;
        while (isNameExists)
        {
            Console.WriteLine("ハンドルネームを入力してください");

            // TODO: 命名規則に従って変数名を変更する
            //CHECKED: キャメルケースへ変更を行った
            var handleName = Console.ReadLine();

            // TODO: 登録の前に既にハンドルネームの重複が存在するか検証すること
            isNameExists = await client.RegisterClientData(handleName, guid);
        }

        //ストリーム(ClientStreamingResult)作成
        var streaming = await client.SaveCommentAsync();

        //入力受付前にHelpを一度表示
        Console.WriteLine("""
                              コメントを入力してください。
                              -a | --archive : 履歴を表示させたい場合
                              -f | --finish  : 終了したい場合
                              -h | --help    : helpを表示 
                          """);

        //ループの継続を制御する変数
        //サーバからClientStreamからのresponseが返ってくるとfalseになりループが終了する
        var canContinue = true;

        //サーバからfalseが返る(終了コマンドが入力される)まで
        while (canContinue)
        {
            //コメントならびに入力コマンドを標準入力から受ける
            var description = Console.ReadLine();

            //入力に対する指示を受ける
            var direction = description.DistinguishEntry();

            // TODO: 下記の処理の分岐を Swhich 式での記述を検討してみて
            //       それぞれの処理をメソッドとして切り出すと可読性が向上します。
            //"-a or --archive"が入力されると履歴表示
            if (direction is Direction.Archive)
            {
                var comments = await client.GetArchiveAsync();
                foreach (var comment in comments)
                {
                    Console.WriteLine(comment);
                }
            }
            //"-f or --finish"が入力されると終了処理
            //サーバのForEachAsyncを止めて、返り値(false)をcanContinueに受け取りTCPコネクションを切断
            else if (direction is Direction.Finish)
            {
                await streaming.RequestStream.CompleteAsync();
                canContinue = await streaming.ResponseAsync;
                Console.WriteLine("終了しました");
            }
            //-h or --helpが入力されるとヘルプを表示
            else if (direction is Direction.Help)
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
                // TODO: カスタム構造体を使用することで、コメントとハンドルネームを一緒に保持することができると思います。
                await streaming.RequestStream.WriteAsync(description);
            }
        }
    }
}

