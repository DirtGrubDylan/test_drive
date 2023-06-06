using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    private const string highScorePlayerPrefKey = "HighScore";
    private const int highScorePlayerPrefDefault = 0;

    [SerializeField] private TextMeshProUGUI uiText = null;
    [SerializeField] private float scoreValuePerSecond = 1.0f;

    private float currentScore = 0;

    void Update()
    {
        currentScore += Time.deltaTime * scoreValuePerSecond;

        uiText.text = Mathf.FloorToInt(currentScore).ToString();
    }

    void OnDestroy()
    {
        int newHighScoreValue = Math.Max(Mathf.FloorToInt(currentScore), GetHighScore());

        StoreHighScore(newHighScoreValue);
    }

    public static int GetHighScore()
    {

        return PlayerPrefs.GetInt(highScorePlayerPrefKey, highScorePlayerPrefDefault);
    }

    public static void StoreHighScore(int score)
    {

        PlayerPrefs.SetInt(highScorePlayerPrefKey, score);
    }
}
