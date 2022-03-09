import socket
import time
import os

class Rover:

    def init(self):
        self.client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

    # Marche avant vitesse maximum avec contrôle de vitesse (40)
    def avancer(rep):
        trame = b"\xd4\xd4"
        for i in range(rep):
            client.send(trame)
            print("[+] Avancer -> ", trame)
            time.sleep(0.25)

    # Marche arriere vitesse maximum avec contrôle de vitesse (40)
    def reculer(rep):
        trame = b"\x94\x94"
        for i in range(rep):
            client.send(trame)
            print("[+] Reculer -> ", trame)
            time.sleep(0.25)
    # Left = 20 Right = -20
    def droite(rep):
        trame = b"\xD4\x94"
        for i in range(rep):
            client.send(trame)
            print("[+] droite -> ", trame)
            time.sleep(0.25)

    # Left = -20 Right = 20
    def gauche(rep):
        trame = b"\x94\xD4"
        for i in range(rep):
            client.send(trame)
            print("[+] gauche -> ", trame)
            time.sleep(0.25)

if __name__ == '__main__':
    print('----- POC WifiBot -----\n')
    ip = "127.0.0.1" # Adresse IP du serveur sur lequel s’exécute le programme wifibotLabSim
    port = 15020 # Port sur lequel le serveur écoute 
    rover = Rover
    rover.client.connect((ip, port))
    print("[√] Connexion au robot réussie.")
    rover.avancer(5)
    Sleep(1000)
    rover.client.close()
    print("[!] Fin du script.")
    exit(0)