# Mythic Home Receiver Enclosure & Housing Options

Transitioning your sensor node from a large, open breadboard to a clean, permanent enclosure protects the electronics and elevates the "magical" immersion in the room—100% solder-free!

## 1. Moving Beyond the Breadboard (Solderless Footprint Reduction)

You don't need a soldering iron to shrink the footprint of your project. Here are the best ways to keep it compact without soldering a single wire:

* **Direct Female-to-Female Jumper Wires (Easiest & Smallest):**
  Ditch the breadboard entirely! Slip 3 Female-to-Female jumper wires directly over the pins of your ESP32 board (`3V3`, `GND`, and `GPIO18`). Connect the other ends directly to the 3 legs of the TSOP IR receiver sensor.

  * *Tip:* Wrap the bare legs of the TSOP sensor with a small strip of electrical tape or heat-shrink tubing so they don't touch and cause a short circuit inside the box.

* **Mini 170-Point Solderless Breadboards (Tiny Footprint):**
  If you prefer having a board structure, buy a mini 170-tie-point solderless breadboard (about \$1–$2 each, measuring only $1.8\times 1.4$ inches). They have adhesive backing on the bottom and allow you to plug everything in just like a regular breadboard, but in a fraction of the space.

## 2. Mounting Electronics Inside a Plastic Project Box (e.g., LeMotech)

When using a standard ABS plastic project box, you want to keep the ESP32 board elevated and secure so the pins underneath don't rattle or touch each other.

### Method A: Heavy-Duty Double-Sided Foam Tape (Recommended)

* **How it works:** Apply a small strip of **3M VHB double-sided mounting tape** or heavy-duty foam mounting tape to the flat plastic underside of the ESP32 board (the smooth side between the header pin rows).

* **Why it works:** The thick foam acts as an electrical insulator for the bottom pins while providing a rock-solid grip against the inside wall of the enclosure.

### Method B: Adhesive Nylon PCB Standoffs

* **How it works:** Stick 4 small self-adhesive nylon standoffs onto the interior floor of the enclosure. The corner mounting holes on the ESP32 board snap directly onto the pins, raising the board off the bottom surface.

* **Cost:** Very inexpensive (\$3–$5 for a multi-pack on Amazon).

### Method C: Hot Glue Mount

* **How it works:** Place 4 small dabs of hot glue on the interior floor of the enclosure and press the corner plastic edges of the ESP32 board into them before it hardens. It holds the board firmly in place, but can easily be peeled up if you ever need to remove the board.

## 3. Preparing the Box for Power & IR Receiver

1. **IR Sensor Hole:** Drill a 5mm hole in the front face of the box. Push the curved lens dome of the TSOP sensor through the hole from the inside. A tiny dab of hot glue or superglue around the interior rim holds it firmly in place.

2. **USB-C Power Port:**

   * **Option A:** Use a round drill bit to drill a 6mm hole on the back panel aligned with the ESP32's USB-C port so your charging cable plugs straight in from the outside.

   * **Option B:** File or drill a small U-shaped notch along the rim of the box lid where the cable can pass through when the box is closed.

## 4. Craft & Fantasy Enclosure Alternatives (100% Solderless)

These DIY options blend naturally into a room's decor or wizarding theme without needing any technical manufacturing:

### 🪵 Mini Wooden Treasure Box / Spell Tome

* **Materials:** Small wooden craft box or hollow "fake book" from a craft store (Michaels, Joann, or Amazon) for \$3–$5.

* **Assembly:**

  1. Drill a tiny 5mm hole in the front face for the TSOP IR receiver's curved eye.

  2. Drill or notch a small hole in the back for the USB-C power cable.

  3. Secure the ESP32 inside using double-sided foam tape.

  4. Stain or paint with metallic/mythic accents.

### 🔮 Frosted Crystal Orb / Potion Bottle

* **Materials:** Frosted plastic potion bottle, craft orb, or acrylic container.

* **Why it works:** **Infrared (38kHz) light passes right through many clear or semi-translucent frosted plastics**, so the IR receiver can sit entirely *inside* the container without needing a hole drilled for the sensor eye!

* **Assembly:** Place the ESP32 and sensor inside, pass the power cable out the back or bottom cap, and plug it in.

## 5. 3D-Printed Themed Enclosures

If you decide to order a 3D print or use a local library/maker-space printer:

* **MagiQuest / Wand Target Models:** Search Printables or Thingiverse for `"MagiQuest sensor"`, `"Wand target"`, or `"ESP32 snap box"`.

* **Look for Snap-Fit Cases:** Choose models designed with snap-fit lids or internal mounting posts specifically made for standard pre-header ESP32 dev boards so no screws or soldering are required.

## 6. Mounting & IR Line-of-Sight Tips

1. **Keep the IR Eye Exposed or Unobstructed:** The TSOP receiver has a wide reception cone (\~90° viewing angle), but it works best when pointing toward the area where your son flicks the wand (e.g., facing the bed or doorway).

2. **Wall Mounting:** Use Command Picture Hanging Strips (velcro-style) to easily mount the project box to a wall near a power outlet without damaging paint or drywall.