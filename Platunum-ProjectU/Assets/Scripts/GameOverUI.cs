using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour {

    public Button pressAbutton;

    public void Update()
    {
        if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick2Button4")))
        {
            pressAbutton.onClick.Invoke();
        }
    }
    public void ClickTheButton()
    {
        SceneManager.LoadScene("Lobby");
    }
}
