﻿using ChatApplication.ServiceDefinition;
using MagicOnion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApplication.Client.CommandProseccing;
public static class CommandProcesser
{
    public static async void ShowArchiveAsync(this IChatService client)
    {
        var comments = await client.GetArchiveAsync();
        foreach (var comment in comments)
        {
            Console.WriteLine(comment);
        }
    }

    public static async Task<bool> FinishClientStreamAsync(this ClientStreamingResult<string, bool> streaming)
    {
        await streaming.RequestStream.CompleteAsync();
        return await streaming.ResponseAsync;
    }

    public static void ShowHelp()
    {
        Console.WriteLine("""
                            -a | --archive : 履歴を表示させたい場合
                            -f | --finish  : 終了したい場合
                            -h | --help    : helpを表示 
                          """);
    }
}
