//AlmostLogical Software - http://www.almostlogical.com
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace comunity
{
    public class CreateNewLine : EditorWindow
    {
        [MenuItem("Edit/Insert New Line &\r")]
        static void InsertNewLine()
        {
            EditorGUIUtility.systemCopyBuffer = System.Environment.NewLine;
            EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Paste"));
        }
    }
}