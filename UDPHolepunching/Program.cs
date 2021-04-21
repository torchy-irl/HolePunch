using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Windows;

namespace UDPHolepunching
{
    class Program
    {
        //Definerer variabler der skal være globale
        private static TcpClient client = new TcpClient(); //Klienten der håndterer at oprette forbindelse
        private static NetworkStream stream; //Streamen hvor informationen er
        private static Thread thr; //Threaden hvor receive er

        static void Main(string[] args)
        {
            Connection();
            bool conn = true;
            while (conn == true)
            //TODO Opsæt mere elegant måde at håndtere kommandoer på hvis der kommer flere
            {
                string inp = Console.ReadLine();
                if (inp == "!DISCONNECT")
                {
                    Disconnect();
                    break;
                }
                else
                {
                    Send(inp);
                }
                
            }
            

        }

        static void Connection() //Skal laves om så method returner false hvis forbindelse ikke kan laves
        {
            //Lad brugeren indtaste IP og port
            Console.WriteLine("Enter server IP:");
            string ip = Console.ReadLine();
            Console.WriteLine("Enter port nr:");
            int port = Convert.ToInt32(Console.ReadLine());

            //Connect og nap en stream
            client.Connect(ip, port);
            stream = client.GetStream();

            //Start en ny thr for Recieve
            thr = new Thread(new ThreadStart(Recieve));
            thr.Start();
        }
        static void Recieve()
        {
            while (true)
            {
                //Sætter makslængde af beskeden
                Byte[] bytes = new Byte[256];
                //Læse fra datastream, encoder som UTF-8, printer til konsollen
                int data = stream.Read(bytes, 0, bytes.Length);
                //Kontrollerer, at der conn_check func ikke spammer konsollen med mellemrum
                if (Encoding.UTF8.GetString(bytes, 0, data) == " ")
                { }
                else { Console.Write(Encoding.UTF8.GetString(bytes, 0, data) + "\n"); }  
            }
        }

        static void Send(string inp)
        {
            Byte[] msg = Encoding.UTF8.GetBytes(inp);
            Byte[] length = BitConverter.GetBytes(msg.Length);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(length);
            }
            Console.Write(msg);
            //Sender headeren
            stream.Write(length, 0, 4);
            //Sender selve beskeden
            stream.Write(msg, 0, msg.Length);
        }
        static void Disconnect()
        {
            thr.Abort();
            stream.Close();
            //Sig godnat
            Console.WriteLine("Forbindelsen er nu lukket");
        }

        
    }
}
