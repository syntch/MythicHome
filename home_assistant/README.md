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

## 📖 Automation Guides

The guides in this directory contain copy-pasteable YAML automations and visual editor instructions:

* **[Home Assistant Spell Automation Guide](home_assistant_spell_automation_guide.md):** Deep-dive into transition timing, inserting delays, and toggling physical plugs safely.
* **[Fireball Spell Automation Guide](fireball_spell_automation_guide.md):** A 4-stage explosive sequence: 0.8s charge swell, instant 100% crimson flash, lingering ember desk lamp flickers, and 3-second thermal dissipation.
* **[Lightning Spell Automation Guide](lightning_spell_automation_guide.md):** Strobe lighting patterns, electric crackles, and instant transitions.
* **[Wizard Lighting Mobile Dashboard](wizard_lighting/README.md):** 7 interactive fantasy lighting scenes (Torchlit Cave, Poison Swamp, Enchanted Forest, etc.) and a phone-optimized Lovelace dashboard for young wizards.

