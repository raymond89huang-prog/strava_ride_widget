import requests
import time
from datetime import datetime, timedelta

class StravaAPI:
    def __init__(self, auth_handler):
        self.auth = auth_handler
        self.base_url = "https://www.strava.com/api/v3"
        self.athlete_id = None

    def get_headers(self):
        token = self.auth.get_valid_token()
        if not token:
            return None
        return {'Authorization': f'Bearer {token}'}

    def get_athlete_id(self):
        if self.athlete_id:
            return self.athlete_id
        
        headers = self.get_headers()
        if not headers:
            return None
            
        try:
            response = requests.get(f"{self.base_url}/athlete", headers=headers)
            response.raise_for_status()
            data = response.json()
            self.athlete_id = data.get('id')
            return self.athlete_id
        except Exception as e:
            print(f"Error fetching athlete info: {e}")
            return None

    def get_year_stats(self, athlete_id):
        headers = self.get_headers()
        if not headers or not athlete_id:
            return None
            
        try:
            response = requests.get(f"{self.base_url}/athletes/{athlete_id}/stats", headers=headers)
            response.raise_for_status()
            return response.json()
        except Exception as e:
            print(f"Error fetching stats: {e}")
            return None

    def fetch_activities(self, after_timestamp=None):
        headers = self.get_headers()
        if not headers:
            return []
        
        params = {'per_page': 50}
        if after_timestamp:
            params['after'] = int(after_timestamp)
            
        try:
            response = requests.get(f"{self.base_url}/athlete/activities", headers=headers, params=params)
            response.raise_for_status()
            return response.json()
        except Exception as e:
            print(f"Error fetching activities: {e}")
            return []

    def get_summary(self):
        # Calculate timestamps
        now = datetime.now()
        start_of_today = now.replace(hour=0, minute=0, second=0, microsecond=0)
        # Find start of week (Monday)
        start_of_week = start_of_today - timedelta(days=start_of_today.weekday())
        
        # DEBUG: Fetch last 30 days to ensure we get SOMETHING
        start_debug = now - timedelta(days=30)
        activities = self.fetch_activities(after_timestamp=start_debug.timestamp())
        
        # Get Athlete ID and Year Stats
        aid = self.get_athlete_id()
        year_stats_raw = self.get_year_stats(aid) if aid else None
        
        # Helper for formatting
        def format_time(seconds):
            m, s = divmod(seconds, 60)
            h, m = divmod(m, 60)
            return f"{int(h)}:{int(m):02d}"

        def format_time_long(seconds):
            m, s = divmod(seconds, 60)
            h, m = divmod(m, 60)
            return f"{int(h)}:{int(m):02d}:{int(s):02d}"

        # Default Structure
        result = {
            "today": {"distance_km": 0, "time": "0:00:00", "elev_m": 0},
            "last": {"distance_km": 0, "avg_power": 0, "avg_hr": 0},
            "week": {"distance_km": 0, "time": "0:00"},
            "year": {
                "year_val": now.year,
                "range": f"{now.year}年1月1日 - {now.year}12月31日",
                "distance_km": 0,
                "elev_m": 0,
                "time": "0h 0m"
            },
            "updated_at": now.strftime("%H:%M"),
            "status": "ok (no activities)"
        }

        # --- Process Recent Activities (Today / Last / Week) ---
        today_stats = {'distance': 0, 'moving_time': 0, 'elevation': 0}
        week_stats = {'distance': 0, 'moving_time': 0}
        last_ride = {}

        if activities:
            # Sort by start_date descending
            activities.sort(key=lambda x: x['start_date'], reverse=True)

            # Filter for cycling activities only (Ride, VirtualRide, EBikeRide)
            activities = [act for act in activities if act.get('type') in ['Ride', 'VirtualRide', 'EBikeRide']]

            if activities:
                last = activities[0]
                last_ride = {
                    'distance_km': round(last.get('distance', 0) / 1000, 1),
                    'avg_power': int(last.get('average_watts', 0)),
                    'avg_hr': int(last.get('average_heartrate', 0)) if 'average_heartrate' in last else 0
                }
                result['status'] = "ok"

            for act in activities:
                act_date = datetime.strptime(act['start_date_local'], "%Y-%m-%dT%H:%M:%SZ")
                
                # Today
                if act_date >= start_of_today:
                    today_stats['distance'] += act.get('distance', 0)
                    today_stats['moving_time'] += act.get('moving_time', 0)
                    today_stats['elevation'] += act.get('total_elevation_gain', 0)
                
                # This Week
                if act_date >= start_of_week:
                    week_stats['distance'] += act.get('distance', 0)
                    week_stats['moving_time'] += act.get('moving_time', 0)

            # Update Result for Recent
            result['today'] = {
                "distance_km": round(today_stats['distance'] / 1000, 1),
                "time": format_time_long(today_stats['moving_time']),
                "elev_m": int(today_stats['elevation'])
            }
            result['last'] = last_ride
            result['week'] = {
                "distance_km": round(week_stats['distance'] / 1000, 0),
                "time": format_time(week_stats['moving_time'])
            }

        # --- Process Year Stats ---
        if year_stats_raw:
            # stats object has 'ytd_ride_totals', 'recent_ride_totals', 'all_ride_totals'
            ytd = year_stats_raw.get('ytd_ride_totals', {})
            if ytd:
                y_dist = ytd.get('distance', 0)
                y_elev = ytd.get('elevation_gain', 0)
                y_time = ytd.get('moving_time', 0)
                
                # Format Year Time as "123h 45m"
                y_h, y_m = divmod(y_time // 60, 60)
                
                result['year'] = {
                    "year_val": now.year,
                    "range": f"{now.year}年1月1日 - {now.year}年12月31日",
                    "distance_km": int(y_dist / 1000),
                    "elev_m": int(y_elev),
                    "time": f"{int(y_h)}h {int(y_m)}m"
                }

        return result
