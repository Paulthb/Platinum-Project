using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarManager : MonoBehaviour {
    public GameObject GameOverUI;
    public GameObject WinGameUI;
    public GameObject PartitionManagerUI;

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
        ConductorCustom.songCompletedEvent += EndGame;
    }

    public void EndGame()
    {
        BossBar.Instance.hide();
        GameOverUI.SetActive(true);
        PartitionManagerUI.SetActive(false);
        //Destroy(SongInfoCustom.Instance.gameObject);
        Destroy(PlayerManager.Instance.gameObject);
    }

    public void WinGame()
    {
        BossBar.Instance.hide();
        WinGameUI.SetActive(true);
        PartitionManagerUI.SetActive(false);
        //Destroy(SongInfoCustom.Instance.gameObject);
        Destroy(PlayerManager.Instance.gameObject);
    }
}
