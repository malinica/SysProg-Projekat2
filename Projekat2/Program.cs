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

            kes.DodajUKes("key1", 10, "data1", 1000);
            kes.DodajUKes("key2", 20, "data2", 1000);
            kes.DodajUKes("key3", 30, "data3", 1000);

            Console.WriteLine("Trenutno stanje nakon dodavanja 3 stavke:");
            kes.TrenutnoStanje();

            kes.DodajUKes("key4", 40, "data4", 1000);

            Console.WriteLine("Trenutno stanje nakon dodavanja jos jedne stavke:");
            kes.TrenutnoStanje();

            Console.WriteLine("Citanje stavke sa kljucem 'key2':");
            kes.StampajStavkuKesa("key2");

            Console.WriteLine("Brisanje stavke sa kljucem 'key2':");
            kes.ObrisiIzKesa("key2");

            Console.WriteLine("Trenutno stanje nakon brisanja stavke sa kljucem 'key2':");
            kes.TrenutnoStanje();

            Console.WriteLine("Sadrzaj celog kesa:");
            kes.StampajSadrzajKesa();

            Console.WriteLine("Brisanje celog kesa:");
            kes.ObrisiCeoKes();

            Console.WriteLine("Trenutno stanje nakon brisanja celog kesa:");
            kes.TrenutnoStanje();

            Console.WriteLine("Testiranje završeno.");
        }
    }
}
