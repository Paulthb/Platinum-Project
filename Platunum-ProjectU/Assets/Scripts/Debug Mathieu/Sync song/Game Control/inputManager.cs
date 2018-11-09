using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inputManager : MonoBehaviour {

    //singleton
    public static inputManager instance;
    public KeyCodePlayer[] keyCodePlayer;
    public KeyCode GetKeyCode(int partitionId, int i)
    {
        return keyCodePlayer[partitionId].keys[i];
    }

    void Awake()
    {
        //singleton
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    [System.Serializable]
    public class KeyCodePlayer
    {
        public KeyCode[] keys;
    }
}
