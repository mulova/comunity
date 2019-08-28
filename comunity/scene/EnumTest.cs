using UnityEngine;

    public class EnumTest : MonoBehaviour
    {
        [SerializeField, EnumType] private string enumType;
        [EnumPopup("enumType")] public string enumValue;
    }
