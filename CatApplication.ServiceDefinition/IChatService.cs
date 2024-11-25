using MagicOnion;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApplication.ServiceDefinition;
public interface IChatService : IService<IChatService>
{ 
    public void PostComment(string comment);

    public UnaryResult<List<string>> ShowCommentArchive();
}
