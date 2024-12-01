using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatApplication.Client;
public static class EntryDistinguisher
{
    // TODO 拡張メソッドに変換すること
    public static string DistinguishEntry(string comment) 
    {
        if (Regex.IsMatch(comment, "^-a$|^--archive$"))
        {
            return "ARCHIVE";
        }
        else if (Regex.IsMatch(comment, "^-f$|^--finish$"))
        {
            return "FINISH";
        }
        else if (Regex.IsMatch(comment, "^-h$|^--help$")) 
        {
            return "HELP";
        }
        else
        {
            return "COMMENT";
        }
    }
}
