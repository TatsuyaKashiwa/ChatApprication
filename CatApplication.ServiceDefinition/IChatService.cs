using MagicOnion;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ChatApplication.ServiceDefinition;
public interface IChatService : IService<IChatService>
{ 
    public void PostComment(HttpContent sessionId,string comment);

    public UnaryResult<List<string>> ShowCommentArchive();
}
