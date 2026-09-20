# 🎭 MythicHome: Physical & Digital Props

Props in MythicHome are divided into **Physical Smart Props** (smart lights, smart plugs, mechanical actuators) and **Digital Props** (streaming 3D game encounters and living video portraits).

All props subscribe directly to MQTT topics or are orchestrated by Home Assistant automations.

---

## 🖼️ Digital Prop: Living Video Portrait Frame (`magic_picture/`)

An old Amazon Kindle Fire tablet mounted on a wall or stand acts as an interactive magical painting that reacts to wand flicks.

![Living Video Portrait Prototype](../receivers/magic_picture_prototype.jpg)  
*Figure: The Kindle Fire living portrait prototype on a display shelf with the disguised treasure chest receiver.*

* **Technology:** Vanilla HTML5, CSS crossfades, and Paho MQTT over WebSockets (`ws://<YOUR_HOME_ASSISTANT_IP>:1884`).
* **Display Hardware:** Amazon Fire 7" or 8" tablet using the native **Silk Browser**.
* **Behavior:** Loops an idle video (`serena_idle.mp4`). When a wand event (`/fireball` or `/wave`) is received over WebSockets, it seamlessly crossfades to a reaction video (`serena_wave.mp4`) and returns smoothly to idle.
* **Guides & Files:**
  * **[Kindle Fire Setup Guide](magic_picture/kindle_fire_setup_guide.md):** Developer Options configuration, keeping the screen awake indefinitely, and Home Assistant local web server paths.
  * **[magic_picture_index.html](magic_picture/magic_picture_index.html):** The standalone HTML/JavaScript player for `/config/www/portrait/`.

---

## 🐉 Digital Prop: 3D Boss Battles via Moonlight (`unity_scenes/`)

Interactive real-time 3D boss encounters streamed directly from your gaming desktop to any television, projector, or streaming device.

* **Technology:** Unity 3D Engine + `M2Mqtt` + **Sunshine** (host streamer) + **Moonlight** (client receiver).
* **Display Hardware:** Any TV or projector connected to an Apple TV, Fire TV Stick 4K, Nvidia Shield, or mini PC running the Moonlight client.
* **Gameplay Mechanics:**
  * Subscribes to `mythichome/events/cast/#` directly over TCP port 1883.
  * Each registered spell hit applies damage to the boss, updates the on-screen health bar, and triggers synchronized particle VFX and audio roars.
  * When health drops to 0, the boss plays a death animation and displays a victory screen.
  * Any subsequent wand cast instantly reloads the scene for the next adventurer.
  * Sunshine "Undo Commands" automatically terminate the Unity process when the Moonlight stream disconnects.
* **C# Scripts in [`unity_scenes/Scripts/`](unity_scenes/Scripts/):**
  * **`DragonCombatManager.cs`:** Health state, UI slider updates, death logic, and scene resets.
  * **`DragonSpellReceiver.cs`:** `M2Mqtt` client handling connection, topic subscriptions, and spell routing.
  * **`DragonAudioEvents.cs`:** Animation-synchronized sound triggers (roars, footfalls).
  * **`UnityMainThreadDispatcher.cs`:** Thread-safe bridge routing background MQTT events to Unity's main thread.
  * **`RandomScreamBehaviour.cs`:** State machine variation to randomize idle behaviors.
  * **`FrameLimiter.cs`:** Optimizes host GPU/CPU utilization during standby.

---

## ⚡ Physical Smart Props

* **Smart RGB Bulbs:** Environmental room lighting that pulses, Strobes (Lightning), or swells and flickers (Fireball). Controlled via Home Assistant automations.
* **Smart Plugs:** Toggling ambient items such as themed lamps, UV blacklights, sound machines, or fog machines.

---

## 🔮 Phase 4 Actuator Roadmap

* **Servo-Driven Treasure Chest Latch:** An SG90 micro servo integrated into a wooden chest to physically unlock and pop the lid when a correct spell sequence is cast.
* **Addressable WS2812B Spell Strips:** Wall-mounted LED strips running WLED or ESP32 firmware to simulate beams of magical energy traveling from the wand sensor to props.

