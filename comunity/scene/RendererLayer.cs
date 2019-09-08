using UnityEngine;

namespace mulova.comunity
{
	public class RendererLayer : MonoBehaviour{}
	
	public static class RendererEx {
		public static void SetRenderQueue(this Renderer r, int renderQueue, bool shared = false)
		{
			if (r == null) { return; }
			if (shared) {
				foreach (Material m in r.sharedMaterials) {
					m.renderQueue = renderQueue;
				}
			} else {
				foreach (Material m in r.materials) {
					m.renderQueue = renderQueue;
				}
			}
		}
		
	}
}