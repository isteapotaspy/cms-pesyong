namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;

public static class ClientApiUrls
{
    public static string ApiBaseUrl =>
#if ANDROID
        "http://10.0.2.2:5010/";
#else
        "http://localhost:5010/";
#endif

    public static string ToAbsolute(string? rawUrl)
    {
        if (string.IsNullOrWhiteSpace(rawUrl))
            return string.Empty;

        var value = rawUrl.Trim();

        if (value.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            value.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
#if ANDROID
            value = value.Replace("http://localhost:5010/", ApiBaseUrl, StringComparison.OrdinalIgnoreCase);
            value = value.Replace("https://localhost:5010/", ApiBaseUrl, StringComparison.OrdinalIgnoreCase);
            value = value.Replace("http://127.0.0.1:5010/", ApiBaseUrl, StringComparison.OrdinalIgnoreCase);
            value = value.Replace("https://127.0.0.1:5010/", ApiBaseUrl, StringComparison.OrdinalIgnoreCase);
#endif
            return value;
        }

        return new Uri(new Uri(ApiBaseUrl), value.TrimStart('/')).ToString();
    }
}