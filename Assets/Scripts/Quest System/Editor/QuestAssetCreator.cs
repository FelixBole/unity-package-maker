using UnityEditor;
using UnityEngine;

namespace Slax.QuestSystem
{
    public static class QuestAssetCreator
    {
        public static void CreateQuestLine(string path)
        {
            if (path == "") path = "Assets";
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<QuestLineSO>(), $"{path}/QL0.asset");
            Debug.Log($"Created asset at path {path}/QL0");
        }

        public static void CreateQuest(string path)
        {
            if (path == "") path = "Assets";
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<QuestLineSO>(), $"{path}/QL0_Q0.asset");
            Debug.Log($"Created asset at path {path}/QL0_Q0");
        }

        public static void CreateQuestStep(string path)
        {
            if (path == "") path = "Assets";
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<QuestLineSO>(), $"{path}/QL0_Q0_S0.asset");
            Debug.Log($"Created asset at path {path}/QL0_Q0_SO");
        }
    }
}
