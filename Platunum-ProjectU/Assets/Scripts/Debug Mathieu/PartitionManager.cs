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
                offsetX = -20;
            }
            else
            {
                offsetX = 0;
            }
        }

        foreach (Player player in PlayerManager.Instance.GetPlayers())
        {
            GameObject partitionObject = Instantiate(new GameObject(), new Vector3(offsetX, finishLineY, 0), Quaternion.identity, transform);
            Partition partition = partitionObject.AddComponent<Partition>();
            float sizeX = PartitionSize + 2;
            float sizeY = Mathf.Abs(removeLineY - startLineY + 2);
            GameObject background = Instantiate(new GameObject(), new Vector3(offsetX, finishLineY + sizeY/3, 0), Quaternion.identity, partitionObject.transform);
            SpriteRenderer backgroundSprite = background.AddComponent<SpriteRenderer>();
            background.transform.localScale = new Vector3(sizeX, sizeY, 1f);

            Texture2D tex = new Texture2D(1, 1);
            Color backgroundColor = player.color;
            backgroundColor.a = 0.5f;
            tex.SetPixel(0, 0, backgroundColor);
            tex.Apply();
            backgroundSprite.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 1.0f);

            partition.idplayer = player.id;
            partition.partitionId = player.id;
            player.SetPartition(partition);
            partition.CurrentRole = player.Personnage.AvailableRole[0];
            offsetX += PartitionSpace;
        }
    }
}
