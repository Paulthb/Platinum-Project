using UnityEngine;

public class DestroyWhenSongCompleted : MonoBehaviour 
{

	void Awake()
	{
		ConductorCustom.songCompletedEvent += DestroyWhenCompleted;
	}

	void OnDestroy()
	{
		ConductorCustom.songCompletedEvent -= DestroyWhenCompleted;
	}

	void DestroyWhenCompleted()
	{
		Destroy(gameObject);
	}
}
