# Complete Functionality & Fix Interaction Issues

## 1. Widget Interaction Fixes (WPF)
The issue "buttons cannot be clicked" is likely caused by the `Window_MouseLeftButtonDown` event handler unconditionally calling `DragMove()`, which intercepts mouse events for child controls.

### Implementation Steps:
1.  **Update `MainWindow.xaml`**:
    *   Add a **Close (X)** button to the top-right corner.
    *   Add a **Settings/Login** button (gear icon) to trigger authentication if needed.
2.  **Update `MainWindow.xaml.cs`**:
    *   Modify `Window_MouseLeftButtonDown` to check `e.OriginalSource`. If the source is a Button or interactive control, skip `DragMove()`.
    *   Implement click handlers for the new buttons.

## 2. Strava OAuth & Data Retrieval (Python Server)
Implement the full OAuth2 flow to allow the user to log in via their browser and fetch real data.

### Implementation Steps:
1.  **Dependency Management**:
    *   Create `requirements.txt` containing `requests` and `flask` (for the callback server).
    *   Update `start_server.bat` to install dependencies automatically.
2.  **Configuration**:
    *   Create `config.json` template for `client_id` and `client_secret`.
    *   Create `tokens.json` to store `access_token` and `refresh_token`.
3.  **Auth Module (`rideboard/server/src/auth.py`)**:
    *   Implement `check_login_status()`: Verify if valid tokens exist.
    *   Implement `start_login_flow()`:
        *   Start a temporary local web server (Flask).
        *   Open the system browser to Strava's OAuth URL.
        *   Handle the redirect callback to capture the `code`.
        *   Exchange `code` for tokens and save to `tokens.json`.
4.  **Data Fetching Module (`rideboard/server/src/strava_api.py`)**:
    *   Implement `get_summary()`: Fetch athlete activities.
    *   Calculate **Today**, **Last Ride**, and **This Week** metrics.
    *   Handle token refreshing automatically.
5.  **Server Integration (`rideboard/server/src/server.py`)**:
    *   Update the `/strava` endpoint to return real data from `strava_api`.
    *   Add a `/login` endpoint to manually trigger the login flow from the Widget.

## 3. Verification & Self-Test
1.  **Verify Interaction**: Ensure the new "Close" button works and the window can still be dragged.
2.  **Verify Login Flow**: Run `start_server.bat`, confirm browser opens, and login succeeds.
3.  **Verify Data**: Confirm the Widget displays data fetched from the (mock or real) API.
