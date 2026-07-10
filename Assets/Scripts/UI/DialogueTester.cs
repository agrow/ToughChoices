using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTester : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button clickAnywhereButton;

    [Header("Choice UI")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button[] choiceButtons;

    private int dialogueIndex = 0;

    private readonly string[] dialogueLines =
    {
        "Hey! I heard there's a leadership position open in our organization. I think you'd be a great fit.",
        "Have you thought about applying?",
    };

    private readonly string[] choiceLabels =
    {
        "VERY LIKELY",
        "SOMEWHAT LIKELY",
        "UNSURE",
        "SOMEWHAT UNLIKELY",
        "VERY UNLIKELY"
    };

    private void Start()
    {
        // Show dialogue immediately
        dialogueUI.SetActive(true);
        choicePanel.SetActive(false);

        // Set first dialogue
        speakerNameText.text = "STEVE";
        dialogueIndex = 0;
        ShowCurrentDialogue();

        // Advance dialogue when clicking anywhere
        clickAnywhereButton.onClick.AddListener(AdvanceDialogue);

        // Hook up the choice buttons
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int capturedIndex = i;
            choiceButtons[i].onClick.AddListener(() => SelectChoice(capturedIndex));
        }
    }

    private void ShowCurrentDialogue()
    {
        dialogueText.text = dialogueLines[dialogueIndex];
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
        // Keep the dialogue visible
        // Just reveal the choice panel
        choicePanel.SetActive(true);

        // Stop allowing dialogue advancement
        clickAnywhereButton.gameObject.SetActive(false);
    }

    private void SelectChoice(int index)
    {
        Debug.Log("Choice selected: " + choiceLabels[index]);

        choicePanel.SetActive(false);

        dialogueText.text = "You selected: " + choiceLabels[index];
    }
}