using System;


namespace KCK_BOT
{
    class Program
    {
       static string Input;
        static void Main(string[] args)
        {
            Bot Bot = new Bot("Czarek");
            StringSift2 SS2 = new StringSift2();
           Console.WriteLine( SS2.Similarity("Zjem Cie Noobie","Zjem cie st"));
            Console.ReadLine();
        }
    }
    }
