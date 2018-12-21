using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinGameUI : MonoBehaviour {

    private void Start()
    {
        StartCoroutine(WaitUntilLobby());
    }

    IEnumerator WaitUntilLobby()
    {
        PlayerManager.Instance.EndGame();
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene("Lobby");
    }
}
