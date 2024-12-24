using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI MoneyDisplayText;
    [SerializeField] private TextMeshProUGUI HealthDisplayText;
    [SerializeField] private int StartingMoney;
    [SerializeField] private GameObject GameOverPanel;
    private int CurrentMoney;
    private int CurrentHealth = 100;

    private void Start()
    {
        CurrentMoney = StartingMoney;
        MoneyDisplayText.SetText($"${StartingMoney}");
        HealthDisplayText.SetText($"Health: {CurrentHealth}");
        GameOverPanel.SetActive(false);
    }

    public void AddMoney(int MoneyToAdd)
    {
        CurrentMoney += MoneyToAdd;
        MoneyDisplayText.SetText($"${CurrentMoney}");
    }

    public void ReduceHealth(int amount)
    {
        CurrentHealth -= amount;
        HealthDisplayText.SetText($"Health: {CurrentHealth}");
        if (CurrentHealth <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("Player has died!");
        GameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public int GetMoney()
    {
        return CurrentMoney;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        EntitySummoner.ClearEnemies();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}