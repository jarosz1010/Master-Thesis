using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praca_magisterska
{
    public class Train
    {
        public int czas;
        public List<int> zajete_stoliki = new List<int>();
        public string odwiedzone_stoliki;
        public Train()
        {
            czas = 0;
            odwiedzone_stoliki = "";
        }

        // Funkcja obliczajaca przez ktore stoliki inne pociagi nie moga przejechac
        public void dodajStoliki(string przystanki, int cel)
        {

            zajete_stoliki.Clear();
            zajete_stoliki.Add(cel);
            string[] words = przystanki.Split('r');
            foreach (var word in words)
            {
                if (word != "0")
                    zajete_stoliki.Add(Int32.Parse(word));
            }
        }

        // Funkcja sprawdzajaca zajetosc trasy na podstawie zajetych stolikow innych pociagow
        public int sprawdz_zajetosc(int cel, string przystanki, List<int> stoliki_2_pociagu)
        {
            int wartosc_zwracana = 1;
            foreach (var stolik in stoliki_2_pociagu)
            {
                if (przystanki.Contains(stolik.ToString()))
                    wartosc_zwracana = 0;
                if (stolik == cel)
                    wartosc_zwracana = 0;
            }
            return wartosc_zwracana;
        }
    }
}
