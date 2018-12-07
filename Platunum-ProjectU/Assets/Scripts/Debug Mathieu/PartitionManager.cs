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
    public Color[] trackColor = { Color.green, Color.red, Color.blue, Color.yellow };

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
                offsetX = -7.25f;
            }
            else
            {
                offsetX = -5f;
            }
        }
        else
        {
            if (playersCount > 1)
            {
                offsetX = -5;
            }
            else
            {
                offsetX = 0;
            }
        }

        foreach (Player player in PlayerManager.Instance.GetPlayers())
        {
            Partition partition = Instantiate(PartitionPrefabs, new Vector3(offsetX, finishLineY, 0), Quaternion.identity, transform).GetComponent<Partition>();
            partition.idplayer = player.id;
            partition.partitionId = player.id;
            player.SetPartition(partition);
            partition.CurrentRole = player.Personnage.AvailableRole[0];
            offsetX += PartitionSpace;
        }
    }
}
