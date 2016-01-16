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

    //struktura którą wczytujemy z pliku i na której operujemy
    public SynthPair(string key, string ines, string outms)
    {
        List<string> ine = ines.Split(';').ToList<string>();
        List<string> outm = outms.Split(';').ToList<string>();
        //KEY to klucz po którym rozpoznajemy kontekst gdy user zadaje pytanie "czy pamietasz..."
        this.Key_Sentence = key;
        // Przykladowe inputy usera
        this.In_Message = ine;
        //Przykladowe Outputy
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
    public string Previous_Topic { get; set; }
    string[] MEMORY { get; set; }

    public Bot(string name)
    {
        Previous_Key = 0;
        Bot_Name = name;
        Set_Data_Base(@"\\files\students\s384122\Desktop\KCK\C# bot\KCKBOT\KCKBOT\Sprechen2.txt");
        MEMORY = new string[DataBase.Count()];
        Conversation();
    }
    ~Bot()
    {
        Console.WriteLine("Umieram i już nigdy nie wrócę...");
        Console.ReadLine();
    }

    //Wypisuje całą baze danych z pamięci
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
    // wczytuje do pamięci słownik
    public void Set_Data_Base(string path)
    {
        string[] text = System.IO.File.ReadAllLines(@"C:\Users\Miłosz\Source\Repos\CS_BOT\BOT\BOTKCKOWY\BOTKCKOWY\Sprechen2.txt");
        for (int i = 0; i < text.Length; i += 3)
        {
            SynthPair s = new SynthPair(text[i].Replace("X",""), text[i + 1], text[i + 2]);
            DataBase.Add(s);
        }
    }
    //zwraca podobieństwo stringów wedle miary LV
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

    // SQL-owy like na stringu, szukamy podobienstwa userinputu w IN_Message
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
    /*
    Obie funkcje zwracają tablicę, któa wskazuje na indeks KEY w Bazie danych, na najbardziej podony przykladowy input w In_Key_MSG, a dodatkowo funkcja LV zwraca podobieństwo do tego inputu

    */
    // Analiza inputu usera, wykorzystujemy funkcje Brutal_force_finder i Laventshtein_on_user

    public string Why_MODE(string userinput)
    {
        string output;
        if (userinput.StringLike("%ksiazk%"))
        {
            output = "Fabuła była interesująca i ogólnie miło się to czytało.";
        }
        else if (userinput.StringLike("%film%")){
            output = "Swietna fabuła, interesująca akcja i genialny główny wątek!";
        }
        else
        {
            output = "THAT'S THE WAY I AM";
        }

        return output;
    }
    public string Bot_Respond(string userinput)
    {
        //Sprawdzamy ktory input z listy najbardziej pasuje do faktycznego, jakoś działa xD
        string output;
        //obróbka inputu
        userinput = Regex.Replace(userinput, @"\s +", " ");
        userinput = StringExpansion.Remove_Special_Characters(userinput);
        int id = -1; int key = 0;

        if (userinput.StringLike("%dlaczego%"))
        {

            id = DataBase.Count() - 3;

            if (DataBase[Previous_Key].Key_Sentence.StringLike("%ODP%"))
            {
                output = Why_MODE(userinput);
              
            }
            else
            {
                output = "Bo tak";
            }
        }
        else
        {
            float[] LV; int[] BF;
            Random rnd = new Random();
            int rndout = 0;
            // ZNAJDUJEMY WSKAŹNIKI (główny i specyficzny) DO ODPOWIEDNIEGO ZAGADNIENIA
            if (userinput.Count() > 2)
            {
                LV = Laventshtein_On_User(userinput);
                if (LV[2] < 0.45 || DataBase[(int)LV[0]].In_Message[(int)LV[1]].Count() < userinput.Count() / 5 && !DataBase[(int)LV[0]].Key_Sentence.StringLike("%!"))
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
            // jeżeli nei znajdziemy matcha to wypisze output z defaultu
            if (id == -1 || userinput == "")
            {
                rndout = rnd.Next(DataBase[capacity - 1].Out_Message.Count);
                output = DataBase[capacity - 1].Out_Message[rndout];
            }
            // uwzględniamy imie bota, przy pytaniu użytkownika o imie
            else if (id == 0)
            {
                output = DataBase[id].Out_Message[rndout] + " " + Bot_Name;
            }
            // przy pytaniu typu "czy pamietasz" korzystamy z pamięci bota
            else if (DataBase[id].Key_Sentence.StringLike("%Pamiec%"))
            {
                Console.WriteLine("Używam Pamięć ID->" + id + " KEY->" + key);
                Console.WriteLine(DataBase[id].In_Message[key]);
                //  string newstr = userinput.Substring(userinput.LastIndexOf(DataBase[id].In_Message[key]));
                //wyłuskujemy topic-> czyli usuwamy wszystko co user powiedział, oprócz tego co jest PO pasującym IN_Message
                int index = userinput.IndexOf(DataBase[id].In_Message[key]);
                string topic = (index < 0)
                                             ? userinput
                                             : userinput.Remove(index, DataBase[id].In_Message[key].Length);
                float[] LVMEM;
                LVMEM = Laventshtein_On_User(topic);
                Console.WriteLine(MEMORY[(int)LVMEM[0]]);
                if (MEMORY[(int)LVMEM[0]] != null)
                {
                    output = DataBase[id].Out_Message[0] + MEMORY[(int)LVMEM[0]] + "?";
                }

                else
                {
                    output = "Niestety nie pamiętam... :(";
                }
            }
            // bot przypomina o poprzednim temacie rozmowy
            else if (DataBase[id].Key_Sentence.StringLike("%" + "!!") && DataBase[Previous_Key].Key_Sentence != "Pamiec")
            {
                output = DataBase[id].Out_Message[rndout] + StringExpansion.Remove_Special_Characters(DataBase[Previous_Key].Key_Sentence);
            }
            // bot wyłuskuje dane z inputu użytkownika, poda je w outpucie (przy *, przy *M nie wypisze), oraz zapisze do MEMORY
            else if (DataBase[id].Key_Sentence.StringLike("%" + "*") || DataBase[id].Key_Sentence.StringLike("%" + "*M"))
            {
                //string newstr = userinput.Substring(userinput.LastIndexOf(DataBase[id].In_Message[key]));
                int index = userinput.IndexOf(DataBase[id].In_Message[key]);
                string topic = (index < 0)
                                             ? userinput
                                             : userinput.Remove(index, DataBase[id].In_Message[key].Length);
                if (id == 1)
                {
                    User_Name = topic;
                }
                else
                {
                    Previous_Topic = Current_Topic;
                    Current_Topic = topic;
                }
                rndout = rnd.Next(DataBase[id].Out_Message.Count);
                if (DataBase[id].Key_Sentence.StringLike("%" + "*"))
                {
                    output = DataBase[id].Out_Message[rndout] + topic;
                }
                else
                {
                    output = DataBase[id].Out_Message[rndout];
                }
                MEMORY[id] = topic;
                Console.WriteLine("zapisałem w pamięci->" + MEMORY[id] + " id->" + id);
            }

            // wypisanie tekstu, bez uwzględniania danych
            else
            {
                rndout = rnd.Next(DataBase[id].Out_Message.Count);
                output = DataBase[id].Out_Message[rndout];
            }
        }
        Previous_Key = id;
        return output;
    }

    //zaczyna konwersację
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
