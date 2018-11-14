using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

public class PartitionManager : MonoBehaviour {
    public float startLineY;
    public float finishLineY;

    public float removeLineY;

    public float badOffsetY;
    public float goodOffsetY;
    public float perfectOffsetY;

    public float PartitionSize;
    public float PartitionSpace;


    public enum Rank { PERFECT, GOOD, BAD, MISS };
    public static readonly Color[] trackColor = { Color.green, Color.red, Color.blue, Color.yellow };

    public GameObject PartitionPrefabs;
    public GameObject TrackPrefabs;

    //Get instance
    private static PartitionManager instance;
    public static PartitionManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<PartitionManager>();
            if (instance == null)
                Debug.Log("No PartitionManager found");
            return instance;
        }
    }
    private void Start()
    {
        LoadPlayer();
    }

    //Load Players
    public void LoadPlayer()
    {
        float offsetX = 0;
        int playersCount = PlayerManager.Instance.GetPlayersCount();
        if (playersCount == 0)
        {
            PlayerManager.Instance.AddeDebugPlayer();
            playersCount = 1;
        }
        if ( playersCount % 2 == 0)
        {
            if(playersCount > 2)
            {
                offsetX = -30;
            }
            else
            {
                offsetX = -10;
            }
        }
        else
        {
            if (playersCount > 1)
            {
                offsetX = 20;
            }
            else
            {
                offsetX = 0;
            }
        }
        foreach(Player player in PlayerManager.Instance.GetPlayers())
        {
            Partition partition = Instantiate(new GameObject(), new Vector3(offsetX, 0, 0), Quaternion.identity, transform).AddComponent<Partition>();
            partition.idplayer = player.id;
            partition.partitionId = player.id;
            player.SetPartition(partition);
            partition.currentRole = player.Personnage.AvailableRole[0];
            offsetX += PartitionSpace;
        }
    }
}
