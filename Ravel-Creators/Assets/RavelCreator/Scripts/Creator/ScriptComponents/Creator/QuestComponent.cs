using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Base.Ravel.Creator.Components
{
    [AddComponentMenu("Ravel/Interactables/Quest")]
    public partial class QuestComponent : ComponentBase
    {
        public override ComponentData Data
        {
            get { return _data; }
        }

        [SerializeField] private QuestComponentData _data;


        protected override void BuildComponents()
        {
        }

        protected override void DisposeData()
        {
        }

        public void SetQuestAvailable()
        {
        }

        /// <summary>
        /// This is called to start the quest
        /// </summary>
        public void StartQuest()
        {
        }

        /// <summary>
        /// This is called when the requirements for completion are met
        /// </summary>
        public void ReturnQuest()
        {
        }

        /// <summary>
        /// This is called to complete the quest
        /// </summary>
        public void CompleteQuest()
        {
        }

        /// <summary>
        /// This opens the quest UI panel with the quest data
        /// </summary>
        public void ShowQuestPanel()
        {
        }
    }

    [Serializable]
    public class QuestComponentData : ComponentData
    {
        public QuestSo questSo;
        public bool startQuestAvailable;
        public List<QuestEvent> onQuestAvailable;
        public List<QuestEvent> onQuestStarted;
        public List<QuestEvent> onQuestReturn;
        public List<QuestEvent> onQuestCompleted;
    }
}