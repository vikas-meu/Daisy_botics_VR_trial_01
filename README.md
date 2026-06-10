# Daisy_botics_VR_trial_01

## Project Overview

This repository contains a small VR/Unity trial project for Daisy Botics. It includes:

- `hardware/esp32-testing/esp32-testing.ino`: ESP32 Arduino sketch path.
- `scripts/remote-pc-data-receiver.py`: Python TCP server for receiving telemetry.
- `src/`: Unity C# sender scripts for cube position, rotation, and gripper data.
- `docs/`: placeholder for wiring diagrams and setup guides.
- `.github/`: placeholder for workflows and templates.

## Repository Structure

```
DAISY_BOTICS_VR_TRIAL_01/
├── .github/                     # GitHub workflows and templates
├── docs/                        # Wiring diagrams, setup guides, etc.
├── hardware/                    # Microcontroller-related files
│   └── esp32-testing/
│       └── esp32-testing.ino    # Arduino sketch for ESP32
├── scripts/                     # Standalone automation or receiver scripts
│   └── remote-pc-data-receiver.py
├── src/                         # Core Unity / VR application files
│   ├── applicables/             # Production-ready C# scripts
│   │   └── cube-data-connection-set.cs
│   └── trials/                  # Experimental/testing C# scripts
│       └── cube-pos-rot-reader.cs
├── .gitignore                   # Ignored files for Unity/Python
└── README.md                    # Project documentation
```

## What to Download

### 1. Arduino / ESP32

- Arduino IDE (latest version) or Visual Studio Code with the PlatformIO extension.
- ESP32 board support package for Arduino IDE:
  - In Arduino IDE: `Tools > Board > Boards Manager` and install `esp32`.
- USB driver for your ESP32 board if needed.

### 2. Python

- Python 3.8+ (recommended 3.10 or later).
- No extra packages are required for `scripts/remote-pc-data-receiver.py`.

### 3. Unity / VR

- Unity 2021.3 LTS or newer.
- Unity Input System package.
- XR Interaction Toolkit or OpenXR package if you are using Quest or other VR hardware.

### 4. Optional Tools

- Visual Studio Code for editing Python, C#, and Arduino sketches.
- Git for version control.

## How to Use This Project

### Step 1: Prepare the ESP32 sketch

1. Open `hardware/esp32-testing/esp32-testing.ino` in Arduino IDE or PlatformIO.
2. Add your ESP32 WiFi or Serial sending code.
3. Flash it to your ESP32 board.

> Note: This repository currently includes the sketch path, but the `.ino` file is empty. You must insert your ESP32 communication logic.

### Step 2: Run the Python receiver

1. Open a terminal.
2. Change to this repository folder.
3. Run:

```bash
python3 scripts/remote-pc-data-receiver.py
```

4. The receiver listens on `0.0.0.0:8080` and prints incoming telemetry lines.

### Step 3: Configure Unity

1. Open your Unity project.
2. Add the `SendMultiCubeDataOverWiFi` script to a GameObject.
3. Assign `cube1`, `cube2`, and `cube3` transforms.
4. Assign `rightIndexTrigger` and `leftIndexTrigger` Input Action References.
5. Start the scene.
6. When prompted, enter the PC IP address where `remote-pc-data-receiver.py` is running.

### Step 4: Verify the connection

- The Python receiver should show a connection from the Unity sender.
- Incoming lines like `GRIPR`, `GRIPL`, `C1R`, `C2P`, and `C2R` will be printed.

## File Descriptions

### `hardware/esp32-testing/esp32-testing.ino`

- Arduino sketch file for ESP32.
- Use this for microcontroller logic and data transmission.
- Download: Arduino IDE / PlatformIO + ESP32 support.

### `scripts/remote-pc-data-receiver.py`

- Python TCP receiver script.
- Listens on port `8080` for incoming text telemetry.
- Prints received data to the terminal.
- Download: Python 3.8+.

### `src/applicables/cube-data-connection-set.cs`

- Production-ready Unity script.
- Sends cube position, rotation, and VR gripper values over TCP.
- Intended for reliable runtime use.
- Download: Unity + Input System + XR support.

### `src/trials/cube-pos-rot-reader.cs`

- Experimental Unity sender script.
- Similar to the main script but kept in `trials/` for testing.
- Use this file for development or comparison.

### `.github/`

- Reserved for GitHub Actions workflows and issue/PR templates.
- Add automation files here if you want CI or repository templates.

### `docs/`

- Placeholder for wiring diagrams, setup instructions, screenshots, or notes.
- Add design and hardware documentation here.

## Important Notes

- Ensure all devices are on the same local network.
- Make sure port `8080` is not blocked by the firewall.
- Update Unity script IP address to your PC's local IP if necessary.
- The current Unity scripts use TCP and raw ASCII messages.

## Recommended Improvements

- Add a real ESP32 sketch into `hardware/esp32-testing/esp32-testing.ino`.
- Add network configuration or server IP input to the ESP32 sketch.
- Add docs and diagrams under `docs/`.
- Add `.github/workflows` if you want GitHub automation.

I Am Vikas
I Love Robotics 
