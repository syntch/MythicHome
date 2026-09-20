# Lightning Spell Automation Guide

Creating a realistic lightning spell with standard smart bulbs relies on **staggered flashes, high color temperatures, rapid flickers, and an electric afterglow**.

Since smart bulbs over Wi-Fi can process commands in 50–100ms intervals, we can chain quick brightness changes to simulate the crackle and flash of a bolt strike across the room.

## The Lightning Effect Sequence Breakdown

1. **The Initial Spark (Leader Strike):**

   * The **Nightstand bulb** (nearest the wand) flashes instantly to 100% brightness in crisp **Cool Daylight White (6500K)** for 100ms.

2. **The Main Crack (Full Room Blast):**

   * **All 3 bulbs** blast to 100% brightness in bright cool white simultaneously for 150ms.

3. **The Crackle & Aftershocks (Rapid Flickers):**

   * The room abruptly dims to 20% brightness for 80ms, flashes back to 100% on the **Standing Lamp** in electric light blue, and drops back down to simulate flickering electrical discharge.

4. **The Electric Afterglow & Fade:**

   * All 3 bulbs shift to a deep **Electric Cyan / Mana Blue** (`#00E5FF`) and smoothly fade back to warm white over 2–3 seconds.

## Step-by-Step Actions in Visual Editor

If building in the visual GUI editor:

 1. **Action 1:** `Light: Turn on` (Nightstand Bulb)

    * **Color temp:** `6500 K` | **Brightness:** `100%` | **Transition:** `0`

 2. **Action 2:** `Wait for time passes (Delay)` ⏱️

    * **Delay:** `00:00:00.100` (100 milliseconds)

 3. **Action 3:** `Light: Turn on` (All 3 Bulbs)

    * **Color temp:** `6500 K` | **Brightness:** `100%` | **Transition:** `0`

 4. **Action 4:** `Wait for time passes (Delay)` ⏱️

    * **Delay:** `00:00:00.150` (150 milliseconds)

 5. **Action 5:** `Light: Turn on` (All 3 Bulbs)

    * **Brightness:** `20%` | **Transition:** `0`

 6. **Action 6:** `Wait for time passes (Delay)` ⏱️

    * **Delay:** `00:00:00.080` (80 milliseconds)

 7. **Action 7:** `Light: Turn on` (Standing Lamp Bulbs)

    * **Color:** Electric Cyan (`#00E5FF`) | **Brightness:** `100%` | **Transition:** `0`

 8. **Action 8:** `Wait for time passes (Delay)` ⏱️

    * **Delay:** `00:00:00.120` (120 milliseconds)

 9. **Action 9:** `Light: Turn on` (All 3 Bulbs)

    * **Color:** Cyan / Electric Blue | **Brightness:** `70%` | **Transition:** `0`

10. **Action 10:** `Light: Turn on` (All 3 Bulbs)

    * **Color temp:** `3000 K` (Warm White) | **Brightness:** `80%` | **Transition:** `2` seconds

## Full Home Assistant Automation YAML

You can paste this directly into YAML mode (**3 dots in top right → Edit in YAML**):

```
alias: "Mythic Spell: Lightning Bolt Strike"
description: "Flashes nightstand first, blasts all bulbs in cool white, flickers, and fades out in electric blue."
trigger:
  - platform: mqtt
    topic: mythichome/events/cast
action:
  # 1. Initial Leader Spark (Nightstand bulb flashes)
  - action: light.turn_on
    target:
      entity_id: light.nightstand_bulb
    data:
      color_temp_kelvin: 6500
      brightness_pct: 100
      transition: 0

  - delay:
      milliseconds: 100

  # 2. Main Thunderhead Strike (All 3 bulbs blast cool white)
  - action: light.turn_on
    target:
      entity_id:
        - light.nightstand_bulb
        - light.standing_lamp_bulb_1
        - light.standing_lamp_bulb_2
    data:
      color_temp_kelvin: 6500
      brightness_pct: 100
      transition: 0

  - delay:
      milliseconds: 150

  # 3. Flicker 1: Sudden Drop
  - action: light.turn_on
    target:
      entity_id:
        - light.nightstand_bulb
        - light.standing_lamp_bulb_1
        - light.standing_lamp_bulb_2
    data:
      brightness_pct: 20
      transition: 0

  - delay:
      milliseconds: 80

  # 4. Flicker 2: Standing Lamp Re-strike in Electric Cyan
  - action: light.turn_on
    target:
      entity_id:
        - light.standing_lamp_bulb_1
        - light.standing_lamp_bulb_2
    data:
      rgb_color: [0, 229, 255]
      brightness_pct: 100
      transition: 0

  - delay:
      milliseconds: 120

  # 5. Electric Afterglow (All bulbs shift to blue)
  - action: light.turn_on
    target:
      entity_id:
        - light.nightstand_bulb
        - light.standing_lamp_bulb_1
        - light.standing_lamp_bulb_2
    data:
      rgb_color: [0, 150, 255]
      brightness_pct: 70
      transition: 0

  - delay:
      milliseconds: 300

  # 6. Smooth Return to Warm Ambient White over 2.5 seconds
  - action: light.turn_on
    target:
      entity_id:
        - light.nightstand_bulb
        - light.standing_lamp_bulb_1
        - light.standing_lamp_bulb_2
    data:
      color_temp_kelvin: 2800
      brightness_pct: 80
      transition: 2.5
mode: single

```