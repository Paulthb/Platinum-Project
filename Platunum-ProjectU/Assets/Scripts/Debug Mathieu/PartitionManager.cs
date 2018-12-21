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


    public enum Rank { PERFECT, GOOD, BAD, MISS, HARMONIE };
    public Color[] trackColor = { Color.green, Color.red, Color.blue, Color.yellow };

    public GameObject PartitionPrefabs;
    public GameObject TrackPrefabs;

    public GameObject AssassinPrefab;
    public GameObject DemonistePrefab;
    public GameObject DruidePrefab;
    public GameObject RodeurPrefab;

    public GameObject Assassin;
    public GameObject Demoniste;
    public GameObject Druide;
    public GameObject Rodeur;

    private SpriteRenderer assRenderer;
    private SpriteRenderer demRenderer;
    private SpriteRenderer druRenderer;
    private SpriteRenderer rodRenderer;

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
        if (PlayerManager.Instance)
            LoadPlayer();
    }

    //Load Players
    public void LoadPlayer()
    {
        float offsetX = 0;
        int playersCount = PlayerManager.Instance.GetPlayersCount();
        if (playersCount == 0)
        {
            PlayerManager.Instance.AddDebugPlayer();
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
                offsetX = -2.5f;
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
        int i = 0;
        foreach (Player player in PlayerManager.Instance.GetPlayers())
        {
            Partition partition = Instantiate(PartitionPrefabs, new Vector3(offsetX, finishLineY, 0), Quaternion.identity, transform).GetComponent<Partition>();
            partition.idplayer = player.id;
            partition.partitionId = player.Personnage.idPartition;
            partition.maxNbNote = player.Personnage.PartitionJaugeMax;
            player.SetPartition(partition);
            partition.CurrentRole = player.Personnage.AvailableRole[0];
            if (player.Personnage.AvailableRole.Length > 1)
                partition.ChangeRole(player.Personnage.AvailableRole[1]);
            else
                partition.transform.Find("BackgroundStele").gameObject.SetActive(false);
            offsetX += PartitionSpace;
            if (i + 1 == 2)
                offsetX += 0.2f;
            i++;
            if (player.Personnage.id == 0)
            {
                Assassin = Instantiate(AssassinPrefab, partition.transform.position + new Vector3(0, 5.775f, 0), Quaternion.identity, transform) as GameObject;
                assRenderer = Assassin.gameObject.GetComponent<SpriteRenderer>();
                if (partition.idplayer >= 3)
                    assRenderer.flipX = true;
                else
                    assRenderer.flipX = false;
            }
            else if (player.Personnage.id == 1)
            {
                Demoniste = Instantiate(DemonistePrefab, partition.transform.position + new Vector3(0, 5.775f, 0), Quaternion.identity, transform) as GameObject;
                demRenderer = Demoniste.gameObject.GetComponent<SpriteRenderer>();
                if (partition.idplayer >= 3)
                    demRenderer.flipX = true;
                else
                    demRenderer.flipX = false;
            }
            else if (player.Personnage.id == 2)
            {
                Druide = Instantiate(DruidePrefab, partition.transform.position + new Vector3(0, 5.775f, 0), Quaternion.identity, transform) as GameObject;
                druRenderer = Druide.gameObject.GetComponent<SpriteRenderer>();
                if (partition.idplayer >= 3)
                    druRenderer.flipX = true;
                else
                    druRenderer.flipX = false;
            }
            else if (player.Personnage.id == 3)
            {
                Rodeur = Instantiate(RodeurPrefab, partition.transform.position + new Vector3(0, 5.775f, 0), Quaternion.identity, transform) as GameObject;
                rodRenderer = Rodeur.gameObject.GetComponent<SpriteRenderer>();
                if (partition.idplayer >= 3)
                    rodRenderer.flipX = true;
                else
                    rodRenderer.flipX = false;
            }
        }
    }
}
