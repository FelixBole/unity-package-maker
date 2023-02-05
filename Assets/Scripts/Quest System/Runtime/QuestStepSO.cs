using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Slax.QuestSystem
{
    /// <summary>
    /// Every step of a quest from its starting point to the goal of the quest.
    /// The action of starting a quest always corresponds to step 0, so a quest
    /// giver holds quest step 0 of a quest and validates that step to start the quest.
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(menuName = "Slax/QuestSystem/QuestStep", fileName = "QL0_Q0_S0")]
    public class QuestStepSO : ScriptableObject
    {
        [Tooltip("In order to organise steps correctly, it is recommended to use the format QuestLine_Quest_QuestStep in the name: QLX_QX_SX, where X is a number")]
        [SerializeField] private string _name;
        [SerializeField] private bool _completed = false;

        public UnityAction<QuestStepSO> OnCompleted = delegate { };

        public string Name => _name;
        public bool Completed => _completed;
        public void SetCompleted(bool completed)
        {
            _completed = completed;
            OnCompleted.Invoke(this);
        }

        /// <summary>
        /// Initializes the completion state as completed without firing an event.
        /// Used for loading from save data to setup quests
        /// </summary>
        public void InitAsCompleted() => _completed = true;
    }
}
