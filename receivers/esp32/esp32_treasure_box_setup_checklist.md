# ESP32 Treasure Box Node: Software Setup & Testing Guide

Now that your ESP32 board and TSOP4838 IR Receiver are mounted inside your treasure box, follow these steps to flash the firmware and test wand detection.

## Step 1: Connect to PC & Verify Driver

1. Connect the ESP32 to your desktop PC using the USB-C cable passing through the rear box hole.

2. Open **Device Manager** on Windows.

3. Check under **Ports (COM & LPT)** for a device named `CP210x USB to UART Bridge (COMx)` or similar.

   * *If it shows a yellow warning triangle ("Code 28"):* Download and install the [CP210x Drivers from Silicon Labs](https://www.silabs.com/developers/usb-to-uart-bridge-vcp-drivers), right-click the unzipped `.inf` file, and select **Install**.

## Step 2: Choose Your Firmware Approach

Select either **Option A (MicroPython via Thonny)** if you prefer custom Python code, or **Option B (ESPHome via Web Browser)** for native Home Assistant integration with zero code.

### Option A: MicroPython Setup (Thonny IDE)

1. Launch **Thonny IDE** on your PC.

2. Go to **Tools → Options → Interpreter**.

3. Set the interpreter to **MicroPython (ESP32)** and select your board's **COM Port**.

4. Click **Install or Update MicroPython** (if you haven't flashed MicroPython to this board before).

#### Testing Signal Capture (`magiquest_decoder.py`)

1. Open `magiquest_decoder.py` in Thonny.

2. Ensure `ir_pin = Pin(18, Pin.IN)` matches your signal pin (**GPIO 18**).

3. Click the **Run Script (Green Play Button)** in Thonny.

4. Point your wand at the front hole of the treasure chest and flick it!

5. Watch the Thonny Shell console—you should see:

   ```
   ✨ SPELL CAST DETECTED!
      ► Wand ID (Hex): 0x3A8F12C0
      ► Magnitude:     45
   
   ```

#### Live Wi-Fi & MQTT Publishing (`magiquest_mqtt.py`)

1. Open `magiquest_mqtt.py` in Thonny.

2. Update the network variables at the top of the file:

   * `WIFI_SSID`: Your home 2.4GHz Wi-Fi network name.

   * `WIFI_PASS`: Your Wi-Fi password.

   * `MQTT_BROKER`: The IP address of your central server / Home Assistant PC.

   * `SENSOR_ID`: Set to `"treasure_box_node"`.

3. Save the file to the ESP32 as **`main.py`** so it automatically runs whenever the box receives USB power.

### Option B: ESPHome Browser Flashing (Easiest for Home Assistant)

1. Open **Google Chrome** or **Microsoft Edge** and go to [**web.esphome.io**](https://web.esphome.io).

2. Click **Connect** and select your ESP32 board's COM port.

3. Click **Install ESPHome** to prepare the board.

4. Paste the following configuration block:

```
esphome:
  name: mythic-treasure-box

esp32:
  board: esp32dev

wifi:
  ssid: "YOUR_WIFI_NAME"
  password: "YOUR_WIFI_PASSWORD"

api:
ota:
logger:

remote_receiver:
  pin: 18
  dump: magiquest

```

5. Click **Install / Flash**. Once finished, the board will connect to Wi-Fi and broadcast wand casts directly to Home Assistant.

## Step 3: Verification & Alignment Checklist

* \[ \] **Power Check:** The ESP32's onboard power LED lights up when plugged into USB power.

* \[ \] **Line-of-Sight Check:** The TSOP receiver's dome lens is facing straight out through the 5mm front hole.

* \[ \] **Flick Detection:** A wand flick within 6–10 feet of the box triggers a printed Wand ID log in Thonny or ESPHome.

* \[ \] **Cooldown Verification:** Rapid flicks within 3 seconds are filtered out so Home Assistant doesn't trigger duplicate spell events.