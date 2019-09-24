using mulova.unicore;

public static class PlatformConfig
{
    public static string PACKAGE_NAME { get; private set; }

    public static bool DEBUGGABLE { get; private set; }

    public static string CRITTERCISM { get; private set; }

    public static string ADBRIX_APPKEY { get; private set; }

    public static string ADBRIX_HASHKEY { get; private set; }

    public static string APPSFLYER_DEVKEY { get; private set; }

    public static string MARKET { get; private set; }

    public static string HELPSHIFT_APIKEY { get; private set; }

    public static string HELPSHIFT_APPID { get; private set; }

    public static string HELPSHIFT_GCM { get; private set; }

    public static string HELPSHIFT_DOMAIN { get; private set; }

    static PlatformConfig()
    {
		PACKAGE_NAME = Platform.conf.GetString("package_name", string.Empty);
        DEBUGGABLE = Platform.conf.GetBool("debuggable", false);
        MARKET = Platform.conf.GetString("market", string.Empty);

        CRITTERCISM = Platform.conf.GetString("crittercism_appid", string.Empty);
        ADBRIX_APPKEY = Platform.conf.GetString("adbrix_appkey", string.Empty);
        ADBRIX_HASHKEY = Platform.conf.GetString("adbrix_hashkey", string.Empty);
        APPSFLYER_DEVKEY = Platform.conf.GetString("appsflyer_devkey", string.Empty);
        HELPSHIFT_APIKEY = Platform.conf.GetString("helpshift_apikey", string.Empty);
        HELPSHIFT_APPID = Platform.conf.GetString("helpshift_appid", string.Empty);
        HELPSHIFT_GCM = Platform.conf.GetString("helpshift_gcm", string.Empty);
        HELPSHIFT_DOMAIN = Platform.conf.GetString("helpshift_domain", string.Empty);
    }
}