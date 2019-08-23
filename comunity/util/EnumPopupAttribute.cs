using UnityEngine;

public class EnumPopupAttribute : PropertyAttribute
{
    public readonly string enumVar;

    public EnumPopupAttribute(string enumVar)
    {
        this.enumVar = enumVar;
    }
}
