using Projekat1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat2
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            WebServer server = new WebServer("https://collectionapi.metmuseum.org/public/collection/v1/search", 10);
            await server.Start();
        }
    }
}