
using UnityEngine;

namespace comunity {

	public enum AssetType {
		Asset,
		StreamingAudio,
		RawTexture,
		Bytes,
	}

	public static class AssetTypeEx {
		public static bool IsRawAsset(this AssetType type) {
			return !type.IsAssetBundle();
		}

		public static bool IsAssetBundle(this AssetType type) {
			return type == AssetType.Asset;
		}

		public static bool IsStreamingAudio(this AssetType type) {
			return type == AssetType.StreamingAudio;
		}
	}
}