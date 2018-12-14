using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO.Ports;

namespace Manager
{
    public class LobbyManager : MonoBehaviour {
        private PlayerManager playerManager;
        private Dictionary<int,bool> ReadyPlayerList;
        public Transform[] PlayerUI;
        public float ReadyTimeHold;
        public float timer;
        public string SceneToLoad;
        private float hpTeam = 0;
        private float shieldTeam = 0;
        private float manaTeam = 0;
        public Transform HPBarTeam;
        public Transform ShieldBarTeam;
        public Transform ManaBarTeam;

        //SERIALPORT
        public gamepads[] Gamepads;

        //Input list
        private List<KeyCode> keyList;

        //Personnage
        public Personnage[] PersonnageAvailable;
        public bool[] SelectedList;
        
        // Use this for initialization
        void Start () {
            playerManager = PlayerManager.Instance;
            ReadyPlayerList = new Dictionary<int, bool>();
            keyList = new List<KeyCode>();
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                string keyName = key.ToString();
                if (keyName.Contains("Joystick") && char.IsDigit(keyName[8]))
                    keyList.Add(key);
            }

            Gamepads = new gamepads[SerialPort.GetPortNames().Length];
            int i = 0;
            foreach (string str in SerialPort.GetPortNames())
            {
                Gamepads[i] = new gamepads((int)char.GetNumericValue(str[str.Length - 1]));
                i++;
            }

            SelectedList = new bool[PersonnageAvailable.Length];

            LoadRatioStatsToBar(HPBarTeam, hpTeam, 950);
            LoadRatioStatsToBar(ShieldBarTeam, shieldTeam, 300);
            LoadRatioStatsToBar(ManaBarTeam, manaTeam, 900);
            HPBarTeam.gameObject.SetActive(true);
            ShieldBarTeam.gameObject.SetActive(true);
            ManaBarTeam.gameObject.SetActive(true);
    }
	
	    // Update is called once per frame
	    void Update () {
            //Par chaque touche filter on regarde l'etat
            for (int i = 0; i < Gamepads.Length; i++)
            {
                Gamepads[i].Update();
            }
            for (int i = 0; i < Gamepads.Length; i++)
            {
                if(!playerManager.IsPlayerAlreadyInLobby(Gamepads[i].portNum, true))
                {
                    if (Gamepads[i].GetKeyDown(4))
                    {
                        //add player=
                        AddPlayerToLobby(Gamepads[i].portNum, Gamepads[i]);
                    }
                }
                else
                {
                    Player player = playerManager.GetPlayer(playerManager.GetPlayerByControllerId(Gamepads[i].portNum));
                    bool currentReadyState = ReadyPlayerList[player.id];
                    //Personnage Gestion
                    if (Gamepads[i].GetKeyDown(0))
                    {
                        changePersonnage(player, 4);
                    }
                    if (Gamepads[i].GetKeyDown(3))
                    {
                        changePersonnage(player, 5);
                    }
                    //Ready Gestion
                    if (Gamepads[i].GetKeyDown(4))
                    {
                        //Player Ready
                        ChangeReadyState(player.id, !currentReadyState);
                    }

                }

            }

            foreach (KeyCode code in keyList)
            {
                if (Input.GetKeyDown(code))
                {
                    string keyCode = code.ToString().Substring(8);
                    int idController = 0;
                    int idButton = -1;

                    //Cas Joystick commun
                    if (!char.IsDigit(keyCode[0])){
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
                        if (!playerManager.IsPlayerAlreadyInLobby(idController, false))
                        {
                            AddPlayerToLobby(idController);
                        }
                        else
                        {
                            Player player = playerManager.GetPlayer(playerManager.GetPlayerByControllerId(idController));
                            bool currentReadyState = ReadyPlayerList[player.id];
                            if (!currentReadyState && (idButton == 4 || idButton == 5) && PlayerManager.Instance.GetPlayersCount() < 4)
                            {
                                //Le joueur de se controleur souhaite changer de class
                                int CurrentPerso = player.Personnage.id; ;//récupère la classes du joueur
                                SelectedList[CurrentPerso] = false;

                                int newPerso = CurrentPerso;//la variable newclass déterminera la prochaine classes
                                                            // Gère les problèmes de tableau pour evité le out of range
                                if (idButton == 5)
                                {
                                    changePersonnage(player, 5);
                                }
                                else //Button 4
                                {
                                    changePersonnage(player, 4);
                                }
                            }
                            if (idButton == 0)
                            {
                                //Change la valeur ready
                                ChangeReadyState(player.id, !currentReadyState);
                            }
                        }
                    }
                }
            }
            CheckReadyPlayer();
        }

        private void CountTeamStats()
        {
            
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
                PlayerUI[PlayerId - 1].Find("State").gameObject.SetActive(state);
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

        private void LoadRatioStatsToBar(Transform bar ,float value, float maxValue)
        {
            float ratio;
            ratio = value / maxValue;
            bar.localScale = new Vector3(ratio, 1, 1);
        }

        private void CheckAvailableRole(Player player, Image atkRole, Image shieldRole, Image manaRole)
        {
            Color c = atkRole.color;
            Color d = shieldRole.color;
            Color e = manaRole.color;

            c.a = 0;
            d.a = 0;
            e.a = 0;

            atkRole.color = c;
            shieldRole.color = d;
            manaRole.color = e;

            c.a = 1;
            d.a = 1;
            e.a = 1;

            for (int i = 0; i < player.Personnage.AvailableRole.Length; i++)
            {
                if (player.Personnage.AvailableRole[i].RoleState == Role.RoleStates.Attack)
                    atkRole.color = c;
                if (player.Personnage.AvailableRole[i].RoleState == Role.RoleStates.Defence)
                    shieldRole.color = d;
                if (player.Personnage.AvailableRole[i].RoleState == Role.RoleStates.Mana)
                    manaRole.color = e;
            }

        }

        public void AddPlayerToLobby(int id, gamepads pads = null)
        {
            int i = 0;
            while (SelectedList[i])
            {
                i++;
            }
            SelectedList[i] = true;
            Player player = playerManager.AddPlayer(id, PersonnageAvailable[i], pads);
            ReadyPlayerList.Add(player.id, false);

            //Update PlayerUI
            //Sprite sprite = player.Personnage.Sprite;
            //PlayerUI[player.id - 1].GetComponent<Image>().color = player.color;
            //PlayerUI[player.id - 1].Find("Sprite").GetComponent<Image>().sprite = sprite;
            //PlayerUI[player.id - 1].Find("Sprite").GetComponent<Image>().preserveAspect = true;
            //PlayerUI[player.id - 1].Find("ClassName").GetComponent<Text>().text = player.Personnage.name;

            updateUI(player);
            player.pads.SetLed(player.id);
        }


        private void changePersonnage(Player player, int idButton)
        {
            //Le joueur de se controleur souhaite changer de class
            int CurrentPerso = player.Personnage.id; ;//récupère la classes du joueur
            SelectedList[CurrentPerso] = false;

            int newPerso = CurrentPerso;//la variable newclass déterminera la prochaine classes
                                        // Gère les problèmes de tableau pour evité le out of range
            int incrementation = 0;
            if (idButton == 5)
            {
                incrementation++;
                //newPerso = (CurrentPerso + 1) % PersonnageAvailable.Length;
            }
            else //Button 4
            {
                incrementation--;
            }
            do
            {
                //newPerso += incrementation;
                if (incrementation > 0)
                {
                    newPerso = (newPerso + 1) % PersonnageAvailable.Length;
                }
                else
                {
                    if (newPerso > 0)
                        newPerso--;
                    else
                        newPerso = PersonnageAvailable.Length -1;
                }
            } while (SelectedList[newPerso]);
            //ajoute la nouvelle classe au joueur
            SelectedList[newPerso] = true;
            player.Personnage = PersonnageAvailable[newPerso];

            updateUI(player);
        }

        private void updateUI(Player player)
        {
            hpTeam = shieldTeam = manaTeam = 0;
            //UI Update
            PlayerUI[player.id - 1].Find("Sprite").GetComponent<Image>().sprite = player.Personnage.Sprite;
            //LoadSpriteCadre
            PlayerUI[player.id - 1].Find("Image").GetComponent<Image>().sprite = player.Personnage.cadreSprite;
            //LoadSpriteReady
            PlayerUI[player.id - 1].Find("State").GetComponent<Image>().sprite = player.Personnage.spriteReady;
            //LoadStats
            LoadRatioStatsToBar(PlayerUI[player.id - 1].Find("HPBar"), player.Personnage.HP, 400);
            LoadRatioStatsToBar(PlayerUI[player.id - 1].Find("ManaBar"), player.Personnage.Mana, 350);
            LoadRatioStatsToBar(PlayerUI[player.id - 1].Find("ShieldBar"), player.Personnage.Shield, 100);

            //LoadRole
            CheckAvailableRole(player, PlayerUI[player.id - 1].Find("AtkLogo").GetComponent<Image>(), PlayerUI[player.id - 1].Find("DefenseLogo").GetComponent<Image>(), PlayerUI[player.id - 1].Find("ManaLogo").GetComponent<Image>());
            
            //SetActive
            PlayerUI[player.id - 1].gameObject.SetActive(true);

            if (PlayerManager.Instance.GetPlayersCount() == 1)
            {
                hpTeam = player.Personnage.HP;
                manaTeam = player.Personnage.Mana;
                shieldTeam = player.Personnage.Shield;
                
            }
            else if(PlayerManager.Instance.GetPlayersCount() > 1){
                List<Player> listPlayerVisible = PlayerManager.Instance.GetPlayers();
                foreach(Player playerVisible in listPlayerVisible)
                {
                    hpTeam += playerVisible.Personnage.HP;
                    shieldTeam += playerVisible.Personnage.Shield;
                    manaTeam += playerVisible.Personnage.Mana;
                }
            }

            LoadRatioStatsToBar(HPBarTeam, hpTeam, 950);
            LoadRatioStatsToBar(ShieldBarTeam, shieldTeam, 300);
            LoadRatioStatsToBar(ManaBarTeam, manaTeam, 900);
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

