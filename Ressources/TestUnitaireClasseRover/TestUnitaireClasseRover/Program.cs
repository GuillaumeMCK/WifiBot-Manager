using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestUnitaireClasseRover
{
    class Program
    {
        static readonly byte[] avancer = { 0xc7, 0xc7 },
                               reculer = { 0x8f, 0x8f },
                               droite  = { 0xc7, 0x8f },
                               gauche  = { 0x8f, 0xc7 },
                               stop    = { 0xc0, 0xc0 };

        static void Main(string[] args)
        {
            // Creation d'un rover
            var rover = new Rover("127.0.0.1", 15020);
            // Connexion au rover
            rover.Connection();
            // Definition de la commande
            rover.Command = avancer;
            Thread.Sleep(2000);
            rover.Command = reculer;
            Thread.Sleep(3000);
            rover.Command = stop;
            Thread.Sleep(1000);
            rover.Command = gauche;
            Thread.Sleep(2000);
            rover.Command = droite;
            Thread.Sleep(1000);
            rover.Command = stop;
            // Déconnection
            rover.Disconnect();
        }
    }
}
