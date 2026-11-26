
using System.Collections.Generic;
using UnityEngine;

public class ScenarioSequenceManager : MonoBehaviour
{
    [Header("All Scenarios")]
    [SerializeField] private List<ScenarioData> allScenarios;

    private List<ScenarioData> randomizedScenarios =
        new List<ScenarioData>();

    private HashSet<int> completedScenarioIDs =
        new HashSet<int>();

    private int currentScenarioIndex = 0;

    private void Awake()
    {
        CreateRandomScenarioOrder();
    }

    // Create a randomized sequence of available scenarios.
    public void CreateRandomScenarioOrder()
    {
        randomizedScenarios.Clear();

        foreach (ScenarioData scenario in allScenarios)
        {
            if (scenario != null &&
                scenario.isAvailable &&
                !IsScenarioCompleted(scenario.scenarioID))
            {
                randomizedScenarios.Add(scenario);
            }
        }

        ShuffleScenarios();

        currentScenarioIndex = 0;
    }

    // Randomize using Fisher-Yates shuffle.
    private void ShuffleScenarios()
    {
        for (int i = randomizedScenarios.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            ScenarioData temp = randomizedScenarios[i];

            randomizedScenarios[i] =
                randomizedScenarios[randomIndex];

            randomizedScenarios[randomIndex] = temp;
        }
    }

    // Administrator chooses which scenario plays first.
    public bool SelectStartingScenario(ScenarioData selectedScenario)
    {
        if (selectedScenario == null ||
            !selectedScenario.isAvailable ||
            IsScenarioCompleted(selectedScenario.scenarioID) ||
            !allScenarios.Contains(selectedScenario))
        {
            Debug.LogWarning("Scenario cannot be selected.");
            return false;
        }

        // Generate a new randomized list.
        CreateRandomScenarioOrder();

        // Remove selected scenario from its random position.
        randomizedScenarios.Remove(selectedScenario);

        // Insert selected scenario at the beginning.
        randomizedScenarios.Insert(0, selectedScenario);

        currentScenarioIndex = 0;

        Debug.Log(
            "Starting with scenario: " +
            selectedScenario.scenarioName
        );

        return true;
    }

    public ScenarioData GetCurrentScenario()
    {
        if (randomizedScenarios.Count == 0 ||
            currentScenarioIndex >= randomizedScenarios.Count)
        {
            return null;
        }

        return randomizedScenarios[currentScenarioIndex];
    }

    public ScenarioData GetNextScenario()
    {
        currentScenarioIndex++;

        if (currentScenarioIndex >= randomizedScenarios.Count)
        {
            return null;
        }

        return randomizedScenarios[currentScenarioIndex];
    }

    public bool HasMoreScenarios()
    {
        return currentScenarioIndex <
               randomizedScenarios.Count - 1;
    }

    // Mark a scenario completed after its response is submitted.
    public void MarkScenarioCompleted(int scenarioID)
    {
        completedScenarioIDs.Add(scenarioID);

        Debug.Log(
            "Scenario completed: " + scenarioID
        );
    }

    public bool IsScenarioCompleted(int scenarioID)
    {
        return completedScenarioIDs.Contains(scenarioID);
    }

    public bool IsScenarioAvailable(ScenarioData scenario)
    {
        return scenario != null &&
               scenario.isAvailable &&
               allScenarios.Contains(scenario) &&
               !IsScenarioCompleted(scenario.scenarioID);
    
    }

    public bool OverrideCurrentScenario(ScenarioData selectedScenario)
    {
        if (!IsScenarioAvailable(selectedScenario))
        {
            Debug.LogWarning(
                "Scenario is unavailable or already completed."
            );

            return false;
        }

        // Randomize all remaining available and uncompleted scenarios.
        CreateRandomScenarioOrder();

        // Move the selected scenario to the beginning.
        randomizedScenarios.Remove(selectedScenario);
        randomizedScenarios.Insert(0, selectedScenario);

        currentScenarioIndex = 0;

        Debug.Log(
            "Admin override selected: " +
            selectedScenario.scenarioName
        );

        return true;
    }

    // Start a fresh participant session.
    public void ResetScenarioSequence()
    {
        completedScenarioIDs.Clear();

        CreateRandomScenarioOrder();
    }

}
