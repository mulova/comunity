using UnityEngine;
using UnityEditor;
 
public static class PropertyUI {


	public static bool DrawObject<T>(Rect r, SerializedProperty p, bool allowSceneObject) {
		return DrawObject<T>(r, p.name, p, allowSceneObject);
	}

	public static bool DrawObject<T>(Rect r, string name, SerializedProperty p, bool allowSceneObject) {
		Object oldVal = p.objectReferenceValue;
		Object newVal = EditorGUI.ObjectField(r, name, oldVal, typeof(T), allowSceneObject);
		if (oldVal != newVal) {
			p.objectReferenceValue = newVal;
			return true;
		}
		return false;
	}

	public static bool DrawVector3(Rect r, SerializedProperty p) {
		Vector3 oldVal = p.vector3Value;
		Vector3 newVal = EditorGUI.Vector3Field(r, p.name, oldVal);
		if (oldVal != newVal) {
			p.vector3Value = newVal;
			return true;
		}
		return false;
	}

	public static bool DrawVector4(Rect r, SerializedProperty p) {
		Vector4 oldVal = p.vector4Value;
		Vector4 newVal = EditorGUI.Vector4Field(r, p.name, oldVal);
		if (oldVal != newVal) {
			p.vector4Value = newVal;
			return true;
		}
		return false;
	}

	public static bool DrawQuaternion(Rect r, SerializedProperty p) {
		Vector4 oldVal = QuaternionToVector4(p.quaternionValue);
		Vector4 newVal = EditorGUI.Vector4Field(r, p.name, oldVal);
		if (oldVal != newVal) {
			p.quaternionValue = Vector4ToQuaternion(newVal);
			return true;
		}
		return false;
	}

	private static Vector4 QuaternionToVector4(Quaternion rot) {
		return new Vector4(rot.x, rot.y, rot.z, rot.w);
	}

	private static Quaternion Vector4ToQuaternion(Vector4 v4) {
		return new Quaternion(v4.x, v4.y, v4.z, v4.w);
	}
 
}