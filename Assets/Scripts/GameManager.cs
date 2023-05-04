using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] TileBoard tileBoard;
    [SerializeField] CanvasGroup gameOverCG;
    [SerializeField] TextMeshProUGUI scoreText; 
    [SerializeField] TextMeshProUGUI highestScoreText; 
    private int highestScore;
    private int score = 0;
    private void Start() {
        highestScore = PlayerPrefs.GetInt("HighestScore", 0);
        NewGame();
    }
    public void NewGame(){
        gameOverCG.interactable = false;
        StartCoroutine(GameOverFade(gameOverCG, 0f, 1f));
        tileBoard.ClearBoard();
        SetScore(0);
        tileBoard.CreateTile();
        tileBoard.CreateTile();
        tileBoard.enabled = true;
    }
    public void GameOver(){
        tileBoard.enabled = false;
        CheckIfNewHighScore();
        gameOverCG.interactable = true;
        StartCoroutine(GameOverFade(gameOverCG, .9f, 1f));
    }
    private IEnumerator GameOverFade(CanvasGroup group, float to, float duration){
        float elapsed = 0f;
        float startingAlpha = group.alpha;
        while(elapsed < duration){
            group.alpha = Mathf.Lerp(startingAlpha, to, elapsed/duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    public void AddPoints(int points){
        SetScore(score + points);
    }
    private void SetScore(int score){
        this.score = score;
        scoreText.text = score.ToString();
    }
    private void CheckIfNewHighScore(){
        if(highestScore >= score) 
            highestScoreText.text = "Best Score - " + highestScore;
        else{
            highestScore = score;
            PlayerPrefs.SetInt("HighestScore", highestScore);
            PlayerPrefs.Save();
            highestScoreText.text = "Best Score - " + highestScore;
        }
    }
}
