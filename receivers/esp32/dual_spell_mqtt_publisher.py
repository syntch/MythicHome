from machine import Pin, time_pulse_us
import time
import network
import usocket as socket
import json

# ==============================================================================
# CONFIGURATION - UPDATE THESE VALUES FOR YOUR HOME NETWORK
# ==============================================================================
WIFI_SSID = "<YOUR_WIFI_SSID>"                # Replace with your 2.4GHz Wi-Fi SSID
WIFI_PASS = "<YOUR_WIFI_PASSWORD>"            # Replace with your Wi-Fi Password
MQTT_BROKER = "<YOUR_HOME_ASSISTANT_IP>"      # Replace with your Home Assistant's IP address
MQTT_PORT = 1883                              # Default MQTT Port
MQTT_USER = "<YOUR_MQTT_USERNAME>"            # Home Assistant MQTT Username
MQTT_PASS = "<YOUR_MQTT_PASSWORD>"            # Home Assistant MQTT Password

# Topic configuration
TOPIC_LIGHTNING = "mythichome/events/cast/lightning"
TOPIC_FIREBALL  = "mythichome/events/cast/fireball"

# Magnitude threshold: flicks equal to or above this value trigger Fireball,
# while lighter flicks trigger Lightning. Adjust based on testing!
MAGNITUDE_THRESHOLD = 200

SENSOR_ID = "spell_node"             # Identifier for this receiver location
CAST_COOLDOWN_SEC = 3               # Minimum seconds to wait between registered spell casts

# IR Pin configuration (TSOP receiver connected to GPIO 18)
IR_PIN_NUM = 18

class SimpleMQTTClient:
    """
    Minimal standalone MQTT v3.1.1 client for MicroPython.
    Requires no external libraries!
    """
    def __init__(self, client_id, server, port=1883, user=None, password=None):
        self.client_id = client_id
        self.server = server
        self.port = port
        self.user = user
        self.password = password
        self.sock = None

    def connect(self):
        print(f"Connecting to MQTT Broker at {self.server}:{self.port}...")
        self.sock = socket.socket()
        addr = socket.getaddrinfo(self.server, self.port)[0][-1]
        self.sock.connect(addr)
        
        # Build MQTT Connect Packet (v3.1.1)
        clean_session = 0x02
        connect_flags = clean_session
        if self.user:
            connect_flags |= 0x80
        if self.password:
            connect_flags |= 0x40

        keep_alive = 60
        variable_header = b'\x00\x04MQTT\x04' + bytes([connect_flags]) + keep_alive.to_bytes(2, 'big')
        
        id_bytes = self.client_id.encode('utf-8')
        payload = len(id_bytes).to_bytes(2, 'big') + id_bytes

        if self.user:
            user_bytes = self.user.encode('utf-8')
            payload += len(user_bytes).to_bytes(2, 'big') + user_bytes

        if self.password:
            pwd_bytes = self.password.encode('utf-8')
            payload += len(pwd_bytes).to_bytes(2, 'big') + pwd_bytes

        remaining_len = len(variable_header) + len(payload)
        
        len_bytes = bytearray()
        l = remaining_len
        while True:
            digit = l % 128
            l //= 128
            if l > 0:
                digit |= 0x80
            len_bytes.append(digit)
            if l == 0:
                break

        packet = b'\x10' + len_bytes + variable_header + payload
        self.sock.send(packet)
        response = self.sock.recv(4)
        if response and response[0] == 0x20 and response[3] == 0x00:
            print("Connected to MQTT Broker!")
            return True
        else:
            code = response[3] if response else 'No response'
            print(f"MQTT Connection failed (Return code: {code})")
            return False

    def publish(self, topic, msg):
        if not self.sock:
            return False
        
        topic_bytes = topic.encode('utf-8')
        msg_bytes = msg.encode('utf-8') if isinstance(msg, str) else msg
        
        variable_header = len(topic_bytes).to_bytes(2, 'big') + topic_bytes
        payload = msg_bytes
        remaining_len = len(variable_header) + len(payload)
        
        # Build Remaining Length bytes (supports lengths > 127)
        len_bytes = bytearray()
        l = remaining_len
        while True:
            digit = l % 128
            l //= 128
            if l > 0:
                digit |= 0x80
            len_bytes.append(digit)
            if l == 0:
                break
                
        packet = b'\x30' + len_bytes + variable_header + payload
        try:
            self.sock.send(packet)
            return True
        except Exception as e:
            print(f"Failed to publish MQTT message: {e}")
            return False

def connect_wifi():
    wlan = network.WLAN(network.STA_IF)
    wlan.active(True)
    if not wlan.isconnected():
        print(f"Connecting to Wi-Fi: {WIFI_SSID}...")
        wlan.connect(WIFI_SSID, WIFI_PASS)
        timeout = 20
        while not wlan.isconnected() and timeout > 0:
            time.sleep(0.5)
            timeout -= 1
            print(".", end="")
        print("")
        
    if wlan.isconnected():
        print(f"Wi-Fi Connected! IP Address: {wlan.ifconfig()[0]}")
        return True
    else:
        print("Wi-Fi Connection Failed. Check SSID/Password.")
        return False

ir_pin = Pin(IR_PIN_NUM, Pin.IN)

print("==================================================")
print("🪄  MagiQuest Dual-Spell Sensor Active!")
print("==================================================")

# Connect to Network & Broker
if connect_wifi():
    client_id = f"esp32_magiquest_{SENSOR_ID}"
    mqtt = SimpleMQTTClient(client_id, MQTT_BROKER, MQTT_PORT, MQTT_USER, MQTT_PASS)
    mqtt_connected = mqtt.connect()
else:
    mqtt_connected = False

print("\nFlick your wand at the IR sensor...\n")

last_cast_time = 0

while True:
    # TSOP sensor pulls LOW (0) on active IR pulse detection
    if ir_pin.value() == 0:
        marks = []
        
        # Capture up to 50 pulse widths
        for _ in range(50):
            dur = time_pulse_us(ir_pin, 0, 3000)
            if dur < 0:
                break
            marks.append(dur)
            time.sleep_us(400)

        if len(marks) >= 48:
            bits = []
            valid = True
            
            start_offset = 2 if len(marks) >= 50 else 0
            
            for mark in marks[start_offset : start_offset + 48]:
                if 150 <= mark <= 400:       # Logic '0'
                    bits.append(0)
                elif 420 <= mark <= 850:     # Logic '1'
                    bits.append(1)
                else:
                    valid = False
                    break
            
            if valid and len(bits) == 48:
                now = time.time()
                if (now - last_cast_time) < CAST_COOLDOWN_SEC:
                    # Ignore rapid double-bursts within the cooldown window
                    time.sleep_ms(300)
                    continue

                last_cast_time = now

                wand_id = 0
                for b in bits[0:32]:
                    wand_id = (wand_id << 1) | b
                
                magnitude = 0
                for b in bits[32:48]:
                    magnitude = (magnitude << 1) | b
                
                wand_hex = f"0x{wand_id:08X}"
                
                # Determine spell type based on flick strength
                if magnitude >= MAGNITUDE_THRESHOLD:
                    target_topic = TOPIC_FIREBALL
                    spell_type = "Fireball (Heavy Flick)"
                else:
                    target_topic = TOPIC_LIGHTNING
                    spell_type = "Lightning (Light Flick)"

                print(f"✨ SPELL CAST DETECTED: {spell_type}")
                print(f"   ► Wand ID: {wand_hex} ({wand_id})")
                print(f"   ► Magnitude: {magnitude}")
                
                # Construct JSON event payload
                payload = {
                    "wand_id": wand_hex,
                    "wand_id_dec": wand_id,
                    "magnitude": magnitude,
                    "spell_type": spell_type,
                    "sensor_id": SENSOR_ID,
                    "timestamp": time.time()
                }
                
                json_payload = json.dumps(payload)
                
                # Publish to MQTT Broker
                if mqtt_connected:
                    success = mqtt.publish(target_topic, json_payload)
                    if success:
                        print(f"   📡 Published to MQTT: {target_topic}")
                    else:
                        print("   ⚠️ MQTT Publish failed. Attempting reconnect...")
                        mqtt_connected = mqtt.connect()
                else:
                    print("   ⚠️ Not connected to MQTT Broker (Offline Mode)")
                    
                print("")
                time.sleep_ms(400)  # Debounce delay