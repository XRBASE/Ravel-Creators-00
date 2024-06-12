using System;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Ravel/Quest", fileName = "Quest")]
public class QuestSo : ScriptableObject
{
    public QuestData questData;

    public void CreateQuest()
    {
        questData = new QuestData();
        questData.guid = Guid.NewGuid().ToString();
        questData.environmentId = Guid.NewGuid().ToString();
        SaveQuest();
    }

    public void SaveQuest()
    {
        string json = JsonConvert.SerializeObject(questData);
        PlayerPrefs.SetString(questData.guid, json);
    }

    public void DeleteQuest()
    {
        PlayerPrefs.DeleteKey(questData.guid);
        questData = null;
    }

    public void LoadQuest()
    {
        questData = JsonConvert.DeserializeObject<QuestData>(PlayerPrefs.GetString(questData.guid));
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(QuestSo))]
    private class QuestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            QuestSo instance = (QuestSo) target;
            DrawDefaultInspector();

            if (GUILayout.Button("Create Quest"))
            {
                instance.CreateQuest();
                EditorUtility.SetDirty(instance);
            }

            if (GUILayout.Button("Save Quest"))
            {
                instance.SaveQuest();
                EditorUtility.SetDirty(instance);
            }

            if (GUILayout.Button("Load Quest"))
            {
                instance.LoadQuest();
                EditorUtility.SetDirty(instance);
            }

            if (GUILayout.Button("Delete Quest"))
            {
                instance.DeleteQuest();
                EditorUtility.SetDirty(instance);
            }
        }
    }
#endif
}