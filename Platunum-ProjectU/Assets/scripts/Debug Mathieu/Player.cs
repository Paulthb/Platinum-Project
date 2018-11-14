using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Manager
{
    public class Player : MonoBehaviour
    {
        public int id;
        public Color color;
        public Personnage Personnage;
        public int ControllerId { get; set; }
        private KeyCode[] trackKey;
        private Partition partition; 

        // Use this for initialization
        public void LoadPlayer(int Id, Color Color, Personnage playerPerso, int Controller)
        {
            id = Id;
            color = Color;
            Personnage = playerPerso;
            ControllerId = Controller;
            trackKey = LoadTrackKey();
        }

        public int GetId()
        {
            return id;
        }

        public Color GetColor()
        {
            return color;
        }

        public int GetControllerId()
        {
            return ControllerId;
        }

        private KeyCode[] LoadTrackKey()
        {
            KeyCode[] keyCode = new KeyCode[4];
            for (int i = 0; i < keyCode.Length; i++)
            {
                keyCode[i] = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick"+ControllerId+"Button"+i);
            }
            return keyCode;
        }
        private void Update()
        {
            for (int i = 0; i < trackKey.Length; i++)
            {
                if (Input.GetKey(trackKey[i]) && partition != null)
                {
                    //Send TrackKey input
                    partition.PlayerInputted(i);
                }
            }

            //Handle Role Switch

        }

        private void RoleSwitch()
        {
            if (Personnage.AvailableRole.Length > 1)
            {
                if (Input.GetKeyDown("Joystick" + ControllerId + "Button4"))
                {
                    partition.currentRole = Personnage.AvailableRole[0];
                }
                if (Input.GetKeyDown("Joystick" + ControllerId + "Button5"))
                {
                    partition.currentRole = Personnage.AvailableRole[1];
                }
            }
        }

        public void SetPartition(Partition partition)
        {
            this.partition = partition;
        }
    }
}
