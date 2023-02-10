using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Slax.QuestSystem
{
    /// <summary>
    /// Component to add to any interactable item or character that can start a quest
    /// or complete a quest step.
    /// </summary>
    public class QuestPoint : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private QuestStepSO _questStep = null;
        public bool Completed => _questStep.Completed;

        [Header("Events")]
        /// <summary>
        /// Event fired when a step has already been validated and trying
        /// to complete it again.
        /// </summary>
        public UnityEvent<QuestStepSO> OnStepAlreadyValidated;

        /// <summary>
        /// Event fired when a step is validated. It is worth noting that
        /// the Quest Manager also fires an event with the step, the associated
        /// quest and the questline (QuestEventInfo), but this event allows for
        /// some additionnal easy direct customization before the event fired
        /// by the Quest Manager Singleton Instance
        /// </summary>
        public UnityEvent<QuestStepSO> OnStepValidated;

        /// <summary>
        /// Tries to complete the quest step. If the step has already been completed
        /// will fire the OnStepAlreadyValidated event and return. Otherwise, it will
        /// first fire the OnStepValidated event then launch the Quest Manager step
        /// validation pipeline resulting in the Quest Manager firing the full QuestEventInfo
        /// </summary>
        public void DoQuestStep()
        {
            if (Completed)
            {
                OnStepAlreadyValidated.Invoke(_questStep);
                return;
            }
            else OnStepValidated.Invoke(_questStep);
            _questStep.SetCompleted(true);
        }
    }
}
