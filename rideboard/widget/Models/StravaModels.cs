using System.Text.Json.Serialization;

namespace RideBoard.Widget.Models
{
    public class TodayModel
    {
        [JsonPropertyName("distance_km")]
        public double? DistanceKm { get; set; }
        [JsonPropertyName("time")]
        public string? Time { get; set; }
        [JsonPropertyName("elev_m")]
        public int? ElevationM { get; set; }
    }

    public class LastModel
    {
        [JsonPropertyName("distance_km")]
        public double? DistanceKm { get; set; }
        [JsonPropertyName("avg_power")]
        public int? AvgPower { get; set; }
        [JsonPropertyName("avg_hr")]
        public int? AvgHr { get; set; }
    }

    public class WeekModel
    {
        [JsonPropertyName("distance_km")]
        public double? DistanceKm { get; set; }
        [JsonPropertyName("time")]
        public string? Time { get; set; }
    }

    public class YearModel
    {
        [JsonPropertyName("year_val")]
        public int YearVal { get; set; }
        [JsonPropertyName("range")]
        public string? Range { get; set; }
        [JsonPropertyName("distance_km")]
        public int? DistanceKm { get; set; }
        [JsonPropertyName("elev_m")]
        public int? ElevM { get; set; }
        [JsonPropertyName("time")]
        public string? Time { get; set; }
    }

    public class StravaPayload
    {
        [JsonPropertyName("today")]
        public TodayModel? Today { get; set; }
        [JsonPropertyName("last")]
        public LastModel? Last { get; set; }
        [JsonPropertyName("week")]
        public WeekModel? Week { get; set; }
        [JsonPropertyName("year")]
        public YearModel? Year { get; set; }
        [JsonPropertyName("updated_at")]
        public string? UpdatedAt { get; set; }
        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }
}
