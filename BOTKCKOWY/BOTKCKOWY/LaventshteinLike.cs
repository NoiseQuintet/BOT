using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StringSift2
{
    private int maxOffset;

    public StringSift2() : this(5) { }

    public StringSift2(int maxOffset)
    {
        this.maxOffset = maxOffset;
    }

    public float Distance(string s1, string s2)
    {
        if (String.IsNullOrEmpty(s1))
            return
            String.IsNullOrEmpty(s2) ? 0 : s2.Length;
        if (String.IsNullOrEmpty(s2))
            return s1.Length;
        int c = 0;
        int offset1 = 0;
        int offset2 = 0;
        int dist = 0;
        while ((c + offset1 < s1.Length)
        && (c + offset2 < s2.Length))
        {
            if (s1[c + offset1] != s2[c + offset2])
            {
                offset1 = 0;
                offset2 = 0;
                for (int i = 0; i < maxOffset; i++)
                {
                    if ((c + i < s1.Length)
                    && (s1[c + i] == s2[c]))
                    {
                        if (i > 0)
                        {
                            dist++;
                            offset1 = i;
                        }
                        goto ender;
                    }
                    if ((c + i < s2.Length)
                    && (s1[c] == s2[c + i]))
                    {
                        if (i > 0)
                        {
                            dist++;
                            offset2 = i;
                        }
                        goto ender;
                    }
                }
                dist++;
            }
            ender:
            c++;
        }
        return dist + (s1.Length - offset1
        + s2.Length - offset2) / 2 - c;
    }

    public float Similarity(string s1, string s2)
    {
        float dis = Distance(s1, s2);
        int maxLen = Math.Max(s1.Length, s2.Length);
        if (maxLen == 0) return 1;
        else
            return 1 - dis / maxLen;
    }
}