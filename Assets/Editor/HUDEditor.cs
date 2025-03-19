using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(HUD))]
public class HUDEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // �⺻ �ν����� ǥ��
        DrawDefaultInspector();

        // Ÿ�� ������Ʈ
        HUD hud = (HUD)target;

        // InfoType�� ���� �ٸ� ������ �ν����Ϳ� ǥ��
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

        // ��������� ����� Ÿ�� ������Ʈ�� ������Ʈ
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
