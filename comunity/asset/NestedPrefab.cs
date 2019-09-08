#if FULL
using UnityEngine;
using System.Collections;

namespace mulova.comunity
{
	public class NestedPrefab : MonoBehaviour {
		public bool instantiateOnStart;	
		public bool destroyOnInstantiation;	
		[SerializeField]
		private GameObject prefab;
		[SerializeField]
		private bool linked;
		
		void Start() {
			if (prefab != null) {
				if (instantiateOnStart) {
					Instantiate();
				}
			}
		}
		
		public void Instantiate() {
			GameObject instance = prefab.InstantiateEx();
			Transform dst = instance.transform;
			dst.parent = transform.parent;
			dst.localPosition = transform.localPosition;
			dst.localScale = transform.localScale;
			dst.localRotation = transform.localRotation;
			if (destroyOnInstantiation) {
				Object.Destroy(gameObject);
			}
		}
		
		public bool IsLinked() {
			return linked;
		}
		
		public GameObject GetPrefab() {
			return prefab;
		}
		
		public void Link(GameObject prefab) {
			this.linked = true;
			this.prefab = prefab;
		}
		
		public void ClearLink() {
			this.linked = false;
			this.prefab = null;
		}
	}
}

#endif