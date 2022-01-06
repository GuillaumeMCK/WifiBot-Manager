using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestUnitaireClasseRover
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
        Task roverTask;

        //-------------
        // Propriétés
        //-------------
        public bool ConnectionState { get { return connected; } } // Getter du statut de la connexion au rover
        public byte[] Command { set { command = value; } } // Setter de la command a envoyer au rover

        //--------------
        // Constructeur
        //--------------
        public Rover(string adresse, int port_TCP)
        {
            server_address = adresse;
            server_port = port_TCP;
            command = new Byte[2] { 0xc0, 0xc0 };
            connected = false;
        }

        //----------------------------------------------------
        // Méthode de connexion au rover
        //----------------------------------------------------
        public void Connection()
        {
            if (!connected)
            {
                try
                {
                    clientSocket = new TcpClient();                    // Creation d'un client TCP
                    clientSocket.Connect(server_address, server_port); // Connexion au serveur avec l'adresse IP et le Port spécifié.
                    networkStream = clientSocket.GetStream();          // Si la connexion réussi on attache le flux au canal de communication crée (socket)
                    connected = true;
                    roverTask = new Task(() => { while (connected) SendCommand(command); });
                    roverTask.Start();
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
        private void SendCommand(Byte[] bytes)
        {
            if (connected)
            {
                try
                {
                    networkStream.Write(bytes, 0, 2); // On écrit dans le flux la commande;
                    Thread.Sleep(200);
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
            // Arret de l'envoi de la command au rover
            if (connected)
            {
                // Fin de la tache asynchrone
                connected = false;
                roverTask.Wait();
                roverTask.Dispose();
            }
            // Liberation des ressources
            if (networkStream != null)
                networkStream.Close();
            if (clientSocket != null)
                clientSocket.Close();
        }
    }

}
