using mulova.comunity;
using mulova.preprocess;
using mulova.unicore;
using UnityEngine;
using UnityEngine.Ex;

namespace mulova.build
{
    public class RendererBuildProcessor : ComponentBuildProcess
	{
		protected override void Verify(Component comp)
		{
			Renderer r = comp as Renderer;
			// Dereference cdn texture
			DerefCdnTexture(r);
		}

		private void DerefCdnTexture(Renderer r)
		{
			if (r.sharedMaterials.Length == 1&&r.sharedMaterials[0] != null)
			{
				var tex = r.sharedMaterial.mainTexture;
				MeshTexLoader loader = r.GetComponent<MeshTexLoader>();
				if (tex != null&&AssetBundlePath.inst.IsCdnAsset(tex)
					&& (loader == null || IsTextureMismatch(r)))
				{
					loader = r.FindComponent<MeshTexLoader>();
					loader.rend = r;
					MeshTexSetter setter = r.FindComponent<MeshTexSetter>();
					var aref = new AssetRef();
					aref.SetPath(tex);
					setter.textures.Clear();
					setter.textures.Add(aref);
					EditorUtil.SetDirty(r.gameObject);
                    EditorUtil.SetDirty(loader);
                    EditorUtil.SetDirty(setter);
				}
			}
		}

		// check if applied by prefab and instance has different texture
		private bool IsTextureMismatch(Renderer r)
		{
			MeshTexSetter s = r.GetComponent<MeshTexSetter>();
			if (s == null || s.textures.Count == 0)
			{
				return false;
			}
			return s.textures.Count == 1 && s.textures[0].path != EditorAssetUtil.GetAssetRelativePath(r.sharedMaterial.mainTexture);
		}

		protected override void Preprocess(Component comp)
		{
		}

		protected override void Postprocess(Component c)
		{
		}

		public override System.Type compType
		{
			get
			{
				return typeof(Renderer);
			}
		}
	}
}
