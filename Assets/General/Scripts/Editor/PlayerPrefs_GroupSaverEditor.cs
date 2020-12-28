#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;

public class PlayerPrefs_GroupSaverEditor : OdinEditorWindow
{
    public DataField[] dataFields;

    [MenuItem("Tools/PlayerPrefs_Group")]
    public static void OpenWindow()
    {
        var window = GetWindow<PlayerPrefs_GroupSaverEditor>();

        // Nifty little trick to quickly position the window in the middle of the editor.
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(400, 400);
    }

    protected override IEnumerable<object> GetTargets()
    {
        PlayerPrefsSave_Group ps = Selection.activeGameObject.GetComponent<PlayerPrefsSave_Group>();
        if (ps != null) dataFields = ps.dataFields;
        else { Debug.LogWarning("Please select object with PlayerPrefsSave_Group component attached!"); yield break; }

        // Draws this instance using Odin
        yield return this;

    }
}
#endif

