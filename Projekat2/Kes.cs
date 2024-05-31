using Projekat1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Projekat2
{
    internal class Kes
    {
        private Semaphore _kesLock;
        private Dictionary<string, Stavka> _kes;
        private int kesKapacitet;
        private Queue<string> red;
        private int interval = 30000;
        private bool _istekao = false;

        public Kes(int Kapacitet)
        {
            kesKapacitet = Kapacitet;
            _kesLock = new Semaphore(1, 1);
            _kes = new Dictionary<string, Stavka>(kesKapacitet);
            red = new Queue<string>(kesKapacitet);
            PokreniPeriodicnoBrisanje();
        }

        private async void PokreniPeriodicnoBrisanje()
        {
            while (true) 
            {
                await Task.Delay(interval);
                _istekao = true;
            }
        }

        public void DodajUKes(string key, Stavka s)
        {
            try
            {
                _kesLock.WaitOne();
                if (_kes.ContainsKey(key))
                {
                    Console.WriteLine("Element je vec u kesu.");
                    return;
                }

                if (_kes.Count == kesKapacitet)
                {
                    string kljucZaBrisanje = red.Dequeue();
                    _kes.Remove(kljucZaBrisanje);
                }

                red.Enqueue(key);
                _kes.Add(key, s);
                TrenutnoStanje();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _kesLock.Release();
            }
        }

        public void TrenutnoStanje()
        {
            Console.WriteLine("Kljucevi koji se nalaze u kesu su:");
            foreach (var key in _kes.Keys)
            {
                if (red.Contains(key))
                    Console.WriteLine($" {key} ");
            }

            Console.WriteLine("Redosled za izbacivanje iz kesa:");
            foreach (var key in red)
            {
                if (_kes.ContainsKey(key))
                    Console.WriteLine($" {key} ");
            }
            Console.WriteLine("\n\n");
        }

        public void StampajStavkuKesa(string key)
        {
            _kesLock.WaitOne();
            try
            {
                if (_kes.TryGetValue(key, out Stavka k))
                {
                    Console.WriteLine($"Kljuc je: {key}, {k.ToString()} \n");
                }
                else
                {
                    Console.WriteLine("Kljuc nije pronadjen.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _kesLock.Release();
            }
        }

        public void StampajSadrzajKesa()
        {
            _kesLock.WaitOne();
            try
            {
                Console.WriteLine("Sadrzaj kesa:\n");
                foreach (var k in _kes)
                {
                    Console.WriteLine($"Kljuc je: {k.Key}, {k.Value.ToString()} \n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _kesLock.Release();
            }
        }

        public void ObrisiCeoKes()
        {
            _kesLock.WaitOne();
            try
            {
                _kes.Clear();
                red.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _kesLock.Release();
            }
        }

        public void PeriodicnoBrisanje()
        {
            if(_istekao)
            {
                _kesLock.WaitOne();
                try
                {
                    DateTime trenutnoVreme=DateTime.Now;
                    List<string> istekliKljucevi = new List<string>();

                    foreach(var k in _kes)
                    {
                        if(trenutnoVreme.Subtract(k.Value.VremeKreiranja).TotalMilliseconds >= interval)
                        {
                            istekliKljucevi.Add(k.Key);
                        }
                    }

                    foreach(string key in istekliKljucevi)
                    {
                        _kes.Remove(key);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    _istekao = false;
                    _kesLock.Release();
                }
            }
        }

        public void ObrisiIzKesa(string key)
        {
            _kesLock.WaitOne();
            try
            {
                if (_kes.Remove(key))
                {
                    var tempQueue = new Queue<string>(); ;
                    foreach (var k in red)
                    {
                        if (k != key)
                        {
                            tempQueue.Enqueue(k);
                        }
                    }
                    red = tempQueue;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _kesLock.Release();
            }
        }

        public Stavka CitajIzKesa(string key)
        {
            _kesLock.WaitOne();
            try
            {
                if (_kes.TryGetValue(key, out Stavka stavka))
                    return stavka;
                else
                    return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                _kesLock.Release();
            }
        }

        public bool ImaKljuc(string key)
        {
            _kesLock.WaitOne();
            try
            {
                return _kes.ContainsKey(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                _kesLock.Release();
            }
        }
    }
}
