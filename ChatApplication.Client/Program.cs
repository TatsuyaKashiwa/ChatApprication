using ChatApplication.ServiceDefinition;
using Grpc.Net.Client;
using MagicOnion.Client;
using System.Xml.Linq;

namespace ChatApplication.Client;

internal class Program
{
    internal static async Task Main(string[] args)
    {
        var channel = GrpcChannel.ForAddress("https://localhost:7101");

        var client = MagicOnionClient.Create<IChatService>(channel);
        var streaming = await client.SaveAndShowCommentAsync();

        var canContinue = true;

        while (canContinue)
        {
            Console.WriteLine("コメントを入力してください。\n履歴を表示させたい場合はarchiveを入力してください");
            var description = Console.ReadLine();
            if (description.Equals("archive"))
            {
                var comments = await client.GetArchiveAsync();
                foreach (var comment in comments) 
                {
                    Console.WriteLine(comment);
                }
            }
            else if (description.Equals("finish")) 
            {
                await streaming.RequestStream.CompleteAsync();
                canContinue = await streaming.ResponseAsync;
                Console.WriteLine("終了しました");
            }
            else
            {
                await streaming
                    .RequestStream
                    .WriteAsync(description);
            }
        }
    }
}
