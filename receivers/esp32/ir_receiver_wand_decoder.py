from machine import Pin, time_pulse_us
import time

# TSOP IR receiver connected to GPIO 18 (active-low signal)
ir_pin = Pin(18, Pin.IN)

print("==================================================")
print("🪄  MagiQuest Wand Decoder Active!")
print("    Flick your wand at the IR sensor...")
print("==================================================\n")

while True:
    # TSOP sensor idles HIGH (1) and pulls LOW (0) when 38kHz IR light is detected
    if ir_pin.value() == 0:
        marks = []
        
        # MagiQuest frames consist of up to 50 pulse bursts (2 header + 32 ID + 16 magnitude)
        for _ in range(50):
            # Measure pulse duration in microseconds while pin stays LOW
            dur = time_pulse_us(ir_pin, 0, 3000)
            if dur < 0:
                break
            marks.append(dur)
            # Short delay between pulses to allow signal space to clear
            time.sleep_us(400)

        if len(marks) >= 48:
            bits = []
            valid = True
            
            # Offset past header start bits if 50 marks were captured
            start_offset = 2 if len(marks) >= 50 else 0
            
            for mark in marks[start_offset : start_offset + 48]:
                if 150 <= mark <= 400:     # Logic '0' (~288µs pulse mark)
                    bits.append(0)
                elif 420 <= mark <= 850:   # Logic '1' (~576µs pulse mark)
                    bits.append(1)
                else:
                    valid = False
                    break
            
            if valid and len(bits) == 48:
                # Calculate 32-bit Wand ID
                wand_id = 0
                for b in bits[0:32]:
                    wand_id = (wand_id << 1) | b
                
                # Calculate 16-bit Magnitude (flick power/flick strength)
                magnitude = 0
                for b in bits[32:48]:
                    magnitude = (magnitude << 1) | b
                
                print("✨ SPELL CAST DETECTED!")
                print(f"   ► Wand ID (Hex): 0x{wand_id:08X}")
                print(f"   ► Wand ID (Dec): {wand_id}")
                print(f"   ► Magnitude:     {magnitude}\n")
                
                # Debounce delay before reading the next flick
                time.sleep_ms(400)