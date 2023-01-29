using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Slax.QuestSystem
{
    /// <summary>A Quest Line is a series of quests put together that should be done in succession.</summary>
    [CreateAssetMenu(menuName = "Slax/QuestSystem/QuestLine")]
    [System.Serializable]
    public class QuestLineSO : ScriptableObject
    {
        [Tooltip("The name of the Quest Line")]
        [SerializeField] private string _name;
        public string Name => _name;

        [Tooltip("List of quests")]
        [SerializeField] private List<QuestSO> _quests = new List<QuestSO>();
        public List<QuestSO> Quests => _quests;

        public UnityAction OnCompleted = delegate { };

        private bool _completed = false;
        public bool Completed => _completed;

        /// <summary>
        /// Should take in some QuestLine data from the save system or other and setup the completion state
        /// OR SHOULD TAKE IN STRING JSON DATA AND CONVERT IT
        /// </summary>
        public void Initialize(List<QuestSO> savedQuests)
        {
            _quests = savedQuests;
            _completed = AllQuestsCompleted(savedQuests);

            foreach (QuestSO quest in _quests)
            {
                if (!quest.Completed)
                {
                    quest.OnCompleted += HandleQuestCompletedEvent;
                }
            }
        }

        /// <summary>Checks if all quests in the questline are completed</summary>
        private bool AllQuestsCompleted()
        {
            return !_quests.Find((QuestSO quest) => quest.Completed == false);
        }

        private bool AllQuestsCompleted(List<QuestSO> quests)
        {
            return quests.Find((QuestSO quest) => quest.Completed == false);
        }

        /// <summary>Handles the event fired by a quest when it's completed</summary>
        private void HandleQuestCompletedEvent(QuestSO quest)
        {
            quest.OnCompleted -= HandleQuestCompletedEvent;
            _completed = AllQuestsCompleted();
            if (_completed) OnCompleted.Invoke();
        }

        public string ToJSON()
        {
            return JsonUtility.ToJson(this);
        }
    }
}