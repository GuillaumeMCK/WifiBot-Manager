using System;
using System.Net.Sockets;

namespace GestionnaireWifiBot.Model
{
    class Rover
    {
        //-------------
        // Attributs
        //-------------
        private int server_port;       // Numéro de port TCP utilisé
        private string server_address; // Nom du serveur
        private Byte[] command;        // Tableau de 2 octets contenant les valeurs à envoyer
        private bool connected;        // Statut de la connexion
                                       
        TcpClient clientSocket;        // Instanciation d'un client TCP qui va permettre de se connecter au serveur du robot
        NetworkStream networkStream;   // Instanciation d'un flux réseau rattaché au socket

        public bool ConnectionState { get { return connected; } } // Getter du statut de la connexion au rover
        
        //--------------
        // Constructeur
        //--------------
        public Rover(string adresse, int port_TCP)
        {
            server_address = adresse;
            server_port = port_TCP;
            command = new Byte[2] { 0x00, 0x00 };
            clientSocket = new TcpClient();
            connected = false;
        }

        public void Connection()
        {
            if (!connected)
            {
                try
                {
                    clientSocket.Connect(server_address, server_port);   // Connexion au serveur avec l'adresse IP et le Port spécifié.
                    networkStream = clientSocket.GetStream(); // Si la connexion réussi on attache le flux au canal de communication crée (socket)
                    connected = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    connected = false;
                }
            }
        }

        //----------------------------------------------------
        // Méthode qui permet d'envoyer une commande au rover
        //----------------------------------------------------
        public void Commander(Byte[] bytes)
        {
            command = bytes;

            if (connected)
            {
                try
                {
                    networkStream.Write(bytes, 0, 2); // On écrit dans le flux la commande;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Disconnect();
                }
            }
        }

        //----------------------------------------------------
        // Méthode de déconnexion du rover
        //----------------------------------------------------
        public void Disconnect()
        {
            if (connected)
            {
                networkStream.Dispose();
                clientSocket.Dispose();
                connected = false;
            }
        }
    }
}