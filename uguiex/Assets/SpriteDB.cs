
using System.Collections.Generic;
using UnityEngine;

namespace asset.ex {
	public class SpriteDB : MonoBehaviour {
		public Sprite[] sprites;
		public string editorDir;
		private Dictionary<string, Sprite> map;

		public Sprite GetSprite(string name)
		{
			if (map == null) 
			{
				map = new Dictionary<string, Sprite>();
				foreach (Sprite s in sprites)
				{
					map[s.name] = s;
				}
			}
			return map.Get(name);
		}
	}
}
