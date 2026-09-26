# 🛠️ MythicHome: Hardware & Bill of Materials (BOM)

This document provides a comprehensive list of hardware, electronic components, tools, and materials needed to build the MythicHome interactive environment.
This is in process of being split into the appropriate guides.

> [!NOTE]
> **Personal Testing & Purchasing Disclaimer:**  
> The specific hardware items and links provided in this document are the components I have personally purchased, built with, and verified working in my home setup. They are provided as known-working references—there may be better, cheaper, or alternative options that suit your setup equally well!

---

## 📋 Component Overview & Shopping List

| Component | Category | Purpose | Recommended Model / Specs | Notes & Purchasing Examples |
| :--- | :--- | :--- | :--- | :--- |
| **MagiQuest Wand** | Controller | Infrared spell transmitter | Any authentic MagiQuest wand (Great Wolf Lodge or Creative Kingdoms) | Purchase on-site at Great Wolf Lodge. Standard 38kHz IR. |
| **Living Portrait Display** | Digital Prop | Always-on interactive video picture frame | Amazon Fire 7" or 8" Tablet (7th Gen or newer) | [Amazon Fire Tablet](https://www.amazon.com/s?k=fire+tablet) (I used an older model which works great). Any tablet with a modern browser will work. |
| **Tablet Wall Mount / Stand** | Digital Prop | Mounting the portrait on a wall or bookshelf | Acrylic picture frame stand or low-profile wall mount | [Amazon Tablet Stand](https://www.amazon.com/s?k=tablet+stand+holder) or wooden picture easel. I have not personally tried this yet. |
| **Smart RGB Bulbs - Bright** | Physical Prop | Standing lamp / room scale spell lighting effects (Fireball/Lightning) | Zigbee / Wi-Fi Color Bulbs (Philips Hue, Kasa, Sengled, Govee) | [Bright Smart Color Bulbs](https://www.amazon.com/dp/B0964DN9TV?th=1). Ensure local Home Assistant integration support. |
| **Smart RGB Bulbs - Table** | Physical Prop | Table lamp spell lighting effects (Fireball/Lightning) | Zigbee / Wi-Fi Color Bulbs (Philips Hue, Kasa, Sengled, Govee) | [Smart Color Bulbs](https://www.amazon.com/dp/B08TB6VXFL?th=1). Ensure local Home Assistant integration support. |
| **Smart Plugs** | Physical Prop | Switching themed lamps, blacklights, or noise machines | Zigbee / Wi-Fi Smart Plugs (Sonoff, Kasa, TP-Link, ThirdReality) | [Smart Plugs](https://www.amazon.com/dp/B091FXQQMQ?th=1). |
| **Central Host Machine** | Server | Runs Home Assistant, Mosquitto, and Unity Boss fight | Desktop PC / Home Server (Windows / Linux) with dedicated GPU | Existing gaming PC or dedicated mini-PC (e.g. Intel N100) running Home Assistant OS / Container. |
| **Display Client** | Digital Prop | Streaming Unity 3D boss encounters to big screen | TV with Moonlight client (Fire TV stick, Apple TV, Nvidia Shield, PC, etc.) | [onn. 4K Streaming Device with Google TV](https://www.amazon.com/dp/B0GRCYGS64) or any device running the Moonlight app. |
| **Projector** | Digital Prop | Streaming Unity 3D boss encounters to big screen | Projector compatible with the streaming device | The projector I purchased I don't recommend. Look for one that you can turn on remotely via Home Assistant or smart plug. |

---

## 🔮 Upcoming Phase 4 Actuators (Roadmap)

These components are planned for Phase 4 expansions:

| Component | Category | Purpose | Recommended Model / Specs | Notes & Links |
| :--- | :--- | :--- | :--- | :--- |
| **Micro Servo Motor** | Physical Prop | Actuating a physical treasure chest latch/lid | SG90 9g Micro Servo or MG90S (metal gear) | [Amazon SG90 Servos](https://www.amazon.com/s?k=sg90+servo). Driven directly from ESP32 PWM pin. |
| **Addressable LED Strip** | Physical Prop | Animated spell beams traveling along walls/molding | WS2812B 5V Addressable RGB LED Strip (60 LEDs/m, IP30 or IP65) | [Amazon WS2812B LED Strip](https://www.amazon.com/s?k=ws2812b+led+strip+5v). Controlled via WLED or ESP32 FastLED/Neopixel. |

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

