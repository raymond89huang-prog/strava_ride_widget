using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using RideBoard.Widget.Models;

namespace RideBoard.Widget.Services
{
    public class StravaService
    {
        private readonly HttpClient _http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        private StravaPayload? _cache;
        private DateTime _lastApiFetch = DateTime.MinValue;
        private readonly TimeSpan _apiInterval = TimeSpan.FromMinutes(10);

        public async Task<(StravaPayload? payload, bool online)> GetDataAsync(CancellationToken ct, bool forceRefresh = false)
        {
            var shouldCall = forceRefresh || DateTime.UtcNow - _lastApiFetch >= _apiInterval || _cache == null;
            if (!shouldCall)
            {
                return (_cache, true);
            }
            try
            {
                var url = "http://127.0.0.1:8787/strava";
                if (forceRefresh) 
                {
                    url += "?refresh=true&_t=" + DateTime.UtcNow.Ticks;
                }
                else
                {
                    url += "?_t=" + DateTime.UtcNow.Ticks;
                }

                var resp = await _http.GetAsync(url, ct);
                resp.EnsureSuccessStatusCode();
                var json = await resp.Content.ReadAsStringAsync(ct);
                var payload = JsonSerializer.Deserialize<StravaPayload>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                _cache = payload;
                _lastApiFetch = DateTime.UtcNow;
                return (_cache, true);
            }
            catch
            {
                return (_cache, false);
            }
        }
    }
}

