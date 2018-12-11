﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class PlayerManager : MonoBehaviour
{
    //public GameObject PlayerPrefabs;
    public Color[] PlayerColorList;
    private List<Player> PlayerList = new List<Player>();
    private IDictionary<Player, Transform> PlayerTransformList;

    private static bool created = false;
    public Personnage DebugPerso;
    //Get Instance
    private static PlayerManager instance;
    public static PlayerManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<PlayerManager>();
            if (instance == null)
            {
                instance = Instantiate(new GameObject()).AddComponent<PlayerManager>();
                instance.PlayerColorList = new Color[4] { Color.green, Color.red, Color.blue, Color.yellow };
                instance.DebugPerso = (Personnage)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Demoniste", null)[0]), typeof(Personnage));
            }
            return instance;
        }
    }

    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
    }

    // Use this for initialization
    void Start()
    {
        /*
        //Spawn Player
        PlayerList.Add(new Player(1, Color.red, 1));
        PlayerList.Add(new Player(2, Color.blue, 2));
        PlayerList.Add(new Player(3, Color.green, 3));
        PlayerList.Add(new Player(4, Color.magenta, 4));
        foreach (Player player in PlayerList)
        {
            GameObject playerObject = Instantiate(PlayerPrefabs, SpawnPositions[player.GetId() - 1].position, Quaternion.identity, PlayerFolder);
            playerObject.GetComponent<SpriteRenderer>().color = player.GetColor();
        }*/
    }


    public bool IsPlayerAlreadyInLobby(int id, bool pads)
    {
        bool AlreadyExist = false;
        if (!pads)
        {
            AlreadyExist = PlayerList.Exists(item => item.ControllerId == id);
        }
        else
        {
            int i = 0;
            while (!AlreadyExist && i < PlayerList.Count)
            {
                if(PlayerList[i].pads != null)
                {
                    if (PlayerList[i].pads.portNum == id)
                        AlreadyExist = true;
                }
                i++;
            }
            //AlreadyExist = PlayerList.Exists(item => item.pads.portNum == id);
        }
        return AlreadyExist;
    }


    public Player AddPlayer(int ControllerId, Personnage playerPersonnage, gamepads gamepads=null)
    {
        int id = PlayerList.Count + 1;
        GameObject playerObject = new GameObject("Player " + id);
        Player player = Instantiate(playerObject, transform).AddComponent<Player>();
        player.LoadPlayer(id, PlayerColorList[id-1], playerPersonnage, ControllerId, gamepads);
        PlayerList.Add(player);
        Debug.Log("Controller id:" + ControllerId + " added as Player n°" + id);
        return player;
    }

    public int GetPlayerByControllerId(int id)
    {
        //while(Player)
        if (PlayerList.Exists(item => item.ControllerId == id))
        {
            Player player = PlayerList.Find(item => item.ControllerId == id);
            return player.id;
        }
        else if (PlayerList.Exists(item => item.pads.portNum == id))
        {
            Player player = PlayerList.Find(item => item.ControllerId == id);
            return player.id;
        }
        else
            return -1;
    }

    public Player GetPlayer(int id)
    {
        if (PlayerList.Exists(item => item.id == id))
        {
          return PlayerList.Find(item => item.id == id);
        }
        else
            return null;
    }

    public void LoadScene()
    {
        /*for (int i = 0; i < PlayerList.Count; i++)
        {
            GameObject NewPlayer = Instantiate()
            Transform playerTransform = transform.GetChild(i);
            playerTransform.gameObject.SetActive(true);
            Transform CurrentSpawn = GameObject.Find("SpawnTransformFolder").transform.GetChild(i);
            Transform Character = playerTransform.GetChild(0);
            //Character.GetComponent<SpriteRenderer>().color = playerTransform.GetComponent<Manager.Player>().color;
            Character.position = CurrentSpawn.position;
            Transform anchor = playerTransform.GetChild(1);
            anchor.position = new Vector3(CurrentSpawn.position.x, CurrentSpawn.position.y + anchor.position.y, CurrentSpawn.position.z);
        }*/
        /*foreach (var item in PlayerList)
        {
            Transform CurrentSpawn = GameObject.Find("SpawnTransformFolder").transform.GetChild(item.id);
            GameObject NewPlayer = Instantiate(GameObject.Find("LobbyManager").GetComponent<LobbyManager>().GetPrefabsById(item.idPrefabs), CurrentSpawn.position, Quaternion.identity, transform);
            NewPlayer.GetComponent<Player>().Load(item.id, item.idPrefabs, item.ControllerId);
        }*/
        //Do Something when Level is loaded
    }

    public List<Player> GetPlayers()
    {
        return PlayerList;
    }

    public int GetPlayersCount()
    {
        return PlayerList.Count;
    }

    public void AddDebugPlayer()
    {
        if(Input.GetJoystickNames().Length > 0)
        {
            if(Input.GetJoystickNames()[0] != "")
                AddPlayer(1, DebugPerso);
        }
        if(System.IO.Ports.SerialPort.GetPortNames().Length > 0)
        {
            string str = System.IO.Ports.SerialPort.GetPortNames()[0];
            gamepads pads = new gamepads((int)char.GetNumericValue(str[str.Length - 1]));
            AddPlayer(1, DebugPerso, pads);
        }
    }
}

