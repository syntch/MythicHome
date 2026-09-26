# 🐉 Cavern Dragon Boss Battle: Unity & Phone Spellbook Setup Guide

This guide walks through creating the **Cavern Dragon** tactical boss fight scene inside your existing Unity project and building two separate `.exe` files (`MeadowDragon.exe` and `CavernDragon.exe`) that share assets.

---

## 1. Architecture Overview: Phone Spellbook + Wand Cast

Unlike the **Meadow Dragon** scene (which decides between Fireball and Lightning purely based on how fast the wand is flicked), the **Cavern Dragon** scene turns your son's phone into an active **Spellbook**:

1. **Tap on Phone ("Battle Spellbook" tab):**
   - Publishes `"fireball"`, `"lightning"`, `"ice_spear"`, or `"shield"` to `mythichome/player/spell_select` (`retain: true`).
   - Simultaneously shifts the bedroom bedside lamp to the color of the equipped element!
   - Unity immediately updates the TV UI (*"Equipped: ❄️ ICE SPEAR"*).
2. **Flick MagiQuest Wand:**
   - The ESP32 sensor node publishes to `mythichome/events/cast/#`.
   - Unity fires whichever spell is currently loaded in the wand!

---

## 2. Step-by-Step Unity Scene Setup

### Step 1: Import the Cavern Battle Scripts
Copy the new scripts from [`Scripts/CavernBattle/`](Scripts/CavernBattle/) into your Unity project's `Assets/Scripts/` folder:
* `CavernCombatManager.cs` $\rightarrow$ `Assets/Scripts/CavernBattle/CavernCombatManager.cs`
* `CavernSpellReceiver.cs` $\rightarrow$ `Assets/Scripts/CavernBattle/CavernSpellReceiver.cs`
* `MultiSceneBuilder.cs` $\rightarrow$ `Assets/Scripts/CavernBattle/Editor/MultiSceneBuilder.cs` *(Must be inside a folder named `Editor`)*

### Step 2: Duplicate Your Existing Scene
1. In Unity's **Project** window, click on your existing Meadow Dragon `.unity` scene file.
2. Press **`Ctrl + D`** (Duplicate) and rename the copy to **`CavernScene`**.
3. Double-click **`CavernScene`** to open it, and swap the environment/skybox to your cavern environment while keeping your configured Dragon, Camera, Canvas, and `UnityMainThreadDispatcher`!

### Step 3: Swap the Combat & MQTT Scripts
On your scene's combat manager GameObject:
1. Remove (or disable) `DragonCombatManager` and `DragonSpellReceiver`.
2. Add **`CavernCombatManager`** and **`CavernSpellReceiver`**.
3. In **`CavernSpellReceiver`**:
   * Set **Broker Address**, **Mqtt User**, and **Mqtt Pass** to match your Home Assistant broker.
   * Drag your **`CavernCombatManager`** into the `Combat Manager` slot.

### Step 4: Wire the UI & Visual Elements in Inspector
On your **Canvas** and **Dragon**, create and link the following slots in `CavernCombatManager`:

| Inspector Field | Component Type | Purpose |
| :--- | :--- | :--- |
| **`Dragon Health Bar`** | `UI Slider` | Existing Dragon HP slider (`MaxDragonHealth = 12`). |
| **`Player Health Bar`** | `UI Slider` | Duplicate the HP slider and place it in the bottom corner for the Player (`MaxPlayerHealth = 3`). |
| **`Equipped Spell Text`** | `TextMeshProUGUI` | Displays the currently selected phone spell (e.g., `Equipped: ❄️ ICE SPEAR`). |
| **`Combat Warning Text`** | `TextMeshProUGUI` | Center-screen banner showing Dragon Shield alerts and incoming Dragon Breath countdowns (`4.8s`). |
| **`Victory Defeat Text`** | `TextMeshProUGUI` | Existing Victory text object (automatically shows either `🏆 VICTORY!` or `💀 DEFEATED!`). |
| **`Dragon Shield Visual`** | `GameObject` | A glowing sphere/aura ParticleSystem childed to the Dragon (starts inactive; turns on when Dragon shields). |
| **`Player Shield Visual`** | `GameObject` | A semi-transparent green/cyan UI Panel or 3D barrier in front of the camera (turns on when Player casts Shield). |

### Step 5: Dragon Animator Trigger Check
Ensure your Dragon's **Animator Controller** has an **`AttackTrigger`** parameter wired to a fire-breath or claw-swipe animation state (in addition to your existing `DieTrigger`, `ScreamTrigger`, `HitLightning`, and `HitFireball` triggers).

---

## 3. Testing in the Unity Editor (Keyboard Shortcuts)

You can test the entire combat loop inside Unity Play Mode even without picking up the phone or wand:
* Press **`1`** $\rightarrow$ Equip **🔥 Fireball**
* Press **`2`** $\rightarrow$ Equip **⚡ Lightning Bolt**
* Press **`3`** $\rightarrow$ Equip **❄️ Ice Spear** (Shatters Dragon Shield!)
* Press **`4`** $\rightarrow$ Equip **🛡️ Arcane Shield** (Blocks Dragon Breath!)
* Press **`Spacebar`** $\rightarrow$ Simulate a **MagiQuest Wand Cast**!

---

## 4. Building Two Separate Executables (`MeadowDragon.exe` & `CavernDragon.exe`)

Because both scenes live in the same Unity project, you can build two standalone executables without duplicating project files:

### Option A: One-Click Automated Build Menu
1. Open `Assets/Scripts/CavernBattle/Editor/MultiSceneBuilder.cs` and verify `MeadowScenePath` and `CavernScenePath` match your `.unity` file paths.
2. In Unity's top menu bar, click **`MythicHome -> Build Both Executables (Meadow & Cavern)`**.
3. Unity will automatically compile:
   * `Builds/MeadowDragon/MeadowDragon.exe`
   * `Builds/CavernDragon/CavernDragon.exe`

### Option B: Manual Build Settings
1. Open **`File -> Build Settings`**.
2. Check **only** `MeadowScene` $\rightarrow$ click **Build** $\rightarrow$ save as `Builds/MeadowDragon/MeadowDragon.exe`.
3. Uncheck `MeadowScene`, check **only** `CavernScene` $\rightarrow$ click **Build** $\rightarrow$ save as `Builds/CavernDragon/CavernDragon.exe`.
4. Add both `.exe` applications to **Sunshine** on your host PC so you can launch either encounter from **Moonlight** on your TV!
