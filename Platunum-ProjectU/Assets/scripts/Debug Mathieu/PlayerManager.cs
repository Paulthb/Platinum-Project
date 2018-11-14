using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public class PlayerManager : MonoBehaviour
    {
        //public GameObject PlayerPrefabs;
        public Color[] PlayerColorList;
        private List<Player> PlayerList = new List<Player>();
        private IDictionary<Player, Transform> PlayerTransformList;

        private static bool created = false;

        //Get Instance
        private static PlayerManager instance;
        public static PlayerManager Instance
        {
            get
            {
                if (instance == null)
                    instance = GameObject.FindObjectOfType<PlayerManager>();
                if (instance == null)
                    Debug.Log("No tutorial found");
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


        public bool IsPlayerAlreadyInLobby(int controllerId)
        {
            bool AlreadyExist = PlayerList.Exists(item => item.ControllerId == controllerId);
            ///Debug.Log(AlreadyExist);
            return AlreadyExist;
        }


        public Player AddPlayer(int ControllerId, Personnage playerPersonnage)
        {
            int id = PlayerList.Count + 1;
            Player player = new Player(id, PlayerColorList[id-1], playerPersonnage, ControllerId);
            PlayerList.Add(player);
            Debug.Log("Controller id:" + ControllerId + " added as Player n°" + id);
            return player;
        }

        public int GetPlayerByControllerId(int controllerId)
        {
            //while(Player)
            if (PlayerList.Exists(item => item.ControllerId == controllerId))
            {
                Player player = PlayerList.Find(item => item.ControllerId == controllerId);
                return player.id;
            }
            else
                return -1;
        }

        public Player GetPlayer(int id)
        {
            if (PlayerList.Exists(item => item.id == id))
            {
                Player player = PlayerList.Find(item => item.id == id);
                return player;
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
    }
}

