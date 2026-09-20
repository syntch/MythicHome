# 🪄 MythicHome: MagiQuest Home Automation & Adventure System

Transform your living space into an interactive fantasy realm using real **MagiQuest** infrared wands, ESP32 microcontrollers, Home Assistant, and interactive digital props.

MythicHome bridges the physical and digital worlds: flick your wand to trigger room-wide lighting spells, battle 3D animated dragons streamed to your television, and awaken living portraits on framed tablets.

---

## 🌟 Key Features

* **Decoupled Architecture:** Sensor nodes (inputs) detect wand flicks independently and publish standard JSON events over MQTT. Digital and physical props (outputs) subscribe and react autonomously.
* **Flick Magnitude & Spell Classification:** MicroPython decodes the raw 48-bit MagiQuest IR pulse protocol, distinguishing between subtle flicks (**Lightning**) and forceful casts (**Fireball**).
* **Home Assistant Integration:** Seamlessly triggers smart plugs, smart bulbs, sound machines, and complex light transitions with realistic ember flickers and cool-downs.
* **Interactive 3D Boss Battles (Unity + Moonlight):** A complete Unity boss encounter featuring health bars, spell particle VFX, sound effects, and auto-reset, streamed from a gaming PC to any TV via Sunshine/Moonlight.
* **Living Video Portrait Frame:** An always-on Amazon Kindle Fire tablet running a lightweight HTML5/JavaScript web app that seamlessly crossfades between idle and reaction video clips over direct MQTT WebSockets.

---

## 🏗️ System Architecture

```text
                     ┌─────────────────────────────────────────┐
                     │        CENTRAL SERVER (Host PC)         │
                     │  • MQTT Broker (Mosquitto :1883/:1884)  │
                     │  • Home Assistant (Automations/Web)     │
                     │  • Sunshine (Moonlight Game Streaming)  │
                     └────────────────────┬────────────────────┘
                                          │
                      WiFi (MQTT / JSON over WebSockets)
                                          │
         ┌────────────────────────────────┼────────────────────────────────┐
         ▼                                ▼                                ▼
┌─────────────────┐              ┌─────────────────┐              ┌─────────────────┐
│  SENSOR NODES   │              │   SMART PROPS   │              │  DIGITAL PROPS  │
│  ESP32 + TSOP   │              │  Smart Bulbs    │              │  Unity Dragon   │
│  IR Receiver    │              │  Smart Plugs    │              │  Kindle Frame   │
└─────────────────┘              └─────────────────┘              └─────────────────┘
```

---

## 📁 Repository Structure

```text
MythicHome/
├── docs/
│   └── plan.md                         # Architecture overview & phased roadmap
├── home_assistant/
│   ├── fireball_spell_automation_guide.md  # Explosive color transitions & delays
│   ├── home_assistant_spell_automation_guide.md # Transition timing & toggle logic
│   └── lightning_spell_automation_guide.md # Rapid flash & strobe sequences
├── props/
│   ├── magic_picture/
│   │   ├── kindle_fire_setup_guide.md  # Silk browser developer settings & setup
│   │   └── magic_picture_index.html    # HTML5/JS dual-video player with MQTT WS
│   └── unity_scenes/
│       └── Scripts/
│           ├── DragonAudioEvents.cs    # Animation-synchronized sound triggers
│           ├── DragonCombatManager.cs  # Health bar, death animation & scene reset
│           ├── DragonSpellReceiver.cs  # M2Mqtt subscriber for live spell damage
│           ├── FrameLimiter.cs         # Background rendering optimizer
│           ├── RandomScreamBehaviour.cs # Idle animation state machine variation
│           └── UnityMainThreadDispatcher.cs # Thread-safe MQTT event dispatching
└── receivers/
    ├── enclosure_housing_guide.md      # Wooden treasure box assembly instructions
    └── esp32/
        ├── dual_spell_mqtt_publisher.py    # MicroPython dual-spell classifier
        ├── single_spell_mqtt_publisher.py  # Single-topic basic publisher
        ├── ir_receiver_wand_decoder.py     # Standalone terminal IR pulse decoder
        ├── hardware_assembly_guide.md      # Pinouts & wiring schematics
        ├── firmware_flashing_testing_guide.md # Flashing MicroPython & Thonny setup
        └── esp32_treasure_box_setup_checklist.md # Production deployment checklist
```

---

## 🛠️ Hardware & Bill of Materials (BOM)

| Component | Purpose | Recommended Model |
| :--- | :--- | :--- |
| **MagiQuest Wand** | Magic wand transmitter | Any Great Wolf Lodge / MagiQuest wand (38kHz IR) |
| **Microcontroller** | IR signal capture & MQTT publisher | ESP32 DevKit V1 (ESP-WROOM-32) |
| **IR Receiver** | Detect 38kHz modulated signals | TSOP38238 or VS1838B (38kHz) |
| **Enclosure** | Themed prop housing | Small wooden craft treasure box or 3D print |
| **Smart Lighting** | Environmental spell feedback | Zigbee, Z-Wave, or Wi-Fi RGB bulbs (e.g. Philips Hue, Kasa) |
| **Smart Plugs** | Prop/lamp power switching | Any Home Assistant compatible smart plug |
| **Digital Frame** | Living video portrait display | Amazon Kindle Fire (7" or 8") or spare tablet |
| **Host Machine** | Central server & game rendering | Always-on PC running Home Assistant & Mosquitto |

---

## 📡 The MagiQuest Wand IR Protocol

MagiQuest wands transmit unencoded 38kHz infrared bursts when flicked. An internal spring mechanism triggers an onboard IC to emit a **48-bit transmission**:

* **Bits 0–31 (32 bits):** Unique Wand ID (identifies the player).
* **Bits 32–47 (16 bits):** Flick Magnitude (strength of the physical flick).

The ESP32 receiver measures pulse durations via `time_pulse_us()`:
* **Logic `0`:** ~150µs to 400µs mark duration
* **Logic `1`:** ~420µs to 850µs mark duration

### Dual-Spell Classification

```text
Flick Magnitude >= 200  ──►  mythichome/events/cast/fireball   (Heavy Flick)
Flick Magnitude <  200  ──►  mythichome/events/cast/lightning  (Light Flick)
```

**Published MQTT Payload:**
```json
{
  "wand_id": "0x0123ABCD",
  "wand_id_dec": 19114957,
  "magnitude": 284,
  "spell_type": "Fireball (Heavy Flick)",
  "sensor_id": "spell_node",
  "timestamp": 1726850000
}
```

---

## 🚀 Quick-Start Guide

### 1. Set Up the ESP32 Sensor Node
1. Flash MicroPython onto your ESP32 (see [firmware_flashing_testing_guide.md](file:///d:/dev/MythicHome/MythicHome/receivers/esp32/firmware_flashing_testing_guide.md)).
2. Connect your TSOP38238 receiver:
   * **VCC** $\rightarrow$ `3V3`
   * **GND** $\rightarrow$ `GND`
   * **OUT** $\rightarrow$ `GPIO 18`
3. Edit [dual_spell_mqtt_publisher.py](file:///d:/dev/MythicHome/MythicHome/receivers/esp32/dual_spell_mqtt_publisher.py) with your Wi-Fi and Home Assistant MQTT details:
   ```python
   WIFI_SSID = "<YOUR_WIFI_SSID>"
   WIFI_PASS = "<YOUR_WIFI_PASSWORD>"
   MQTT_BROKER = "<YOUR_HOME_ASSISTANT_IP>"
   MQTT_USER = "<YOUR_MQTT_USERNAME>"
   MQTT_PASS = "<YOUR_MQTT_PASSWORD>"
   ```
4. Copy the script as `main.py` onto the ESP32 so it runs automatically on boot.

### 2. Configure Home Assistant
1. Ensure the **Mosquitto MQTT Broker** add-on is installed with port `1883` (TCP) and `1884` (WebSockets) open.
2. Follow the guides in [home_assistant/](file:///d:/dev/MythicHome/MythicHome/home_assistant) to paste the YAML automations into Home Assistant for the Fireball and Lightning effects.

### 3. Deploy the Living Picture Frame
1. Copy [magic_picture_index.html](file:///d:/dev/MythicHome/MythicHome/props/magic_picture/magic_picture_index.html) and your video loops (`serena_idle.mp4`, `serena_wave.mp4`) into your Home Assistant `/config/www/portrait/` folder.
2. Update the broker IP and credentials in `index.html`.
3. Follow the [kindle_fire_setup_guide.md](file:///d:/dev/MythicHome/MythicHome/props/magic_picture/kindle_fire_setup_guide.md) to configure your Kindle Fire to stay awake and open the URL in Silk Browser:
   ```text
   http://<YOUR_HOME_ASSISTANT_IP>:8123/local/portrait/index.html
   ```

### 4. Run the Unity Dragon Encounter
1. Add [DragonSpellReceiver.cs](file:///d:/dev/MythicHome/MythicHome/props/unity_scenes/Scripts/DragonSpellReceiver.cs) and [DragonCombatManager.cs](file:///d:/dev/MythicHome/MythicHome/props/unity_scenes/Scripts/DragonCombatManager.cs) to your Unity scene.
2. Configure your MQTT broker address and credentials in the Unity Inspector.
3. Stream the game to your living room TV using Sunshine and Moonlight!

---

## 🗺️ Project Roadmap

- [x] **Phase 1: Foundation & Smart Plug Controller**
  - Mosquitto MQTT broker setup.
  - MicroPython MagiQuest IR pulse decoder.
  - Basic Home Assistant lamp toggles.
- [x] **Phase 2: Debounce & Physical Enclosure**
  - 3-second cooldown debounce logic.
  - Wooden treasure box sensor housing.
- [x] **Phase 3: Digital Encounters**
  - Unity Dragon combat prototype with live MQTT spell damage.
  - Kindle Fire "Living Portrait" HTML5 client over WebSockets.
  - Sunshine/Moonlight streaming integration.
- [ ] **Phase 4: Advanced Actuators & Multi-Node Quests (Upcoming)**
  - Servo-driven treasure chest lock/unlock mechanism.
  - Addressable WS2812B LED spell beam strips across walls.
  - Multi-step quest state machine tracking player wand IDs and progress in Home Assistant.

---

## 🤝 Contributing

Contributions, issues, and creative prop ideas are welcome! If you build your own custom prop, sensor housing, or Home Assistant sequence:
1. Fork the Project.
2. Create your Feature Branch (`git checkout -b feature/AmazingProp`).
3. Commit your Changes (`git commit -m 'Add new potion bottle prop'`).
4. Push to the Branch (`git push origin feature/AmazingProp`).
5. Open a Pull Request.

---

## 📄 License

Distributed under the **MIT License**. See [LICENSE](LICENSE) for more information.
