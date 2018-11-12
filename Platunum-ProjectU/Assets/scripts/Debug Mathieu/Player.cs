using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Manager
{
    public class Player : MonoBehaviour
    {
        public int id;
        public Color color;
        public Class Class;
        public int ControllerId { get; set; }
        private KeyCode[] trackKey;
        public Partition partition; 

        // Use this for initialization
        public Player(int Id, Color Color, Class playerClass, int Controller)
        {
            id = Id;
            color = Color;
            Class = playerClass;
            ControllerId = Controller;
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
        private void Start()
        {
            if (!(ControllerId > 0))
                ControllerId = id;
            trackKey = LoadTrackKey();
        }
        private void Update()
        {
            for (int i = 0; i < trackKey.Length; i++)
            {
                if (Input.GetKey(trackKey[i]))
                {
                    //Send TrackKey input
                    partition.PlayerInputted(i);
                }
            }
        }
    }
}
