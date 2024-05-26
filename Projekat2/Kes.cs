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
        private int kesKapacitet;
        private Queue<string> red;

        public Kes(int Kapacitet)
        {
            kesKapacitet = Kapacitet;
            _kesLock = new ReaderWriterLockSlim();
            _kes = new Dictionary<string, Stavka>(kesKapacitet);
            red = new Queue<string>(kesKapacitet);
        }

        public void DodajUKes(string key,Stavka s )
        {
            try
            {
                if (!_kesLock.TryEnterWriteLock(10))
                    return;
                if (_kes.ContainsKey(key))
                    throw new Exception("Element je vec u kesu.\n");

                if (_kes.Count == kesKapacitet)
                {
                    string kljucZaBrisanje = red.Dequeue();
                    _kes.Remove(kljucZaBrisanje);
                }

                red.Enqueue(key);
                _kes.Add(key, s);
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
            foreach (var key in red)
            {
                Console.WriteLine(key);
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
                red.Clear();
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
                if (_kes.Remove(key))
                {
                    var tempQueue = new Queue<string>(); ;
                    foreach(var k in red)
                    {
                        if(k!=key)
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
                _kesLock.ExitWriteLock();
            }
        }

        public Stavka CitajIzKesa(string key)
        {
            _kesLock.EnterReadLock();
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
