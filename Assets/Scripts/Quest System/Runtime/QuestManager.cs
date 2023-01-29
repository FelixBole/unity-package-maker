using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace Slax.QuestSystem
{
    /// <summary>
    /// Manager for all created and ongoing quests. Handles interactions and holds all relations to necessary entities.
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        [Tooltip("This references a class that holds the List of quest lines to be able to use the JsonUtility to convert it to JSON")]
        [SerializeField] private SerializableQuestLines _questLines;

        /// <summary>List of all quest lines in the game</summary>
        public List<QuestLineSO> QuestLines => _questLines.QuestLines;

        /// <summary>
        /// Sets up the quest lines from the save data
        /// </summary>
        public void Initialize(string jsonData)
        {
            List<QuestLineSO> savedQuestLines = JsonUtility.FromJson<List<QuestLineSO>>(jsonData);
            Debug.Log(savedQuestLines);
        }

        /// <summary>
        /// Returns the quest lines and their completion state in a
        /// saveable ready JSON format that the manager is able to read
        /// from on start to restore the quest lines' data
        /// </summary>
        [ContextMenu("Get Save Data")]
        public string GetSaveData()
        {
            string saveData = JsonUtility.ToJson(_questLines.QuestLines[0]);
            return saveData;
        }
    }

    [System.Serializable]
    public class SerializableQuestLines
    {
        public List<QuestLineSO> QuestLines = new List<QuestLineSO>();
    }
}
