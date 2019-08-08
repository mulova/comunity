using UnityEditor;
using UnityEngine;
using comunity;
using mulova.commons;

namespace uguiex {
	[CustomEditor(typeof(SpriteDB))]
	public class SpriteDBInspector : Editor {
		private SpriteDB db;
		private ObjArrInspector<Sprite> inspector;
		private Object dir;
		
		void OnEnable() {
			db = target as SpriteDB;
			inspector = new ObjArrInspector<Sprite>(db, "sprites", false);
			if (db.editorDir != null) {
				dir = AssetDatabase.LoadAssetAtPath(db.editorDir, typeof(Object));
			}
		}

		public override void OnInspectorGUI () 
		{
			inspector.OnInspectorGUI();
			if (EditorGUIUtil.ObjectField<Object>("Dir", ref dir, false))
			{
				if (dir == null) 
				{
					db.editorDir = string.Empty;
				} else 
				{
					db.editorDir = AssetDatabase.GetAssetPath(dir);
					db.sprites = SearchSprites(db.editorDir);
					EditorUtility.SetDirty(db);
				}
			}
		}

		public static Sprite[] SearchSprites(string dir) {
			string absDir = EditorAssetUtil.GetAssetFileFullPath(dir);
            foreach (Sprite s in EditorAssetUtil.ListAssets<Sprite>(absDir, FileType.Image)) {
			}
			return null;
		}


	}
	
}