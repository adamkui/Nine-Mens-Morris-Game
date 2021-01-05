using System;
using System.Collections.Generic;
using System.Linq;

namespace Malom
{
    class Program
    {
        public static List<Pont> pontok = new List<Pont>();
        public static Dictionary<int, int[]> oszlopRendezés = new Dictionary<int, int[]>();
        public static Dictionary<Játékos, List<string[]>> malmok = new Dictionary<Játékos, List<string[]>>();
        public static string[] bábúk = { "$", "O", "X" };
        public static string e1 = "Hibás érték - nem számot adtál meg!";
        public static string e2 = "Hibás érték - adj meg egy számot 1 és 24 között!";
        public static string e3 = "Hibás érték - Kiütéshez használj az ellenfél által elfoglalt mezőt";
        public static string e4 = "Hibás érték - a mező már foglalt";

        public class Játékos
        {
            public string név;
            public string bábú;
            public int bábúkSzáma;

            public Játékos(string név, string bábú, int bábúkSzáma)
            {
                this.név = név;
                this.bábú = bábú;
                this.bábúkSzáma = bábúkSzáma;
            }

            public void LerakásKérdezz(bool kiütés)
            {
                string üzenet = kiütés ? $"{this.név}, adj meg egy számot kiütéshez" : $"{this.név}, adj meg egy számot";
                Console.WriteLine(üzenet);
                int válasz = VálaszValidálás(kiütés, false);
                pontok[válasz - 1].érték = kiütés ? bábúk[0] : this.bábú; //Tábla frissítése a játékos bábújával
                Console.Clear();
                Tábla.TáblaKiírás();
            }

            public void LépegetésKérdezz()
            {
                for (int i = 0; i < 2; i++)
                {
                    this.SzámláljBábút();
                    string üzenet = (i == 0) ? $"{this.név} adj meg egy számot ahonnan ellépsz" : $"{this.név} adj meg egy számot ahová ellépsz";
                    Console.WriteLine(üzenet);
                    int válasz = VálaszValidálás(false, (i == 0) ? true : false);
                    //Tábla update
                    Console.Clear();
                    Tábla.TáblaKiírás();
                }
            }

            public int VálaszValidálás(bool kiütés, bool lépegetés)
            {
                bool számRendben;
                int válasz;
                do
                {
                    számRendben = int.TryParse(Console.ReadLine(), out válasz);
                    if (!számRendben) { Console.WriteLine(e1); } 
                    if (számRendben && (válasz <= 0 || válasz >= 25)) { Console.WriteLine(e2); számRendben = false; }
                    if (számRendben && (kiütés && (pontok[válasz - 1].érték == "$" || pontok[válasz - 1].érték == this.bábú))) { Console.WriteLine(e3); számRendben = false; }
                    if (számRendben && ((!kiütés) && pontok[válasz - 1].érték != "$")) { Console.WriteLine(e4); számRendben = false; } //ezzel baj lesz a lépegetés közbeni első meghívásnál
                    //Ellenőrizd hogy csak egyet lépett-e a játékos (a megadott első ponthoz képest a saját oszlopában vagy sorában mozog-e)
                } while (!számRendben);
                return válasz;
            }

            public void MalomEllenőrzés()
            {
                //Minden sort és oszlopot megnézünk hogy van e 3 ugyanolyan érték
                for (int i = 1; i < 9; i++)
                {
                    IEnumerable<Pont> sorPontjai = pontok.Where(pont => pont.sor == i);
                    IEnumerable<Pont> oszlopPontjai = pontok.Where(pont => pont.oszlop == i);
                    BelsőEllenőrzés(sorPontjai);
                    BelsőEllenőrzés(oszlopPontjai);
                }

                void BelsőEllenőrzés(IEnumerable<Pont> pontok)
                {
                    string[] értékek = pontok.Select(pont => pont.érték).ToArray();       //Deklarálunk egy string tömböt az adott sor/oszlop pontjainak értékére
                    if (!(értékek.Any(érték => érték != értékek[0])) && this.bábú == értékek[0]) 
                    { 
                        string[] malomPontok = pontok.Select(pont => pont.név).ToArray(); //Megvizsgáljuk hogy bármelyik érték a fent létrehozott tömbben eltér-e a 0.dik elemtől és hogy az adott játékoshoz tartozik e.
                        if (!(malmok[this].Any(malom => malom.SequenceEqual(malomPontok))))
                        {
                            Console.WriteLine($"{this.név}, malmod van! ({String.Join(" - ", malomPontok)})");
                            malmok[this].Add(malomPontok);
                            this.LerakásKérdezz(true); //Kiütés
                        }
                    }
                }
            }

            public void SzámláljBábút() 
            {
                IEnumerable<Pont> sajátPontok = pontok.Where(pont => pont.érték == this.bábú);
                this.bábúkSzáma = sajátPontok.Count();
            }
        }

        public class Pont
        {
            public int id;
            public string név;
            public int sor;
            public int oszlop;
            public string érték;

            public Pont(int id, string név, int sor, int oszlop, string érték)
            {
                this.id = id;
                this.név = név;
                this.sor = sor;
                this.oszlop = oszlop;
                this.érték = érték;
            }
        }

        public class Tábla
        {
            public static void PontokLétrehozása()
            {
                //Oszlopba rendezés szabályai
                oszlopRendezés.Add(1, new int[] { 1, 10, 22 });
                oszlopRendezés.Add(2, new int[] { 4, 11, 19 });
                oszlopRendezés.Add(3, new int[] { 7, 12, 16 });
                oszlopRendezés.Add(4, new int[] { 2, 5, 8 });
                oszlopRendezés.Add(5, new int[] { 17, 20, 23 });
                oszlopRendezés.Add(6, new int[] { 9, 13, 18 });
                oszlopRendezés.Add(7, new int[] { 6, 14, 21 });
                oszlopRendezés.Add(8, new int[] { 3, 15, 24 });

                int i = 1;
                for (int s = 1; s < 9; s++) {  
                    for (int o = 1; o < 4; o++) {
                        IEnumerable<KeyValuePair<int, int[]>> tartalmazóOszlop = oszlopRendezés.Where(oszlop => oszlop.Value.Contains(i)); //Keresd meg azt az oszlopRendezés értéket amely tartalmazza az i-t
                        int oszlop = tartalmazóOszlop.ToArray()[0].Key; //Mivel csak egy ilyen lehetséges KeyValuePair van, ezért a 0. elem kulcsmezője megadja az oszlopszámot ahová a pont tartozik
                        pontok.Add(new Pont(i, $"p{i}", s, oszlop, bábúk[0])); 
                        i++;
                    }
                }
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
                Console.WriteLine($"{pontok[0].érték}-------------------{pontok[1].érték}--------------------{pontok[2].érték}");
                Console.WriteLine($"|                   |                    |");
                Console.WriteLine($"|    {pontok[3].érték}--------------{pontok[4].érték}--------------{pontok[5].érték}     |");
                Console.WriteLine($"|    |              |              |     |");
                Console.WriteLine($"|    |       {pontok[6].érték}------{pontok[7].érték}------{pontok[8].érték}       |     |");
                Console.WriteLine($"|    |       |             |       |     |");
                Console.WriteLine($"{pontok[9].érték}----{pontok[10].érték}-------{pontok[11].érték}             {pontok[12].érték}-------{pontok[13].érték}-----{pontok[14].érték}");
                Console.WriteLine($"|    |       |             |       |     |");
                Console.WriteLine($"|    |       {pontok[15].érték}------{pontok[16].érték}------{pontok[17].érték}       |     |");
                Console.WriteLine($"|    |              |              |     |");
                Console.WriteLine($"|    {pontok[18].érték}--------------{pontok[19].érték}--------------{pontok[20].érték}     |");
                Console.WriteLine($"|                   |                    |");
                Console.WriteLine($"{pontok[21].érték}-------------------{pontok[22].érték}--------------------{pontok[23].érték}" + Environment.NewLine);
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

            public static List<Játékos> JátékosokLétrehozása()
            {
                List<Játékos> játékosok = new List<Játékos>();
                for (int i = 1; i < 3; i++)
                {
                    Console.WriteLine($"{i}.játékos add meg a neved!");
                    string név = Console.ReadLine();
                    játékosok.Add(new Játékos(név, bábúk[i], 0));
                    malmok.Add(játékosok[i - 1], new List<string[]>());
                    Console.Clear();
                }
                return játékosok;
            }

            public static void Lerakás(List<Játékos> játékosok)
            {
                for (int i = 1; i <= 18; i++)
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
                    játékosok[idx].LépegetésKérdezz();
                    i++;
                } while (i <= 4);  
            }
        }
            
        static void Main(string[] args)
        {
            Játék.Köszöntő();
            List<Játékos> játékosok = Játék.JátékosokLétrehozása();
            Tábla.PontokLétrehozása();
            Tábla.TáblaKiírás();
            Játék.Lerakás(játékosok);
            Játék.Lépegetés(játékosok);
        }
    }
}
