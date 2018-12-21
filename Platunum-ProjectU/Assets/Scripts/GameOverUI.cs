using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour {

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
