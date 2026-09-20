# Mythic Home Adventure System: Architecture & Master Plan

This document outlines the hardware, software, and phased project roadmap for transforming your home into an interactive Mythic Home adventure using your existing desktop PC as the central game server.

## 1. System Architecture Overview

To keep the system modular and flexible, **Sensing Nodes (Inputs)** are strictly decoupled from **Props & Actuators (Outputs)**. All communication flows through a central server.

```text
                      ┌─────────────────────────────────────────┐
                      │        CENTRAL SERVER (Desktop PC)      │
                      │  • Quest Engine & Database (Python/Node)│
                      │  • MQTT Broker (Mosquitto)              │
                      │  • Sunshine/Moonlight (Game Streaming)  │
                      │  • Home Assistant (Automations/Web)     │
                      └────────────────────┬────────────────────┘
                                           │
                        WiFi (MQTT / JSON over WebSockets)
                                           │
         ┌─────────────────────────────────┼─────────────────────────────────┐
         ▼                                 ▼                                 ▼
┌─────────────────┐               ┌─────────────────┐               ┌─────────────────┐
│ SENSOR NODES    │               │ SMART PROPS     │               │ DIGITAL PROPS   │
│ • ESP32 + IR    │               │ • Smart Plugs   │               │ • Unity Scenes  │
│ • Wall/Desk Mount│              │ • Smart Bulbs   │               │ • Kindle Tablets│
└─────────────────┘               └─────────────────┘               └─────────────────┘
```

### Core Architecture Principles

1. **Input/Output Decoupling:** Sensor nodes only care about detecting wand flicks and identifying the wand. They notify the central server, which decides what action to take.
2. **Communication Standard (MQTT):** Sensors publish JSON events over Wi-Fi. Digital props (like Unity and HTML Web pages) subscribe to these topics using standard MQTT or WebSockets to trigger reactions.
3. **Versatile Actuation:** The server commands physical props via Home Assistant and digital props via real-time network messages.

## 2. Central Game Server Layer

Your existing always-on gaming desktop serves as the "brain" for the entire system.

* **MQTT Broker (Mosquitto):** Handles real-time messaging. Exposes Port `1883` for standard clients (ESP32, Unity) and Port `1884` for WebSocket clients (Web Browsers).
* **Home Assistant:** Listens to MQTT events, manages spell cooldowns, sequences smart lights/plugs, and hosts local web apps (like the living portrait) via the `www` folder.
* **Sunshine/Moonlight:** Streams high-end Unity environments from the host PC to client screens (TVs, projectors) around the house.

## 3. Interactive Virtual Encounters (Unity + Moonlight)

Using Unity, you can create immersive "boss battles" or interactive scenes streamed to displays. 

### The Dragon Battle Prototype
* **Visuals:** A 3D dragon rendered in Unity with idle, scream, hit, and death animations.
* **Network:** Uses `M2Mqtt` to subscribe directly to `mythichome/events/cast/#`.
* **Combat System:**
  * C# Scripts (`DragonSpellReceiver` & `DragonCombatManager`) handle incoming MQTT spell data.
  * Spells deal damage to the dragon, updating a UI health bar and triggering particle/sound effects (Lightning/Fireball).
  * Upon reaching 0 health, the dragon plays a death animation and displays a victory screen.
  * Casting a spell after victory instantly reloads the scene for the next player.
* **Shutdown:** Sunshine "Undo Commands" automatically kill the Unity `.exe` process on the host PC when the Moonlight stream disconnects.

## 4. Digital Prop Concept: Living Video Picture Frame

An old Kindle Fire tablet acts as an interactive "living portrait" directly responding to wand casts without needing dedicated apps.

### Architecture
1. **Hosting:** Home Assistant's local web server (`/config/www/portrait/index.html`) serves a custom HTML page to the tablet.
2. **Client:** The Fire tablet uses the native Silk Browser to load the page and stay awake via Developer Options.
3. **Behavior:**
   * Two stacked HTML5 `<video>` elements (an idle loop and a reaction clip).
   * Paho MQTT (WebSockets) connects the browser directly to the Mosquitto broker on port `1884`.
   * A wand cast triggers a seamless JavaScript crossfade to the reaction video, then smoothly returns to the idle loop.

## 5. Master Actuator & Physical Prop Ecosystem

### A. Environmental & Atmospheric Effects
* **Smart RGB Bulbs & Plugs:** Room ambient illumination, color changing, and power toggles.
* **Addressable LED Strips (WS2812B):** Moving light animations for spell beams.

### B. Audio & Haptic Sensory Feedback
* **Wi-Fi Speakers:** Dynamic sound effects, spell impact audio, ambient room loops.
* **Unity Audio Sources:** Tied to animation events (like the exact frame a dragon opens its mouth to roar).

## 6. Phased Implementation Roadmap

### Phase 1: Smart Plug Light Controller (COMPLETE)
* [x] Install Mosquitto MQTT broker on Home Assistant.
* [x] Flash ESP32 with MicroPython MagiQuest IR decoder & MQTT publisher.
* [x] Create Home Assistant MQTT Automation to toggle lamp on wand flick.

### Phase 2: Cooldown & Enclosure Housing (COMPLETE)
* [x] Implement 3-second de-bounce / cooldown timer in MicroPython script.
* [x] Install ESP32 & IR Receiver into wooden craft treasure box.

### Phase 3: Digital Encounter Prototyping (COMPLETE)
* [x] Build Unity Dragon combat scene with health, VFX, and animations.
* [x] Connect Unity directly to MQTT for live spell damage.
* [x] Set up Kindle Fire "Living Portrait" HTML interface using WebSockets.
* [x] Configure local network routing for Kindle Fire access to Home Assistant `www` assets.

### Phase 4: Multi-Node Quests & Advanced Actuators (UPCOMING)
* [ ] Build Servo-driven Treasure Chest latch.
* [ ] Integrate addressable WS2812B spell strips on walls.
* [ ] Create multi-step sequence state machine in Home Assistant.