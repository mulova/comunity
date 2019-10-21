using System;
using System.Collections.Generic;
using System.Text.Ex;
using mulova.commons;
using mulova.comunity;
using mulova.preprocess;
using mulova.unicore;
using UnityEditor;
using UnityEngine;

namespace mulova.build
{
    public class LexiconRegistryBuildProcessor : AssetBuildProcess
	{
        public override string title => "Lexicon Registry";
        public override Type assetType => typeof(LexiconRegistry);

		protected override void Preprocess(string path, UnityEngine.Object obj)
		{
			LexiconRegistry reg = obj as LexiconRegistry;
			if (reg.assetDir.isValid)
			{
				string dir = AssetDatabase.GUIDToAssetPath(reg.assetDir);
				if (!dir.IsEmpty())
				{
					dir = EditorAssetUtil.GetAssetRelativePath(dir);
					TextAsset[] assets = EditorAssetUtil.ListAssets<TextAsset>(dir, FileType.Text);
					AddAssets(reg, assets);
				}
			}
		}

		private void AddAssets(LexiconRegistry reg, TextAsset[] assets)
		{
			List<AssetRef> list = new List<AssetRef>();
			foreach (var a in assets)
			{
				if (a != reg.initial)
				{
					var r = new AssetRef();
					r.SetPath(a);
					list.Add(r);
				}
			}
			reg.assets = list.ToArray();
            EditorUtil.SetDirty(reg);
		}

		protected override void Verify(string path, UnityEngine.Object obj)
		{
		}

        protected override void Postprocess(string path, UnityEngine.Object obj)
        {
        }
    }
}
