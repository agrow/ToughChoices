using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public class DataExportManager : MonoBehaviour
{
    public static DataExportManager Instance { get; private set;}

    private string filePath;

    private string userID;
    private string sessionID;
    private DateTime sessionDate;

    private int scenarioID;
    private int scenarioOrder;

    private DateTime scenarioBeginTS;

    private DateTime likert1SubmitTS;
    private int likert1Value;
    private DateTime open1SubmitTS;
    private string open1Response;

    private DateTime likert2SubmitTS;
    private int likert2Value;
    private DateTime open2SubmitTS;
    private string open2Response;

    private float volume;
    private float textSpeed;
    private string appVersion;
    private bool completed;

    private string errorCode;
    private string errorMessage;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, "ToughChoicesTest.tsv");

        appVersion = Application.version;
    }

    public void SetUserID(string id)
    {
        userID = id.Trim();
        sessionDate = DateTime.Now;
        sessionID = userID + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");

        Debug.Log("Saved user_id: " + userID);
        Debug.Log("Session ID: " + sessionID);
    }

    private void AppendCurrentScenarioToTSV()
    {
        bool fileExists = File.Exists(filePath);

        string header =
            "user_id\t" +
            "session_id\t" +
            "session_date\t" +
            "scenario_id\t" +
            "scenario_order\t" +
            "scenario_begin_ts\t" +
            "likert1_submit_ts\t" +
            "likert1_value\t" +
            "open1_submit_ts\t" +
            "open1_response\t" +
            "likert2_submit_ts\t" +
            "likert2_value\t" +
            "open2_submit_ts\t" +
            "open2_response\t" +
            "volume\t" +
            "text_speed\t" +
            "app_version\t" +
            "completed\t" +
            "error_code\t" +
            "error_message";

        string row =
            CleanText(userID) + "\t" +
            CleanText(sessionID) + "\t" +
            sessionDate.ToString("yyyy-MM-dd") + "\t" +
            scenarioID + "\t" +
            scenarioOrder + "\t" +
            FormatTimestamp(scenarioBeginTS) + "\t" +
            FormatTimestamp(likert1SubmitTS) + "\t" +
            likert1Value + "\t" +
            FormatTimestamp(open1SubmitTS) + "\t" +
            CleanText(open1Response) + "\t" +
            FormatTimestamp(likert2SubmitTS) + "\t" +
            likert2Value + "\t" +
            FormatTimestamp(open2SubmitTS) + "\t" +
            CleanText(open2Response) + "\t" +
            volume.ToString(
                "0.00",
                CultureInfo.InvariantCulture
            ) + "\t" +
            textSpeed.ToString(
                "0.00",
                CultureInfo.InvariantCulture
            ) + "\t" +
            CleanText(appVersion) + "\t" +
            completed.ToString().ToLower() + "\t" +
            CleanText(errorCode) + "\t" +
            CleanText(errorMessage);

        using (StreamWriter writer =
            new StreamWriter(
                filePath,
                true,
                Encoding.UTF8
            ))
        {
            if (!fileExists)
            {
                writer.WriteLine(header);
            }

            writer.WriteLine(row);
        }

        Debug.Log($"Exported Scenario {scenarioID} data.");
    }

    private string FormatTimestamp(DateTime timestamp)
    {
        if (timestamp == default)
        {
            return "";
        }

        return timestamp
            .ToUniversalTime()
            .ToString(
                "yyyy-MM-dd'T'HH:mm:ss.fff'Z'"
            );
    }

    private string CleanText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return "";
        }

        return text
            .Replace("\t", " ")
            .Replace("\r", " ")
            .Replace("\n", " ");
    }

    public void BeginScenario(int id, int order)
    {
        scenarioID = id;
        scenarioOrder = order;
        scenarioBeginTS = DateTime.Now;

        likert1SubmitTS = default;
        likert1Value = 0;

        open1SubmitTS = default;
        open1Response = "";

        likert2SubmitTS = default;
        likert2Value = 0;

        open2SubmitTS = default;
        open2Response = "";

        errorCode = "";
        errorMessage = "";

        completed = false;

        Debug.Log(
            $"Started Scenario {scenarioID}, order {scenarioOrder}"
        );
    }

    public void RecordLikert1(int value)
    {
        likert1Value = value;
        likert1SubmitTS = DateTime.Now;

        Debug.Log("Recorded Likert 1: " + value);
    }

    public void RecordOpen1(string response)
    {
        open1Response = response;
        open1SubmitTS = DateTime.Now;

        Debug.Log("Recorded Open 1: " + response);
    }

    private void CaptureCurrentSettings()
    {
        volume = SettingsManager.MasterVolume;
        textSpeed = SettingsManager.TextSpeedMultiplier;
    }

    public void CompleteScenario()
    {
        completed = true;

        CaptureCurrentSettings();

        AppendCurrentScenarioToTSV();
    }

    public void RecordError(string code, string message)
    {
        errorCode = code;
        errorMessage = message;

        Debug.LogError($"[{code}] {message}");
    }

}
