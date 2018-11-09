using UnityEngine;
using System;


public class SongInfoCustom : MonoBehaviour 
{
	public static SongInfoCustom Instance = null;

	public SongInfo currentSong;

	void Start()
	{
		Instance = this;

		DontDestroyOnLoad(gameObject);
	}

}
