using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTester : MonoBehaviour
{
    [Serializable]
    public class DialogueLine
    {
        public string speakerName;

        [TextArea(2, 5)]
        public string dialogueText;
    }

    [Header("Scenario Content")]
    [SerializeField] private DialogueLine[] dialogueLines;

    [SerializeField] private string[] choiceLabels =
    {
        "VERY LIKELY",
        "SOMEWHAT LIKELY",
        "UNSURE",
        "SOMEWHAT UNLIKELY",
        "VERY UNLIKELY"
    };

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private RectTransform dialoguePanelSize;
    [SerializeField] private RectTransform dialogueTextSize;
    [SerializeField] private Button clickAnywhereButton;

    [Header("Choice UI")]
    [SerializeField] private GameObject choicesUI;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private TMP_Text[] choiceButtonTexts;

    private int dialogueIndex = 0;

    private void Start()
    {
        dialogueUI.SetActive(true);
        choicesUI.SetActive(false);

        dialogueIndex = 0;
        ShowCurrentDialogue();

        clickAnywhereButton.onClick.AddListener(AdvanceDialogue);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int capturedIndex = i;

            if (i < choiceButtonTexts.Length && i < choiceLabels.Length)
            {
                choiceButtonTexts[i].text = choiceLabels[i];
            }

            choiceButtons[i].onClick.AddListener(() => SelectChoice(capturedIndex));
        }
    }

    private void ShowCurrentDialogue()
    {
        if (dialogueLines.Length == 0)
        {
            Debug.LogWarning("No dialogue lines assigned.");
            return;
        }

        DialogueLine currentLine = dialogueLines[dialogueIndex];

        nameText.text = currentLine.speakerName;
        dialogueText.text = currentLine.dialogueText;
    }

    private void AdvanceDialogue()
    {
        dialogueIndex++;

        if (dialogueIndex >= dialogueLines.Length)
        {
            ShowChoices();
            return;
        }

        ShowCurrentDialogue();
    }

    private void ShowChoices()
    {
        dialoguePanelSize.sizeDelta = new Vector2(280f, dialoguePanelSize.sizeDelta.y);
        dialogueTextSize.sizeDelta = new Vector2(240f, dialogueTextSize.sizeDelta.y);

        choicesUI.SetActive(true);
        clickAnywhereButton.gameObject.SetActive(false);
    }

    private void SelectChoice(int index)
    {
        string selectedChoice = choiceLabels[index];

        Debug.Log("Choice selected: " + selectedChoice);

        choicesUI.SetActive(false);
        dialogueText.text = "You selected: " + selectedChoice;
    }
}