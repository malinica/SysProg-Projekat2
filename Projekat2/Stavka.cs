using System;
using System.Collections.Generic;

namespace Projekat2
{
    public class Stavka
    {
        public Dictionary<int, string> stavka { get; set; }
        public DateTime VremeKreiranja { get; set; }


        public Stavka()
        {
            stavka = new Dictionary<int, string>();
            VremeKreiranja = DateTime.UtcNow;
        }

        public void Dodaj(int id, string ime)
        {
            stavka.Add(id, ime);
        }
        public override string ToString()
        {
            string rezultat = "";
            foreach (var par in stavka)
            {
                rezultat += $"ID stavke: {par.Key}, Naslov dela: {par.Value} \n";
            }
            return rezultat;
        }
    }
}
