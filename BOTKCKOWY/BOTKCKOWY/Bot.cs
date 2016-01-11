using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public struct SynthPair
{
    //public bool CZYQuestion;
    public  string Key_Sentence;
    public  List<string> In_Message;
    public  List<string> Out_Message;

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
    public static List<SynthPair> DataBase= new List<SynthPair>();
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


    public void  Get_Data_Base()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        foreach (SynthPair sp in DataBase)
        {

                Console.WriteLine("NEW KEY: " + sp.Key_Sentence);

            
            foreach(string s in sp.In_Message)
            {
                Console.WriteLine("IN: " + s);
            }
            foreach (string s in sp.Out_Message)
            {
                Console.WriteLine("OUT: "+s);
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



    public int Analyse_Input(string userinput)
    {
        bool PrimeMethod=false;
        bool ifound = false;
        int pointer = -1;
        int oldval = 0;
        int counter = 0;
        userinput = Regex.Replace(userinput, @"\s +", " ");
        userinput = StringExpansion.Remove_Special_Characters(userinput);
        foreach (SynthPair sp in DataBase)
        {
           
            for (int i=0; i<sp.In_Message.Count;++i)
            {
                if (userinput.StringLike("%"+sp.In_Message[i]+"%")){
                    ifound = true;
                    pointer = counter;
                    if (pointer == 1)
                    {
                        string newstr=userinput.Substring(userinput.LastIndexOf(sp.In_Message[i]));

                        int index = newstr.IndexOf(sp.In_Message[i]);
                        User_Name= (index < 0)
                                    ? newstr
                                    : newstr.Remove(index, sp.In_Message[i].Length);
                    }
                    if (pointer == 5 || pointer == 8)
                    {
                        string newstr= userinput.Substring(userinput.LastIndexOf(sp.In_Message[i]));
                        int index = newstr.IndexOf(sp.In_Message[i]);
                        Current_Topic = (index < 0)
                                    ? newstr
                                    : newstr.Remove(index, sp.In_Message[i].Length);
                    }
                }
                
                if (ifound == true)
                {
                    
                    break;
                }
               
            }
            ++counter;
        }
        if (ifound == false) {
            List<string> Luserinput = userinput.Split(' ').ToList<string>();


            for (int i = 0; i < DataBase.Count; i++)
            {
                int newvalue = StringExpansion.Compare_LStrings(Luserinput, DataBase[i].In_Message);
                if (oldval < newvalue)
                {
                    oldval = newvalue;
                    pointer = i;
                }
            }
        }

        return pointer;
    }


    public string Bot_Respond (string userinput)
    {
        //Sprawdzamy ktory input z listy najbardziej pasuje do faktycznego, jakoś działa xD
        string output;
        Random rnd = new Random();
        int id = Analyse_Input(userinput);
        if (Previous_Key == 4 && id==13)
        {
            int num = rnd.Next(DataBase[id].Out_Message.Count);
            output = DataBase[id].Out_Message[num];
        }
        else
        {
            
            if (id == -1 || userinput == "")
            {
                int num = rnd.Next(DataBase[DataBase.Count - 1].Out_Message.Count);
                output = DataBase[DataBase.Count - 1].Out_Message[num];
            }
            else
            {
                int num = rnd.Next(DataBase[id].Out_Message.Count);
                output = DataBase[id].Out_Message[num];
            }
            if (id == 0)
            {
                output = output + " " + Bot_Name;
            }
            else if (id == 1)
            {
                output = output + User_Name;
            }
            else if (id == 5 || id == 8)
            {
                output = output + Current_Topic;
            }
        }
        Previous_Key = id;
        return output;
    }

    public bool Conversation()
    {
        bool i = true;
        Console.Write("Witam waćpana, nazywam się "+Bot_Name +", a Ty ?\n>");
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
