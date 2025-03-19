using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(HUD))]
public class HUDEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 인스펙터 표시
        DrawDefaultInspector();

        // 타겟 오브젝트
        HUD hud = (HUD)target;

        // InfoType에 따라 다른 변수를 인스펙터에 표시
        switch (hud.type)
        {
            case HUD.InfoType.Exp:
                EditorGUILayout.LabelField("Experience Info", EditorStyles.boldLabel);
                hud.curExp = EditorGUILayout.FloatField("Current Exp", hud.curExp);
                hud.maxExp = EditorGUILayout.FloatField("Max Exp", hud.maxExp);
                break;
            case HUD.InfoType.Health:
                EditorGUILayout.LabelField("Health Info", EditorStyles.boldLabel);
                hud.curHealth = EditorGUILayout.FloatField("Current Health", hud.curHealth);
                hud.maxHealth = EditorGUILayout.FloatField("Max Health", hud.maxHealth);
                break;
            case HUD.InfoType.Level:
            case HUD.InfoType.Kill:
            case HUD.InfoType.Time:
                EditorGUILayout.LabelField("Text Info", EditorStyles.boldLabel);
                break;
        }

        // 변경사항이 생기면 타겟 오브젝트를 업데이트
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
