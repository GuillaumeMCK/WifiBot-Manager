
valeur = range(0,41)

print("-- Valeurs pour avancer -- ")
for v in valeur:
    binary_value = bin(192+v)
    hex_value = hex(192+v)
    print(f"{v},{binary_value},{hex_value}")

print("-- Valeurs pour reculer -- ")
for v in valeur:
    binary_value = bin(128+v)
    hex_value = hex(128+v)
    print(f"{v},{binary_value},{hex_value}")

