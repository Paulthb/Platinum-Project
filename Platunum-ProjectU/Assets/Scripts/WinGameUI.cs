﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinGameUI : MonoBehaviour {

    private void Start()
    {
        Destroy(PlayerManager.Instance);
        StartCoroutine(WaitUntilLobby());
    }

    IEnumerator WaitUntilLobby()
    {
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene("Lobby");
    }
}
