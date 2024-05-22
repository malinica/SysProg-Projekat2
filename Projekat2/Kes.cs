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
        private ReaderWriterLockSlim _kesLock;
        private Dictionary<string, Stavka> _kes;
        private const int kesKapacitet = 2;
        private Red red;

        public Kes()
        {
            _kesLock = new ReaderWriterLockSlim();
            _kes = new Dictionary<string, Stavka>(kesKapacitet);
            red = new Red();
        }

        public void DodajUKes(string key, int ukupno1, string podaci1, int timeout)
        {
            try
            {
                if (!_kesLock.TryEnterWriteLock(timeout))
                    return;
                if (_kes.ContainsKey(key))
                    throw new Exception("Element je vec u kesu.\n");

                Stavka stavka = new Stavka(ukupno1, podaci1);

                if (_kes.Count == kesKapacitet)
                {
                    Stavka zaBrisanje = red.UzmiIzReda();
                    string kljucZaBrisanje = null;
                    foreach (var k in _kes)
                    {
                        if (k.Value == zaBrisanje)
                        {
                            kljucZaBrisanje = k.Key;
                            break;
                        }
                    }
                    _kes.Remove(kljucZaBrisanje);
                }

                red.DodajURed(stavka);
                _kes.Add(key, stavka);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _kesLock.ExitWriteLock();
            }
        }

        public void TrenutnoStanje()
        {
            Console.WriteLine("Kljucevi koji se nalaze u kesu su:");
            foreach (var key in _kes.Keys)
            {
                Console.WriteLine($" {key} ");
            }

            Console.WriteLine("Redosled za izbacivanje iz kesa:");
            foreach (var stavka in red.SviElementi())
            {
                Console.WriteLine(stavka.ToString());
            }
            Console.WriteLine("\n\n");
        }

        public void StampajStavkuKesa(string key)
        {
            _kesLock.EnterReadLock();
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
                _kesLock.ExitReadLock();
            }
        }

        public void StampajSadrzajKesa()
        {
            _kesLock.EnterReadLock();
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
                _kesLock.ExitReadLock();
            }
        }

        public void ObrisiCeoKes()
        {
            _kesLock.EnterWriteLock();
            try
            {
                _kes.Clear();
                red.Obrisi();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _kesLock.ExitWriteLock();
            }
        }

        public void ObrisiIzKesa(string key)
        {
            _kesLock.EnterWriteLock();
            try
            {
                if (_kes.Count != 0 && ImaKljuc(key))
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
                _kesLock.ExitWriteLock();
            }
        }

        public Stavka CitajIzKesa(string key)
        {
            _kesLock.EnterReadLock();
            try
            {
                return _kes.ContainsKey(key) ? _kes[key] : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                _kesLock.ExitReadLock();
            }
        }

        public bool ImaKljuc(string key)
        {
            _kesLock.EnterReadLock();
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
                _kesLock.ExitReadLock();
            }
        }
    }
}
