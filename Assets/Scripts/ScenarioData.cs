using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewScenario",
    menuName = "Tough Choices/Scenario Data"
)]

public class ScenarioData : ScriptableObject
{
    [Serializable]
    public class DialogueLine
    {
        public string speakerName;

        [TextArea(2, 5)]
        public string dialogueText;

        public float typingSpeed;
        
        public DialogueLine()
        {
            typingSpeed = 0.03f;
        }
    }

    [Header("Scenario")]
    public string scenarioName;

    [Header("Environment")]
    public string sceneName;

    [Header("Dialogue")]
    public DialogueLine[] dialogueLines;

    [Header("Question")]
    [TextArea(2, 5)]
    public string questionText;
}