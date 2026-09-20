# Phase 1: ESP32 Firmware & Testing Guide

Now that your ESP32 board and TSOP4838 IR Receiver are wired up, it's time to flash firmware to test the sensor and capture your wand's signal.

## Driver Setup (Required First)

If Windows shows **Code 28: The drivers for this device are not installed** for **CP2102 USB to UART Bridge Controller**:

1. **Download Driver:** Download the official [Silicon Labs CP210x VCP Drivers](https://www.silabs.com/developers/usb-to-uart-bridge-vcp-drivers).

2. **Extract Zip:** Unzip the downloaded file onto your PC.

3. **Install:**

   * **Method A:** Right-click the file named `silabser.inf` inside the extracted folder and select **Install**.

   * **Method B:** In Device Manager, right-click **CP2102 USB to UART Bridge Controller**, select **Update Driver**, choose **Browse my computer for drivers**, and select the extracted folder.

4. Once installed, a new COM port (e.g., `CP210x USB to UART Bridge (COM3)`) will appear!

## Method A: ESPHome (Recommended - Easiest & Direct Home Assistant Integration)

ESPHome requires **zero coding**. You configure a short text file, flash it directly from your web browser (Chrome or Edge) using Web Serial, and it integrates natively with Home Assistant.

### Step 1: Install & Flash via Browser

1. Connect your ESP32 to your PC using your USB-C cable.

2. Open **Google Chrome** or **Microsoft Edge** and go to [**web.esphome.io**](https://web.esphome.io).

3. Click **Connect**. Select the newly listed COM port (e.g., `CP210x USB to UART Bridge (COM3)`).

4. Click **Install ESPHome** / **Prepare for first use** and follow the on-screen prompts to prepare the board.

### Step 2: ESPHome YAML Configuration

In ESPHome, you simply paste this configuration block to enable the MagiQuest decoder on GPIO 18:

```
esphome:
  name: mythic-sensor-desk

esp32:
  board: esp32dev

wifi:
  ssid: "YOUR_WIFI_NAME"
  password: "YOUR_WIFI_PASSWORD"

# Enable Web Serial / Native API for Home Assistant
api:
ota:

logger:

# IR Receiver configuration
remote_receiver:
  pin: 18
  dump: magiquest

```

When you flick your wand near the sensor, the ESPHome logs will immediately output:
`Received MagiQuest: wand_id=0x3A8F12C0 magnitude=42`

## Method B: MicroPython (For Direct Custom Python Scripts)

If you prefer to program the ESP32 using Python directly without Home Assistant middleware:

### Step 1: Install Thonny IDE

1. Download and install [**Thonny IDE**](https://thonny.org/) on your desktop PC.

2. Plug in the ESP32 via USB-C.

3. In Thonny, go to **Tools -> Options -> Interpreter**.

4. Select **MicroPython (ESP32)** and choose your board's COM port.

5. Click **Install or Update MicroPython** to flash the MicroPython firmware onto the board.

### Step 2: Test Script (`main.py`)

Save the following script to the ESP32 device in Thonny:

```
from machine import Pin
import time

# TSOP sensor OUT pin connected to GPIO 18
ir_pin = Pin(18, Pin.IN)

print("Mythic Home Receiver Active! Flick your wand at the sensor...")

last_state = 1
pulse_count = 0

while True:
    val = ir_pin.value()
    # Detect high-to-low pulse transitions from the TSOP receiver
    if val == 0 and last_state == 1:
        pulse_count += 1
        print(f"✨ Pulse detected! (Total: {pulse_count})")
        time.sleep_ms(50) # Basic debounce
    last_state = val

```

## Troubleshooting Connection & COM Port Issues

1. **Only COM1 Appears:** This indicates missing CP2102 drivers (see Driver Setup above) or a charge-only USB cable.

2. **Installation Fails / Times Out:** Press and hold the **BOOT** (or `IO0`) button on the ESP32 right as you click **Connect** or **Install** in Chrome until the installation progress bar starts.