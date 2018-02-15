using UnityEngine;

namespace comunity
{
    public static class SystemLanguageEx 
    {
        public static bool IsEasternCurrency(this SystemLanguage lang) {
            switch (lang) {
            case SystemLanguage.Korean:
            case SystemLanguage.Japanese:
            case SystemLanguage.Chinese:
                return true;
            default:
                return false;
            }
        }

        public static string GetCode(this SystemLanguage lang) {
            switch (lang)
            {
                case SystemLanguage.Korean:
                    return "ko-KR";
                case SystemLanguage.Japanese:
                    return "ja-JP";
                case SystemLanguage.Chinese:
                    return "zh-CN";
                case SystemLanguage.Afrikaans:
                    return "af-ZA";
                case SystemLanguage.Arabic:
                    return "ar-AE"; // Arab Emirate
                case SystemLanguage.Basque:
                    return "eu-ES";
                case SystemLanguage.Belarusian:
                    return "be-BY";
                case SystemLanguage.Bulgarian:
                    return "bg-BG";
                case SystemLanguage.Catalan:
                    return "ca-ES";
                case SystemLanguage.Czech:
                    return "cs-CZ";
                case SystemLanguage.Danish:
                    return "da-DK";
                case SystemLanguage.Dutch:
                    return "nl-NL"; // netherlands
                case SystemLanguage.English:
                    return "en-US";
                case SystemLanguage.Estonian:
                    return "et-EE";
                case SystemLanguage.Faroese:
                    return "fo-FO";
                case SystemLanguage.Finnish:
                    return "fi-FI";
                case SystemLanguage.French:
                    return "fr-FR"; // france
                case SystemLanguage.German:
                    return "de-DE"; // germany
                case SystemLanguage.Greek:
                    return "el-GR";
                case SystemLanguage.Hebrew:
                    return "he-IL";
                case SystemLanguage.Hungarian:
                    return "hu-HU";
                case SystemLanguage.Icelandic:
                    return "is-IS";
                case SystemLanguage.Indonesian:
                    return "id-ID";
                case SystemLanguage.Italian:
                    return "it-IT"; // italy
                case SystemLanguage.Latvian:
                    return "lv-LV";
                case SystemLanguage.Lithuanian:
                    return "lt-LT";
                case SystemLanguage.Norwegian:
                    return "nn-NO"; //Nynorsk
                case SystemLanguage.Polish:
                    return "pl-PL";
                case SystemLanguage.Portuguese:
                    return "pt-PT"; // portugal
                case SystemLanguage.Romanian:
                    return "ro-RO";
                case SystemLanguage.Russian:
                    return "ru-RU";
                case SystemLanguage.SerboCroatian:
                    return "hr-HR";
                case SystemLanguage.Slovak:
                    return "sk-SK";
                case SystemLanguage.Slovenian:
                    return "sl-SI";
                case SystemLanguage.Spanish:
                    return "es-ES"; // spain
                case SystemLanguage.Swedish:
                    return "sv-SE"; // sweden
                case SystemLanguage.Thai:
                    return "th-TH";
                case SystemLanguage.Turkish:
                    return "tr-TR";
                case SystemLanguage.Ukrainian:
                    return "uk-UA";
                case SystemLanguage.Vietnamese:
                    return "vi-VN";
                case SystemLanguage.ChineseSimplified:
                    return "zh-CHS";
                case SystemLanguage.ChineseTraditional:
                    return "zh-CHT";
                case SystemLanguage.Unknown:
                    return "";
                default:
                    return "";
            }
        }
    }

}

