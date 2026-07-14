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
    [SerializeField] private RectTransform dialoguePanelRect;
    [SerializeField] private RectTransform dialogueTextRect;
    [SerializeField] private Button clickAnywhereButton;

    [Header("Choice UI")]
    [SerializeField] private GameObject choicesUI;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private TMP_Text[] choiceButtonTexts;

    [Header("Results UI")]
    [SerializeField] private GameObject resultsUI;
    [SerializeField] private TMP_Text selectedChoiceText;
    [SerializeField] private TMP_InputField explanationInputField;
    [SerializeField] private Button submitButton;

    [Header("Background UI")]
    [SerializeField] private GameObject backgroundUI;

    private string selectedChoice;

    private int dialogueIndex = 0;

    private void Start()
    {
        dialogueUI.SetActive(true);
        choicesUI.SetActive(false);
        resultsUI.SetActive(false);
        backgroundUI.SetActive(false);
        submitButton.onClick.AddListener(SubmitExplanation);

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
        dialoguePanelRect.sizeDelta = new Vector2(285f, dialoguePanelRect.sizeDelta.y);
        dialogueTextRect.sizeDelta = new Vector2(245f, dialogueTextRect.sizeDelta.y);
        dialogueText.fontSize = 14.5f;

        choicesUI.SetActive(true);
        clickAnywhereButton.gameObject.SetActive(false);
    }

    private void SelectChoice(int index)
    {
        selectedChoice = choiceLabels[index];

        Debug.Log("Choice selected: " + selectedChoice);

        choicesUI.SetActive(false);
        dialogueUI.SetActive(false);

        ShowResultsScreen();
    }

    private void ShowResultsScreen()
    {
        resultsUI.SetActive(true);
        backgroundUI.SetActive(true);

        selectedChoiceText.text = selectedChoice;
        explanationInputField.text = "";
        explanationInputField.ActivateInputField();
    }

    private void SubmitExplanation()
    {
        string explanation = explanationInputField.text;

        Debug.Log("Selected choice: " + selectedChoice);
        Debug.Log("Explanation: " + explanation);

        resultsUI.SetActive(false);
    }
}