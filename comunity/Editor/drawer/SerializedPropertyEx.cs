using System;
using Object = UnityEngine.Object;

namespace UnityEditor
{
    public static class SerializedPropertyEx
    {
        public static object GetValue(this SerializedProperty p)
        {
            switch (p.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return p.intValue;
                case SerializedPropertyType.Boolean:
                    return p.boolValue;
                case SerializedPropertyType.Float:
                    return p.floatValue;
                case SerializedPropertyType.String:
                    return p.stringValue;
                case SerializedPropertyType.Color:
                    return p.colorValue;
                case SerializedPropertyType.ObjectReference:
                    return p.objectReferenceValue;
                case SerializedPropertyType.LayerMask:
                    return p.intValue;
                case SerializedPropertyType.Enum:
                    return p.enumValueIndex;
                case SerializedPropertyType.Vector2:
                    return p.vector2Value;
                case SerializedPropertyType.Vector3:
                    return p.vector3Value;
                case SerializedPropertyType.Vector4:
                    return p.vector4Value;
                case SerializedPropertyType.Rect:
                    return p.rectValue;
                case SerializedPropertyType.ArraySize:
                    return p.arraySize;
                case SerializedPropertyType.Character:
                    return p.intValue;
                case SerializedPropertyType.AnimationCurve:
                    return p.animationCurveValue;
                case SerializedPropertyType.Bounds:
                    return p.boundsValue;
                case SerializedPropertyType.Gradient:
                    return p.colorValue;
                case SerializedPropertyType.Quaternion:
                    return p.quaternionValue;
                case SerializedPropertyType.ExposedReference:
                    return p.exposedReferenceValue;
                case SerializedPropertyType.FixedBufferSize:
                    return p.fixedBufferSize;
                case SerializedPropertyType.Vector2Int:
                    return p.vector2IntValue;
                case SerializedPropertyType.Vector3Int:
                    return p.vector3IntValue;
                case SerializedPropertyType.RectInt:
                    return p.rectIntValue;
                case SerializedPropertyType.BoundsInt:
                    return p.boundsIntValue;
                default:
                    throw new Exception("Unreachable");
            }
        }
    }
}

