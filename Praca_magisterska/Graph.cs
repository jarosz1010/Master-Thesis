using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praca_magisterska
{
    public class Graph
    {
        public int[,] graph_table;
        int number_of_tables;
        public int[] distance;

        public Graph(int Number_of_Tables)
        {
            graph_table = new int[Number_of_Tables, Number_of_Tables];
            number_of_tables = Number_of_Tables;
        }


        public void readFromFile(string plik)
        {
            List<List<int>> tabela = new List<List<int>>();
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(plik);
            int nr_linii = 0;
            while ((line = file.ReadLine()) != null)
            {   if (nr_linii > 0)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        char[] delimiterChars = { ' ', ',', '.', ';', ':', '\t' };
                        string[] words = line.Split(delimiterChars);
                        List<int> wiersz = new List<int>();
                        for (int i = 0; i < words.Length; i++)
                        {
                            wiersz.Add(Int32.Parse(words[i]));
                        }
                        tabela.Add(wiersz);
                    }
                }
                nr_linii = nr_linii + 1;
            }
            file.Close();
            for (int i = 0; i < tabela.Count(); i++)
            {
                for (int j = 0; j < tabela[i].Count(); j++)
                    graph_table[i, j] = tabela[i][j];
            }
  
        }

        public void initiateZeros()
        {
            // Funkcja inicjuje tablice kosztow wypelniajac ja zerami
            for (int i = 0; i < number_of_tables; i++)
            {
                for (int j = 0; j < number_of_tables; j++)
                {
                    graph_table[i, j] = 0;
                }
            }
        }

        public void addEdge(int start, int stop, int koszt)
        {
            // Funkcja dodaje trase do tablicy kosztow
            graph_table[start, stop] = koszt;
            graph_table[stop, start] = koszt;

        }

        int minDistance(int[] dist,
                bool[] sptSet)
        {
            int min = int.MaxValue, min_index = -1;

            for (int v = 0; v < number_of_tables; v++)
                if (sptSet[v] == false && dist[v] <= min)
                {
                    min = dist[v];
                    min_index = v;
                }

            return min_index;
        }

        public void calculateRoute()
        {
            // Funkcja oblicza koszt najkrotszej trasy od wierzcholka startowego i podaje punkty przelotowe

            int[] dist = new int[number_of_tables]; // Tablica przechowujaca dystanse dla poszczegolnych punktow
            string[] route = new string[number_of_tables]; // Tablica przechowujaca trasy dla poszczegolnych punktow


            bool[] previous = new bool[number_of_tables];

            // Ustawienie odleglosci na nieskonczonosc i poprzednikow na zbior pusty
            for (int i = 0; i < number_of_tables; i++)
            {
                dist[i] = int.MaxValue;
                previous[i] = false;
            }

            // Ustawienie odleglosci dla punktu startowego na zero
            dist[0] = 0;

            // 
            for (int count = 0; count < number_of_tables - 1; count++)
            {
                // Obliczenie do ktorego wierzcholka odleglosc jest najmniejsza
                int u = minDistance(dist, previous);

                previous[u] = true;


                for (int v = 0; v < number_of_tables; v++)
                {

                    if (!previous[v] && graph_table[u, v] != 0 &&
                         dist[u] != int.MaxValue && dist[u] + graph_table[u, v] < dist[v])
                    {
                        dist[v] = dist[u] + graph_table[u, v];
                        if (route[u] == "0")
                            route[v] = u.ToString();
                        else
                        {
                            if (route[u] != null)
                                route[v] = route[u] + "r" + u;
                            else
                                route[v] = route[u] + u;
                        }
                    }
                }
            }
            distance = dist;
            using (var sw = new StreamWriter(@"C:\Users\Michal\source\repos\Praca_magisterska\Praca_magisterska\plik_tymczasowy_trasy.txt"))
            {
                for (int i = 1; i < number_of_tables; i++)
                {
                    //Console.Write(i + " \t\t " + dist[i] + " \t\t " + route[i] + "\n");
                    sw.WriteLine(i + " " + route[i] + " " + dist[i]);
                }
            }
        }

    }
}
