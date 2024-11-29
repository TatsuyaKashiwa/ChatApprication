using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatApplication.Client;
internal static class EntryDistinguisher
{
    internal static string DistinguishEntry(string comment) 
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
