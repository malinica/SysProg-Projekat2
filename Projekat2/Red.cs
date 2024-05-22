using Projekat1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Projekat2
{
    internal class Red
    {
        private int mestoCitanja, mestoPisanja;
        private Stavka[] red;
        private const int kapacitet = 10;

        public Red()
        {
            mestoCitanja = mestoPisanja = 0;
            red = new Stavka[kapacitet];
        }

        public void DodajURed(Stavka element)
        {
            if (mestoPisanja < kapacitet)
            {
                red[mestoPisanja] = element;
                mestoPisanja++;
            }
            else
            {
                Console.WriteLine("Ne moze se dodati element jer je red pun.\n");
            }
        }

        public void ObrisiIzReda()
        {
            if (mestoCitanja < mestoPisanja)
            {
                for (int i = mestoCitanja; i < mestoPisanja - 1; i++)
                {
                    red[i] = red[i + 1];
                }
                red[mestoPisanja - 1] = null;
                mestoPisanja--;
            }
            else
            {
                Console.WriteLine("Red je prazan, nema nista za brisanje.\n");
            }
        }

        public Stavka[] SviElementi()
        {
            Stavka[] elementi = new Stavka[Velicina()];
            int indeks = 0;
            for(int i=mestoCitanja; i!=mestoPisanja;i=(i+1)%kapacitet)
            {
                elementi[indeks++] = red[i];
            }
            return elementi;
        }

        public int Velicina()
        {
            if(mestoPisanja>=mestoCitanja)
            {
                return mestoPisanja - mestoCitanja;
            }
            else
            {
                return kapacitet - mestoCitanja + mestoPisanja;
            }
        }

        public Stavka UzmiIzReda()
        {
            if (mestoCitanja == mestoPisanja)
            {
                Console.WriteLine("Red je prazan, nema nista za uzimanje.\n");
                return null;
            }
            Stavka element = red[mestoCitanja];
            red[mestoCitanja] = null;
            mestoCitanja = (mestoCitanja + 1) % kapacitet;
            return element;
        }

        public bool Prazan()
        {
            return mestoPisanja == mestoCitanja;
        }

        public void Obrisi()
        {
            red = new Stavka[kapacitet];
            mestoCitanja = mestoPisanja = 0;
        }
    }
}
