using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tic_Tac_Toe_Server
{

    class Program
    {

        static void Main(string[] args)
        {
            Server server = new Server();
            server.StartServer();
        }

    }
}
