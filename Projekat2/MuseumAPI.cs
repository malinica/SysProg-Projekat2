using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Projekat2
{
    internal class MuseumAPI
    {

        private readonly string baseUrl;
        private readonly HttpClient client;

        public MuseumAPI(string baseUrl)
        {
            this.baseUrl = baseUrl;
            client = new HttpClient();
        }

        public async Task<Stavka> GetPodaci(string kljuc)
        {
            try
            {
                //                    Stopwatch stopwatch = new Stopwatch();
                SemaphoreSlim semaphore = new SemaphoreSlim(80);
                string url = baseUrl + "?" + kljuc;
                HttpResponseMessage response = await client.GetAsync(url);
                // Console.WriteLine(url);

                if (response.IsSuccessStatusCode)
                {
                    var rezultatString = await response.Content.ReadAsStringAsync();
                    JToken rezultatJSON = JToken.Parse(rezultatString);
                    int[] objectIDs = rezultatJSON["objectIDs"]
                                        .Select(id => (int)id)
                                        .ToArray();
                    Stavka s = new Stavka();

                    //                   stopwatch.Start();
                    List<Task> tasks = new List<Task>();

                    foreach (int o in objectIDs)
                    {
                        await semaphore.WaitAsync();

                        Task task = Task.Run(async () =>
                        {
                            try
                            {
                                HttpResponseMessage response2 = await client.GetAsync($"https://collectionapi.metmuseum.org/public/collection/v1/objects/{o}");

                                if (response2.IsSuccessStatusCode)
                                {
                                    var rezultatStringUpit = await response2.Content.ReadAsStringAsync();
                                    JToken rezultatJSONUpit = JToken.Parse(rezultatStringUpit);
                                    var ime = (rezultatJSONUpit["title"]).ToString();
                                    s.Dodaj(o, ime);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        });

                        tasks.Add(task);
                    }
                    await Task.WhenAll(tasks);
                    //                   stopwatch.Stop();
                    //                   Console.WriteLine($"Vreme izvršavanja: {stopwatch.ElapsedMilliseconds} ");
                    return s;
                }
                else
                {
                    throw new Exception("Greska u pribavljanju ID");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }



    }
}

