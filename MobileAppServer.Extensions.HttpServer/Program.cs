using System;

namespace MobileAppServer.Extensions.HttpServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HttpModule module = new HttpModule("http://localhost:9000/");
            module.Load(new ServerObjects.Server());

            Console.ReadKey();
        }
    }
}
