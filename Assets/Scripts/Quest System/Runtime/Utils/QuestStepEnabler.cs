using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slax.QuestSystem
{
    /// <summary>
    /// Class handling the component's enable state depending on
    /// the quest step it should be available for
    /// </summary>
    public class QuestStepEnabler : MonoBehaviour
    {
        public enum Handles { GameObject, PublicBool };
        [SerializeField] private QuestSO _quest;
        [SerializeField] private QuestStepSO _questStep;

        [Header("If the component should handle the enable state of the gameObject or share a public bool for another script to handle")]
        public Handles Manage = Handles.GameObject;
        private bool _isEnabled;
        public bool IsEnabled => _isEnabled;

        private void Awake()
        {
            int stepIdx = _quest.GetStepIndex(_questStep);
            if (stepIdx == -1) throw new System.Exception("Cannot use a step that isn't in a quest.");

            _isEnabled = _quest.AllPreviousStepsCompleted(_questStep);
            
            if (Manage == Handles.GameObject) gameObject.SetActive(_isEnabled);
        }

        private void OnEnable()
        {
            _questStep.OnCompleted += HandleStepComplete;
        }

        private void OnDisable()
        {
            _questStep.OnCompleted -= HandleStepComplete;
        }

        private void HandleStepComplete(QuestStepSO step)
        {
            _isEnabled = false;
            if (Manage == Handles.GameObject) gameObject.SetActive(_isEnabled);
        }
    }

}