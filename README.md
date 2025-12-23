# RideBoard - Desktop Cycling Widget (Strava)

RideBoard is a minimalist, always-on-top Windows desktop widget that displays your Strava cycling data in real-time. Designed to provide low-friction access to your training stats without opening a browser or mobile app.
<img width="577" height="453" alt="image" src="https://github.com/user-attachments/assets/d992ca53-528d-4d65-bfa1-fc4a66ba4784" />
<img width="609" height="485" alt="image" src="https://github.com/user-attachments/assets/d1733982-98e7-485f-9819-80ef684718bc" />
<img width="363" height="352" alt="image" src="https://github.com/user-attachments/assets/10c6a638-240d-422f-bcc0-09b92a409ef3" />




## Features

*   **Real-time Dashboard**:
    *   **TODAY**: Distance, Moving Time, Elevation.
    *   **LAST RIDE**: Distance, Avg Power, Avg Heart Rate.
    *   **THIS WEEK**: Total Distance, Total Time.
*   **Yearly Summary**: Toggle to view Year-to-Date (YTD) stats including total distance, elevation, and moving time.
*   **Non-intrusive UI**:
    *   Dark, semi-transparent, borderless design.
    *   Always-on-top (Topmost) behavior.
    *   Drag to move; snaps to screen edges.
    *   Click-through support (optional).
*   **System Integration**:
    *   System Tray support (Minimize to tray).
    *   **Start on Boot**: Option to launch automatically with Windows.
*   **Data & Privacy**:
    *   Runs a local Python server for Strava API communication.
    *   Secure OAuth2 login flow.
    *   Local caching to minimize API rate limits.
*   **Cycling Focused**: Filters activities to only include Ride, VirtualRide, and EBikeRide.

## Installation

1.  Clone this repository.
2.  Run `start_widget.bat`.
    *   This will automatically check for and install the required .NET 10 SDK if missing.
    *   It compiles the widget and starts the backend service silently.

## Configuration

1.  **First Run**:
    *   Click the **Gear (Settings)** icon on the widget.
    *   Click **"Connect Strava (关联账号)"**.
    *   A browser window will open. Authorize the app to access your Strava data.
    *   Once authorized, data will populate automatically.
2.  **Settings**:
    *   **Start on Boot**: Enable in the Settings window.
    *   **Refresh**: Click the refresh icon (⟳) in the bottom-right footer to force an update.

## Tech Stack

*   **Frontend**: C# / WPF (.NET 10.0)
*   **Backend**: Python 3 (http.server)
*   **Architecture**: Local HTTP API + MVVM Client

## Development

*   **Widget**: Located in `rideboard/widget/`. Open with VS Code or Visual Studio.
*   **Server**: Located in `rideboard/server/`. Python scripts for data fetching.

## License

MIT
