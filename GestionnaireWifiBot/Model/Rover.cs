using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace GestionnaireWifiBot.Model
{
    class Rover
    {
        //-------------
        // Attributs
        //-------------
        private int port_du_serveur;                // Numéro de port TCP utilisé
        private string adresse_du_server;           // Nom du serveur
        private Byte[] commande;                    // Tableau de 2 octets contenant les valeurs à envoyer
        private bool connected;                     // Statut de la connexion

        TcpClient clientSocket;                     // Instanciation d'un client TCP qui va permettre de se connecter au serveur du robot
        NetworkStream networkStream;                // Instanciation d'un flux réseau rattaché au socket

        public bool ConnectionState { get { return connected; } } // Getter du statut de la connexion au rover
        
        //--------------
        // Constructeur
        //--------------
        public Rover(string adresse, int port_TCP)
        {
            adresse_du_server = adresse;
            port_du_serveur = port_TCP;
            commande = new Byte[2] { 0x00, 0x00 };
            clientSocket = new TcpClient();
            connected = false;
        }

        public void Connection()
        {
            if (!connected)
            {
                try
                {
                    clientSocket.Connect(adresse_du_server, port_du_serveur);   // Connexion au serveur avec l'adresse IP et le Port spécifié.
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
            commande = bytes;

            if (connected)
            {
                try
                {
                    networkStream.Write(bytes, 0, 2); // On écrit dans le flux la commande;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Deconnexion();
                }
            }
        }

        //----------------------------------------------------
        // Méthode de déconnexion du rover
        //----------------------------------------------------
        public void Deconnexion()
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
