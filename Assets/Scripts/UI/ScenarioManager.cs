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
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button adminButton;

    [Header("Start Screen UI")]
    [SerializeField] private GameObject startScreenUI;
    [SerializeField] private Button startButton;

    [Header("Settings Screen UI")]
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private Button confirmSettingsButton;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private RectTransform dialoguePanelRect;
    [SerializeField] private RectTransform dialogueTextRect;
    [SerializeField] private Button clickAnywhereButton;
    [SerializeField] private Button rightNextButton;
    [SerializeField] private Button leftNextButton;

    [Header("Dialogue Layout")]
    [SerializeField] private Vector2 fullDialoguePanelSize = new Vector2(1840f, 390f);
    [SerializeField] private Vector2 choiceDialoguePanelSize = new Vector2(1111f, 390f);

    [SerializeField] private Vector2 fullDialogueTextSize = new Vector2(1495f, 250f);
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

    private bool startScreenWasActive;
    private bool dialogueWasActive;
    private bool choicesWereActive;
    private bool resultsWereActive;
    private bool backgroundWasActive;
    private bool clickAnywhereWasActive;

    private string selectedChoice;
    private int dialogueIndex = 0;

    private Coroutine typingCoroutine;
    private bool isTyping = false;

    private void Start()
    {
        ShowStartScreen();

        //homeButton.onClick.AddListener(ShowStartScreen);
        settingsButton.onClick.AddListener(ShowSettingsScreen);
        confirmSettingsButton.onClick.AddListener(HideSettingsScreen);
        startButton.onClick.AddListener(ShowDialogue);
        submitButton.onClick.AddListener(SubmitExplanation);
        clickAnywhereButton.onClick.AddListener(ShowNextDialogue);
        rightNextButton.onClick.AddListener(ShowNextDialogue);
        leftNextButton.onClick.AddListener(ShowPreviousDialogue);

        settingsUI.SetActive(false);

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

        settingsButton.gameObject.SetActive(true);
        clickAnywhereButton.gameObject.SetActive(true);
        adminButton.gameObject.SetActive(false);

        if (explanationInputField != null)
        {
            explanationInputField.text = "";
        }
    }

    private void ShowSettingsScreen()
    {
        // Remember the current UI state
        startScreenWasActive = startScreenUI.activeSelf;
        dialogueWasActive = dialogueUI.activeSelf;
        choicesWereActive = choicesUI.activeSelf;
        resultsWereActive = resultsUI.activeSelf;
        backgroundWasActive = backgroundUI.activeSelf;
        clickAnywhereWasActive = clickAnywhereButton.gameObject.activeSelf;

        // Hide the current screen
        startScreenUI.SetActive(false);
        dialogueUI.SetActive(false);
        choicesUI.SetActive(false);
        resultsUI.SetActive(false);
        backgroundUI.SetActive(false);
        clickAnywhereButton.gameObject.SetActive(false);

        // Show Settings
        settingsUI.SetActive(true);
        backgroundUI.SetActive(true);
        adminButton.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(false);
    }

    private void HideSettingsScreen()
    {
        settingsUI.SetActive(false);
        backgroundUI.SetActive(false);
        adminButton.gameObject.SetActive(false);

        // Restore whichever UI was active before Settings opened
        startScreenUI.SetActive(startScreenWasActive);
        dialogueUI.SetActive(dialogueWasActive);
        choicesUI.SetActive(choicesWereActive);
        resultsUI.SetActive(resultsWereActive);
        backgroundUI.SetActive(backgroundWasActive);
        clickAnywhereButton.gameObject.SetActive(clickAnywhereWasActive);

        settingsButton.gameObject.SetActive(true);
    }
    
    private void ShowAdminPin()
    {
        
    }

    private void ShowScenarioSelectionScreen()
    {
        
    }

    private void ShowDialogue()
    {
        dialogueUI.SetActive(true);
        startScreenUI.SetActive(false);
        settingsUI.SetActive(false);
        backgroundUI.SetActive(false);
        choicesUI.SetActive(false);
        resultsUI.SetActive(false);

        settingsButton.gameObject.SetActive(true);
        homeButton.gameObject.SetActive(false);

        ApplyFullDialogueLayout();

        dialogueIndex = 0;
        ShowCurrentDialogue();
    }

    private void UpdateDialogueNavigation()
    {
        bool hasMultipleLines = dialogueLines.Length > 1;
        bool isFirstLine = dialogueIndex == 0;
        bool isLastLine = dialogueIndex == dialogueLines.Length - 1;

        leftNextButton.gameObject.SetActive(
            hasMultipleLines && !isFirstLine
        );

        rightNextButton.gameObject.SetActive(
            hasMultipleLines && !isLastLine
        );

        choicesUI.SetActive(isLastLine);

        if (isLastLine)
        {
            ApplyChoiceDialogueLayout();
            clickAnywhereButton.gameObject.SetActive(false);
        }
        else
        {
            ApplyFullDialogueLayout();
            clickAnywhereButton.gameObject.SetActive(true);
        }
    }

    private void ShowCurrentDialogue()
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            Debug.LogWarning("No dialogue lines assigned.");
            return;
        }

        dialogueIndex = Mathf.Clamp(
            dialogueIndex,
            0,
            dialogueLines.Length - 1
        );

        UpdateDialogueNavigation();

        DialogueLine currentLine = dialogueLines[dialogueIndex];

        nameText.text = currentLine.speakerName;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        float adjustedTypingSpeed = currentLine.typingSpeed / SettingsManager.TextSpeedMultiplier;
        typingCoroutine = StartCoroutine(TypeDialogue(currentLine.dialogueText,adjustedTypingSpeed)
        
        );

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

        private bool FinishTypingImmediately()
    {
        if (!isTyping)
        {
            return false;
        }

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.text =
            dialogueLines[dialogueIndex].dialogueText;

        isTyping = false;
        return true;
    }

    private void ShowNextDialogue()
    {
        if (FinishTypingImmediately())
        {
            return;
        }

        if (dialogueIndex >= dialogueLines.Length - 1)
        {
            return;
        }

        dialogueIndex++;
        ShowCurrentDialogue();
    }

    private void ShowPreviousDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;

        if (dialogueIndex <= 0)
        {
            return;
        }

        dialogueIndex--;
        ShowCurrentDialogue();
    }

    private void ShowChoices()
    {
        ApplyChoiceDialogueLayout();

        choicesUI.SetActive(true);
        leftNextButton.gameObject.SetActive(true);
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

        settingsButton.gameObject.SetActive(true);
        homeButton.gameObject.SetActive(false);

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