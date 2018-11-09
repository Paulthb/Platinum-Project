using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Manager
{
    public class LobbyManager : MonoBehaviour {
        private PlayerManager playerManager;
        private Dictionary<int,bool> ReadyPlayerList;
        public Transform[] PlayerUI;
        public float ReadyTimeHold;
        public float timer;
        public string SceneToLoad;
        //public bool StartGameBool = false;
        //private static bool created = false;
        public ClassManager classManager;

        //Input list
        private List<KeyCode> keyList;
        /*
        void Awake()
        {
            if (!created)
            {
                DontDestroyOnLoad(this.gameObject);
                created = true;
            }
            SceneManager.sceneLoaded += onSceneLoaded;
        }*/
        // Use this for initialization
        void Start () {
            playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>(); 
            ReadyPlayerList = new Dictionary<int, bool>();
            keyList = new List<KeyCode>();
            //Debug.Log(Input.GetJoystickNames().);
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                string keyName = key.ToString();
                if (keyName.Contains("Joystick") && char.IsDigit(keyName[8]))
                    keyList.Add(key);
            }
        }
	
	    // Update is called once per frame
	    void Update () {
            foreach (KeyCode code in keyList)
            {
                if (Input.GetKeyDown(code))
                {
                    string keyCode = code.ToString().Substring(8);
                    int idController = 0;
                    int idButton = -1;
                    //Cas Joystick commun
                    if (!char.IsDigit(keyCode[0])){
                        Debug.Log("manette commune");
                        //String to int
                        idController = 0;
                        idButton = int.Parse(keyCode.Substring(6));
                    }
                    //Cas joystick id >=10
                    else if (char.IsDigit(keyCode[1]))
                    {
                        //String to int
                        idController = int.Parse(keyCode.Substring(0, 2));
                        idButton = int.Parse(keyCode.Substring(8));
                    }
                    //Cas joystick < 10
                    else
                    {
                        idController = int.Parse(keyCode.Substring(0, 1));
                        idButton = int.Parse(keyCode.Substring(7));
                    }
                    if (idController != 0)
                    {
                        if (!playerManager.IsPlayerAlreadyInLobby(idController))
                        {
                            Player player = playerManager.AddPlayer(idController);
                            ReadyPlayerList.Add(player.id, false);

                            //Update PlayerUI
                            Sprite sprite = player.Class.icon;
                            PlayerUI[player.id - 1].Find("Sprite").GetComponent<Image>().sprite = sprite;
                            PlayerUI[player.id - 1].Find("Sprite").GetComponent<Image>().preserveAspect = true;
                            PlayerUI[player.id - 1].gameObject.SetActive(true);
                        }
                        else
                        {
                            Player player = playerManager.GetPlayer(playerManager.GetPlayerByControllerId(idController));
                            bool currentReadyState = ReadyPlayerList[player.id];
                            if (!currentReadyState && (idButton == 4 || idButton == 5))
                            {
                                //Le joueur de se controleur souhaite changer de class
                                int CurrentClass = classManager.GetClassIdByName(player.Class.name);//récupère la classes du joueur
                                int newclass = CurrentClass;//la variable newclass déterminera la prochaine classes
                                                            // Gère les problèmes de tableau pour evité le out of range
                                if (idButton == 5)
                                {
                                    newclass = (CurrentClass + 1) % classManager.GetClassCount();
                                }
                                else //Button 4
                                {
                                    if (CurrentClass > 0)
                                        newclass = CurrentClass - 1;
                                    else
                                        newclass = classManager.GetClassCount() - 1;

                                }
                                //ajoute la nouvelle classe au joueur
                                player.Class = classManager.GetClassById(newclass);
                                //UI Update
                                PlayerUI[player.id - 1].Find("Sprite").GetComponent<Image>().sprite = player.Class.icon;
                            }
                            if (idButton == 0)
                            {
                                //Le joueur de ce controlleur est ready
                                int PlayerId = playerManager.GetPlayerByControllerId(idController); //recupère l'id
                                //Change la valeur ready
                                ChangeReadyState(PlayerId, !currentReadyState);
                            }
                        }
                    }
                }
            }
            CheckReadyPlayer();
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
            //Si tout les joueurs ont étaient détecté ready
            if (flag && ReadyPlayerList.Count > 0)
            {
                timer += Time.deltaTime;
            }
            else //un joueur n'est plus pret
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
            //StartGameBool = true;
        }

        //déclenche le chargement du niveau
        /*
        private void onSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (StartGameBool)
            {
                playerManager.LoadScene();
                Destroy(this.gameObject);
            }
        }
        */
    }
}

