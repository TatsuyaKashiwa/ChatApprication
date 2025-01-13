using ChatApplication.Client.CommandProseccing;
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
    YourCommnet,
    Comment,
};


public class Program
{
    public static async Task Main(string[] args)
    {
        //チャネル作成
        var channel = GrpcChannel.ForAddress("https://localhost:7101");

        //クライアントインスタンス作成
        var client = MagicOnionClient.Create<IChatService>(channel);

        //常に新規アカウントを作成
        var isNameExists = true;
        
        //handlenameはコメント投稿時等、Main関数全体で利用するためここで定義
        var handleName = "";
        
        while (isNameExists)
        {
            Console.WriteLine("ハンドルネームを入力してください");

            handleName = Console.ReadLine();

            //重複がなければメソッドがfalseを返し、次の処理へ進む
            isNameExists = await client.ExistsName(handleName); ;
        }

        // ユーザ情報登録時にサーバからGUIDが発行される
        var guid = await client.RegisterClientData(handleName);

        //ストリーム(ClientStreamingResult)作成
        var streaming = await client.SaveCommentAsync();

        //入力受付前にHelpを一度表示
        Console.WriteLine("""
                              コメントを入力してください。
                              -a | --archive  : 履歴を表示させたい場合
                              -f | --finish   : 終了したい場合
                              -h | --help     : helpを表示 
                              -y | --yourcomment: 自分で投稿したコメントを表示
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

            switch (direction)
            {
                //"-a or --archive"が入力されると履歴表示
                case Direction.Archive:
                    client.ShowArchiveAsync();
                    break;
                //"-f or --finish"が入力されると終了処理
                //サーバのForEachAsyncを止めて、返り値(false)をcanContinueに受け取りTCPコネクションを切断
                case Direction.Finish:
                    canContinue = await streaming.FinishClientStreamAsync();
                    break;
                //-h or --helpが入力されるとヘルプを表示
                case Direction.Help:
                    CommandProcesser.ShowHelp();
                    break;
                //-m or --mycommentが入力されると自身の投稿したコメントを表示
                case Direction.YourCommnet:
                    client.ShowYourCommentAsync(guid);
                    break;
                // それ以外の入力はコメントとして認識されServiceDefinitionで定義したカスタム構造体にまとめてサーバへ送る
                default:
                    var commentInformation = description.SetCommentInformation(handleName, guid);
                    await streaming.RequestStream.WriteAsync(commentInformation);
                    break;
            }
        }
    }
}

