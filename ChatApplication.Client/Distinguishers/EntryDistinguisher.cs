using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatApplication.Client.Distinguishers;
public static class EntryDistinguisher
{
    // TODO 拡張メソッドに変換すること
    public static Directions DistinguishEntry(this string comment)
    {
        var selection = Directions.Archive;
        if (Regex.IsMatch(comment, "^-a$|^--archive$"))
        {
            return Directions.Archive;
        }
        else if (Regex.IsMatch(comment, "^-f$|^--finish$"))
        {
            return Directions.Finish;
        }
        else if (Regex.IsMatch(comment, "^-h$|^--help$"))
        {
            return Directions.Help;
        }
        else
        {
            return Directions.Comment;
        }
    }
}
