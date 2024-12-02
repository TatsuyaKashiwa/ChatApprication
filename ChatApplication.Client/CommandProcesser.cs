using ChatApplication.ServiceDefinition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApplication.Client;
public class CommandProcesser
{
    public async void ShowArchiveAsync(IChatService client) 
    {
        var comments = await client.GetArchiveAsync();
        foreach (var comment in comments)
        {
            Console.WriteLine(comment);
        }
    }
}
