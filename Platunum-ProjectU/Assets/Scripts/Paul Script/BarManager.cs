using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarManager : MonoBehaviour {
    public GameObject GameOverUI;
    public GameObject WinGameUI;
    public GameObject PartitionManagerUI;
    public bool endGame;

    private static BarManager instance;
    public static BarManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<BarManager>();
            if (instance == null)
                Debug.Log("No BarManager found");
            return instance;
        }
    }

    private void Start()
    {
        endGame = false;
        ConductorCustom.songCompletedEvent += EndGame;
    }

    public void EndGame()
    {
        endGame = true;
        ConductorCustom.Instance.pause();
        BossBar.Instance.hide();
        GameOverUI.SetActive(true);
        PartitionManagerUI.SetActive(false);
    }

    public void WinGame()
    {
        endGame = true;
        ConductorCustom.Instance.pause();
        BossBar.Instance.hide();
        WinGameUI.SetActive(true);
        PartitionManagerUI.SetActive(false);
    }
}
