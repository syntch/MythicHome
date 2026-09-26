# 🏠 MythicHome: Home Assistant & MQTT Automations

Home Assistant acts as the central brain of MythicHome. It hosts the Mosquitto MQTT broker, manages spell cooldowns, synchronizes ambient lighting effects, and serves local web assets for digital props.

---

## 📡 Mosquitto MQTT Broker Configuration

All communication flows through the **Mosquitto broker** add-on in Home Assistant.

* **Port 1883 (TCP):** Standard MQTT protocol used by ESP32 sensor nodes, MicroPython, and the Unity Game Engine.
* **Port 1884 (WebSockets):** WebSocket listener used by browser-based digital props (such as the Kindle Fire living portrait running Paho MQTT).

> [!NOTE]
> To enable WebSockets in the Home Assistant Mosquitto add-on, ensure port `1884` is exposed in the add-on configuration tab under **Network**.

---

## 🏷️ MQTT Topic Architecture

Sensors publish events into the `mythichome/events/cast` namespace:

| Topic | Payload Trigger | Intended Prop Reaction |
| :--- | :--- | :--- |
| `mythichome/events/cast/lightning` | Wand cast with magnitude < 200 | Rapid white/blue strobe, sudden zap, electric audio crackle |
| `mythichome/events/cast/fireball` | Wand cast with magnitude $\ge$ 200 | Yellow-to-orange charge swell, crimson blast, flickering embers, warm cool-down |
| `mythichome/events/cast` | Any wand cast (generic) | Lamp toggle, basic chest latch release |

---

## 💡 Lighting Automation Principles

Creating convincing magical effects with smart lights requires understanding how Home Assistant processes light transitions:

1. **The Transition Trap:** Setting `transition: 3` sends a command to the bulb and returns *immediately*—it does **not** block the automation.
2. **Explicit Delays:** You must insert explicit `delay` actions after a transition command to let the light finish fading before sending the next color command.
3. **Restoring State:** Automations should restore bulbs to their pre-cast ambient state (e.g. warm white 2800K) so the room feels natural between spells.

---

## 📖 Spell Automation Guides (Wand Triggered)

The guides in this directory contain copy-pasteable YAML automations and visual editor instructions for wand-triggered effects:

* **[Home Assistant Spell Automation Guide](home_assistant_spell_automation_guide.md):** Deep-dive into transition timing, inserting delays, and toggling physical plugs safely.
* **[Fireball Spell Automation Guide](fireball_spell_automation_guide.md):** A 4-stage explosive sequence: 0.8s charge swell, instant 100% crimson flash, lingering ember desk lamp flickers, and 3-second thermal dissipation.
* **[Lightning Spell Automation Guide](lightning_spell_automation_guide.md):** Strobe lighting patterns, electric crackles, and instant transitions.

---

## 📱 Mobile Dashboard: Wizard Lighting (`wizard_lighting/`)

In addition to wand-cast reactions, MythicHome includes a phone-optimized Lovelace dashboard (**"Wizard Lighting"**) designed for a phone or tablet so young wizards can change their room's ambient fantasy mood at any time with a tap.

* **[Wizard Lighting Setup Guide](wizard_lighting/README.md):** Complete overview of the 7 fantasy lighting scenes (*Torchlit Cave*, *Poison Swamp*, *Enchanted Forest*, *Dragon's Caldera*, *Crystal Sanctum*, *Lumos*, and *Nox*) and phone PWA setup instructions.
* **[`wizard_lighting/scripts.yaml`](wizard_lighting/scripts.yaml):** Home Assistant script definitions driving the room's standing lamp and bedside table lamp.
* **[`wizard_lighting/dashboard.yaml`](wizard_lighting/dashboard.yaml):** Lovelace 2-column mobile button grid with custom visual accents and manual brightness sliders.

---

## 🛠️ Smart Lighting & Plugs Bill of Materials

> [!NOTE]
> **Personal Testing & Purchasing Disclaimer:**  
> The specific hardware items and links provided in this document are the components I have personally purchased, built with, and verified working in my home setup. They are provided as known-working references—there may be better, cheaper, or alternative options that suit your setup equally well!

| Component | Category | Purpose | Recommended Model / Specs | Notes & Purchasing Examples |
| :--- | :--- | :--- | :--- | :--- |
| **Smart RGB Bulbs - Bright** | Physical Prop | Standing lamp / room-scale spell & scene lighting effects | Zigbee / Wi-Fi Color Bulbs (Philips Hue, Kasa, Sengled, Govee) | [Bright Smart Color Bulbs](https://www.amazon.com/dp/B0964DN9TV?th=1). Ensure local Home Assistant integration support. |
| **Smart RGB Bulbs - Table** | Physical Prop | Bedside table lamp focal point & spell effects | Zigbee / Wi-Fi Color Bulbs (Philips Hue, Kasa, Sengled, Govee) | [Smart Color Bulbs](https://www.amazon.com/dp/B08TB6VXFL?th=1). Ensure local Home Assistant integration support. |
| **Smart Plugs** | Physical Prop | Switching themed lamps, blacklights, or noise machines | Zigbee / Wi-Fi Smart Plugs (Sonoff, Kasa, TP-Link, ThirdReality) | [Smart Plugs](https://www.amazon.com/dp/B091FXQQMQ?th=1). |


