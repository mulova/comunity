using mulova.commons;
using UnityEditor;
using System;

namespace comunity
{
    public static class TextureImporterEx
    {
		private static bool allPlatforms = false;
		public static string[] platforms {
			get {
				if (allPlatforms)
				{
					return new string[] { "Android", "iPhone" };
				} else
				{
					#if UNITY_ANDROID
					return new string[] { "Android" };
					#else
					return new string[] {"iPhone"};
					#endif
				}
			}
		}

        public static TextureImporterFormat GetFormat(this TextureImporter im)
        {
            TextureImporterPlatformSettings setting = im.GetDefaultPlatformTextureSettings();
            return setting.format;
        }

        public static void SetFormat(this TextureImporter im, TextureImporterFormat format)
        {
            TextureImporterPlatformSettings setting = im.GetDefaultPlatformTextureSettings();
            setting.overridden = true;
            setting.format = format;
            im.SetPlatformTextureSettings(setting);
        }

        public static void SetFormat(this TextureImporter im, PlatformId platformId, TextureImporterFormat format)
        {
            TextureImporterPlatformSettings setting = im.GetDefaultPlatformTextureSettings();
            setting.overridden = true;
            setting.format = format;
            im.SetPlatformTextureSettings(setting);
        }

        public static int GetMaxTextureSize(this TextureImporter im)
        {
            TextureImporterPlatformSettings setting = im.GetDefaultPlatformTextureSettings();
            return setting.maxTextureSize;
        }

        public static void SetMaxTextureSize(this TextureImporter im, int maxSize)
        {
			im.ForEachPlatform((setting, path) => {
				if (setting.maxTextureSize != maxSize)
				{
					setting.maxTextureSize = maxSize;
					return true;
				} else{
					return false;
				}
			});
        }

		public static bool ForEachPlatform(this TextureImporter im, Func<TextureImporterPlatformSettings, string, bool> action, bool overridenOnly = false)
		{
			bool changed = false;
			foreach (string p in platforms) {
				var setting = im.GetPlatformTextureSettings(p);
				if ((setting.overridden || !overridenOnly) && action(setting, p)) {
					im.SetPlatformTextureSettings(setting);
					im.SaveAndReimport();
					changed = true;
				}
			}
			return changed;
		}
    }

    public class PlatformId : EnumClass<PlatformId>
    {
        public static readonly PlatformId STANDALONE = new PlatformId("Standalone");
        public static readonly PlatformId IPHONE = new PlatformId("iPhone");
        public static readonly PlatformId ANDROID = new PlatformId("Android");
        public static readonly PlatformId WEBGL = new PlatformId("WebGL");
        public static readonly PlatformId WINDOWS_STORE_APP = new PlatformId("Windows Store Apps");
        public static readonly PlatformId TIZEN = new PlatformId("Tizen");
        public static readonly PlatformId PS2 = new PlatformId("PSP2");
        public static readonly PlatformId PS4 = new PlatformId("PS4");
        public static readonly PlatformId XBOX = new PlatformId("XboxOne");
        public static readonly PlatformId SAMSUNG_TV = new PlatformId("Samsung TV");
        public static readonly PlatformId NINTENDO_3DS = new PlatformId("Nintendo 3DS");
        public static readonly PlatformId WIIU = new PlatformId("WiiU");
        public static readonly PlatformId TVOS = new PlatformId("tvOS");
        public static readonly PlatformId[] PLATFORMS = { ANDROID, IPHONE, WEBGL, STANDALONE };

        private PlatformId(string name) : base(name) {}
    }
}
