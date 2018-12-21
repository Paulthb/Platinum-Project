using System.Collections;
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

                #if UNITY_EDITOR
                    instance.DebugPerso = (Personnage)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Demoniste", null)[1]), typeof(Personnage));
                #endif
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

    public List<Player> GetPlayers()
    {
        return PlayerList;
    }

    public int GetPlayersCount()
    {
        return PlayerList.Count;
    }

    public int PlayersByRole(Role.RoleStates roleStates)
    {
        int count = 0;
        for (int i = 0; i < PlayerList.Count; i++)
        {
            if (PlayerList[i].GetPartition().CurrentRole.RoleState == roleStates)
                count++;
        }
        return count;
    }

    public void AddDebugPlayer()
    {
        if (System.IO.Ports.SerialPort.GetPortNames().Length > 0)
        {
            string str = System.IO.Ports.SerialPort.GetPortNames()[0];
            gamepads pads = new gamepads((int)char.GetNumericValue(str[str.Length - 1]));
            AddPlayer(1, DebugPerso, pads);
        }
        else if (Input.GetJoystickNames().Length > 0)
        {
            if(Input.GetJoystickNames()[0] != "")
                AddPlayer(1, DebugPerso);
        }
        else
        {
            AddPlayer(-1, DebugPerso);
        }
    }

    public void EndGame()
    {
        foreach (Player player in PlayerList)
        {
            if (player.pads != null)
            {
                player.pads.SetLed(0);
                player.pads.CloseSerial();
            }
        }
        PlayerList.Clear();
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }
}

