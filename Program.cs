using System;
using System.Collections.Generic;
using System.Linq;

namespace Malom
{
    class Program
    {
        public static Dictionary<Játékos, List<string[]>> malmok = new Dictionary<Játékos, List<string[]>>();
        public static Dictionary<string, string> pontHalmaz = new Dictionary<string, string>();
        public static Dictionary<int, List<String>> sorok = new Dictionary<int, List<String>>();
        public static Dictionary<int, List<String>> oszlopok = new Dictionary<int, List<String>>();
        public static string[] bábúk = { "$", "O", "X" };
        public static Dictionary<int, Dictionary<string, string[]>> lépegetésLehetőségek = new Dictionary<int, Dictionary<string, string[]>>();

        public class Játékos
        {
            public string név;
            public string bábú;

            public Játékos(string név, string bábú)
            {
                this.név = név;
                this.bábú = bábú;
            }

            public static List<Játékos> Létrehozás()
            {
                List<Játékos> játékosok = new List<Játékos>();
                for (int i = 1; i < 3; i++)
                {
                    Console.WriteLine($"{i}.játékos add meg a neved!");
                    string név = Console.ReadLine();
                    játékosok.Add(new Játékos(név, bábúk[i]));
                    malmok.Add(játékosok[i - 1], new List<string[]>());
                    Console.Clear();
                }
                return játékosok;
            }

            public void LerakásKérdezz(bool kiütés)
            {
                string üzenet = kiütés ? $"{this.név}, adj meg egy számot kiütéshez" : $"{this.név}, adj meg egy számot";
                Console.WriteLine(üzenet);

                bool számRendben;
                int válasz;
                do {
                    számRendben = int.TryParse(Console.ReadLine(), out válasz);
                    if (!számRendben) { Console.WriteLine("Hibás érték - nem számot adtál meg!"); }
                    if (válasz <= 0 || válasz >= 25) { Console.WriteLine("Hibás érték - adj meg egy számot 1 és 24 között!"); számRendben = false; }
                    if (kiütés && (pontHalmaz[$"p{válasz}"] == "$" || pontHalmaz[$"p{válasz}"] == this.bábú)) { Console.WriteLine("Hibás érték - Kiütéshez használj az ellenfél által elfoglalt mezőt"); számRendben = false;  }
                    if ((!kiütés) && pontHalmaz[$"p{válasz}"] != "$") { Console.WriteLine("Hibás érték - a mező már foglalt"); számRendben = false; }
                } while (!számRendben);

                //Tábla frissítése a játékos bábújával
                pontHalmaz[$"p{válasz}"] = kiütés ? bábúk[0] : this.bábú;
                Console.Clear();
                Tábla.TáblaKiírás();
            }

            public void MalomEllenőrzés()
            {
                foreach(KeyValuePair<int, List<string>> sor in sorok) { BelsőEllenőrzés(sor); }
                foreach (KeyValuePair<int, List<string>> oszlop in oszlopok) { BelsőEllenőrzés(oszlop); };
                void BelsőEllenőrzés(KeyValuePair<int, List<String>> entry)
                {
                    //Deklarálunk egy string tömböt az adott sor/oszlop pontjaira
                    String[] entries = new string[] { pontHalmaz[entry.Value[0]], pontHalmaz[entry.Value[1]], pontHalmaz[entry.Value[2]] };
                    //Megvizsgáljuk hogy bármelyik érték a fent létrehozott tömbben eltér-e a 0.dik elemtől és hogy az adott játékoshoz tartozik e.
                    if (!(entries.Any(pontÉrték => pontÉrték != entries[0])) && (this.bábú == entries[0]))
                    {
                        string[] malomElemek = { entry.Value[0], entry.Value[1], entry.Value[2] };
                        //ellenőrizd hogy nincs e még ilyen malom
                        if (!(malmok[this].Any(malom => malom.SequenceEqual(malomElemek))))
                        {
                            Console.WriteLine($"{this.név}, malmod van! ({String.Join(" - ", malomElemek)})");
                            malmok[this].Add(malomElemek);
                            this.Kiütés();
                        }
                    }
                }
            }

            public void Kiütés() { this.LerakásKérdezz(true); }

            public int SzámláljBábút() 
            {
                int bábúkSzáma = 0;
                for(int i = 0; i < pontHalmaz.Count; i++) { if(pontHalmaz[pontHalmaz.Keys.ElementAt(i)] == this.bábú) { bábúkSzáma++; } }
                return bábúkSzáma;
            }
        }

        public class Tábla
        {
            public static void PontokLétrehozása()
            {
                //Pontok létrehozása
                for (int i = 1; i < 25; i++) { pontHalmaz.Add($"p{i}", bábúk[0]); }
                //Sorokba rendezés
                int c = 1; 
                for (int i = 0; i < 8; i++) { sorok.Add(i, new List<string> { $"p{c}", $"p{c + 1}", $"p{c + 2}" }); c += 3; }
                //Oszlopokba rendezés
                oszlopok.Add(0, new List<string> { "p1", "p10", "p22" }); 
                oszlopok.Add(1, new List<string> { "p2", "p5", "p8" });
                oszlopok.Add(2, new List<string> { "p3", "p15", "p24" });
                oszlopok.Add(3, new List<string> { "p4", "p11", "p19" });
                oszlopok.Add(4, new List<string> { "p6", "p14", "p21" });
                oszlopok.Add(5, new List<string> { "p7", "p12", "p16" });
                oszlopok.Add(6, new List<string> { "p9", "p13", "p18" });
                oszlopok.Add(7, new List<string> { "p17", "p20", "p23" });

                //Feltölteni a lépegetésLehetőségek változót!!!!
                //
                //
                //
                //
                //
                //
                //
            }

            public static void TáblaKiírás()
            {
                //Segédlet
                Console.WriteLine("Malom Játék" + Environment.NewLine + Environment.NewLine + "Segédlet: " + Environment.NewLine);
                Console.WriteLine("1-------------------2--------------------3");
                Console.WriteLine("|                   |                    |");
                Console.WriteLine("|    4--------------5--------------6     |");
                Console.WriteLine("|    |              |              |     |");
                Console.WriteLine("|    |       7------8------9       |     |");
                Console.WriteLine("|    |       |             |       |     |");
                Console.WriteLine("10---11------12            13------14----15");
                Console.WriteLine("|    |       |             |       |     |");
                Console.WriteLine("|    |       16-----17-----18      |     |");
                Console.WriteLine("|    |              |              |     |");
                Console.WriteLine("|    19-------------20-------------21    |");
                Console.WriteLine("|                   |                    |");
                Console.WriteLine("22------------------23------------------24");
                //Játékfelület
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(Environment.NewLine + Environment.NewLine + "Játéktábla: " + Environment.NewLine);
                Console.WriteLine($"{pontHalmaz["p1"]}-------------------{pontHalmaz["p2"]}--------------------{pontHalmaz["p3"]}");
                Console.WriteLine($"|                   |                    |");
                Console.WriteLine($"|    {pontHalmaz["p4"]}--------------{pontHalmaz["p5"]}--------------{pontHalmaz["p6"]}     |");
                Console.WriteLine($"|    |              |              |     |");
                Console.WriteLine($"|    |       {pontHalmaz["p7"]}------{pontHalmaz["p8"]}------{pontHalmaz["p9"]}       |     |");
                Console.WriteLine($"|    |       |             |       |     |");
                Console.WriteLine($"{pontHalmaz["p10"]}----{pontHalmaz["p11"]}-------{pontHalmaz["p12"]}             {pontHalmaz["p13"]}-------{pontHalmaz["p14"]}-----{pontHalmaz["p15"]}");
                Console.WriteLine($"|    |       |             |       |     |");
                Console.WriteLine($"|    |       {pontHalmaz["p16"]}------{pontHalmaz["p17"]}------{pontHalmaz["p18"]}       |     |");
                Console.WriteLine($"|    |              |              |     |");
                Console.WriteLine($"|    {pontHalmaz["p19"]}--------------{pontHalmaz["p20"]}--------------{pontHalmaz["p21"]}     |");
                Console.WriteLine($"|                   |                    |");
                Console.WriteLine($"{pontHalmaz["p22"]}-------------------{pontHalmaz["p23"]}--------------------{pontHalmaz["p24"]}" + Environment.NewLine);
                Console.ResetColor();
            }
        }

        public class Játék
        {
            public static void Köszöntő()
            {
                Console.WriteLine("Malom Játék");
                Console.WriteLine("A játék engedélyezi a csiki csuki módszert, de nem javasoljuk használatát.");
                Console.WriteLine("Jó játékot kívánunk! A továbbhaladáshoz nyomj meg egy gombot.");
                Console.ReadLine();
                Console.Clear();
            }

            public static void Lerakás(List<Játékos> játékosok)
            {
                int körökSzáma = 18;
                for(int i = 1; i <= körökSzáma; i++)
                {
                    int idx = ((i % 2 != 0) ? 0 : 1);
                    játékosok[idx].LerakásKérdezz(false);
                    játékosok[idx].MalomEllenőrzés();
                }
                Console.WriteLine("Lerakás vége!");
            }

            public static void Lépegetés(List<Játékos> játékosok)
            {
                int i = 1;
                do 
                {
                    Console.WriteLine($"{i}. lépegetés kör");
                    int idx = ((i % 2 == 0) ? 0 : 1);
                    játékosok[idx].SzámláljBábút();
                    //Tényleges lépegetés(csak szomszédba) //Honnan - hova
                    i++;
                } while (i <= 4);  
            }
        }
            
        static void Main(string[] args)
        {
            Játék.Köszöntő();
            List<Játékos> játékosok = Játékos.Létrehozás();
            Tábla.PontokLétrehozása();
            Tábla.TáblaKiírás();
            Játék.Lerakás(játékosok);
            Játék.Lépegetés(játékosok);
        }
    }
}
