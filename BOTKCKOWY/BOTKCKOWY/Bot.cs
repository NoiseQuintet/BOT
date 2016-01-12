using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public struct SynthPair
{
    //public bool CZYQuestion;
    public string Key_Sentence;
    public List<string> In_Message;
    public List<string> Out_Message;

    public SynthPair(string key, string ines, string outms)
    {
        // List<string> km = key.Split(';').ToList<string>();
        List<string> ine = ines.Split(';').ToList<string>();
        List<string> outm = outms.Split(';').ToList<string>();

        this.Key_Sentence = key;
        this.In_Message = ine;
        this.Out_Message = outm;

    }

}

public class Bot
{

    public string Current_Input { get; set; }
    public string Previous_Input { get; set; }
    public string Bot_Previous_Input { get; set; }
    public int Previous_Key { get; set; }
    public static List<SynthPair> DataBase = new List<SynthPair>();
    public string Bot_Name { get; set; }
    public string User_Name { get; set; }
    public string Current_Topic { get; set; }

    public Bot(string name)
    {
        Bot_Name = name;
        Set_Data_Base(@"\\files\students\s384122\Desktop\KCK\C# bot\KCKBOT\KCKBOT\Sprechen2.txt");
        Conversation();
    }
    ~Bot()
    {
        Console.WriteLine("Umieram i już nigdy nie wrócę...");
        Console.ReadLine();
    }


    public void Get_Data_Base()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        foreach (SynthPair sp in DataBase)
        {

            Console.WriteLine("NEW KEY: " + sp.Key_Sentence);


            foreach (string s in sp.In_Message)
            {
                Console.WriteLine("IN: " + s);
            }
            foreach (string s in sp.Out_Message)
            {
                Console.WriteLine("OUT: " + s);
            }
        }
    }

    public void Set_Data_Base(string path)
    {
        string[] text = System.IO.File.ReadAllLines(@"D:\ALLSHIT\C#\KCK\Sprechen2.txt");
        for (int i = 0; i < text.Length; i += 3)
        {
            SynthPair s = new SynthPair(text[i], text[i + 1], text[i + 2]);
            DataBase.Add(s);
        }
    }

    public float[] Laventshtein_On_User(string userinput)
    {
        float[] Point_Key_Simi = new float[3] { 0, 0, 0 };
        int KEY = 0;
        float newval = 0;
        int counter = 0;
        StringSift2 SS2 = new StringSift2();
        foreach (SynthPair sp in DataBase)
        {
            KEY = 0;
            foreach (string s in sp.In_Message)
            {
                newval = SS2.Similarity(userinput, s);
                /*  Console.WriteLine("KEY: " + s + " USER: " + userinput + " SIMILARITY-> " + SS2.Similarity(s, userinput));
                 Console.ReadLine();*/
                if (newval > Point_Key_Simi[2])
                {
                    Point_Key_Simi[0] = counter;
                    Point_Key_Simi[1] = KEY;
                    Point_Key_Simi[2] = newval;

                }
                ++KEY;
            }
            ++counter;
        }
        Console.WriteLine("Metoda pomiaru odl. Laventshteina osiagnela podobienstwo=" + Point_Key_Simi[2] + "  wybrane zagadnienie-> " + DataBase[(int)Point_Key_Simi[0]].Key_Sentence + " Zgodność z-> " + DataBase[(int)Point_Key_Simi[0]].In_Message[(int)Point_Key_Simi[1]] + "\n");
        return Point_Key_Simi;
    }

    public int[] Brutal_Force_Finder(string userinput)
    {
        int counter = 0, KEY = 0;
        bool FirstFound = false;
        int[] Point_Key = new int[2] { 0, 0 };
        foreach (SynthPair sp in DataBase)
        {
            KEY = 0;
            foreach (string s in sp.In_Message)
            {
                if (userinput.StringLike("%" + s + "%"))
                {
                    FirstFound = true;
                    Point_Key[0] = counter;
                    Point_Key[1] = KEY;
                }
                ++KEY;
            }
            if (FirstFound == true)
            {
                break;
            }
            ++counter;
        }
        return Point_Key;
    }


    public string Bot_Respond(string userinput)
    {
        //Sprawdzamy ktory input z listy najbardziej pasuje do faktycznego, jakoś działa xD
        string output;
        //obróbka inputu
        userinput = Regex.Replace(userinput, @"\s +", " ");
        userinput = StringExpansion.Remove_Special_Characters(userinput);
        int id = -1; int key = 0;
        float[] LV; int[] BF;

        Random rnd = new Random();
        int rndout = 0;

        // ZNAJDUJEMY WSKAŹNIKI (główny i specyficzny) DO ODPOWIEDNIEGO ZAGADNIENIA
        if (userinput.Count() > 2)
        {
            LV = Laventshtein_On_User(userinput);
            if (LV[2] < 0.6 || DataBase[(int)LV[0]].In_Message[(int)LV[1]].Count() < userinput.Count() / 4)
            {
                Console.WriteLine("\nLaventshtein sie nie powiódł\n");
                BF = Brutal_Force_Finder(userinput);
                id = BF[0];
                key = BF[1];
            }
            else
            {
                id = (int)LV[0];
                key = (int)LV[1];
            }
        }
        //QNIEC
        int capacity = DataBase.Count();
        if (id==-1 || userinput == "")
        {
            rndout = rnd.Next(DataBase[capacity-1].Out_Message.Count);
            output = DataBase[capacity - 1].Out_Message[rndout];
        }

        else if (DataBase[id].Key_Sentence.StringLike("%" + "*"))
        {
            string newstr = userinput.Substring(userinput.LastIndexOf(DataBase[id].In_Message[key]));
            int index = newstr.IndexOf(DataBase[id].In_Message[key]);
            string topic = (index < 0)
                                         ? newstr
                                         : newstr.Remove(index, DataBase[id].In_Message[key].Length);
           if (id == 1)
            {
                User_Name = topic;
            }
           else
            {
                Current_Topic = topic;
            }
            rndout = rnd.Next(DataBase[id].Out_Message.Count);
            output = DataBase[id].Out_Message[rndout] + topic;
        }
        else
        {
            rndout = rnd.Next(DataBase[id].Out_Message.Count);
            output = DataBase[id].Out_Message[rndout];
        }
        Previous_Key = id;
        return output;
    }

    public bool Conversation()
    {
        bool i = true;
        Console.Write("Witam waćpana, nazywam się " + Bot_Name + ", a Ty ?\n>");
        while (true)
        {
            Current_Input = Console.ReadLine();
            if (Current_Input != "quit")
            {
                if (Current_Input == Previous_Input)
                {
                    Console.WriteLine("Powtarszasz się" + User_Name);
                }
                else
                {
                    Console.WriteLine(Bot_Respond(Current_Input.ToLower()));
                }
                Previous_Input = Current_Input.ToLower();
            }
            else
            {
                break;
            }
        }
        // Console.WriteLine(StringExpansion.Remove_Special_Characters( Current_Input));
        // Get_Data_Base();
        //List<string> inp = "uga nuga jak czujesz samo ".Split(' ').ToList<string>();
        // Console.WriteLine(StringExpansion.Compare_LStrings(inp, DataBase[2].In_Message));
        // Console.WriteLine(DataBase[2].In_Message[0]);
        return i;
    }



};
