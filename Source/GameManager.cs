using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;
    public BotManager botManager;
    public PlayerMyself mainCharacter;
    public CheckpointManager checkpointManager;
    public List<Rank> rank = new List<Rank>();
    public List<string> finishedRank = new List<string>();

    public int gameStartCooldown = 3;
    bool isGameStarted = false;
    bool showCountdown = true;
    float currentStartCooldown;

    void Awake()
    {
        uiManager.gameManager = this;
    }

    void Start()
    {
        ResetGame();
    }

    void Update()
    {
        if (showCountdown)
            UpdateCountdown();

        if (isGameStarted)
            UpdateRank();
    }

    public void ResetGame()
    {
        isGameStarted = false;
        showCountdown = true;

        currentStartCooldown = Time.time + gameStartCooldown;

        botManager.ResetBots();
        mainCharacter.ResetMainCharacter();

        uiManager.ResetUIs();
        finishedRank.Clear();
    }

    void UpdateCountdown()
    {
        float remainingTime = currentStartCooldown - Time.time;

        if (remainingTime <= 0.0f)
        {
            botManager.UpdateCanMove(true);
            mainCharacter.canMove = true;
            isGameStarted = true;

            if (remainingTime <= -1.0f)
            {
                uiManager.startGameUI.Hide();
                showCountdown = false;
            }
            else uiManager.UpdateCountdownText("Go!");
        }
        else uiManager.UpdateCountdownText(Mathf.Ceil(remainingTime).ToString());
    }

    void UpdateRank()
    {
        rank.Clear();

        float distance = float.MaxValue;

        if (mainCharacter.isFinished)
        {
            int index = finishedRank.FindIndex(player => player == "You");
            if (index == -1)
            {
                finishedRank.Add("You");
                index = finishedRank.FindIndex(player => player == "You");
                uiManager.endGameUI.UpdateStatistics("You finished at \n" + (index + 1) + ". place!");
                uiManager.endGameUI.Show();
            }
        }
        else
        {
            distance = Vector3.Distance(mainCharacter.transform.position, checkpointManager.goFinish.transform.position);
            rank.Add(new Rank("You", distance));
        }

        foreach(GameObject bot in botManager.bots)
        {
            if(bot.GetComponent<Bot>().isFinished)
            {
                int index = finishedRank.FindIndex(player => player == bot.name);
                if (index == -1)
                    finishedRank.Add(bot.name);
            }
            else
            {
                distance = Vector3.Distance(bot.transform.position, checkpointManager.goFinish.transform.position);
                rank.Add(new Rank(bot.name, distance));
            }
        }

        rank.Sort((player1, player2) =>
        {
            return player1.distance.CompareTo(player2.distance);
        });

        uiManager.rankUI.UpdateRank(finishedRank, rank);
    }
}

public class Rank
{
    public string name;
    public float distance;

    public Rank(string _name, float _distance)
    {
        name = _name;
        distance = _distance;
    }
}