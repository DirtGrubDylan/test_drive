using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class LifeSystem : MonoBehaviour
{
    private const string livesPlayerPrefKey = "Lives";
    private const int livesPlayerPrefDefault = 5;
    private const string livesDateTimePlayerPrefKey = "LivesDateTime";
    private const string livesDateTimeFormat = "o";

    [SerializeField] private int maxLives = 5;
    [SerializeField] private int lifeGainDurationSeconds = 60;
    [SerializeField] private string livesReadyMessage = "You Have a Life!";

    void Update()
    {
        if (ShouldGenerateLives())
        {
            SetLives(maxLives);
            SetLifeGeneratedUtcNow();
        }
    }

    bool ShouldGenerateLives()
    {
        if (GetLives() > 0)
        {
            return false;
        }

        return GetSecondsUntilLivesRegenerated() <= 0;
    }

    public int GetSecondsUntilLivesRegenerated()
    {
        DateTime now = DateTime.UtcNow;
        DateTime previousLivesGeneratedDateTime = GetLivesGeneratedDateTime();

        TimeSpan interval = now - previousLivesGeneratedDateTime;

        Debug.Log($"Dates: {now} - {previousLivesGeneratedDateTime} = {interval}");
        Debug.Log($"Interval Seconds: {interval.TotalSeconds}");

        return lifeGainDurationSeconds - Convert.ToInt32(interval.TotalSeconds);
    }

    public static DateTime GetLivesGeneratedDateTime()
    {
        string lifeGeneratedDateTimeString =
            PlayerPrefs.GetString(livesDateTimePlayerPrefKey, string.Empty);

        if (lifeGeneratedDateTimeString == string.Empty)
        {
            SetLifeGeneratedUtcNow();
        }

        return DateTime.Parse(lifeGeneratedDateTimeString, null, DateTimeStyles.AdjustToUniversal);
    }

    public static void SetLifeGeneratedUtcNow()
    {
        PlayerPrefs.SetString(livesDateTimePlayerPrefKey, GetUtcNowFormatted());
    }

    public static int GetLives()
    {
        return PlayerPrefs.GetInt(livesPlayerPrefKey, livesPlayerPrefDefault);
    }

    public static void SetLives(int lives)
    {
        PlayerPrefs.SetInt(livesPlayerPrefKey, lives);
    }

    public static void ReduceLives()
    {
        PlayerPrefs.SetInt(livesPlayerPrefKey, GetLives() - 1);
    }

    public static string GetUtcNowFormatted()
    {
        return DateTime.UtcNow.ToString(livesDateTimeFormat);
    }
}
