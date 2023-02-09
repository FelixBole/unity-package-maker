using UnityEngine;
using TMPro;
using Slax.QuestSystem;

public class QuestEventUIReceiver : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void OnEnable()
    {
        QuestManager.Instance.OnStepComplete += HandleStepComplete;
        QuestManager.Instance.OnQuestComplete += HandleQuestComplete;
        QuestManager.Instance.OnQuestLineComplete += HandleQuestLineComplete;
    }

    private void OnDisable()
    {
        QuestManager.Instance.OnStepComplete -= HandleStepComplete;
        QuestManager.Instance.OnQuestComplete -= HandleQuestComplete;
        QuestManager.Instance.OnQuestLineComplete -= HandleQuestLineComplete;
    }

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void HandleQuestLineComplete(QuestEventInfo eventInfo)
    {
        _text.text = $"Quest Line {eventInfo.QuestLine.Name} complete !";
    }

    private void HandleQuestComplete(QuestEventInfo eventInfo)
    {
        _text.text = $"Quest {eventInfo.Quest.Name} complete !";
    }

    private void HandleStepComplete(QuestEventInfo eventInfo)
    {
        string text;
        if (eventInfo.IsFirstStep) {
            text = $"Quest {eventInfo.Quest.Name} started !";
        } else {
            text = $"Step {eventInfo.Step.Name} of {eventInfo.Quest.Name} complete !";
        }
        _text.text = text;
    }
}
