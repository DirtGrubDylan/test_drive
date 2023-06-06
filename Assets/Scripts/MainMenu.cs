using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScoreUIText = null;
    [SerializeField] private string highScoreTextPrefix = "High Score: ";
    [SerializeField] private TextMeshProUGUI livesUIText = null;
    [SerializeField] private string livesTextPrefix = "Lives: ";
    [SerializeField] private Button buttonUI = null;
    [SerializeField] private TextMeshProUGUI buttonUIText = null;
    [SerializeField] private string buttonUITextNoLives = "NO LIVES!";
    [SerializeField] private string buttonUITextPlay = "PLAY";
    [SerializeField] private string mainLevelSceneName = "Level";

    private int currentLives = 0;

    void Start()
    {
        currentLives = LifeSystem.GetLives();

        SetHighScoreText();
    }

    void Update()
    {
        currentLives = LifeSystem.GetLives();

        SetLivesScoreText();
        UpdatePlayButton();
    }

    public void Play()
    {
        LifeSystem.ReduceLives();

        SceneManager.LoadScene(mainLevelSceneName);
    }

    void SetHighScoreText()
    {
        highScoreUIText.text = Append(highScoreTextPrefix, ScoreSystem.GetHighScore());
    }

    void SetLivesScoreText()
    {
        livesUIText.text = Append(livesTextPrefix, currentLives);
    }

    void UpdatePlayButton()
    {
        if (currentLives == 0)
        {
            buttonUI.interactable = false;
            buttonUIText.text = buttonUITextNoLives;
        }
        else
        {
            buttonUI.interactable = true;
            buttonUIText.text = buttonUITextPlay;
        }
    }

    string Append(string prefix, int value)
    {
        return prefix + $"{value}";
    }
}
