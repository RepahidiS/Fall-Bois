using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;

    public RankUI rankUI;
    public EndGameUI endGameUI;
    public StartGameUI startGameUI;

    void Awake()
    {
        endGameUI.gameManager = gameManager;
        endGameUI.SetClickListener();
    }

    public void ResetUIs()
    {
        rankUI.Reset();
        endGameUI.Hide();
        startGameUI.Show();
    }

    void FixedUpdate()
    {
        if(startGameUI.goParent.activeSelf)
            startGameUI.IncreaseFontSize();
    }

    public void UpdateCountdownText(string newText)
    {
        startGameUI.UpdateCountdownText(newText);
    }
}

public class UIBase
{
    [HideInInspector] public GameManager gameManager;
    public GameObject goParent;

    public void Show()
    {
        goParent.SetActive(true);
    }

    public void Hide()
    {
        goParent.SetActive(false);
    }
}

[System.Serializable]
public class RankUI : UIBase
{
    public TextMeshProUGUI[] txtRank = new TextMeshProUGUI[6];

    public void Reset()
    {
        for(int i = 0; i < txtRank.Length; i++)
        {
            txtRank[i].text = "";
            txtRank[i].color = Color.white;
        }
    }

    public void UpdateRank(List<string> finishedRank, List<Rank> rank)
    {
        Reset();

        int currentIndex = 0;
        bool playerShowing = false;

        for (int i = 0; i < finishedRank.Count; i++)
        {
            currentIndex++;

            if (i > txtRank.Length - 2)
                break;
            txtRank[i].text = (i + 1) + ". " + finishedRank[i];
            txtRank[i].color = Color.green;
            if (finishedRank[i] == "You")
                playerShowing = true;
        }

        for(int i = 0; i < txtRank.Length - 1; i++)
        {
            if (i + currentIndex > txtRank.Length - 2)
                break;

            txtRank[i + currentIndex].text = (i + currentIndex + 1) + ". " + rank[i].name;
            if (rank[i].name == "You")
            {
                playerShowing = true;
                txtRank[i + currentIndex].color = Color.yellow;
            }
        }

        if(!playerShowing)
        {
            int index = rank.FindIndex(player => player.name == "You");
            if (index == -1)
                index = finishedRank.FindIndex(player => player == "You") + 1;
            else index += finishedRank.Count + 1;
            txtRank[txtRank.Length - 1].text = (index) + ". You";
            txtRank[txtRank.Length - 1].color = Color.yellow;
        }
    }
}

[System.Serializable]
public class EndGameUI : UIBase
{
    public TextMeshProUGUI txtStatistics;
    public Button btnRestart;

    public void SetClickListener()
    {
        btnRestart.onClick.AddListener(() => gameManager.ResetGame()); 
    }

    public void UpdateStatistics(string text)
    {
        txtStatistics.text = text;
    }
}

[System.Serializable]
public class StartGameUI : UIBase
{
    public TextMeshProUGUI txtCountdown;
    int currentFontSize = 20;

    public void UpdateCountdownText(string newText)
    {
        if(txtCountdown.text != newText)
        {
            txtCountdown.text = newText;
            currentFontSize = 20;
            txtCountdown.fontSize = currentFontSize;
        }
    }

    public void IncreaseFontSize()
    {
        currentFontSize += 2;
        txtCountdown.fontSize = currentFontSize;
    }
}