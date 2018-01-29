using UnityEditor;
using UnityEngine;

namespace etc {
	class RenderCubemapWizard : ScriptableWizard {
		public Transform renderFromPosition = null;
		public string cubemapName = "cubemap";
		public TextureFormat textureFormat = TextureFormat.ARGB32;
		public bool mipmap = false;
		public Cubemap cubemap;
		
		void OnWizardUpdate () {
			if (renderFromPosition == null) {
				errorString = "Select transform to render from and cubemap to render into";
			}
			isValid = (renderFromPosition != null);
		}
		
		void OnWizardCreate () {
			// create temporary camera for rendering
			GameObject go = new GameObject( "CubemapCamera", typeof(Camera) );
			// place it on the object
			go.transform.position = renderFromPosition.position;
			go.transform.rotation = Quaternion.identity;
			
			if (cubemap == null) {
				cubemap = new Cubemap(128, textureFormat, mipmap);
				AssetDatabase.CreateAsset(cubemap, "Assets/"+cubemapName+".cubemap");
			}
			
			// render into cubemap        
			go.GetComponent<Camera>().RenderToCubemap( cubemap );
			
			// destroy temporary camera
			DestroyImmediate( go );
		}
		
		[MenuItem("Tools/unilova/Camera/Render into Cubemap")]
		static void RenderCubemap () {
			ScriptableWizard.DisplayWizard<RenderCubemapWizard>("Render cubemap", "Render!");
		}
	}
}
