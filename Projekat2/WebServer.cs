//using Newtonsoft.Json.Linq;
using Projekat2;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Projekat1
{
    public class WebServer
    {
        private readonly MuseumAPI museum;
        private readonly Kes kes;
        private readonly HttpListener listener;
        private const int port = 8080;
        private const string adresa = "http://localhost";

        public WebServer(string baseUrl, int kapacitetKesa)
        {
            museum = new MuseumAPI(baseUrl);
            kes = new Kes(kapacitetKesa);
            listener = new HttpListener();
            listener.Prefixes.Add($"{adresa}:{port}/");
        }

        public async Task Start()
        {
            try
            {
                listener.Start();
                Console.WriteLine("Web server startovan...");

                while (true)
                {
                    HttpListenerContext context = listener.GetContext();
                    await Task.Run(() =>
                    {
                        Task task = ProcessRequest(context);
                    });
                    kes.PeriodicnoBrisanje();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task ProcessRequest(HttpListenerContext zahtev)
        {
            try
            {
                if (zahtev == null)
                {
                    return;
                }
                if (zahtev.Request.HttpMethod != "GET")
                    await OdgovoriNaZahtev(400, null, zahtev);
                string Putanja = zahtev.Request.Url.ToString();
                // Console.WriteLine(Putanja);
                if (Putanja.Contains("favicon.ico"))
                {
                    await OdgovoriNaZahtev(400, null, zahtev);
                    return;
                }
                if (!Putanja.StartsWith(($"{adresa}:{port}/")))
                {
                    await OdgovoriNaZahtev(400, null, zahtev);
                    return;
                }
                string parametarPretrage = izdvojiParametre(Putanja);
                if (parametarPretrage == null)
                    await OdgovoriNaZahtev(400, null, zahtev);
                if (kes.ImaKljuc(parametarPretrage))
                {
                    Stavka stavkaIzKesa = new Stavka();
                    stavkaIzKesa = kes.CitajIzKesa(parametarPretrage);
                    await OdgovoriNaZahtev(200, "Key je: " + parametarPretrage + "\n" + stavkaIzKesa.ToString(), zahtev);
                }
                else
                {

                    var rezultat = await museum.GetPodaci(parametarPretrage);
                    if (rezultat == null)
                    {
                        await OdgovoriNaZahtev(400, "Nema podataka za zadati parametar", zahtev);
                        throw new Exception("Nema podataka za zadati parametar");

                    }
                    else
                    {
                        kes.DodajUKes(parametarPretrage, rezultat);
                        await OdgovoriNaZahtev(200, "Key je: " + parametarPretrage + "\n" + rezultat.ToString(), zahtev);

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:: {ex.Message}");
            }
            finally
            {
                await OdgovoriNaZahtev(400, "Server error.", zahtev);
            }
        }

        private async Task OdgovoriNaZahtev(int responseCode, string text, HttpListenerContext context)
        {
            var response = context.Response;
            response.StatusCode = responseCode;

            string body;
            if (responseCode == 400)
            {
                response.ContentType = "text/html";
                body = $@"<html>
                    <head><title>Bad Request</title></head>
                    <body>
                        {text}
                    </body>
                         </html>";
                try
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(body);
                    await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
                    response.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
                return;
            }
            else if (responseCode == 200)
            {
                body = text;
                try
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(body);
                    await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
                    response.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
                return;
            }
        }

        public string izdvojiParametre(string Putanja)
        {
            try
            {
                string[] urlParts = Putanja.Split('?');
                if (urlParts.Length < 1)
                    return null;

                string queryParams = urlParts[1];
                string[] paramsList = queryParams.Split('&');
                int artistOrCulture = -1, isOnView = -1, isHighlight = -1, departmentId = -1;//-1 nije naveden,1 true,0 false
                string q = null, medium = null;
                foreach (var parameter in paramsList)
                {
                    var keyValue = parameter.Split('=');
                    if (keyValue.Length == 2)
                    {
                        switch (keyValue[0])
                        {
                            case "artistOrCulture":
                                {
                                    if (keyValue[1] == "true")
                                        artistOrCulture = 1;
                                    else
                                        artistOrCulture = 0;

                                    break;
                                }
                            case "isOnView":
                                {
                                    if (keyValue[1] == "true")
                                        isOnView = 1;
                                    else
                                        isOnView = 0;

                                    break;
                                }
                            case "isHighlight":
                                {
                                    if (keyValue[1] == "true")
                                        isHighlight = 1;
                                    else
                                        isHighlight = 0;

                                    break;
                                }

                            case "q":
                                q = keyValue[1];
                                break;
                            case "medium":
                                medium = keyValue[1];
                                break;
                            case "departmentId":
                                int.TryParse(keyValue[1], out departmentId);
                                break;
                            default:
                                {
                                    return null;
                                }
                        }
                    }
                }
                string parametarPretrage = null;
                if (departmentId != -1)
                {
                    parametarPretrage += "departmentId=" + (departmentId.ToString()) + "&";
                }
                if (isOnView != -1)
                {
                    parametarPretrage += "isOnView=" + (isOnView == 1) + "&";
                }
                if (artistOrCulture != -1)
                {
                    parametarPretrage += "artistOrCulture=" + (artistOrCulture == 1) + "&";
                }
                if (isHighlight != -1)
                {
                    parametarPretrage += "isHighlight=" + (isHighlight == 1) + "&";
                }
                if (q != null)
                {
                    parametarPretrage += "q=" + q + "&";
                }
                if (medium != null)
                {
                    parametarPretrage += "medium=" + medium;
                }
                if (parametarPretrage.EndsWith("&"))
                {
                    parametarPretrage = parametarPretrage.Substring(0, parametarPretrage.Length - 1);
                }
                return parametarPretrage;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
    }
}