using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praca_magisterska
{
    public class Route
    {
        public int routeNumber;
        public int firstNode;
        public int secondNode;
        public int cost;
        public int occupied;
        public Route(int RouteNumber, int FirstNode, int SecondNode, int Cost)
        {
            routeNumber = RouteNumber;
            firstNode = FirstNode;
            secondNode = SecondNode;
            cost = Cost;
        }

   
    }
}
