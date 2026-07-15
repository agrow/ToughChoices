using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioManager : MonoBehaviour
{
    [Serializable]
    public class DialogueLine
    {
        public string speakerName;

        [TextArea(2, 5)]
        public string dialogueText;

        public float typingSpeed = 0.03f;
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

    [Header("Navigation")]
    [SerializeField] private Button homeButton;

    [Header("Start Screen UI")]
    [SerializeField] private GameObject startScreenUI;
    [SerializeField] private Button startButton;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private RectTransform dialoguePanelRect;
    [SerializeField] private RectTransform dialogueTextRect;
    [SerializeField] private Button clickAnywhereButton;

    [Header("Dialogue Layout")]
    [SerializeField] private Vector2 fullDialoguePanelSize = new Vector2(1840f, 390f);
    [SerializeField] private Vector2 choiceDialoguePanelSize = new Vector2(1111f, 390f);

    [SerializeField] private Vector2 fullDialogueTextSize = new Vector2(1700f, 250f);
    [SerializeField] private Vector2 choiceDialogueTextSize = new Vector2(939f, 250f);

    [SerializeField] private float fullDialogueFontSize = 55f;
    [SerializeField] private float choiceDialogueFontSize = 50f;

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

    private Coroutine typingCoroutine;
    private bool isTyping = false;

    private void Start()
    {
        ShowStartScreen();

        homeButton.onClick.AddListener(ShowStartScreen);
        startButton.onClick.AddListener(ShowDialogue);
        submitButton.onClick.AddListener(SubmitExplanation);
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

    private void ShowStartScreen()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        isTyping = false;
        dialogueIndex = 0;
        selectedChoice = "";

        startScreenUI.SetActive(true);
        backgroundUI.SetActive(true);
        dialogueUI.SetActive(false);
        choicesUI.SetActive(false);
        resultsUI.SetActive(false);

        clickAnywhereButton.gameObject.SetActive(true);

        if (explanationInputField != null)
        {
            explanationInputField.text = "";
        }
    }

    private void ShowDialogue()
    {
        startScreenUI.SetActive(false);
        backgroundUI.SetActive(false);
        dialogueUI.SetActive(true);
        choicesUI.SetActive(false);
        resultsUI.SetActive(false);

        ApplyFullDialogueLayout();

        dialogueIndex = 0;
        ShowCurrentDialogue();
    }

    private void ShowCurrentDialogue()
    {
        clickAnywhereButton.gameObject.SetActive(true);

        if (dialogueLines.Length == 0)
        {
            Debug.LogWarning("No dialogue lines assigned.");
            return;
        }

        DialogueLine currentLine = dialogueLines[dialogueIndex];

        nameText.text = currentLine.speakerName;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeDialogue(currentLine.dialogueText, currentLine.typingSpeed));
    }

    private IEnumerator TypeDialogue(string line, float typingSpeed)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void AdvanceDialogue()
    {
        if (isTyping)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            dialogueText.text = dialogueLines[dialogueIndex].dialogueText;
            isTyping = false;
            return;
        }

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
        ApplyChoiceDialogueLayout();

        choicesUI.SetActive(true);
        clickAnywhereButton.gameObject.SetActive(false);
    }

    private void ApplyFullDialogueLayout()
    {
        dialoguePanelRect.sizeDelta = fullDialoguePanelSize;
        dialogueTextRect.sizeDelta = fullDialogueTextSize;
        dialogueText.fontSize = fullDialogueFontSize;
    }

    private void ApplyChoiceDialogueLayout()
    {
        dialoguePanelRect.sizeDelta = choiceDialoguePanelSize;
        dialogueTextRect.sizeDelta = choiceDialogueTextSize;
        dialogueText.fontSize = choiceDialogueFontSize;
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
        backgroundUI.SetActive(false);
    }
}