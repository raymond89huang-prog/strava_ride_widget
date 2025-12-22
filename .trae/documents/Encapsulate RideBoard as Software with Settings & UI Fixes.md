I will implement the requested features to package the widget as a complete software application with settings and UI optimizations.

### 1. "Software-like" Behavior & System Tray
- **Hide Console**: I will modify the application startup logic so the Python data server runs in the background (hidden) without a visible terminal window.
- **System Tray Icon**: I will add a **Taskbar Notification Icon (System Tray)**.
  - The app will show an icon in the system tray.
  - You can minimize/close the main window, and the app will keep running.
  - Right-click menu on tray icon: **Show Widget**, **Exit**.

### 2. Settings & Auto-start
- **New Settings Interface**: I will create a new **Settings Window** (triggered by the Gear icon).
- **Auto-start**: Add a checkbox "开机自动启动" (Start on Boot) in the settings.
  - This will toggle a Registry key to let Windows start the widget automatically.
- **Login Button**: Move the Strava Login trigger into this Settings window.

### 3. UI Optimizations
- **Status Text**: Change the bottom-left "OK" text to **"更新时间：HH:mm"** as requested.
- **Yearly Summary Layout**: I will increase the default window height (from 240 to 280) and adjust margins to ensure the "Total Moving Time" and other data are fully visible.

### Implementation Steps
1.  **Modify `App.xaml.cs`**: Implement System Tray icon and background Python process management (start server silently).
2.  **Create `SettingsWindow.xaml`**: Design the settings dialog.
3.  **Update `WidgetViewModel.cs`**: Add logic for the new footer text format and handle settings interactions.
4.  **Update `MainWindow.xaml`**: Adjust window height and bind the new footer text.
5.  **Update `StartupManager.cs`**: Implement the "Start on Boot" registry logic.
6.  **Update `start_widget.bat`**: Simplify it to just launch the widget (which now manages the server).
