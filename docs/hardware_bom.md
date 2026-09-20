# 🛠️ MythicHome: Hardware & Bill of Materials (BOM)

This document provides a comprehensive list of hardware, electronic components, tools, and materials needed to build the MythicHome interactive environment.

---

## 📋 Component Overview & Shopping List

| Component | Category | Purpose | Recommended Model / Specs | Notes & Purchasing Options |
| :--- | :--- | :--- | :--- | :--- |
| **MagiQuest Wand** | Controller | Infrared spell transmitter | Any authentic MagiQuest wand (Great Wolf Lodge or Creative Kingdoms) | Purchase on-site at Great Wolf Lodge or secondhand via [eBay](https://www.ebay.com) / [Mercari](https://www.mercari.com). Standard 38kHz IR. |
| **ESP32 Dev Board** | Sensor Node | MicroPython microcontroller for IR decoding & MQTT | ESP32-WROOM-32 DevKit V1 (30-pin or 38-pin) | [Amazon ESP32 Dev Boards](https://www.amazon.com/s?k=esp32+devkit+v1) (buy a multi-pack for multiple rooms). Micro-USB or USB-C. |
| **IR Receiver Module** | Sensor Node | 38kHz demodulating infrared receiver | **TSOP38238** or **VS1838B** (38kHz carrier frequency) | [Amazon TSOP38238](https://www.amazon.com/s?k=tsop38238) or [Adafruit TSOP38238](https://www.adafruit.com/product/157). Avoid non-demodulated IR photodiodes. |
| **Jumper Wires & Breadboard** | Prototyping | Connecting sensors to ESP32 for bench testing | Female-to-Female & Male-to-Female Dupont wires (20cm) | [Amazon Dupont Jumper Wire Kit](https://www.amazon.com/s?k=dupont+jumper+wires). |
| **USB Power Adapter & Cable** | Power Supply | Powering ESP32 sensor nodes 24/7 | 5V 1A–2A USB Wall Adapter + Micro-USB / USB-C cable | Any standard phone charger block. |
| **Sensor Enclosure** | Themed Prop | Hiding electronics in a fantasy-themed housing | Small unfinished wooden craft treasure chest (approx. 4"x3"x3") | [Amazon Wooden Craft Treasure Boxes](https://www.amazon.com/s?k=unfinished+wood+treasure+chest) or craft stores like Michaels / Hobby Lobby. |
| **Living Portrait Display** | Digital Prop | Always-on interactive video picture frame | Amazon Fire 7" or 8" Tablet (7th Gen or newer) | [Amazon Fire Tablet](https://www.amazon.com/s?k=fire+tablet) (older used/refurbished models work great). Any tablet with a modern browser will work. |
| **Tablet Wall Mount / Stand** | Digital Prop | Mounting the portrait on a wall or bookshelf | Acrylic picture frame stand or low-profile wall mount | [Amazon Tablet Stand](https://www.amazon.com/s?k=tablet+stand+holder) or wooden picture easel. |
| **Smart RGB Bulbs** | Physical Prop | Room-scale spell lighting effects (Fireball/Lightning) | Zigbee / Wi-Fi Color Bulbs (Philips Hue, Kasa, Sengled, Govee) | [Amazon Smart Color Bulbs](https://www.amazon.com/s?k=kasa+smart+bulb+color). Ensure local Home Assistant integration support. |
| **Smart Plugs** | Physical Prop | Switching themed lamps, blacklights, or noise machines | Zigbee / Wi-Fi Smart Plugs (Sonoff, Kasa, TP-Link, ThirdReality) | [Amazon Smart Plugs](https://www.amazon.com/s?k=smart+plugs+home+assistant). |
| **Central Host Machine** | Server | Runs Home Assistant, Mosquitto, and Unity Boss fight | Desktop PC / Home Server (Windows / Linux) with dedicated GPU | Existing gaming PC or dedicated mini-PC (e.g. Intel N100) running Home Assistant OS / Container. |
| **Display Client** | Digital Prop | Streaming Unity 3D boss encounters to big screen | TV with Moonlight client (Fire TV stick, Apple TV, Nvidia Shield, or PC) | [Amazon Fire TV Stick 4K](https://www.amazon.com/s?k=fire+tv+stick+4k) or any device running the Moonlight app. |

---

## 🔮 Upcoming Phase 4 Actuators (Roadmap)

These components are planned for Phase 4 expansions:

| Component | Category | Purpose | Recommended Model / Specs | Notes & Links |
| :--- | :--- | :--- | :--- | :--- |
| **Micro Servo Motor** | Physical Prop | Actuating a physical treasure chest latch/lid | SG90 9g Micro Servo or MG90S (metal gear) | [Amazon SG90 Servos](https://www.amazon.com/s?k=sg90+servo). Driven directly from ESP32 PWM pin. |
| **Addressable LED Strip** | Physical Prop | Animated spell beams traveling along walls/molding | WS2812B 5V Addressable RGB LED Strip (60 LEDs/m, IP30 or IP65) | [Amazon WS2812B LED Strip](https://www.amazon.com/s?k=ws2812b+led+strip+5v). Controlled via WLED or ESP32 FastLED/Neopixel. |
| **5V High-Current Power Supply** | Power Supply | Dedicated power injection for LED strips | 5V 10A–20A DC Switching Power Supply | [Amazon 5V Power Supply](https://www.amazon.com/s?k=5v+power+supply+10a) (required if running long LED runs). |

---

## 💡 Hardware Selection Tips

### 1. Infrared Receivers: TSOP38238 vs. Raw Photodiodes
* **Always use an integrated IR receiver IC** like the **TSOP38238**, **VS1838B**, or **TSOP4838**.
* These chips contain an internal photodiode, preamplifier, automatic gain control (AGC), and a bandpass filter tuned specifically to **38kHz**.
* *Do not use raw 2-pin IR photodiodes or phototransistors*—they lack carrier demodulation and will be blinded by ambient room light and sunlight.

### 2. ESP32 Pin Selection
* The TSOP receiver data output is active-low (idles at 3.3V, pulls to 0V when 38kHz IR is detected).
* Use GPIO pins that support input and interrupts and do not have boot-strapping conflicts (recommended: **GPIO 18**, **GPIO 19**, **GPIO 23**, or **GPIO 25**).

### 3. Smart Lighting Responsiveness
* For explosive spells like Fireball and Lightning, lighting latency matters.
* **Zigbee** and **Local Wi-Fi** (e.g. Kasa local or ESPHome) respond in under 50ms, enabling instant flashes and flickers.
* Cloud-dependent bulbs (Tuya/Smart Life without local control) may introduce noticeable delays.
