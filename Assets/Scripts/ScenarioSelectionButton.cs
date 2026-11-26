
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioSelectionButton : MonoBehaviour
{
    [Header("Scenario")]
    [SerializeField] private ScenarioData scenarioData;

    [Header("UI References")]
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text scenarioNameText;
    [SerializeField] private GameObject completeIcon;

    [Header("Visuals")]
    [SerializeField] private Image buttonBackground;
    [SerializeField] private Image scenarioIcon;

    private readonly Color availableBackground = Color.white;

    private readonly Color disabledBackground =
        new Color(0.55f, 0.55f, 0.55f, 1f);

    private readonly Color availableTextColor = Color.white;

    private readonly Color disabledTextColor = Color.gray;

    private ScenarioSequenceManager sequenceManager;
    private ScenarioManager scenarioManager;
    
    private void Awake()
    {
        sequenceManager =
            FindFirstObjectByType<ScenarioSequenceManager>();

        scenarioManager =
            FindFirstObjectByType<ScenarioManager>();
    }

    private void Start()
    {
        button.onClick.AddListener(OnScenarioClicked);

        RefreshButton();
    }

    public void RefreshButton()
    {
        if (scenarioData == null || sequenceManager == null)
        {
            return;
        }

        bool completed =
            sequenceManager.IsScenarioCompleted(
                scenarioData.scenarioID
            );

        bool available =
            sequenceManager.IsScenarioAvailable(scenarioData);

        // Only available scenarios can be clicked.
        button.interactable = available;

        // Display completion checkmark.
        completeIcon.SetActive(completed);

        // Update label.
        if (completed)
        {
            scenarioNameText.text = scenarioData.scenarioName;
        }
        else if (!scenarioData.isAvailable)
        {
            scenarioNameText.text = "Unavailable";
        }
        else
        {
            scenarioNameText.text = scenarioData.scenarioName;
        }

        // Update colors.
        buttonBackground.color =
            available ? availableBackground : disabledBackground;

        scenarioNameText.color =
            available ? availableTextColor : disabledTextColor;

        if (scenarioIcon != null)
        {
            scenarioIcon.color =
                available ? availableTextColor : disabledTextColor;
        }
    }

    private void OnScenarioClicked()
    {
        if (scenarioData == null ||
            sequenceManager == null ||
            scenarioManager == null)
        {
            return;
        }

        Debug.Log(
            "Administrator selected scenario: " +
            scenarioData.scenarioName
        );

        scenarioManager.AdminSelectScenario(scenarioData);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnScenarioClicked);
        }
    }
}
