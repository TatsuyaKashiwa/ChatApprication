using ChatApplication.ServiceDefinition;
using Grpc.Net.Client;
using MagicOnion.Client;

namespace ChatApplication.Client;

internal class Program
{
    internal static async Task Main(string[] args)
    {
        var channel = GrpcChannel.ForAddress("https://localhost:7101");
        
        var client = MagicOnionClient.Create<IChatService>(channel);
        var streaming = await client.SaveCommentAsync();

        var canContinue = true;

        while (canContinue)
        {
            Console.WriteLine("コメントを入力してください。\n履歴を表示させたい場合はarchiveを\n終了したい場合はfinishを入力してください");
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
