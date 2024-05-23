using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Kes kes = new Kes();

            kes.DodajUKes("kljuc1", 100, "podaci1", 1000);
            kes.DodajUKes("kljuc2", 200, "podaci2", 1000);
            kes.DodajUKes("kljuc3", 300, "podaci3", 1000); // Ovo bi trebalo da izbaci "kljuc1" zbog kapaciteta

            kes.TrenutnoStanje();

            kes.StampajStavkuKesa("kljuc2");

            kes.StampajSadrzajKesa();

            var stavka = kes.CitajIzKesa("kljuc2");
            if (stavka != null)
            {
                Console.WriteLine($"Procitano iz kesa: {stavka.Ukupno}, {stavka.Podaci}");
            }
            else
            {
                Console.WriteLine("Kljuc nije pronadjen.");
            }

            kes.ObrisiIzKesa("kljuc2");
            kes.TrenutnoStanje();

            kes.ObrisiCeoKes();
            kes.TrenutnoStanje();
        }
    }
}
