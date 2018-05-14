using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tic_Tac_Toe_Server
{
    public class PlayerBound
    {
        public string Player1;
        public string Player2;
    }
    public class ClientIDs
    {
        public string ClientID { get; set; }
        public EndPoint Ip { get; set; }
    }

    class Server
    {

        public TcpListener tcpListener = new TcpListener(IPAddress.Any, 12345);
        public List<Socket> clientList = new List<Socket>();
        List<PlayerBound> Players = new List<PlayerBound>();
        static IList<ClientIDs> ClientList = new List<ClientIDs>();
        string receivedMessage;
        public ClientIDs Player1;
        public ClientIDs Player2;
        NetworkStream player1;
        NetworkStream player2;
        List<TcpClient> Clients = new List<TcpClient>();



        public void StartServer()
        {
            tcpListener.Start();
          
            //later uit te breiden naar meerdere spelers...
            Thread newThread = new Thread(new ThreadStart(Listening));
            newThread.Start();

            Thread secondThread = new Thread(new ThreadStart(Listening));
            secondThread.Start();


        }

            

        void Listening()
        {
            TcpClient socketForClient = tcpListener.AcceptTcpClient();
            Clients.Add(socketForClient);
            if (socketForClient.Connected)
            {
                Console.WriteLine("player:" + socketForClient.Client.RemoteEndPoint + " now connected to server.");
           

                while (true)
                {
              
                    NetworkStream defaultStream = socketForClient.GetStream();

                    byte[] buffer = new byte[socketForClient.ReceiveBufferSize];
                    int bytesRead = defaultStream.Read(buffer, 0, socketForClient.ReceiveBufferSize);
                    string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    if (dataReceived.Length > 2) receivedMessage = dataReceived.Remove(0, 2);

                    if (dataReceived.Substring(0, 2) == "lo")
                    {
                        ClientList.Add(new ClientIDs { ClientID = receivedMessage, Ip = socketForClient.Client.RemoteEndPoint });
                      
                      
                        string SendClientList = "up";
                        int sizeClientList = ClientList.Count;
                        for (int i = 0; i < sizeClientList; i++)
                        {
                           
                                SendClientList += ClientList[i].ClientID + ",";
                            
                        }
                        byte[] bufi = Encoding.UTF8.GetBytes(SendClientList);


                        defaultStream.Write(bufi, 0, bufi.Length);
                    }


                    if (dataReceived == "re")
                    {
                        string SendClientList = "up";
                        int sizeClientList = ClientList.Count;
                        for (int i = 0; i < sizeClientList; i++)
                        {

                            SendClientList += ClientList[i].ClientID + ",";

                        }
                        byte[] bufi = Encoding.UTF8.GetBytes(SendClientList);


                        defaultStream.Write(bufi, 0, bufi.Length);
                    }

                    if (dataReceived.Substring(0, 2) == "pl")
                    {
                        string[] tmp = receivedMessage.Split(',');
                        Player2 = ClientList.ToList().Find(x => x.ClientID == tmp[0]);
                        Player1 = ClientList.ToList().Find(x => x.ClientID == tmp[1]);
                        player1 = Clients.Find(x => x.Client.RemoteEndPoint.Equals(Player1.Ip)).GetStream();
                        player2 = Clients.Find(x => x.Client.RemoteEndPoint.Equals(Player2.Ip)).GetStream();

                        byte[] bufi = Encoding.UTF8.GetBytes("st" + Player1.ClientID);
                        
                        player2.Write(bufi, 0, bufi.Length);

                    }
                    if (dataReceived.Substring(0, 2)=="p1")
                    {
                        byte[] bufi2 = Encoding.UTF8.GetBytes("gm" + Player1.ClientID);
                        

                        player1.Write(bufi2, 0, bufi2.Length);
                   
                    }
                    if (dataReceived.Substring(0, 2) == "p2")
                    {
                        byte[] bufi2 = Encoding.UTF8.GetBytes("gm" + Player2.ClientID);

                        player2.Write(bufi2, 0, bufi2.Length);
                    }
                    if (dataReceived.Substring(0, 2) == "sw")
                    {
                       Console.WriteLine(receivedMessage);

                       TcpClient otherPlayer = Clients.Find(x => x.Client.RemoteEndPoint != socketForClient.Client.RemoteEndPoint);

                       

                        Byte[] bufi2 = Encoding.UTF8.GetBytes("br" + receivedMessage);

                        if (Player1.Ip == otherPlayer.Client.RemoteEndPoint)
                        {
                            player1.Write(bufi2, 0, bufi2.Length);
                        }
                        else
                        {
                            player2.Write(bufi2, 0, bufi2.Length);
                        }

                
                       
                    }

                }


            }


        }

    }
    }

