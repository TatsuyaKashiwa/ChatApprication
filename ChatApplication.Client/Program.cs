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

        while (true) 
        {
            Console.WriteLine("コメントを入力してください。\n履歴を表示させたい場合はarchiveを入力してください");
            var selection  = Console.ReadLine();
            if (selection.Equals("archive"))
            {
                var commentList = await client.ShowCommentArchive();
                foreach (var comment in commentList)
                {
                    Console.WriteLine(comment);
                }
            }
            else 
            {
                var succession = await client.PostComment(1,selection);
            }
        }
    }
}
