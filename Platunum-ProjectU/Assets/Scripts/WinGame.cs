using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinGame : MonoBehaviour {

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
