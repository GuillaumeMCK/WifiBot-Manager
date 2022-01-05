
"""
                               ┌───┬───┬───┬───┬───┬───┬───┬───┐
                          Bit >│ 7 │ 6 │ 5 │ 4 │ 3 │ 2 │ 1 │ 0 │ 
                      ┌────────┼───┼───┼───┼───┼───┼───┼───┼───┤
                      │ GAUCHE │ 1 │   │   │   │   │   │   │   │
                      ├────────┼───┼───┼───┼───┼───┼───┼───┼───┤
                      │ DROITE │ 1 │   │   │   │   │   │   │   │
                      └────────┴─┬─┴─┬─┴┬──┴───┴───┴───┴───┴──┬┘
                                 │   │  └───────────────┬─────┘ 
                                 V   │                  │
       1 : Controle de vitesse ON    │                  V
       0 : Controle de vitesse OFF   │     0-60 sans contrôle de vitesse
                                     V     0-40 avec contrôle de vitesse
                              0 : Avant 
                              1 : Arriere
"""

def GetValues() -> None:
    vitesse_range = range(41)
    print("── Valeurs pour avancer ──┬── Valeurs pour reculer ──")
    for v in vitesse_range:
        print(f"{v} : {bin(192+v)},{hex(192+v)}", end='')
        print(f"\t  │\t", end='')
        print(f"{v} : {bin(128+v)},{hex(128+v)}")

if __name__ == "__main__":
    GetValues()