using System.Collections.Generic;

namespace Slax.QuestSystem
{
    /// <summary>
    /// Actual class that will be converted to JSON to be saved
    /// </summary>
    [System.Serializable]
    public class QuestSaveData
    {
        /// A list of references of quest steps with whether it has been completed or not
        /// 
        /// WARNING: QuestSteps with the same name/ID will cause error in the save data file
        public List<string> DoneQuestSteps = new List<string>();
    }
}
