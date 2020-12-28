using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameSettingEntity), true)]
public class GameSettingEntityEditor : Editor
{
    GameSettingEntity gme;

    public override void OnInspectorGUI()
    {
        gme = (GameSettingEntity)target;

        base.OnInspectorGUI();

        if (GUILayout.Button("Save Setting"))
        {
            gme.SaveSetting();
        }

        if (GUILayout.Button("Load Setting"))
        {
            gme.LoadSetting();
        }

        if (GUILayout.Button("Load Master Setting"))
        {
            gme.LoadGameSettingFromMaster();
        }   

    }
}
