using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Slax.QuestSystem
{
    [System.Serializable]
    public class QuestStepSO : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private bool _completed;

        public string Name => _name;
        public bool Completed => _completed;

        public void SetCompleted(bool completed) => _completed = completed;
    }
}
