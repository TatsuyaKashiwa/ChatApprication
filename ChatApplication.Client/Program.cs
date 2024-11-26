using Grpc.Net.Client;
using MagicOnion.Client;
using ChatApplication.ServiceDefinition;
using System.Threading.Channels;

namespace ChatApplication.Client;

internal class Program
{
    internal static async Task Main(string[] args)
    {
        var channel = GrpcChannel.ForAddress("https://localhost:7101");

        var client = MagicOnionClient.Create<IChatService>(channel);
        var streaming = await client.SaveAndShowCommentAsync();

        while (true) 
        {
            Console.WriteLine("コメントを入力してください。\n履歴を表示させたい場合はarchiveを入力してください");
            var description  = Console.ReadLine();
            if (description.Equals("archive"))
            {
                await streaming.RequestStream.CompleteAsync();

                var commentList = await streaming.ResponseAsync;

                foreach (var comment in commentList)
                {
                    Console.WriteLine(comment);
                }
            }
            else 
            {
                await streaming.RequestStream.WriteAsync(description);
            }
        }
    }
}
