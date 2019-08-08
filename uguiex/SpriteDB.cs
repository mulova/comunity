using System.Collections.Generic;
using UnityEngine;

namespace uguiex {
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
            if (map.ContainsKey(name))
            {
                return map[name];
            } else
            {
                return null;
            }
		}
	}
}
