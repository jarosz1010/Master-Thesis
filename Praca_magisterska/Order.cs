using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praca_magisterska
{
    public class Order
    {
        public int numerStolika;
        public int wielkoscZamowienia;
        public int czasDostepnosci;
        public int zakonczone;

        public Order(int Stolik, int Wielkosc, int Dostepnosc)
        {
            numerStolika = Stolik;
            wielkoscZamowienia = Wielkosc;
            czasDostepnosci = Dostepnosc;
            zakonczone = 0;
        }
    }

  
}
