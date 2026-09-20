# Home Assistant Spell Automation: Dynamic State Snapshot & Delay Control

## Why the Transition Parameter Doesn't Wait

When you set **Transition: 3 seconds** in a `Light: Turn on` action, Home Assistant sends a message to the physical bulb saying *"start fading to green over the next 3 seconds"* and **immediately moves to the next action in your automation without waiting**.

Because there was no delay step after the first light action, your automation was running all 4 steps in less than 50 milliseconds:

1. Command sent to bulbs: *Fade to Green over 3s* (returns instantly)

2. Dolphin lamp toggles

3. Noise machine toggles

4. Command sent to bulbs: *Fade to White over 3s* (**overwrites the green command instantly!**)

## Correct Action Sequence (With Delays)

To fix this, you must insert **Delay** actions wherever you want Home Assistant to wait for the light transition to finish before moving to the next step.

### Step-by-Step Action List in Visual Editor

1. **Action 1:** `Light: Turn on`

   * **Targets:** `Standing Lamp Bulb 1`, `Standing Lamp Bulb 2`

   * **Color:** Green

   * **Transition:** `3` seconds

2. **Action 2:** `Wait for time passes (Delay)` ⏱️

   * **Delay:** `00:00:03` (3 seconds) — *This gives the bulb time to finish fading to green!*

3. **Action 3:** `Switch: Toggle`

   * **Target:** `Dolphin Lamp`

4. **Action 4:** `Switch: Toggle`

   * **Target:** `Noise Machine (1)`

5. **Action 5:** `Light: Turn on`

   * **Targets:** `Standing Lamp Bulb 1`, `Standing Lamp Bulb 2`

   * **Color / Temperature:** White

   * **Transition:** `3` seconds

## Updated Automation YAML

If you prefer to edit in YAML mode (**3 dots in top right → Edit in YAML**):

```
alias: "Mythic Home Spell: Green Transition & Toggle"
trigger:
  - platform: mqtt
    topic: mythichome/events/cast
action:
  # 1. Start transition to Green
  - action: light.turn_on
    target:
      entity_id:
        - light.standing_lamp_bulb_1
        - light.standing_lamp_bulb_2
    data:
      color_name: green
      transition: 3

  # 2. Wait 3 seconds for the green fade to complete
  - delay:
      seconds: 3

  # 3. Toggle physical devices
  - action: switch.toggle
    target:
      entity_id: switch.dolphin_lamp
  - action: switch.toggle
    target:
      entity_id: switch.noise_machine_1

  # 4. Start transition back to White
  - action: light.turn_on
    target:
      entity_id:
        - light.standing_lamp_bulb_1
        - light.standing_lamp_bulb_2
    data:
      color_temp_kelvin: 3000
      transition: 3
mode: single

```