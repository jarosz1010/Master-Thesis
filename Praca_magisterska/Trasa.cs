using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praca_magisterska
{
    public class Trasa
    {
        public int stolik;
        public string przystanki;
        public int koszt_calkowity;

        public Trasa(int Stolik, string Przystanki, int Koszt_calkowity)
        {
            stolik = Stolik;
            przystanki = Przystanki;
            koszt_calkowity = Koszt_calkowity;
        }
   
    }
}
