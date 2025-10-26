using System;
using System.Text;
using System.Text.Json;

namespace TravelBooking.Application.Utils;

public static class CursorHelper
{
    // Simple opaque Base64 JSON cursor encoder/decoder.
    public static string Encode<T>(T payload)
    {
        var json = JsonSerializer.Serialize(payload);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }

    public static T? Decode<T>(string? cursor)
    {
        if (string.IsNullOrWhiteSpace(cursor)) return default;
        try
        {
            var bytes = Convert.FromBase64String(cursor);
            var json = Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
            return default;
        }
    }
}