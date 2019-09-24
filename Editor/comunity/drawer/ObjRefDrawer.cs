using mulova.unicore;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ObjRef))]
public class ObjRefDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var id = property.FindPropertyRelative("_id");
        var isAsset = property.FindPropertyRelative("_asset");
        var typeStr = property.FindPropertyRelative("_type");
        var obj = ObjRef.GetReference(id.stringValue, isAsset.boolValue, typeStr.stringValue);
        var type = obj?.GetType() ?? typeof(Object);

        var newObj = EditorGUI.ObjectField(position, label, obj, type, true);
        if (newObj != obj)
        {
            var newRef = new ObjRef(newObj);
            id.stringValue = newRef.id;
            isAsset.boolValue = newRef.asset;
            typeStr.stringValue = newRef.type?.FullName;
        }
    }
}
