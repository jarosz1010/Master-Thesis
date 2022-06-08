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
using System.IO;

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
 

        public List<Order> SwapValues(List<Order> zam, int index1, int index2)
        {
            Order temp = zam[index1];
            zam[index1] = zam[index2];
            zam[index2] = temp;
            return zam;
        }
        static void SwapValues2<Order>(List<Order> zam, int index1, int index2)
        {
            Order temp = zam[index1];
            zam[index1] = zam[index2];
            zam[index2] = temp;
        }

        // Uruchomienie symulowanego wyzarazania z zadanymi parametrami
        private void SA_button_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int ilosc_pociagow = 0;
            string wyswietlany_tekst = "";
            List<Trasa> trasy = new List<Trasa>();
            List<Order> zamowienia = new List<Order>();
            List<Order> najlepsze_rozwiazanie = new List<Order>();
            List<Train> pociagi = new List<Train>();
            string plik_grafu = text_Route.Text;
            string plik_zamowien = text_Order.Text;
            zamowienia = OrderRead(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\" + plik_zamowien);
            string line1 = File.ReadLines(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\" + plik_grafu).First();
            Graph gr = new Graph(Int32.Parse(line1)); // o jeden wiekszy bo wierzcholek startowy
            gr.readFromFile(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\"+plik_grafu);
            gr.calculateRoute();
            trasy = TrasaRead(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\plik_tymczasowy_trasy.txt");
            if (train_number_value != null)
            {
                ilosc_pociagow = Convert.ToInt32(train_number_slider.Value);
            }
            // Odtad zaczyna sie algorytm SA
            double T = Convert.ToDouble(text_SA_temperature.Text);
            int i, j;
            int cmax_N = 0;
            int cmax_akt, cmax_best;
            double r, error, diff;
            int ilosc = zamowienia.Count();
            int iteracje = 0;
            double a = Convert.ToDouble(text_SA_alpha.Text);
            int stop = Convert.ToInt32(text_SA_stopcrit.Text);
            int iteracje_dla_temp = Convert.ToInt32(text_SA_iterations.Text);
            double Tend = 1;
            int blad = 0;

            najlepsze_rozwiazanie.Clear();
            for (int g = 0; g < zamowienia.Count; g++)
            {
                Order order = new Order(zamowienia[g].numerStolika, zamowienia[g].wielkoscZamowienia, zamowienia[g].czasDostepnosci);
                najlepsze_rozwiazanie.Add(order);
            }
            cmax_best = koszt_funkcji_celu(zamowienia, trasy, ilosc_pociagow);
            cmax_akt = cmax_best;
            while (T > Tend && iteracje < stop)
            {
                for (int itr = 0; itr < iteracje_dla_temp; itr++)
                {
                    // Losowanie indeksow ktore maja byc zamienione
                    Random rnd = new Random();
                    i = rnd.Next(0, ilosc);
                    j = rnd.Next(0, ilosc);

                    zamowienia = SwapValues(zamowienia, i, j);
                    // Wyzerowanie zamowien konieczne do dzialania algorytmu
                    for (int g = 0; g < zamowienia.Count; g++)
                    {
                        zamowienia[g].zakonczone = 0;
                    }
                    // Obliczenie funkcji celu
                    cmax_N = koszt_funkcji_celu(zamowienia, trasy, ilosc_pociagow);
                    if (cmax_N > cmax_akt)
                    {
                        r = rnd.Next(0, 100);
                        r = r / 100;
                        diff = cmax_N - cmax_akt;
                        double podziel = -diff / T;
                        error = Math.Exp(podziel);

                        if (r < error) zamowienia = SwapValues(zamowienia, i, j);
                        else cmax_akt = cmax_N; //
                    }
                    else cmax_akt = cmax_N; //

                    if (cmax_akt < cmax_best)
                    {
                        cmax_best = cmax_akt;
                        najlepsze_rozwiazanie.Clear();
                        for (int g = 0; g < zamowienia.Count; g++)
                        {
                            Order order = new Order(zamowienia[g].numerStolika, zamowienia[g].wielkoscZamowienia, zamowienia[g].czasDostepnosci);
                            najlepsze_rozwiazanie.Add(order);
                        }

                    }

                }
                if (radio_SA_linear.IsChecked == true)
                {
                    T = T - a;
                }
                else if (radio_SA_geometrical.IsChecked == true)
                {
                    T = a * T;
                }
                else if (radio_SA_logaritmical.IsChecked == true)
                {
                    T = T / Math.Log(1 + iteracje);
                }

                iteracje = iteracje + 1;
            }
            stopwatch.Stop();
            // Wyswietlanie informacji o optymalnej kolejnosci zamowien
            for (int g = 0; g < najlepsze_rozwiazanie.Count; g++)
            {
                najlepsze_rozwiazanie[g].zakonczone = 0;
            }
            pociagi = funkcja_celu(najlepsze_rozwiazanie, trasy, ilosc_pociagow);
            for (int poc = 0; poc < ilosc_pociagow; poc++)
            {
                wyswietlany_tekst = wyswietlany_tekst + "Pociag" + poc + ": " + pociagi[poc].odwiedzone_stoliki + "czas: " + pociagi[poc].czas + "\n";
            }
            for (int g = 0; g < najlepsze_rozwiazanie.Count; g++)
            {
                najlepsze_rozwiazanie[g].zakonczone = 0;
            }

            int koszty = koszt_funkcji_celu(najlepsze_rozwiazanie, trasy, ilosc_pociagow);

            tekst1.Text = wyswietlany_tekst + "\n" + "Koszt calkowity: " + koszty + "\n" + "Czas wykonania" + stopwatch.ElapsedMilliseconds + "ms";
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

          
            int koszt_calkowity = 0;
            int ilosc_pociagow = 0;
            string wyswietlany_tekst = "";
            string plik_grafu = text_Route.Text;
            string plik_zamowien = text_Order.Text;
            List<Trasa> trasy = new List<Trasa>();
            List<Order> zamowienia = new List<Order>();
            List<Train> pociagi = new List<Train>();
            zamowienia = OrderRead(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\" + plik_zamowien);
            string line1 = File.ReadLines(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\" + plik_grafu).First();
            Graph gr = new Graph(Int32.Parse(line1));
            gr.readFromFile(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\" + plik_grafu);

            gr.calculateRoute();
            trasy = TrasaRead(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\plik_tymczasowy_trasy.txt");
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

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int ilosc_pociagow = 0;
            int cmax_best, cmax_curr, cmax_cand;
            int stop = 0;
            int warunek_stop = Convert.ToInt32(text_TS_stopcrit.Text);
            int maxTabuSize = Convert.ToInt32(text_TS_listlength.Text);
            string wyswietlany_tekst = "";
            string plik_grafu = text_Route.Text;
            string plik_zamowien = text_Order.Text;
            List<Trasa> trasy = new List<Trasa>();
            List<Order> zamowienia = new List<Order>();
            List<Order> najlepsze_rozwiazanie = new List<Order>();
            List<Order> najlepszy_kandydat = new List<Order>();
            List<Order> zwykly_kandydat = new List<Order>();
            List<Train> pociagi = new List<Train>();
            zamowienia = OrderRead(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\" + plik_zamowien);
            string line1 = File.ReadLines(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\" + plik_grafu).First();
            Graph gr = new Graph(Int32.Parse(line1)); 
            gr.readFromFile(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\" + plik_grafu);
            gr.calculateRoute();
            trasy = TrasaRead(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\plik_tymczasowy_trasy.txt");
            int ilosc = zamowienia.Count();
            int[,] array = new int[ilosc, ilosc];
            int forb_i = 0, forb_j = 0;
            if (train_number_value != null)
            {
                ilosc_pociagow = Convert.ToInt32(train_number_slider.Value);
            }
            // Odtad zaczyna sie algorytm TS
            for (int i = 0; i < ilosc; i++)
            {
                for (int j = 0; j < ilosc; j++)
                {
                    array[i, j] = 0;
                }
            }
            // Przyjmujemy rozwiazanie startowe jako aktualne
            for (int g = 0; g < zamowienia.Count; g++)
            {
                Order order = new Order(zamowienia[g].numerStolika, zamowienia[g].wielkoscZamowienia, zamowienia[g].czasDostepnosci);
                najlepszy_kandydat.Add(order);

            }
            for (int g = 0; g < zamowienia.Count; g++)
            {
                Order order = new Order(zamowienia[g].numerStolika, zamowienia[g].wielkoscZamowienia, zamowienia[g].czasDostepnosci);
                najlepsze_rozwiazanie.Add(order);

            }


            cmax_best = koszt_funkcji_celu(zamowienia, trasy, ilosc_pociagow);
            // Warunek stopu
            while (stop < warunek_stop)
            {
                zwykly_kandydat.Clear();
                for (int g = 0; g < najlepszy_kandydat.Count; g++)
                {
                    Order order = new Order(najlepszy_kandydat[g].numerStolika, najlepszy_kandydat[g].wielkoscZamowienia, najlepszy_kandydat[g].czasDostepnosci);
                    zwykly_kandydat.Add(order);

                }
                // Obliczamy wartosc rozwiazania aktualnego
                // Zerowanie zamowienia zeby moc jeszcze raz przeliczyc permutacje
                /*
                for (int g = 0; g < zwykly_kandydat.Count; g++)
                {
                    zwykly_kandydat[g].zakonczone = 0;
                }
                cmax_curr = koszt_funkcji_celu(zwykly_kandydat, trasy, ilosc_pociagow);
                */
                cmax_curr = 100000;
                // Generujemy sasiadow
                for (int i = 0; i < ilosc-1; i++)
                {
                    for(int j = i+1; j < ilosc; j++)
                    {
                        if (stop >= array[i, j])
                        {

                            zwykly_kandydat = SwapValues(zwykly_kandydat, i, j);

                            // Zerowanie zamowienia zeby moc jeszcze raz przeliczyc permutacje
                            for (int g = 0; g < zwykly_kandydat.Count; g++)
                            {
                                zwykly_kandydat[g].zakonczone = 0;
                            }

                            cmax_cand = koszt_funkcji_celu(zwykly_kandydat, trasy, ilosc_pociagow);

                            if (cmax_cand < cmax_curr)
                            {
                                najlepszy_kandydat.Clear();
                                for (int g = 0; g < zamowienia.Count; g++)
                                {
                                    Order order = new Order(zwykly_kandydat[g].numerStolika, zwykly_kandydat[g].wielkoscZamowienia, zwykly_kandydat[g].czasDostepnosci);
                                    najlepszy_kandydat.Add(order);
                                }
                                cmax_curr = cmax_cand;
                                
                                forb_i = i;
                                forb_j = j;
                            }

                            zwykly_kandydat = SwapValues(zwykly_kandydat, i, j);
                        }

                    }
                }


                Console.WriteLine(cmax_curr);
                if (cmax_curr < cmax_best)
                    {
                        cmax_best = cmax_curr;
                    najlepsze_rozwiazanie.Clear();
                    for (int g = 0; g < zamowienia.Count; g++)
                    {
                        Order order = new Order(najlepszy_kandydat[g].numerStolika, najlepszy_kandydat[g].wielkoscZamowienia, najlepszy_kandydat[g].czasDostepnosci);
                        najlepsze_rozwiazanie.Add(order);
                    }

                }

                // Wrzucamy rozwiazanie na liste tabu

                array[forb_i, forb_j] = maxTabuSize + stop;


                // Zwiekszamy warunek stopu - ilosc iteracji petli
                stop = stop + 1;
            }
            stopwatch.Stop();
            for (int g = 0; g < najlepsze_rozwiazanie.Count; g++)
            {
                
                najlepsze_rozwiazanie[g].zakonczone = 0;
            }
            pociagi = funkcja_celu(najlepsze_rozwiazanie, trasy, ilosc_pociagow);
            for (int poc = 0; poc < ilosc_pociagow; poc++)
            {
                wyswietlany_tekst = wyswietlany_tekst + "Pociag" + poc + ": " + pociagi[poc].odwiedzone_stoliki + "czas: " + pociagi[poc].czas + "\n";
            }
            for (int g = 0; g < najlepsze_rozwiazanie.Count; g++)
            {
                najlepsze_rozwiazanie[g].zakonczone = 0;
            }

            int koszty = koszt_funkcji_celu(najlepsze_rozwiazanie, trasy, ilosc_pociagow);

            tekst1.Text = wyswietlany_tekst + "\n" + "Koszt calkowity: " + koszty + "\n" + "Czas wykonania" + stopwatch.ElapsedMilliseconds + "ms";
        }


        // Funkcja do generowania losowego grafu o danej gestosci
        private void button_RandomGraph_Click(object sender, RoutedEventArgs e)
        {
            int wielkosc = Convert.ToInt32(text_Graph_tables.Text);
            int procent = 100;
            if (slider_Graph_density != null)
            {
                procent = Convert.ToInt32(slider_Graph_density.Value);
            }
            string nazwa_pliku = text_Graph_file.Text;
            Random rnd = new Random();

            int[,] graph = new int[wielkosc,wielkosc];
            int[,] smaller_graph = new int[wielkosc, wielkosc];
            // Wypelnienie tablicy zerami
            for (int i = 0; i < wielkosc; i++)
            {
                for (int j = 0; j < wielkosc; j++)
                {
                    graph[i,j] = 0;
                }
            }
            // Wypelnienie liczbami losowymi
            for (int i = 0; i < wielkosc; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    graph[i,j] = rnd.Next(1, 20);
                    graph[j,i] = graph[i,j];
                }
            }

           
            // Usuniecie pewnego procenta tras

            double il_tras = 0.5*(wielkosc * wielkosc) - wielkosc;
            double ilosc = (100 - procent) * 0.01 * il_tras;
            int ilosc_int = Convert.ToInt32(ilosc);
            int graf_spojny;
            
            do
            {
                int k = 0;
                graf_spojny = 1;/*
                for (int i = 0; i < wielkosc; i++)
                {
                    for (int j = 0; j < wielkosc; j++)
                    {
                        smaller_graph[i, j] = graph[i, j];
                    }

                }*/
                smaller_graph = graph.Clone() as int[,];
                while (k < ilosc_int)
                {
                    
                    int x = rnd.Next(wielkosc);
                    int y = rnd.Next(wielkosc);
                    if (smaller_graph[x, y] != 0)
                    {
                        smaller_graph[x, y] = 0;
                        smaller_graph[y, x] = 0;
                        k++;
                    }
                }

                // Sprawdzenie czy graf jest spojny - czy wszystkie wezly maja polaczenia
                Graph g = new Graph(wielkosc);
                g.graph_table = smaller_graph;
                g.calculateRoute();
                foreach (int dist in g.distance) 
                { 
                    if (dist > 20000)
                    {
                        graf_spojny = 0;
                    }
                    else
                        graph = smaller_graph.Clone() as int[,];

                }
            }
            while (graf_spojny < 1);

            // Zapisywanie do pliku 
            if (File.Exists(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\"+nazwa_pliku))
            {
                File.Delete(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\"+nazwa_pliku);
            }

            using (StreamWriter w = File.AppendText(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\" + nazwa_pliku))
            {
                w.WriteLine(wielkosc.ToString());
            }
            for (int i = 0; i < wielkosc; i++)
            {
                string line = "";
                for (int j = 0; j < wielkosc; j++)
                {
                    // Nie chcemy srednika na koncu wiersza
                    if (j < wielkosc - 1)
                        line = line + graph[i, j] + ";";
                    else
                        line = line + graph[i, j];
                }
                using (StreamWriter w = File.AppendText(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\"+nazwa_pliku))
                {
                    w.WriteLine(line);
                }
            }
        }

        private void text_SA_temperature_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void silder_Graph_density_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void slider_Graph_density_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (text_Graph_density != null)
            {
                String gestosc_grafu = slider_Graph_density.Value.ToString();
                text_Graph_density.Text = gestosc_grafu;
            }
        }

        private void button_RandomOrder_click(object sender, RoutedEventArgs e)
        {
            string nazwa_pliku = text_Random_Order_File.Text;
            int wielkosc = Convert.ToInt32(text_Random_Order_Number.Text);
            int max_number = Convert.ToInt32(text_Random_Order_Max.Text);
            int odstep = Convert.ToInt32(text_Random_Order_Break.Text);

            List<Order> randomTables = new List<Order>();
            for (int i = 1; i <= wielkosc; i++)
            {
                int czas_innego = 0;
                Random rnd = new Random();
                //int numer = i; // Najprostsze zamowienia zeby odwiedzic wszystkie stoliki
                int numer = rnd.Next(1, max_number);  // Generowanie stolika losowego
                int wielkosc_zamowienia = 0; // Wielkosc jakby robic inna funkcje celu

                // Zakładamy że pomiędzy jednym a drugim klientem mija 300 jednostek czasowych
                foreach (Order r in randomTables)
                {
                    if (r.numerStolika == numer && r.czasDostepnosci > czas_innego)
                    {
                         czas_innego = r.czasDostepnosci + 300;
                    }
                }
                int czas = czas_innego + rnd.Next(odstep);
                Order zamowienie = new Order(numer, wielkosc_zamowienia, czas);
                randomTables.Add(zamowienie);
            }
            // Generowanie zamowien
            // Zapisywanie do pliku 
            if (File.Exists(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\" + nazwa_pliku))
            {
                File.Delete(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\" + nazwa_pliku);
            }

            /* ewentualny pierwszy wiersz z maksymalnym stolikiem
            using (StreamWriter w = File.AppendText(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\" + nazwa_pliku))
            {
                w.WriteLine(wielkosc.ToString());
            }
            */
            for (int i = 0; i < wielkosc; i++)
            {
                string line = "";
                line = randomTables[i].numerStolika + " " + randomTables[i].wielkoscZamowienia + " " + randomTables[i].czasDostepnosci;


                using (StreamWriter w = File.AppendText(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\" + nazwa_pliku))
                {
                    w.WriteLine(line);
                }
            }
        }

        private void Natural_Button_Click(object sender, RoutedEventArgs e)
        {

            int koszt_calkowity = 0;
            int ilosc_pociagow = 0;
            string wyswietlany_tekst = "";
            string plik_grafu = text_Route.Text;
            string plik_zamowien = text_Order.Text;
            List<Trasa> trasy = new List<Trasa>();
            List<Order> zamowienia_r = new List<Order>();
            List<Train> pociagi = new List<Train>();
            zamowienia_r = OrderRead(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\" + plik_zamowien);
            string line1 = File.ReadLines(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\" + plik_grafu).First();
            Graph gr = new Graph(Int32.Parse(line1));
            gr.readFromFile(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\" + plik_grafu);

            gr.calculateRoute();

            List<Order> zamowienia = zamowienia_r.OrderBy(o => o.numerStolika).ToList();
            trasy = TrasaRead(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\plik_tymczasowy_trasy.txt");
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

        private void Available_Button_Click(object sender, RoutedEventArgs e)
        {

            int koszt_calkowity = 0;
            int ilosc_pociagow = 0;
            string wyswietlany_tekst = "";
            string plik_grafu = text_Route.Text;
            string plik_zamowien = text_Order.Text;
            List<Trasa> trasy = new List<Trasa>();
            List<Order> zamowienia_r = new List<Order>();
            List<Train> pociagi = new List<Train>();
            zamowienia_r = OrderRead(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\" + plik_zamowien);
            string line1 = File.ReadLines(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\" + plik_grafu).First();
            Graph gr = new Graph(Int32.Parse(line1));
            gr.readFromFile(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\" + plik_grafu);

            gr.calculateRoute();

            List<Order> zamowienia = zamowienia_r.OrderBy(o => o.czasDostepnosci).ToList();
            trasy = TrasaRead(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\plik_tymczasowy_trasy.txt");
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
    }
}
