using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI uiText = null;
    [SerializeField] private float scoreValuePerSecond = 1.0f;

    private float score = 0;

    // Update is called once per frame
    void Update()
    {
        score += Time.deltaTime * scoreValuePerSecond;

        uiText.text = Mathf.FloorToInt(score).ToString();
    }
}
