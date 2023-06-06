using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScoreUIText = null;
    [SerializeField] private string highScoreTextPrefix = "High Score: ";

    void Start()
    {
        SetHighScoreText();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    void SetHighScoreText()
    {
        highScoreUIText.text = highScoreTextPrefix + $"{ScoreSystem.GetHighScore()}";
    }
}
