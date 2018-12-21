using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarManager : MonoBehaviour {
    public GameObject GameOverUI;
    public GameObject WinGameUI;
    public GameObject PartitionManagerUI;
    public bool endGame;
    public bool GameWin;
    public bool GameOver;
    public GameObject Conductor;
    private AudioSource myAudio;

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
        myAudio = Conductor.GetComponent<AudioSource>();
    }

    public void EndGame()
    {
        Debug.Log("end");
        endGame = true;
        GameOver = true;
        BossManager.Instance.GameOverBoss();
        BossBar.Instance.hide();
        GameOverUI.SetActive(true);
        ConductorCustom.Instance.pause();
        SoundMgr.Instance.PlaySound("DefeatGame");
        foreach (Player player in PlayerManager.Instance.GetPlayers())
            player.GetPartition().gameObject.SetActive(false);
    }

    public void WinGame()
    {
        endGame = true;
        GameWin = true;
        BossBar.Instance.hide();
        WinGameUI.SetActive(true);
        ConductorCustom.Instance.pause();
        SoundMgr.Instance.PlaySound("WinGame");
        foreach (Player player in PlayerManager.Instance.GetPlayers())
            player.GetPartition().gameObject.SetActive(false);
    }
}
