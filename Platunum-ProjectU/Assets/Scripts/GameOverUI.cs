using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour {

    private void Start()
    {
        Destroy(PlayerManager.Instance);
        StartCoroutine(WaitUntilLobby());
    }

    IEnumerator WaitUntilLobby()
    {
        Debug.Log("Start waiting");
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene("Lobby");
    }
}
