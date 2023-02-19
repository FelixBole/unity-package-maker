using UnityEngine;
using UnityEngine.Events;
namespace Slax.QuestSystem
{
    /// <summary>
    /// Class handling the component's enable state depending on
    /// the quest step it should be available for
    /// </summary>
    public class QuestStepEnabler : MonoBehaviour
    {
        [SerializeField] private QuestSO _quest;
        [SerializeField] private QuestStepSO _questStep;
        private bool _isEnabled;
        public bool IsEnabled => _isEnabled;

        public UnityAction<bool> OnEnableChange = delegate { };

        private void Awake()
        {
            int stepIdx = _quest.GetStepIndex(_questStep);
            if (stepIdx == -1) throw new System.Exception("Cannot use a step that isn't in a quest.");

            _isEnabled = _quest.AllPreviousStepsCompleted(_questStep);
            OnEnableChange.Invoke(_isEnabled);
        }

        private void OnEnable()
        {
            QuestManager.Instance.OnStepComplete += VerifyActivation;
            _questStep.OnCompleted += HandleStepComplete;
        }

        private void OnDisable()
        {
            QuestManager.Instance.OnStepComplete -= VerifyActivation;
            _questStep.OnCompleted -= HandleStepComplete;
        }

        private void HandleStepComplete(QuestStepSO step)
        {
            _isEnabled = false;
            OnEnableChange.Invoke(_isEnabled);
        }

        private void VerifyActivation(QuestEventInfo eventInfo)
        {
            if (eventInfo.Quest.name != _quest.name) return;
            _isEnabled = _quest.AllPreviousStepsCompleted(_questStep);
            OnEnableChange.Invoke(_isEnabled);
        }
    }

}