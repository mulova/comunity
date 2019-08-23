using UnityEngine;

    public class EnumTest : MonoBehaviour
    {
        [SerializeField] private string enumType;
        [EnumPopup("enumType")] public string enumValue;
    }
