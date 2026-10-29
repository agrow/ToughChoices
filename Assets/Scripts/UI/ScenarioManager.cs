using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ScenarioManager : MonoBehaviour
{
    [Header("Scenario Content")]
    [SerializeField] private ScenarioData scenarioData;

    [SerializeField] private string[] choiceLabels =
    {
        "VERY LIKELY",
        "SOMEWHAT LIKELY",
        "UNSURE",
        "SOMEWHAT UNLIKELY",
        "VERY UNLIKELY"
    };

    [Header("Scenario Sequence")]
    [SerializeField] private ScenarioSequenceManager scenarioSequenceManager;

    [Header("Navigation")]
    [SerializeField] private Button homeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button adminButton;
    [SerializeField] private Button restartButton;

    [Header("Start Screen UI")]
    [SerializeField] private GameObject startScreenUI;
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsStartButton;
    [SerializeField] private GameObject startButtons;
    [SerializeField] private GameObject userIDButtons;
    [SerializeField] private GameObject userIDPlaceholder;
    [SerializeField] private Button userIDConfirmButton;
    [SerializeField] private TMP_InputField userIDInputField;
    [SerializeField] private TMP_Text userIDErrorText;

    [Header("Settings Screen UI")]
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private Button confirmSettingsButton;

    [Header("Introduction UI")]
    [SerializeField] private GameObject introductionUI;
    [SerializeField] private TMP_Text introductionText;
    [SerializeField] private ProgressDotsUI introductionProgressDots;

    [SerializeField] private string[] introductionLines =
    {
        "In this game, you will be guided through nine different hypothetical scenarios.",

        "As best as you can, try to imagine yourself in each scenario, and then indicate what choice you would make.",

        "You will also have an opportunity to explain your choice.",

        "Ready? Let's go!"
    };
    [SerializeField] private Button nextButton;
    [SerializeField] private Button introductionBackButton;

    [Header("Scenario Selection UI")]
    [SerializeField] private GameObject scenarioSelectionUI;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private ProgressDotsUI dialogueProgressDots;
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

    [Header("Results UI")]
    [SerializeField] private GameObject resultsUI;
    [SerializeField] private Button returnButton;

    [Header("Question 1 UI")]
    [SerializeField] private GameObject question1UI;
    [SerializeField] private Button[] question1ChoiceButtons;
    [SerializeField] private TMP_Text[] question1ChoiceButtonTexts;

    [Header("Question 1 Results UI")]
    [SerializeField] private GameObject question1ResultsUI;
    [SerializeField] private TMP_Text selectedChoiceText1;
    [SerializeField] private TMP_InputField explanationInputField1;
    [SerializeField] private Button submitButton1;

    [Header("Question 2 UI")]
    [SerializeField] private GameObject question2UI;
    [SerializeField] private Button[] question2ChoiceButtons;
    [SerializeField] private TMP_Text[] question2ChoiceButtonTexts;

    [Header("Question 2 Results UI")]
    [SerializeField] private GameObject question2ResultsUI;
    [SerializeField] private TMP_Text selectedChoiceText2;
    [SerializeField] private TMP_InputField explanationInputField2;
    [SerializeField] private Button submitButton2;
    
    [Header("Submit Response Prompt UI")]
    [SerializeField] private GameObject submitResponsePromptUI;
    [SerializeField] private Button confirmResponseButton;
    [SerializeField] private Button cancelResponseButton;

    [Header("Background UI")]
    [SerializeField] private GameObject backgroundUI;

    [Header("Admin Prompt UI")]
    [SerializeField] private GameObject adminPromptUI;
    [SerializeField] private Button adminClickAnywhereButton;
    [SerializeField] private TMP_InputField adminPinInputField;
    [SerializeField] private TMP_Text incorrectPinText;
    [SerializeField] private string correctAdminPin = "1234";

    [Header("Restart Prompt UI")]
    [SerializeField] private GameObject restartPromptUI;
    [SerializeField] private Button restartCancelButton;
    [SerializeField] private Button restartConfirmButton;
    [SerializeField] private Button restartClickAnywhereButton;

    [Header("Conclusion UI")]
    [SerializeField] private GameObject conclusionUI;
    [SerializeField] private Button conclusionRestartButton;

    [Header("Screen Transitions")]
    [SerializeField] private CanvasGroup fadeOverlay;
    [SerializeField] private float fadeDuration = 0.35f;

    private bool startScreenWasActive;
    private bool introductionWasActive;
    
    private bool dialogueWasActive;
    
    private bool resultsWasActive;
    private bool question1WasActive;
    private bool question1ResultsWasActive;
    private bool question2WasActive;
    private bool question2ResultsWasActive;
    private bool conclusionWasActive;
    private bool backgroundWasActive;
    private bool scenarioSelectionWasActive;
    private bool clickAnywhereWasActive;
    private bool settingsButtonWasActive;

    private string selectedChoice;
    private int dialogueIndex = 0;
    private int currentScenarioOrder = 0;
    private int introductionIndex = 0;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    
    private bool adminTransitionInProgress = false;
    private int currentQuestion = 1;
    private bool restartInProgress = false;
    private bool transitionInProgress = false;


    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandleEscapeKey();
        }
    }

    private void Start()
    {
        scenarioData = scenarioSequenceManager.GetCurrentScenario();
        
        ShowStartScreen();

        settingsButton.onClick.AddListener(ShowSettingsScreen);
        settingsStartButton.onClick.AddListener(ShowSettingsScreen);
        adminButton.onClick.AddListener(ShowAdminPin);
        confirmSettingsButton.onClick.AddListener(HideSettingsScreen);
        userIDConfirmButton.onClick.AddListener(ValidateUserID);
        startButton.onClick.AddListener(() =>
        {
            StartCoroutine(FadeToScreen(ShowIntroduction));
        });
        submitButton1.onClick.AddListener(() =>
        {
            Debug.Log(
                "QUESTION 1 SUBMIT TRIGGERED | Frame: " +
                Time.frameCount
            );

            submitResponsePromptUI.SetActive(true);
        });

        submitButton2.onClick.AddListener(() =>
        {
            Debug.Log(
                "QUESTION 2 SUBMIT TRIGGERED | Frame: " +
                Time.frameCount
            );

            submitResponsePromptUI.SetActive(true);
        });
        clickAnywhereButton.onClick.AddListener(ShowNextDialogue);
        rightNextButton.onClick.AddListener(ShowNextDialogue);
        leftNextButton.onClick.AddListener(ShowPreviousDialogue);
        nextButton.onClick.AddListener(NextIntroductionLine);
        introductionBackButton.onClick.AddListener(
            PreviousIntroductionLine
        );
        confirmResponseButton.onClick.AddListener(ConfirmCurrentExplanation);
        cancelResponseButton.onClick.AddListener(() => submitResponsePromptUI.SetActive(false));
        settingsUI.SetActive(false);
        adminPromptUI.SetActive(false);
        dialogueUI.SetActive(false);
        adminClickAnywhereButton.onClick.AddListener(() => adminPromptUI.SetActive(false));
        adminPinInputField.onSubmit.AddListener((_) => ValidateAdminPin());
        restartButton.onClick.AddListener(() => restartPromptUI.SetActive(true));
        restartCancelButton.onClick.AddListener(() => restartPromptUI.SetActive(false));
        restartClickAnywhereButton.onClick.AddListener(() => restartPromptUI.SetActive(false));
        returnButton.onClick.AddListener(() => ReturnToPreviousQuestion());
        restartConfirmButton.onClick.AddListener(RestartExperience);
        conclusionRestartButton.onClick.AddListener(RestartExperience);
        userIDInputField.onSelect.AddListener(
        (_) => userIDPlaceholder.SetActive(false)
        );

        userIDInputField.onDeselect.AddListener(
            (_) =>
            {
                if (string.IsNullOrEmpty(userIDInputField.text))
                {
                    userIDPlaceholder.SetActive(true);
                }
            }
        );

        // Question 1 Likert buttons
        for (int i = 0; i < question1ChoiceButtons.Length; i++)
        {
            int capturedIndex = i;

            if (i < question1ChoiceButtonTexts.Length &&
                i < choiceLabels.Length)
            {
                question1ChoiceButtonTexts[i].text = choiceLabels[i];
            }

            question1ChoiceButtons[i].onClick.AddListener(
                () => SelectChoice(capturedIndex)
            );
        }

        // Question 2 Likert buttons
        for (int i = 0; i < question2ChoiceButtons.Length; i++)
        {
            int capturedIndex = i;

            if (i < question2ChoiceButtonTexts.Length &&
                i < choiceLabels.Length)
            {
                question2ChoiceButtonTexts[i].text = choiceLabels[i];
            }

            question2ChoiceButtons[i].onClick.AddListener(
                () => SelectQuestion2Choice(capturedIndex)
            );
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
        userIDButtons.SetActive(true);
        startButtons.SetActive(false);
        introductionUI.SetActive(false);
        dialogueUI.SetActive(false);
        resultsUI.SetActive(false);
        question1UI.SetActive(false);
        question1ResultsUI.SetActive(false);
        question2UI.SetActive(false);
        question2ResultsUI.SetActive(false);
        submitResponsePromptUI.SetActive(false);
        conclusionUI.SetActive(false);

        settingsButton.gameObject.SetActive(false);
        scenarioSelectionUI.gameObject.SetActive(false);
        restartPromptUI.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        settingsStartButton.gameObject.SetActive(true);
        clickAnywhereButton.gameObject.SetActive(true);
        userIDConfirmButton.gameObject.SetActive(true);
        adminButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);

        if (explanationInputField1 != null)
        {
            explanationInputField1.text = "";
        }

        if (explanationInputField2 != null)
        {
            explanationInputField2.text = "";
        }
    }

    private void HandleEscapeKey()
    {
        // Close highest priority popup first
        if (restartPromptUI.activeSelf)
        {
            restartPromptUI.SetActive(false);
            return;
        }

        if (submitResponsePromptUI.activeSelf)
        {
            submitResponsePromptUI.SetActive(false);
            return;
        }

        if (adminPromptUI.activeSelf)
        {
            adminPromptUI.SetActive(false);
            return;
        }

        if (settingsUI.activeSelf)
        {
            HideSettingsScreen();
            return;
        }

        if (scenarioSelectionUI.activeSelf)
        {
            scenarioSelectionUI.SetActive(false);

            // Hide the admin only Restart button
            restartButton.gameObject.SetActive(false);

            // Hide the Restart confirmation prompt if necessary
            restartPromptUI.SetActive(false);

            Debug.Log("Closed Scenario Selection UI.");

            return;
        }

        if (introductionUI.activeInHierarchy)
        {
            ReturnToStartScreen();
            return;
        }

        if (resultsUI.activeInHierarchy)
        {
            ReturnToPreviousQuestion();
            return;
        }
    }

        private void HandleResultsEnterKey()
    {
        // Do not open another prompt if one is already active
        if (submitResponsePromptUI.activeSelf)
        {
            return;
        }

        // Only allow submission during an explanation screen
        if (!resultsUI.activeInHierarchy)
        {
            return;
        }

        if (question1ResultsUI.activeInHierarchy ||
            question2ResultsUI.activeInHierarchy)
        {
            submitResponsePromptUI.SetActive(true);
        }
    }

    public void ValidateUserID()
    {
        string userID = userIDInputField.text.Trim();

        if (userID.Length != 3)
        {
            userIDErrorText.text =
                "Please enter a 3-digit ID number.";

            return;
        }

        userIDErrorText.text = "";

        DataExportManager.Instance.SetUserID(userID);

        ShowStartButtons();
    }

    public void ValidateAdminPin()
    {
        string adminPin = adminPinInputField.text.Trim();

        if (adminPin.Length != 4)
        {
            incorrectPinText.text = "Incorrect PIN. Try again.";

            return;
        }

        if (adminPin != correctAdminPin)
        {
            incorrectPinText.text = "Incorrect PIN. Try again.";
            adminPinInputField.text = "";
            adminPinInputField.ActivateInputField();

            return;
        }

        if (adminPin.Length >= 4)
        {
            adminPinInputField.DeactivateInputField();
        }

        adminPinInputField.DeactivateInputField();
        adminPinInputField.text = "";
        incorrectPinText.text = "";

        ShowScenarioSelectionScreen();
    }

     private void ShowStartButtons()
    {
        startScreenUI.SetActive(true);
        backgroundUI.SetActive(true);
        startButtons.SetActive(true);
        userIDButtons.SetActive(false);
        dialogueUI.SetActive(false);
        question1UI.SetActive(false);
        question1ResultsUI.SetActive(false);
        question2UI.SetActive(false);
        question2ResultsUI.SetActive(false);
        conclusionUI.SetActive(false);

        settingsButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        settingsStartButton.gameObject.SetActive(true);
        clickAnywhereButton.gameObject.SetActive(true);
        userIDConfirmButton.gameObject.SetActive(true);
        adminButton.gameObject.SetActive(false);
    }

    private void ShowSettingsScreen()
    {
        // Remember the current UI state
        startScreenWasActive = startScreenUI.activeSelf;
        introductionWasActive = introductionUI.activeSelf;
        dialogueWasActive = dialogueUI.activeSelf;
        resultsWasActive = resultsUI.activeSelf;
        question1WasActive = question1UI.activeSelf;
        question1ResultsWasActive = question1ResultsUI.activeSelf;
        question2WasActive = question2UI.activeSelf;
        question2ResultsWasActive = question2ResultsUI.activeSelf;
        conclusionWasActive = conclusionUI.activeSelf;
        backgroundWasActive = backgroundUI.activeSelf;
        scenarioSelectionWasActive = scenarioSelectionUI.activeSelf;
        clickAnywhereWasActive = clickAnywhereButton.gameObject.activeSelf;
        settingsButtonWasActive = settingsButton.gameObject.activeSelf;

        // Hide the current screen
        startScreenUI.SetActive(false);
        introductionUI.SetActive(false);
        dialogueUI.SetActive(false);
        resultsUI.SetActive(false);
        question1UI.SetActive(false);
        question1ResultsUI.SetActive(false);
        question2UI.SetActive(false);
        question2ResultsUI.SetActive(false);
        conclusionUI.SetActive(false);
        backgroundUI.SetActive(false);
        scenarioSelectionUI.SetActive(false);
        restartPromptUI.SetActive(false);
        clickAnywhereButton.gameObject.SetActive(false);

        // Show Settings
        settingsUI.SetActive(true);
        backgroundUI.SetActive(true);
        adminButton.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
    }

    private void HideSettingsScreen()
    {
        settingsUI.SetActive(false);
        backgroundUI.SetActive(false);
        adminButton.gameObject.SetActive(false);

        // Restore whichever UI was active before Settings opened
        startScreenUI.SetActive(startScreenWasActive);
        introductionUI.SetActive(introductionWasActive);
        dialogueUI.SetActive(dialogueWasActive);
        resultsUI.SetActive(resultsWasActive);
        question1UI.SetActive(question1WasActive);
        question1ResultsUI.SetActive(question1ResultsWasActive);
        question2UI.SetActive(question2WasActive);
        question2ResultsUI.SetActive(question2ResultsWasActive);
        conclusionUI.SetActive(conclusionWasActive);
        backgroundUI.SetActive(backgroundWasActive);
        scenarioSelectionUI.SetActive(scenarioSelectionWasActive);
        clickAnywhereButton.gameObject.SetActive(clickAnywhereWasActive);
        settingsButton.gameObject.SetActive(settingsButtonWasActive);

        restartButton.gameObject.SetActive(
            scenarioSelectionWasActive
        );
    }
    
    private void ShowAdminPin()
    {
        adminPinInputField.text = "";
        incorrectPinText.text = "";
        adminPromptUI.SetActive(true);
        Debug.Log("Admin Pin Prompt Triggered!");
    }

    private void ShowScenarioSelectionScreen()
    {
        adminPromptUI.SetActive(false);
        settingsButton.gameObject.SetActive(false);
        adminButton.gameObject.SetActive(false);

        scenarioSelectionUI.SetActive(true);
        settingsButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);

        RefreshScenarioSelectionButtons();
    }

    private void ShowIntroduction()
    {
        introductionIndex = 0;

        introductionProgressDots.Initialize(
            introductionLines.Length
        );

        UpdateIntroduction();

        introductionUI.SetActive(true);
        startScreenUI.SetActive(false);
        backgroundUI.SetActive(true);
        dialogueUI.SetActive(false);
        question1UI.SetActive(false);
        question1ResultsUI.SetActive(false);
        question2UI.SetActive(false);
        question2ResultsUI.SetActive(false);
        scenarioSelectionUI.SetActive(false);

        settingsButton.gameObject.SetActive(true);
        clickAnywhereButton.gameObject.SetActive(false);
        adminButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
    }
    
    private void UpdateIntroduction()
    {
        introductionText.text =
            introductionLines[introductionIndex];

        introductionProgressDots.UpdateProgress(
            introductionIndex
        );
    }

    private void NextIntroductionLine()
    {
        if (transitionInProgress)
        {
            return;
        }

        if (introductionIndex < introductionLines.Length - 1)
        {
            introductionIndex++;
            UpdateIntroduction();
        }
        else
        {
            StartScenario();
        }
    }

    private void PreviousIntroductionLine()
    {
        if (transitionInProgress)
        {
            return;
        }

        if (introductionIndex > 0)
        {
            introductionIndex--;
            UpdateIntroduction();
        }
        else
        {
            StartCoroutine(FadeToScreen(ReturnToStartScreen));
        }
    }

    private IEnumerator LoadScenarioScene()
    {
        if (scenarioData == null)
        {
            Debug.LogWarning("No Scenario Data assigned.");
            yield break;
        }

        string sceneName = scenarioData.sceneName;

        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            yield break;
        }

        AsyncOperation loadOperation =
            SceneManager.LoadSceneAsync(
                sceneName,
                LoadSceneMode.Additive
            );

        while (!loadOperation.isDone)
        {
            yield return null;
        }

        Debug.Log("Loaded scenario scene: " + sceneName);
    }

    private void StartScenario()
    {
        if (transitionInProgress ||
            restartInProgress ||
            adminTransitionInProgress)
        {
            return;
        }

        StartCoroutine(StartScenarioSequence());
    }

    private IEnumerator StartScenarioSequence()
    {
        transitionInProgress = true;
        fadeOverlay.blocksRaycasts = true;

        // Fade to black.
        yield return StartCoroutine(Fade(1f));

        introductionUI.SetActive(false);

        // Load the environment while the screen is black
        yield return StartCoroutine(LoadScenarioScene());

        currentScenarioOrder++;

        DataExportManager.Instance.BeginScenario(
            scenarioData.scenarioID,
            currentScenarioOrder
        );

        // Prepare the new dialogue screen
        ShowDialogue();

        // Fade into the new environment
        yield return StartCoroutine(Fade(0f));

        fadeOverlay.blocksRaycasts = false;
        transitionInProgress = false;
    }

    public void SetScenario(ScenarioData newScenario)
    {
        scenarioData = newScenario;
        dialogueIndex = 0;
    }

    // Starts the scenario manually selected by the administrator
    public void StartSelectedScenario(ScenarioData selectedScenario)
    {
        if (selectedScenario == null)
        {
            Debug.LogWarning("No scenario selected.");
            return;
        }

        SetScenario(selectedScenario);

        // Hide administrator UI
        scenarioSelectionUI.SetActive(false);
        restartButton.gameObject.SetActive(false);
        adminPromptUI.SetActive(false);
        settingsUI.SetActive(false);

        // Start the selected scenario
        StartScenario();
    }

    public void AdminSelectScenario(ScenarioData selectedScenario)
    {
        if (adminTransitionInProgress)
        {
            return;
        }

        if (!scenarioSequenceManager.IsScenarioAvailable(selectedScenario))
        {
            Debug.LogWarning("Selected scenario is unavailable.");
            return;
        }

        StartCoroutine(AdminSwitchScenarioRoutine(selectedScenario));
    }

    private string previousScenarioSceneName;

    private IEnumerator SwitchToNextScenario()
    {
        transitionInProgress = true;
        fadeOverlay.blocksRaycasts = true;

        // STEP 1: Fade to black while ResultsUI remains visible
        yield return StartCoroutine(Fade(1f));

        // STEP 2: Hide the previous UI while fully black
        dialogueUI.SetActive(false);
        resultsUI.SetActive(false);
        backgroundUI.SetActive(false);

        question1UI.SetActive(false);
        question1ResultsUI.SetActive(false);
        question2UI.SetActive(false);
        question2ResultsUI.SetActive(false);

        // STEP 3: Unload the previous environment
        Scene currentScene =
            SceneManager.GetSceneByName(
                previousScenarioSceneName
            );

        if (currentScene.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(
                previousScenarioSceneName
            );
        }

        // STEP 4: Load the next environment
        yield return StartCoroutine(LoadScenarioScene());

        // STEP 5: Begin collecting data for the next scenario
        currentScenarioOrder++;

        DataExportManager.Instance.BeginScenario(
            scenarioData.scenarioID,
            currentScenarioOrder
        );

        // STEP 6: Prepare the new scenario dialogue
        ShowDialogue();

        // STEP 7: Reveal the new environment
        yield return StartCoroutine(Fade(0f));

        fadeOverlay.blocksRaycasts = false;
        transitionInProgress = false;
    }

    private IEnumerator FinishAllScenarios()
    {
        transitionInProgress = true;
        fadeOverlay.blocksRaycasts = true;

        // Fade to black while the final ResultsUI is visible
        yield return StartCoroutine(Fade(1f));

        // Unload the final environment
        string finalSceneName = scenarioData.sceneName;

        Scene finalScene =
            SceneManager.GetSceneByName(finalSceneName);

        if (finalScene.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(
                finalSceneName
            );
        }

        // Show the conclusion screen while fully black
        ShowEndScreen();

        // Fade into ConclusionUI
        yield return StartCoroutine(Fade(0f));

        fadeOverlay.blocksRaycasts = false;
        transitionInProgress = false;
    }

    private void GoToNextScenario()
    {
        if (scenarioSequenceManager.HasMoreScenarios())
        {
            // Save the CURRENT environment before changing scenarioData
            previousScenarioSceneName = scenarioData.sceneName;

            ScenarioData nextScenario =
                scenarioSequenceManager.GetNextScenario();

            SetScenario(nextScenario);

            StartCoroutine(SwitchToNextScenario());
        }
        else
        {
            StartCoroutine(FinishAllScenarios());
        }
    }

    private void ShowEndScreen()
    {
        // Hide gameplay UI
        dialogueUI.SetActive(false);
        resultsUI.SetActive(false);

        question1UI.SetActive(false);
        question1ResultsUI.SetActive(false);
        question2UI.SetActive(false);
        question2ResultsUI.SetActive(false);

        // Hide other UI
        startScreenUI.SetActive(false);
        introductionUI.SetActive(false);
        scenarioSelectionUI.SetActive(false);
        submitResponsePromptUI.SetActive(false);
        settingsUI.SetActive(false);
        adminPromptUI.SetActive(false);
        restartPromptUI.SetActive(false);

        // Hide navigation buttons
        settingsButton.gameObject.SetActive(false);
        adminButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        homeButton.gameObject.SetActive(false);

        // Display conclusion
        backgroundUI.SetActive(true);
        conclusionUI.SetActive(true);

        Debug.Log("All scenarios completed!");
    }

    private void ShowDialogue()
    {
        dialogueUI.SetActive(true);
        startScreenUI.SetActive(false);
        settingsUI.SetActive(false);
        backgroundUI.SetActive(false);
        resultsUI.SetActive(false);
        question1UI.SetActive(false);
        question1ResultsUI.SetActive(false);
        question2UI.SetActive(false);
        question2ResultsUI.SetActive(false);
        introductionUI.SetActive(false);

        settingsButton.gameObject.SetActive(true);
        homeButton.gameObject.SetActive(false);

        explanationInputField1.text = "";
        explanationInputField2.text = "";

        ApplyFullDialogueLayout();

        dialogueIndex = 0;

        // Create one dot for each dialogue line in this scenario
        if (scenarioData != null &&
            scenarioData.dialogueLines != null)
        {
            dialogueProgressDots.Initialize(
                scenarioData.dialogueLines.Length
            );
        }

        ShowCurrentDialogue();
    }

    private void UpdateDialogueNavigation()
    {
        bool hasMultipleLines = scenarioData.dialogueLines.Length > 1;
        bool isFirstLine = dialogueIndex == 0;
        bool isLastLine = dialogueIndex == scenarioData.dialogueLines.Length - 1;

        leftNextButton.gameObject.SetActive(
            hasMultipleLines && !isFirstLine
        );

        rightNextButton.gameObject.SetActive(
            hasMultipleLines && !isLastLine
        );

        question1UI.SetActive(isLastLine);

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
        if (scenarioData == null)
        {
            Debug.LogWarning("No Scenario Data assigned.");
            return;
        }
        
        if (scenarioData.dialogueLines == null || scenarioData.dialogueLines.Length == 0)
        {
            Debug.LogWarning("No dialogue lines assigned.");
            return;
        }

        dialogueIndex = Mathf.Clamp(
            dialogueIndex, 0, scenarioData.dialogueLines.Length - 1
        );

        UpdateDialogueNavigation();

        dialogueProgressDots.UpdateProgress(dialogueIndex);

        ScenarioData.DialogueLine currentLine = scenarioData.dialogueLines[dialogueIndex];

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
        typingCoroutine = null;
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
            scenarioData.dialogueLines[dialogueIndex].dialogueText;

        isTyping = false;

        return true;
    }

    private void ShowNextDialogue()
    {
        if (FinishTypingImmediately())
        {
            return;
        }

        if (dialogueIndex >= scenarioData.dialogueLines.Length - 1)
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

        question1UI.SetActive(true);
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

        int likertValue = 5 - index;

        DataExportManager.Instance.RecordLikert1(
            likertValue
        );

        Debug.Log(
            "Choice selected: " +
            selectedChoice +
            " | Value: " +
            likertValue
        );

        question1UI.SetActive(false);
        dialogueUI.SetActive(false);

        ShowResultsScreen();
    }

    private void ReturnToPreviousQuestion()
    {
        submitResponsePromptUI.SetActive(false);

        if (question2ResultsUI.activeSelf)
        {
            question2ResultsUI.SetActive(false);
            question2UI.SetActive(true);
        }
        else if (question2UI.activeSelf)
        {
            question2UI.SetActive(false);
            question1ResultsUI.SetActive(true);

            currentQuestion = 1;
        }
        else if (question1ResultsUI.activeSelf)
        {
            question1ResultsUI.SetActive(false);

            resultsUI.SetActive(false);
            backgroundUI.SetActive(false);

            dialogueUI.SetActive(true);

            dialogueIndex =
                scenarioData.dialogueLines.Length - 1;

            ShowCurrentDialogue();
            FinishTypingImmediately();
        }
    }

    private void ReturnToStartScreen()
    {
        // Hide Introduction UI
        introductionUI.SetActive(false);

        // Return to the Start screen
        startScreenUI.SetActive(true);
        backgroundUI.SetActive(true);

        // Preserve the validated User ID
        userIDButtons.SetActive(false);
        startButtons.SetActive(true);

        // Restore the correct navigation buttons
        settingsButton.gameObject.SetActive(false);
        settingsStartButton.gameObject.SetActive(true);
        adminButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);

        Debug.Log("Returned to Start Screen from Introduction.");
    }

    private void ShowResultsScreen()
    {

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        resultsUI.SetActive(true);
        backgroundUI.SetActive(true);

        currentQuestion = 1;

        question1UI.SetActive(false);
        question1ResultsUI.SetActive(true);

        question2UI.SetActive(false);
        question2ResultsUI.SetActive(false);

        submitResponsePromptUI.SetActive(false);

        settingsButton.gameObject.SetActive(true);
        homeButton.gameObject.SetActive(false);

        selectedChoiceText1.text = selectedChoice;

        explanationInputField1.ActivateInputField();
    }

    public void SubmitQuestion1Explanation()
    {
        DataExportManager.Instance.RecordOpen1(
            explanationInputField1.text
        );

        submitResponsePromptUI.SetActive(false);

        question1ResultsUI.SetActive(false);
        question2UI.SetActive(true);

        currentQuestion = 2;
    }

    public void SubmitQuestion2Explanation()
    {
        if (transitionInProgress)
        {
            return;
        }

        string explanation = explanationInputField2.text;

        // Record Question 2 explanation
        DataExportManager.Instance.RecordOpen2(explanation);

        // Close the confirmation popup
        submitResponsePromptUI.SetActive(false);

        // Save the completed scenario to TSV
        DataExportManager.Instance.CompleteScenario();

        // Mark the scenario completed
        scenarioSequenceManager.MarkScenarioCompleted(
            scenarioData.scenarioID
        );

        Debug.Log(
            "Scenario completed and exported: " +
            scenarioData.scenarioID
        );

        // Keep ResultsUI visible until the fade finishes
        GoToNextScenario();
    }

    private void SelectQuestion2Choice(int index)
    {
        int likertValue = 5 - index;

        DataExportManager.Instance.RecordLikert2(likertValue);

        selectedChoiceText2.text = choiceLabels[index];

        question2UI.SetActive(false);
        question2ResultsUI.SetActive(true);

        explanationInputField2.ActivateInputField();
    }

    public void ShowSubmitResponsePromptScreen()
    {
        submitResponsePromptUI.SetActive(true);
    }

    private void ConfirmCurrentExplanation()
    {
        if (currentQuestion == 1)
        {
            SubmitQuestion1Explanation();
        }
        else if (currentQuestion == 2)
        {
            SubmitQuestion2Explanation();
        }
    }

    private void RefreshScenarioSelectionButtons()
    {
        ScenarioSelectionButton[] buttons =
            scenarioSelectionUI.GetComponentsInChildren<
                ScenarioSelectionButton>(true);

        foreach (ScenarioSelectionButton button in buttons)
        {
            button.RefreshButton();
        }
    }

    private IEnumerator AdminSwitchScenarioRoutine(
    ScenarioData selectedScenario)
    {
        adminTransitionInProgress = true;

        // Remember the currently playing scenario
        ScenarioData previousScenario = scenarioData;

        // Hide administrator UI
        scenarioSelectionUI.SetActive(false);
        adminPromptUI.SetActive(false);
        settingsUI.SetActive(false);
        restartPromptUI.SetActive(false);
        restartButton.gameObject.SetActive(false);

        // Hide gameplay UI
        dialogueUI.SetActive(false);
        resultsUI.SetActive(false);
        question1UI.SetActive(false);
        question1ResultsUI.SetActive(false);
        question2UI.SetActive(false);
        question2ResultsUI.SetActive(false);
        submitResponsePromptUI.SetActive(false);
        clickAnywhereButton.gameObject.SetActive(false);

        // Stop the current dialogue animation
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;

        // Record the interrupted scenario, if one is active
        DataExportManager.Instance.RecordAdminOverride();

        // Unload the previous environment
        if (previousScenario != null)
        {
            Scene previousScene =
                SceneManager.GetSceneByName(
                    previousScenario.sceneName
                );

            if (previousScene.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(
                    previousScenario.sceneName
                );
            }
        }

        // Rebuild the randomized sequence with the selected scenario first
        if (!scenarioSequenceManager.OverrideCurrentScenario(
            selectedScenario))
        {
            adminTransitionInProgress = false;
            yield break;
        }

        // Update the current scenario
        SetScenario(selectedScenario);

        // Load the new environment.
        yield return StartCoroutine(LoadScenarioScene());

        // Record the next scenario presentation
        currentScenarioOrder++;

        DataExportManager.Instance.BeginScenario(
            scenarioData.scenarioID,
            currentScenarioOrder
        );

        // Start from the first dialogue line
        ShowDialogue();

        adminTransitionInProgress = false;
    }

    public void RestartExperience()
    {
        if (restartInProgress ||
            adminTransitionInProgress ||
            transitionInProgress)
        {
            return;
        }

        StartCoroutine(RestartExperienceRoutine());
    }

    private IEnumerator RestartExperienceRoutine()
    {
        restartInProgress = true;
        transitionInProgress = true;

        Debug.Log("Restarting Tough Choices experience.");

        // Block user interactions during the transition
        fadeOverlay.blocksRaycasts = true;

        // STEP 1: Fade to black while ConclusionUI is visible
        yield return StartCoroutine(Fade(1f));

        // STEP 2: Stop any active dialogue animation
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;

        // STEP 3: Record an interrupted scenario, if one exists
        // This does nothing if the previous scenario was completed
        DataExportManager.Instance.RecordAdminOverride();

        // STEP 4: Hide the previous UI while the screen is black
        restartPromptUI.SetActive(false);
        scenarioSelectionUI.SetActive(false);
        conclusionUI.SetActive(false);
        resultsUI.SetActive(false);
        dialogueUI.SetActive(false);
        introductionUI.SetActive(false);
        submitResponsePromptUI.SetActive(false);

        question1UI.SetActive(false);
        question1ResultsUI.SetActive(false);
        question2UI.SetActive(false);
        question2ResultsUI.SetActive(false);

        // STEP 5: Unload the current scenario environment if loaded
        if (scenarioData != null &&
            !string.IsNullOrEmpty(scenarioData.sceneName))
        {
            Scene currentScene =
                SceneManager.GetSceneByName(scenarioData.sceneName);

            if (currentScene.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(
                    scenarioData.sceneName
                );

                Debug.Log(
                    "Unloaded scenario environment: " +
                    scenarioData.sceneName
                );
            }
        }

        // STEP 6: Reset scenario completion tracking
        scenarioSequenceManager.ResetScenarioSequence();

        // Reset scenario order.
        currentScenarioOrder = 0;

        // Get the first scenario in the new randomized sequence
        scenarioData = scenarioSequenceManager.GetCurrentScenario();

        // STEP 7: Reset dialogue and questionnaire state
        dialogueIndex = 0;
        currentQuestion = 1;
        selectedChoice = "";

        // Clear previous participant information
        userIDInputField.text = "";
        userIDErrorText.text = "";

        // STEP 8: Prepare the User ID start screen while black
        ShowStartScreen();

        // STEP 9: Fade into the start screen
        yield return StartCoroutine(Fade(0f));

        // Restore user interactions
        fadeOverlay.blocksRaycasts = false;

        transitionInProgress = false;
        restartInProgress = false;

        Debug.Log("Tough Choices experience restarted successfully.");
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeOverlay.alpha;
        float elapsed = 0f;

        float duration = Mathf.Max(0.01f, fadeDuration);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            // Smooth acceleration and deceleration.
            t = Mathf.SmoothStep(0f, 1f, t);

            fadeOverlay.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                t
            );

            yield return null;
        }

        fadeOverlay.alpha = targetAlpha;
    }

    private IEnumerator FadeToScreen(Action changeScreen)
    {
        if (transitionInProgress)
        {
            yield break;
        }

        transitionInProgress = true;
        fadeOverlay.blocksRaycasts = true;

        // Fade out.
        yield return StartCoroutine(Fade(1f));

        // Change the active UI while the screen is black.
        changeScreen?.Invoke();

        // Fade back in.
        yield return StartCoroutine(Fade(0f));

        fadeOverlay.blocksRaycasts = false;
        transitionInProgress = false;
    }
    
}