using System.Collections.Generic;
using UnityEngine;

public class ScenarioSequenceManager : MonoBehaviour
{
    [SerializeField] private List<ScenarioData> allScenarios;

    private List<ScenarioData> randomizedScenarios;
    private int currentScenarioIndex = 0;

    private void Awake()
    {
        CreateRandomScenarioOrder();
    }

    private void CreateRandomScenarioOrder()
    {
        randomizedScenarios = new List<ScenarioData>(allScenarios);

        for (int i = randomizedScenarios.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            ScenarioData temp = randomizedScenarios[i];
            randomizedScenarios[i] = randomizedScenarios[randomIndex];
            randomizedScenarios[randomIndex] = temp;
        }

        currentScenarioIndex = 0;
    }

    public ScenarioData GetCurrentScenario()
    {
        if (randomizedScenarios == null ||
            randomizedScenarios.Count == 0)
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
}