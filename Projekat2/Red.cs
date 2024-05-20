﻿using Projekat1;
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
        private const int kapacitet = 2;

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
                Console.WriteLine("Ne moze se dodati element jer je red pun.");
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
                Console.WriteLine("Red je prazan, nema nista za brisanje.");
            }
        }


    }
}
