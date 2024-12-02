using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

namespace ChatApplication.Client.Distinguishers;
public static class EntryDistinguisher
{
    //クライアントにて処理に対応する列挙値を返却する
    public static Direction DistinguishEntry(this string comment)
    {
        if (Regex.IsMatch(comment, "^-a$|^--archive$"))
        {
            return Direction.Archive;
        }
        else if (Regex.IsMatch(comment, "^-f$|^--finish$"))
        {
            return Direction.Finish;
        }
        else if (Regex.IsMatch(comment, "^-h$|^--help$"))
        {
            return Direction.Help;
        }
        else 
        {
            return Direction.Comment;
        }
    }
}
