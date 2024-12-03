using System.Text.RegularExpressions;

namespace ChatApplication.Client.Distinguishers;
public static class EntryDistinguisher
{
    //クライアントからの入力に対して、クライアントにて行う処理に対応する列挙値を返却する
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
        else if (Regex.IsMatch(comment, "^-y$|^--yourcomment$"))
        {
            return Direction.YourCommnet;
        }
        else
        {
            return Direction.Comment;
        }
    }
}
