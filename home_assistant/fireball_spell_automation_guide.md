# Fireball Spell Automation Guide

Creating an explosive fireball effect relies on a **charging swell (yellow to orange)**, an **instant red impact blast**, **rapid ember flickers on the desk lamp**, and a **warm smoke cool-down back to ambient white**.

## The Fireball Effect Sequence Breakdown

1. **The Charge & Igniting Ember (0.8s):**

   * The **Nightstand / Desk bulb** ignites in warm **Golden Yellow (`[255, 180, 0]`)** at 30% brightness and quickly swells up to a fierce **Intense Orange (`[255, 90, 0]`)** at 80% brightness across all 3 bulbs.

2. **The Fireball Impact Blast (250ms):**

   * **All 3 bulbs** blast instantly to 100% brightness in **Crimson Red (`[255, 0, 0]`)** with 0 transition time to simulate the explosion hitting the room.

3. **Crackling Embers & Desk Lamp Flicker:**

   * The standing lamps immediately drop to a dim orange background glow (25%).

   * The **Desk lamp** rapidly flickers 3 times between bright fire orange (`[255, 100, 0]`) and deep ember red (`[150, 15, 0]`) with quick 100ms delays to mimic lingering flames and sparks on the desk.

4. **Smoke Clearing & Thermal Dissipation:**

   * All 3 bulbs smoothly transition over 3 seconds back to standard **Warm White (2800K)** at normal ambient room brightness.

## Step-by-Step Actions in Visual Editor

If building in the visual GUI editor:

 1. **Action 1 (Ignition):** `Light: Turn on` (Nightstand Bulb)

    * **Color:** Gold (`#FFB400`) | **Brightness:** `40%` | **Transition:** `0.3`

 2. **Action 2 (Swell):** `Wait for time passes (Delay)` ⏱️ `00:00:00.300`

 3. **Action 3 (Charge Up):** `Light: Turn on` (All 3 Bulbs)

    * **Color:** Intense Orange (`#FF5A00`) | **Brightness:** `85%` | **Transition:** `0.5`

 4. **Action 4:** `Wait for time passes (Delay)` ⏱️ `00:00:00.500`

 5. **Action 5 (Fireball Blast):** `Light: Turn on` (All 3 Bulbs)

    * **Color:** Crimson Red (`#FF0000`) | **Brightness:** `100%` | **Transition:** `0`

 6. **Action 6:** `Wait for time passes (Delay)` ⏱️ `00:00:00.250`

 7. **Action 7 (Ember Drop):** `Light: Turn on` (Standing Lamp Bulbs)

    * **Color:** Dark Orange | **Brightness:** `25%` | **Transition:** `0`

 8. **Action 8 (Flicker 1):** `Light: Turn on` (Nightstand Bulb)

    * **Color:** Ember Red (`#960F00`) | **Brightness:** `20%` | **Transition:** `0`

 9. **Action 9:** `Wait for time passes (Delay)` ⏱️ `00:00:00.100`

10. **Action 10 (Flicker 2):** `Light: Turn on` (Nightstand Bulb)

    * **Color:** Flame Orange (`#FF6A00`) | **Brightness:** `90%` | **Transition:** `0`

11. **Action 11:** `Wait for time passes (Delay)` ⏱️ `00:00:00.120`

12. **Action 12 (Flicker 3):** `Light: Turn on` (Nightstand Bulb)

    * **Color:** Ember Red (`#960F00`) | **Brightness:** `35%` | **Transition:** `0`

13. **Action 13:** `Wait for time passes (Delay)` ⏱️ `00:00:00.100`

14. **Action 14 (Cool-down):** `Light: Turn on` (All 3 Bulbs)

    * **Color temp:** `2800 K` | **Brightness:** `80%` | **Transition:** `3` seconds

## Full Home Assistant Automation YAML

You can paste this directly into YAML mode (**3 dots in top right → Edit in YAML**):

```
alias: "Mythic Spell: Fireball Blast"
description: "Swells yellow to orange, blasts crimson red, flickers desk lamp like crackling embers, and fades to warm white."
trigger:
  - platform: mqtt
    topic: mythichome/events/cast
action:
  # 1. Spark / Ignition at Desk
  - action: light.turn_on
    target:
      entity_id: light.nightstand_bulb
    data:
      rgb_color: [255, 180, 0]
      brightness_pct: 40
      transition: 0.3

  - delay:
      milliseconds: 300

  # 2. Fireball Charge / Swell to All Bulbs
  - action: light.turn_on
    target:
      entity_id:
        - light.nightstand_bulb
        - light.standing_lamp_bulb_1
        - light.standing_lamp_bulb_2
    data:
      rgb_color: [255, 90, 0]
      brightness_pct: 85
      transition: 0.5

  - delay:
      milliseconds: 500

  # 3. Fireball Explosion Blast (Instant Red Flash)
  - action: light.turn_on
    target:
      entity_id:
        - light.nightstand_bulb
        - light.standing_lamp_bulb_1
        - light.standing_lamp_bulb_2
    data:
      rgb_color: [255, 0, 0]
      brightness_pct: 100
      transition: 0

  - delay:
      milliseconds: 250

  # 4. Standing lamps drop to low embers, Desk lamp starts crackle flicker
  - action: light.turn_on
    target:
      entity_id:
        - light.standing_lamp_bulb_1
        - light.standing_lamp_bulb_2
    data:
      rgb_color: [200, 50, 0]
      brightness_pct: 25
      transition: 0

  # Flicker 1 (Desk Dim Ember)
  - action: light.turn_on
    target:
      entity_id: light.nightstand_bulb
    data:
      rgb_color: [150, 15, 0]
      brightness_pct: 20
      transition: 0

  - delay:
      milliseconds: 100

  # Flicker 2 (Desk Flame Flare)
  - action: light.turn_on
    target:
      entity_id: light.nightstand_bulb
    data:
      rgb_color: [255, 106, 0]
      brightness_pct: 90
      transition: 0

  - delay:
      milliseconds: 120

  # Flicker 3 (Desk Low Flame)
  - action: light.turn_on
    target:
      entity_id: light.nightstand_bulb
    data:
      rgb_color: [150, 15, 0]
      brightness_pct: 35
      transition: 0

  - delay:
      milliseconds: 150

  # 5. Smooth Dissipation back to Warm Ambient White over 3 seconds
  - action: light.turn_on
    target:
      entity_id:
        - light.nightstand_bulb
        - light.standing_lamp_bulb_1
        - light.standing_lamp_bulb_2
    data:
      color_temp_kelvin: 2800
      brightness_pct: 80
      transition: 3
mode: single

```