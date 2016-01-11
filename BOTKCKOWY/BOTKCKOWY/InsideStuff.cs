using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public static class StringExpansion
{
    public static bool StringLike(this string toSearch, string toFind)
    {
        return new Regex(@"\A" + new Regex(@"\.|\$|\^|\{|\[|\(|\||\)|\*|\+|\?|\\").Replace(toFind, ch => @"\" + ch).Replace('_', '.').Replace("%", ".*") + @"\z", RegexOptions.Singleline).IsMatch(toSearch);
    }

    // sprawdzi ile razy string zawiera sie w liscie
    // mozna dzielic zdanie na pary podwojne lub wieksze slow i wyszukwiac ich w bazie
    // na poczatku szukanie calego inputu
    public static bool String_In_List(this string toComp, List<string> Comp)
    {
        bool b = false;
        foreach(string s in Comp)
        {
            if (s.StringLike("%"+toComp+"%") && toComp.Length>1)
            {
                b = true;
            }
        }
        return b;
    }



    // wyliczy ile elementów z jednej listy zawiera się w drugiej
    public static int Compare_LStrings(List<string> toCompare, List<string> Compared)
    {
        int val = 0;
        foreach (string s in toCompare)
        {

            if (s.String_In_List(Compared))
            {
                val += 1;
            }
        }
        
        return val;
    }

    public static string Remove_Special_Characters(string str)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char c in str)
        {
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c==' ')
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    public static string RemoveSpecialCharactersREGULAREXPRESSION(string str)
    {
        return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
    }
}


// zamieniamy listę na stringa
//string compared_single_string = string.Join(",",Compared.ToArray());