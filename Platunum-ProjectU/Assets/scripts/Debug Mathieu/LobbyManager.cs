using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Manager
{
    public class LobbyManager : MonoBehaviour {
        //private List<Player> PlayerList = new List<Player>();
        private PlayerManager playerManager;
        private Dictionary<int,bool> ReadyPlayerList;
        public Transform[] PlayerUI;

        public float ReadyTimeHold;
        public float timer;
        public bool StartGameBool = false;
        public string SceneToLoad;
        private bool GameisReady = false;

        private static bool created = false;
        public ClassManager classManager;

        void Awake()
        {
            if (!created)
            {
                DontDestroyOnLoad(this.gameObject);
                created = true;
            }
            SceneManager.sceneLoaded += onSceneLoaded;
        }
        // Use this for initialization
        void Start () {
            playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>(); 
            ReadyPlayerList = new Dictionary<int, bool>();
            //Debug.Log(Input.GetJoystickNames().);

        }
	
	    // Update is called once per frame
	    void Update () {
            if (!StartGameBool)
            {
                if(Input.anyKey)
                {
                    System.Array values = System.Enum.GetValues(typeof(KeyCode)); //Récupère la liste des touch dispo
                    foreach (KeyCode code in values)
                    {
                        string lastKeyDown = code.ToString();
                        //Si c'est un joystick
                        if (lastKeyDown.Contains("Joystick"))
                        {
                            //Rècupère l'id du joystick
                            char c_ControllerId = lastKeyDown.Substring(8, 1)[0];
                            //Récupère la touch detecté
                            string button = lastKeyDown.Substring(9);

                            //Si c'est un controleur unique
                            if (char.IsDigit(c_ControllerId))
                            {
                                //Converti l'id (String -> Int)
                                int i_ControllerId = (int)char.GetNumericValue(c_ControllerId);
                                //Si l'input est appuyer
                                if(Input.GetKey(code)){
                                    //print(System.Enum.GetName(typeof(KeyCode), code));
                                    if (!playerManager.IsPlayerAlreadyInLobby(i_ControllerId) && button == "Button0")
                                    {

                                        Player player = playerManager.AddPlayer(i_ControllerId);
                                        ReadyPlayerList.Add(player.id, false);

                                        //Update PlayerUI
                                        Sprite sprite = player.Class.icon;
                                        PlayerUI[player.id - 1].Find("Sprite").GetComponent<Image>().sprite = sprite;
                                        PlayerUI[player.id - 1].Find("Sprite").GetComponent<Image>().preserveAspect = true;
                                        PlayerUI[player.id - 1].gameObject.SetActive(true);
                                    }
                                    else
                                    {

                                        if (Input.GetKeyDown(code))
                                        {
                                            if (button == "Button4" || button == "Button5")
                                            {
                                                //Le joueur de se controleur souhaite changer de class
                                                Player player = playerManager.GetPlayer(playerManager.GetPlayerByControllerId(i_ControllerId));
                                                //récupère la classes du joueur
                                                int CurrentClass = classManager.GetClassIdByName(player.Class.name);

                                                int newclass = CurrentClass;//la variable newclass déterminera la prochaine classes

                                                // Gère les problèmes de tableau pour evité le out of range
                                                if (button == "Button5")
                                                {
                                                    newclass = (CurrentClass +1) % classManager.GetClassCount();
                                                }
                                                else
                                                {
                                                    if (CurrentClass > 0)
                                                        newclass = CurrentClass - 1;
                                                    else
                                                        newclass = classManager.GetClassCount() -1;

                                                }
                                                //ajoute la nouvelle classe au joueur
                                                player.Class = classManager.GetClassById(newclass);
                                                //UI Update
                                                PlayerUI[player.id - 1].Find("Sprite").GetComponent<Image>().sprite = player.Class.icon;
                                            }
                                        }
                                        if(button == "Button1")
                                        {
                                            //Le joueur de ce controlleur est ready
                                            int PlayerId = playerManager.GetPlayerByControllerId(i_ControllerId); //recupère l'id
                                            //Change la valeur ready
                                            ChangeReadyState(PlayerId, true);
                                        }
                                    }
                                }
                                else
                                {
                                    //Si la touche pour etre ready n'est pas maintenu -> change la valeur ready du joueur to false
                                    if (button == "Button1" && playerManager.IsPlayerAlreadyInLobby(i_ControllerId))
                                    {
                                        int PlayerId = playerManager.GetPlayerByControllerId(i_ControllerId);
                                        ChangeReadyState(PlayerId, false);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //Aucun input detecté (les joueurs ne m'entienne pas la touche ready
                    //Change tous les joueurs en non ready
                    for (int i = 1; i < ReadyPlayerList.Count+1; i++)
                    {
                        ChangeReadyState(i, false);
                    }
                }
                //Check si tous les joueurs sont ready
                CheckReadyPlayer();
            }
        }

        private void CheckReadyPlayer()
        {
            bool flag = true;
            //Check Si tous les joueurs sont ready
            foreach (KeyValuePair<int,bool> PlayerReadyState in ReadyPlayerList)
            {
                if (!PlayerReadyState.Value)
                    flag = false;
            }
            //Temps total avec tous les joueurs ready
            if (flag && ReadyPlayerList.Count > 0)
                timer += Time.deltaTime;
            else
                timer = 0;
            //Si temps total des joueurs ready atteint lancé le niveau
            if (timer >= ReadyTimeHold)
            {
                StartGame();
            }
        }

        //Fonction pour change la valeur de ready d'un joueur selon son ID
        private void ChangeReadyState(int PlayerId, bool state)
        {
            if (PlayerId > 0)
            {
                PlayerUI[PlayerId - 1].Find("StateText").gameObject.SetActive(state);
                ReadyPlayerList[PlayerId] = state;
            }
        }

        //Lorsque tous les joueurs sont ready
        //Charge la nouvelle scène
        private void StartGame()
        {
            SceneManager.LoadScene(SceneToLoad);
            StartGameBool = true;
        }

        //déclenche le chargement du niveau
        private void onSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (StartGameBool)
            {
                playerManager.LoadScene();
                Destroy(this.gameObject);
            }
        }
    }
}

