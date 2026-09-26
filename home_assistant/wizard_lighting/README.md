# 🪄 Wizard Lighting: Mobile Bedroom Lighting Dashboard

This directory contains Home Assistant scripts and a phone-optimized Lovelace dashboard that allows young wizards to transform their bedroom into atmospheric fantasy realms with a single tap.

---

## 🎨 The Fantasy Environments

Each environment orchestrates the room's **standing lamp** (2 smart bulbs) for ambient environmental mood and the **bedside table lamp** (1 smart bulb) as the themed focal point:

| Environment | Theme & Mood | Standing Lamp (x2) | Bedside Table Lamp (x1) | Special Behavior |
| :--- | :--- | :--- | :--- | :--- |
| **🔥 Torchlit Cave** | Subterranean cavern with a flickering stone-wall torch | Dim Midnight Stone (`[15, 25, 70]`, 15%) | Crackling Torch (`[255, 120, 10]`, 65%) | Rapid 3-stage dynamic flicker on the table lamp before settling into a warm flame glow |
| **🧪 Poison Swamp** | Murky alchemist bog with bubbling toxic concoctions | Murky Toxic Green (`[30, 200, 30]`, 40%) | Bubbling Arcane Purple (`[180, 0, 255]`, 75%) | 2-second crossfade into an eerie dual-tone glow |
| **🌲 Enchanted Forest** | Ethereal woodland canopy with glowing fairy wisps | Deep Emerald Canopy (`[0, 130, 45]`, 35%) | Fairy Gold (`[255, 215, 100]`, 85%) | Gentle 2.5s warm ambient fade |
| **🌋 Dragon's Caldera** | Smoldering volcanic lair with active molten magma | Magma Crimson (`[200, 15, 0]`, 50%) | Blazing Sun Gold (`[255, 180, 0]`, 90%) | Fiery contrast mimicking glowing coals |
| **⚡ Arcane Crystal Sanctum**| High wizard tower channeling leyline energy | Astral Deep Indigo (`[40, 10, 180]`, 30%) | Mana Cyan (`[0, 230, 255]`, 85%) | High-contrast neon magical laboratory |
| **✨ Lumos (Daylight)** | Everyday warm ambient reading, study, and play light | Warm White (3000K, 85%) | Warm White (3000K, 85%) | Smooth 2-second return to comfortable room light |
| **🌙 Nox (Sleep / Embers)**| Sleep timer nightlight for bedtime | **Turned OFF** | Dim Candle Ember (`[255, 50, 0]`, 4%) | Standing lamps shut off; bedside lamp drops to an ultra-dim warm nightlight |

---

## 🛠️ Hardware & Bill of Materials

> [!NOTE]
> **Personal Testing & Purchasing Disclaimer:**  
> The specific hardware items and links provided below are the components I have personally purchased, built with, and verified working in my home setup. They are provided as known-working references—there may be better, cheaper, or alternative options that suit your setup equally well!

| Component | Category | Purpose | Recommended Model / Specs | Notes & Purchasing Examples |
| :--- | :--- | :--- | :--- | :--- |
| **Smart RGB Bulbs - Bright (x2)** | Physical Prop | Standing lamp / room-scale ambient scene & spell lighting | Zigbee / Wi-Fi Color Bulbs (Philips Hue, Kasa, Sengled, Govee) | [Bright Smart Color Bulbs](https://www.amazon.com/dp/B0964DN9TV?th=1). Ensure local Home Assistant integration support. |
| **Smart RGB Bulb - Table (x1)** | Physical Prop | Bedside table lamp focal point (Torch flicker, cauldron glow, nightlight) | Zigbee / Wi-Fi Color Bulbs (Philips Hue, Kasa, Sengled, Govee) | [Smart Color Bulbs](https://www.amazon.com/dp/B08TB6VXFL?th=1). Ensure local Home Assistant integration support. |
| **Mobile Device** | Controller | Runs the Home Assistant Companion App or browser PWA | Any iOS or Android phone / tablet | Configured with a dedicated non-admin user profile defaulted to the `Wizard Lighting` dashboard. |

---

## 📁 Included Configuration Files

* **[`scripts.yaml`](scripts.yaml):** Ready-to-paste Home Assistant YAML script definitions for all 7 environments.
* **[`dashboard.yaml`](dashboard.yaml):** Ready-to-paste Lovelace Dashboard YAML with a 2-column mobile button grid, custom gradient card accents, haptic feedback, and manual brightness sliders.

---

## 🚀 Step-by-Step Setup Guide

### Step 1: Add the Scripts to Home Assistant
Choose either method:

* **Method A (Studio Code Server / File Editor):**
  Open your Home Assistant `config/scripts.yaml` file, copy the contents of **[`scripts.yaml`](scripts.yaml)**, and paste them at the bottom. Save and click **Developer Tools → YAML → Reload Scripts**.

* **Method B (Web UI):**
  1. Go to **Settings → Automations & Scenes → Scripts**.
  2. Click **Add Script**.
  3. Click the 3 dots in the top right corner and choose **Edit in YAML**.
  4. Paste one of the script blocks from **[`scripts.yaml`](scripts.yaml)**, name it, and save.

> [!TIP]
> If your bulb entity IDs differ from `light.standing_lamp_bulb_1`, `light.standing_lamp_bulb_2`, or `light.nightstand_bulb`, do a quick find-and-replace in `scripts.yaml` before saving.

---

### Step 2: Configure the Dashboard

1. In Home Assistant, go to **Settings → Dashboards → Add Dashboard** and create a new empty dashboard titled **"Wizard Lighting"** (with icon `mdi:wand`).
2. Open the new **"Wizard Lighting"** dashboard.
3. Click the **Pencil icon** in the top right (Edit Dashboard).
4. Click the **3 dots** in the top right again $\rightarrow$ select **Raw configuration editor**.
5. Replace the entire contents with the YAML from **[`dashboard.yaml`](dashboard.yaml)**.
6. Click **Save** and exit the editor.

---

### Step 3: Configure Your Son's Phone

To give your son an app-like experience without exposing your main smart home settings:

1. **Create a Dedicated User (Recommended):**
   * Go to **Settings → People → Users → Add User**.
   * Create a username (e.g., `wizard` or his name) and toggle **Local access only** and **Non-admin** permissions.
2. **Set Default Dashboard:**
   * Log into Home Assistant on his phone using his account.
   * Tap his profile icon in the bottom-left sidebar.
   * Under **Dashboard**, set the default dashboard to **"Wizard Lighting"**.
3. **Add to Home Screen (Progressive Web App / Companion App):**
   * **Home Assistant Companion App:** Install from the App Store / Google Play Store for instant loading and native haptic feedback.
   * **Browser PWA (iOS Safari):** Tap the Share button $\rightarrow$ tap **Add to Home Screen**.
   * **Browser PWA (Android Chrome):** Tap the 3 dots $\rightarrow$ tap **Install app** or **Add to Home screen**.
