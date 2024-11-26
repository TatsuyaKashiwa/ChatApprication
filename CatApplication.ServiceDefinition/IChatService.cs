using Grpc.Core;
using MagicOnion;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChatApplication.ServiceDefinition;
public interface IChatService : IService<IChatService>
{
    //public UnaryResult<char> PostComment(ServerCallContext clientContext,string comment);
    public UnaryResult<char> PostComment(int id, string comment);

    public UnaryResult<List<string>> ShowCommentArchive();
}
