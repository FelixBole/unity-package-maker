using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Slax.QuestSystem
{
    [CreateAssetMenu(menuName = "Slax/QuestSystem/Quest")]
    [System.Serializable]
    public class QuestSO : ScriptableObject
    {
        [SerializeField] private string _name;
        [TextArea]
        [SerializeField] private string _description;
        [SerializeField] private List<QuestStepSO> _steps;

        /// <summary>If the quest has been completed by the player</summary>
        private bool _completed = false;

        /// <summary>Event fired when the Quest is completed</summary>
        public UnityAction<QuestSO> OnCompleted = delegate { };

        public string Name => _name;
        public string Description => _description;
        public List<QuestStepSO> Steps => _steps;
        public bool Completed => _completed;

        /// <summary>
        /// TODO Look into making it some kind of ID system instead of a string lookup
        /// </summary>
        public void CompleteStep(string stepName)
        {
            QuestStepSO step = _steps.Find((QuestStepSO s) => s.name == stepName);
            if (step)
            {
                step.SetCompleted(true);

                bool hasIncompleteStep = _steps.Find((QuestStepSO s) => s.Completed == false);
                if (!hasIncompleteStep)
                {
                    OnCompleted.Invoke(this);
                }
            }
        }
    }

}
