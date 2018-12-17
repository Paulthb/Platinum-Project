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
        GameOverUI.SetActive(true);
        PartitionManagerUI.SetActive(false);
    }

    public void WinGame()
    {
        WinGameUI.SetActive(true);
        PartitionManagerUI.SetActive(false);
    }
}
