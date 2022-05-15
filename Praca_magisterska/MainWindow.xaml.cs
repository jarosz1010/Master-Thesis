using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Praca_magisterska
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


 
        public List<Trasa> TrasaRead(string filename)
        {
            List<Trasa> tableOfRoutes = new List<Trasa>();
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(filename);
            while ((line = file.ReadLine()) != null)
            {

                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] words = line.Split(delimiterChars);
                int pole1 = Int32.Parse(words[0]);
                string pole2 = words[1];
                int pole3 = Int32.Parse(words[2]);
                Trasa trasa = new Trasa(pole1, pole2, pole3);
                tableOfRoutes.Add(trasa);

            }
            file.Close();
            return tableOfRoutes;
        }

        public List<Order> OrderRead(string filename)
        {
            List<Order> tableOfOrders = new List<Order>();
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(filename);
            while ((line = file.ReadLine()) != null)
            {

                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
                string[] words = line.Split(delimiterChars);
                int pole1 = Int32.Parse(words[0]);
                int pole2 = Int32.Parse(words[1]);
                int pole3 = Int32.Parse(words[2]);
                Order order= new Order(pole1, pole2, pole3);
                tableOfOrders.Add(order);

            }
            file.Close();
            return tableOfOrders;
        }
        public string wyznaczTrase(int start, int stop)
        {
            string permutacja = "";


            return permutacja;
        }

   /*
        private void przycisk_koszt_Click(object sender, RoutedEventArgs e)
        {

            List<Trasa> tablica = new List<Trasa>();
            Train pociag_1 = new Train();
            Train pociag_2 = new Train();
            tablica = TrasaRead(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\trasa2.txt");
            int koszt = 0;
            int ilosc_zamowien = 7;

            List<int> kolejnosc = new List<int>();
            kolejnosc.Add(1);
            kolejnosc.Add(2);
            kolejnosc.Add(6);
            kolejnosc.Add(4);
            kolejnosc.Add(5);
            kolejnosc.Add(3);
            kolejnosc.Add(7);

            for (int i = 0; i < ilosc_zamowien; i++)
            {
                Trasa s;
                s = tablica[kolejnosc[i]-1]; // Minus jeden zeby byla dobra iteracja tablicy
                
                if (pociag_1.czas <= pociag_2.czas)
                {
                    Console.WriteLine(s.stolik);
                    // Sprawdzenie czy trasa nie jest zajeta
                    int co_robic = pociag_1.sprawdz_zajetosc(kolejnosc[i], s.przystanki, pociag_2.zajete_stoliki);
                    if (co_robic == 0)
                    {
                        pociag_1.czas = pociag_2.czas + 2 * s.koszt_calkowity; // Mnozymy bo uwzgledniamy powrot
                    }
                    else
                        pociag_1.czas = pociag_1.czas + 2 * s.koszt_calkowity; // Mnozymy bo uwzgledniamy powrot
                        pociag_1.dodajStoliki(s.przystanki, kolejnosc[i]);
                    pociag_1.odwiedzone_stoliki = pociag_1.odwiedzone_stoliki + kolejnosc[i] + "-";

                }

                else if (pociag_2.czas < pociag_1.czas)
                {
                    // Dodanie oczekiwania gdy trasa jest zajeta przez inny pociag
                    int co_robic = pociag_2.sprawdz_zajetosc(kolejnosc[i], s.przystanki, pociag_1.zajete_stoliki);
                    if (co_robic == 0)
                    {
                        pociag_2.czas = pociag_1.czas + 2 * s.koszt_calkowity; 
                    }
                    else 
                        pociag_2.czas = pociag_2.czas + 2 * s.koszt_calkowity; // Mnozymy bo uwzgledniamy powrot
                    pociag_2.dodajStoliki(s.przystanki, kolejnosc[i]);
                    pociag_2.odwiedzone_stoliki = pociag_2.odwiedzone_stoliki + kolejnosc[i] + "-";
                }
            }
            tekst1.Text = "Pociag1: " + pociag_1.czas + "Pociag2: " + pociag_2.czas + "Stoliki1: " + pociag_1.odwiedzone_stoliki + "Stoliki2: " + pociag_2.odwiedzone_stoliki; 
        }
   */

        // Funkcja zwracajaca dostepne zamowienia - bierze tylko z dostepnych zamowien
        public Order wezZamowienie_first(List<Order> zamowienia, int czas)
        {
            Order oczekujace = zamowienia[0];
            int prev_time = 1000;
            List<Order> dostepne_zamowienia = new List<Order>();
            // W tej petli sprawdzamy ktore zamowienia sa juz dostepne a nie zostaly wykonane
            for (int i = 0; i < zamowienia.Count; i++)
            {
                if (zamowienia[i].zakonczone == 0 && zamowienia[i].czasDostepnosci <= czas)
                    dostepne_zamowienia.Add(zamowienia [i]);
            }
            // Ten warunek zwraca pierwsze zamowienie FCFS z listy dostepnych. Tutaj wywolac mozna algorytm SA
            if (dostepne_zamowienia.Count > 0)
                return dostepne_zamowienia[0];
            else
                foreach(Order o in zamowienia)
                { 
                    if (o.zakonczone == 0 && o.czasDostepnosci < prev_time)
                        oczekujace = o;
                }
                return oczekujace;
        }

        // Druga wersja - bardziej naiwna - bierze pierwsze niezrealizowane zamowienie z listy
        public Order wezZamowienie_second(List<Order> zamowienia, int czas)
        {
            List<Order> dostepne_zamowienia = new List<Order>();
            for (int i = 0; i < zamowienia.Count; i++)
            {
                if (zamowienia[i].zakonczone == 0)
                    dostepne_zamowienia.Add(zamowienia[i]);
            }
            return dostepne_zamowienia[0];

        }

        // Funkcja do tworzenia grafu
        private void dijkstra_button_Click(object sender, RoutedEventArgs e)
        {
            Graph g = new Graph(8); // o jeden wiekszy bo wierzcholek startowy
            g.initiateZeros();
            g.addEdge(0, 1, 10);
            g.addEdge(1, 2, 10);
            g.addEdge(0, 3, 5);
            g.addEdge(3, 4, 10);
            g.addEdge(4, 5, 5);
            g.addEdge(0, 6, 3);
            g.addEdge(6, 7, 4);
            g.calculateRoute();
        }

        // Funkcja celu bez brania pod uwage ladunku
        public List<Train> funkcja_celu(List<Order> zamowienia, List<Trasa> trasy, int ilosc_pociagow)
        {
            List<Train> pociagi = new List<Train>();
            int koszt_calkowity = 0;

            // Wczytujemy zamowienia i trasy z plikow tekstowych
            int ilosc_zamowien = zamowienia.Count();
            // Dodawanie ilosci pociagow do tablicy
            for (int poc = 0; poc < ilosc_pociagow; poc++)
            {
                pociagi.Add(new Train());
            }
            // Obliczanie calkowitego kosztu poprzez iteracje po poszczegolnych zamowieniach
            for (int i = 0; i < ilosc_zamowien; i++)
            {
                int numer_poc = 4;
                int czas_max = 10000;
                int czas_zajetosci = 0;
                Trasa s;

                // Sprawdzamy ktory pociag ma najmniejszy czas i tego wybieramy do jazdy;
                for (int poc = 0; poc < ilosc_pociagow; poc++)
                {
                    if (pociagi[poc].czas < czas_max)
                    {
                        czas_max = pociagi[poc].czas;
                        numer_poc = poc;
                    }
                }

                Order zamowienieAktualne = wezZamowienie_second(zamowienia, pociagi[numer_poc].czas);

                // Dodajemy opoznienie wynikajace z czekania na zamowienie
                if (zamowienieAktualne.czasDostepnosci > pociagi[numer_poc].czas)
                    pociagi[numer_poc].czas = zamowienieAktualne.czasDostepnosci;
                s = trasy[zamowienieAktualne.numerStolika - 1];

                // Sprawdzenie czy trasa nie jest zajeta
                for (int poc = 0; poc < ilosc_pociagow; poc++)
                {
                    int co_robic = pociagi[numer_poc].sprawdz_zajetosc(zamowienieAktualne.numerStolika, s.przystanki, pociagi[poc].zajete_stoliki);
                    if (co_robic == 0)
                    {
                        if (pociagi[poc].czas > czas_zajetosci)
                            czas_zajetosci = pociagi[poc].czas;
                    }
                }

                // Jezeli trasa byla zajeta to nalezy doliczyc czas kiedy zostanie zwolniona
                if (czas_zajetosci == 0)
                {
                    pociagi[numer_poc].czas = pociagi[numer_poc].czas + 2 * s.koszt_calkowity; // Mnozymy bo uwzgledniamy powrot
                }
                else
                    pociagi[numer_poc].czas = czas_zajetosci + 2 * s.koszt_calkowity;

                // Dodajemy stoliki, do ktorych trasy aktualnie sa zajete i odwiedzone miejsca przez dany pociag
                pociagi[numer_poc].dodajStoliki(s.przystanki, zamowienieAktualne.numerStolika);

                pociagi[numer_poc].odwiedzone_stoliki = pociagi[numer_poc].odwiedzone_stoliki + zamowienieAktualne.numerStolika + "-";
                // Zmiana statusu zamowienia na zrealizowane
                zamowienia[i].zakonczone = 1;

            }
            return pociagi;
        }

        // Zwraca wylacznie inta w postaci kosztu
        public int koszt_funkcji_celu(List<Order> zamowienia, List<Trasa> trasy, int ilosc_pociagow)
        {
            List<Train> pociagi = new List<Train>();
            int koszt_calkowity = 0;
            pociagi = funkcja_celu(zamowienia, trasy, ilosc_pociagow);
            for (int poc = 0; poc < ilosc_pociagow; poc++)
            {
                if (pociagi[poc].czas > koszt_calkowity)
                    koszt_calkowity = pociagi[poc].czas;
            }
            return koszt_calkowity;
        }
        /*
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<Trasa> trasy = new List<Trasa>();
            List<Order> zamowienia = new List<Order>();
            List<Train> pociagi = new List<Train>();
            string tekst = "";

            // Wczytujemy zamowienia i trasy z plikow tekstowych
            zamowienia = OrderRead(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\orders.txt");
            trasy = TrasaRead(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\trasa2.txt");

            int ilosc_zamowien = 7;
            int ilosc_pociagow = (int)train_number_slider.Value;

            // Dodawanie ilosci pociagow do tablicy
            for (int poc = 0; poc < ilosc_pociagow; poc++)
            {
                pociagi.Add(new Train());
            }

            // Obliczanie calkowitego kosztu poprzez iteracje po poszczegolnych zamowieniach
            for (int i = 0; i < ilosc_zamowien; i++)
            {
                int numer_poc = 4;
                int czas_max = 10000;
                int czas_zajetosci = 0;
                Trasa s;

                // Sprawdzamy ktory pociag ma najmniejszy czas i tego wybieramy do jazdy;
                for (int poc = 0; poc < ilosc_pociagow; poc++)
                {
                    if (pociagi[poc].czas < czas_max)
                    {
                        czas_max = pociagi[poc].czas;
                        numer_poc = poc;
                    }
                }

                Order zamowienieAktualne = wezZamowienie_second(zamowienia, pociagi[numer_poc].czas);

                // Dodajemy opoznienie wynikajace z czekania na zamowienie
                if (zamowienieAktualne.czasDostepnosci > pociagi[numer_poc].czas)
                    pociagi[numer_poc].czas = zamowienieAktualne.czasDostepnosci;
                s = trasy[zamowienieAktualne.numerStolika - 1];

                // Sprawdzenie czy trasa nie jest zajeta
                for (int poc = 0; poc < ilosc_pociagow; poc++)
                {
                    int co_robic = pociagi[numer_poc].sprawdz_zajetosc(zamowienieAktualne.numerStolika, s.przystanki, pociagi[poc].zajete_stoliki);
                    if (co_robic == 0)
                    {
                        if (pociagi[poc].czas > czas_zajetosci)
                            czas_zajetosci = pociagi[poc].czas; 
                    }
                }

                // Jezeli trasa byla zajeta to nalezy doliczyc czas kiedy zostanie zwolniona
                if(czas_zajetosci == 0)
                {
                    pociagi[numer_poc].czas = pociagi[numer_poc].czas + 2 * s.koszt_calkowity; // Mnozymy bo uwzgledniamy powrot
                } 
               else
                    pociagi[numer_poc].czas = czas_zajetosci + 2 * s.koszt_calkowity; 

               // Dodajemy stoliki, do ktorych trasy aktualnie sa zajete i odwiedzone miejsca przez dany pociag
               pociagi[numer_poc].dodajStoliki(s.przystanki, zamowienieAktualne.numerStolika);

               pociagi[numer_poc].odwiedzone_stoliki = pociagi[numer_poc].odwiedzone_stoliki + zamowienieAktualne.numerStolika + "-";
               // Zmiana statusu zamowienia na zrealizowane
               zamowienia[i].zakonczone = 1;

            }

            // Wypisywanie obliczonego kosztu i permutacji na ekranie
            for (int poc = 0; poc < ilosc_pociagow; poc++)
            {
                tekst = tekst + "Pociag" + poc + ": " + pociagi[poc].odwiedzone_stoliki + "czas: " + pociagi[poc].czas + "\n";

            }
            tekst1.Text = tekst;
        }
        */

        public List<Order> SwapValues(List<Order> zam, int index1, int index2)
        {
            Order temp = zam[index1];
            zam[index1] = zam[index2];
            zam[index2] = temp;
            return zam;
        }


        // Uruchomienie symulowanego wyzarazania z zadanymi parametrami
        private void SA_button_Click(object sender, RoutedEventArgs e)
        {

            int ilosc_pociagow = 0;
            string wyswietlany_tekst = "";
            List<Trasa> trasy = new List<Trasa>();
            List<Order> zamowienia = new List<Order>();
            List<Train> pociagi = new List<Train>();
            zamowienia = OrderRead(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\orders.txt");
            trasy = TrasaRead(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\trasa2.txt");
            if (train_number_value != null)
            {
                ilosc_pociagow = Convert.ToInt32(train_number_slider.Value);
            }
            // Odtad zaczyna sie algorytm SA
            double T = 1000;
            int i, j;
            int cmax_tmp = 0;
            int cmax_start;
            double r, error, diff;
            int ilosc = zamowienia.Count();

            double a = 0.95;
            double Tend = 1;
            int blad = 0;

            cmax_start = koszt_funkcji_celu(zamowienia, trasy, ilosc_pociagow);

            while (T > Tend)
            {
 
                    Random rnd = new Random();
                    i = rnd.Next(0, ilosc);
                    j = rnd.Next(0, ilosc);

                    zamowienia = SwapValues(zamowienia, i, j);
                   for (int g = 0; g < zamowienia.Count; g++)
                    {
                    zamowienia[g].zakonczone = 0;
                    }

                    cmax_tmp = koszt_funkcji_celu(zamowienia, trasy, ilosc_pociagow);
                    if (cmax_tmp > cmax_start)
                    {
                        r = rnd.Next(0, 100);
                        r = r / 100;
                        diff = cmax_start - cmax_tmp;
                        double podziel = diff / T;
                        error = Math.Exp(podziel);

                        if (r >= error) zamowienia = SwapValues(zamowienia, i, j);

                    }
                T = a * T;
            }

            // Wyswietlanie informacji o optymalnej kolejnosci zamowien
            for (int g = 0; g < zamowienia.Count; g++)
            {
                zamowienia[g].zakonczone = 0;
            }
            pociagi = funkcja_celu(zamowienia, trasy, ilosc_pociagow);
            for (int poc = 0; poc < ilosc_pociagow; poc++)
            {
                wyswietlany_tekst = wyswietlany_tekst + "Pociag" + poc + ": " + pociagi[poc].odwiedzone_stoliki + "czas: " + pociagi[poc].czas + "\n";
            }
            for (int g = 0; g < zamowienia.Count; g++)
            {
                zamowienia[g].zakonczone = 0;
            }

            int koszty = koszt_funkcji_celu(zamowienia, trasy, ilosc_pociagow);

            tekst1.Text = wyswietlany_tekst + "\n" + "Koszt calkowity: " + koszty;
        }


        // Wyswietlanie aktualnie ustawionej liczby pociagow
        private void train_number_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (train_number_value != null)
            {
                String ilosc_pociagow = train_number_slider.Value.ToString();
                train_number_value.Text = ilosc_pociagow;
            }           
        }

        // Funkcja ktora oblicza koszt dla kolejnosci zadanej w pliku
        private void Simple_Button_Click(object sender, RoutedEventArgs e)
        {
            int kkk = 0;
            int koszt_calkowity = 0;
            int ilosc_pociagow = 0;
            string wyswietlany_tekst = "";
            List<Trasa> trasy = new List<Trasa>();
            List<Order> zamowienia = new List<Order>();
            List<Train> pociagi = new List<Train>();
            zamowienia = OrderRead(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\orders.txt");
            trasy = TrasaRead(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\trasa2.txt");
            if (train_number_value != null)
            {
                ilosc_pociagow = Convert.ToInt32(train_number_slider.Value);
            }
            // Wywolanie funkcji celu
            pociagi = funkcja_celu(zamowienia, trasy, ilosc_pociagow);
            for (int poc = 0; poc < ilosc_pociagow; poc++)
            {
                wyswietlany_tekst = wyswietlany_tekst + "Pociag" + poc + ": " + pociagi[poc].odwiedzone_stoliki + "czas: " + pociagi[poc].czas + "\n";
            }
            for (int g = 0; g < zamowienia.Count; g++)
            {
                zamowienia[g].zakonczone = 0;
            }
            koszt_calkowity = koszt_funkcji_celu(zamowienia, trasy, ilosc_pociagow);
            tekst1.Text = wyswietlany_tekst + "\n" + "Koszt calkowity: " + koszt_calkowity;
        }

        private void TS_button_Click(object sender, RoutedEventArgs e)
        {

            int ilosc_pociagow = 0;
            int cmax_best, cmax_temp;
            int stop = 0;
            int maxTabuSize = 100;
            
            string wyswietlany_tekst = "";
            List<Trasa> trasy = new List<Trasa>();
            List<Order> zamowienia = new List<Order>();
            List<Train> pociagi = new List<Train>();
            Queue<int[]> tabuList = new Queue<int[]>();
            zamowienia = OrderRead(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\orders.txt");
            trasy = TrasaRead(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\trasa2.txt");
            int ilosc = zamowienia.Count();
            int[] tabu = new int[ilosc];
            if (train_number_value != null)
            {
                ilosc_pociagow = Convert.ToInt32(train_number_slider.Value);
            }
            // Odtad zaczyna sie algorytm TS
            
            int i, j;
            Console.WriteLine(ilosc);
            for (int zam = 0; zam < ilosc; zam++)
            {
                tabu[zam] = zamowienia[zam].numerStolika;
                Console.WriteLine("elo");
            }
            tabuList.Enqueue(tabu);

            cmax_best = koszt_funkcji_celu(zamowienia, trasy, ilosc_pociagow);
            while(stop < 100)
            {
                
                Random rnd = new Random();
                i = rnd.Next(0, ilosc);
                j = rnd.Next(0, ilosc);
                SwapValues(zamowienia, i, j);
                for (int zam = 0; zam < ilosc; zam++)
                    tabu[zam] = zamowienia[zam].numerStolika;
                if (!tabuList.Contains(tabu))
                {
                    for (int g = 0; g < zamowienia.Count; g++)
                    {
                        zamowienia[g].zakonczone = 0;
                    }

                    cmax_temp = koszt_funkcji_celu(zamowienia, trasy, ilosc_pociagow);
                    if (cmax_temp < cmax_best)
                    {
                        cmax_best = cmax_temp;
                    }
                    for (int zam = 0; zam < ilosc; zam++)
                        tabu[zam] = zamowienia[zam].numerStolika;
                    tabuList.Enqueue(tabu);
                    if (tabuList.Count > maxTabuSize)
                        tabuList.Dequeue();
                }
                stop = stop + 1;
            }

            for (int g = 0; g < zamowienia.Count; g++)
            {
                zamowienia[g].zakonczone = 0;
            }
            pociagi = funkcja_celu(zamowienia, trasy, ilosc_pociagow);
            for (int poc = 0; poc < ilosc_pociagow; poc++)
            {
                wyswietlany_tekst = wyswietlany_tekst + "Pociag" + poc + ": " + pociagi[poc].odwiedzone_stoliki + "czas: " + pociagi[poc].czas + "\n";
            }
            for (int g = 0; g < zamowienia.Count; g++)
            {
                zamowienia[g].zakonczone = 0;
            }

            int koszty = koszt_funkcji_celu(zamowienia, trasy, ilosc_pociagow);

            tekst1.Text = wyswietlany_tekst + "\n" + "Koszt calkowity: " + koszty;
        }
    }
}
