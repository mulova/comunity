//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using commons;

namespace comunity {
	public class RenderUtil {
		
		
		/**
		 * Shader를 map에서 기술된 대로 다른 Shader로 대치한다.
		 * #@param matParams 특정 material 설정을 적용한다.
		 */
        public static void ReplaceShader(GameObject obj, Dictionary<string, string> map, params Apply<Material>[] matParams) {
			Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
			foreach (Renderer r in renderers) {
				foreach (Material m in r.materials) {
					if (map.ContainsKey(m.shader.name)) {
						m.shader = Shader.Find(map[m.shader.name]);
						foreach (Apply<Material> a in matParams) {
							a.Apply(m);
						}
					}
				}
			}
		}
		
		
		/**
		 * 대치할수 있는 Material만 대치한다.
		 */
		public static void ReplaceMaterial(GameObject obj, Dictionary<string, string> map) {
			Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
			foreach (Renderer r in renderers) {
				Material[] mats = new Material[r.materials.Length];
				for (int i=0; i<mats.Length; i++) {
					Material src = r.materials[i];
					string replace = null;
					map.TryGetValue(src.name, out replace);
					if (replace != null) {
						mats[i] = Resources.Load(replace) as Material;
					} else {
						mats[i] = src;
					}
				}
				r.materials = mats;
			}
		}
	}
}

