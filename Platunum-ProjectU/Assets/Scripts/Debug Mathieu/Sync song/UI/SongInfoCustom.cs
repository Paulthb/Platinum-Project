using UnityEngine;
using System;


public class SongInfoCustom : MonoBehaviour 
{
    //Get Instance
    private static SongInfoCustom instance;
    public static SongInfoCustom Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<SongInfoCustom>();
            if (instance == null)
                Debug.Log("No songinfo found");
            return instance;
        }
    }

    public SongInfo currentSong;

	void Start()
	{
		instance = this;

		DontDestroyOnLoad(gameObject);
	}

}
