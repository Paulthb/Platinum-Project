using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    public int id;
    public Color color;
    public Personnage Personnage;

    //arduino
    public gamepads pads;

    //Controller
    public int ControllerId { get; set; }
    private KeyCode[] trackKey;

    private Partition partition;
    public bool hasChanged;

    // Use this for initialization
    public void LoadPlayer(int Id, Color Color, Personnage playerPerso, int Controller, gamepads pads = null)
    {
        id = Id;
        color = Color;
        Personnage = playerPerso;
        ControllerId = Controller;
        if (pads != null)
        {
            this.pads = pads;
            //Debug.Log("pads loaded");
        }
        else
            trackKey = LoadTrackKey();
    }

    public void LoadArduino(gamepads pads)
    {
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
        if(partition != null)
        {
            if (pads != null)
            {
                pads.Update();
                for (int i = 0; i < 4; i++)
                {
                    if (pads.GetKeyDown(i))
                    {
                        //Send TrackKey input
                        partition.PlayerInputted(i);
                    }
                }
            }
            else
            {
                for (int i = 0; i < trackKey.Length; i++)
                {
                    if (Input.GetKeyDown(trackKey[i]) && partition != null)
                    {
                        //Send TrackKey input
                        partition.PlayerInputted(i);
                    }
                }
            }


            if (partition != null && Personnage.AvailableRole.Length > 1)
            {
                //RoleSwitch();
                if (!BossManager.Instance.goHurlement)
                {
                    if(pads != null)
                    {
                        if (pads.GetKeyDown(4))
                            SwitchRole();
                    }
                    else if(Input.GetKeyDown(KeyCodeUtils.GetKeyCode("Joystick" + ControllerId + "Button5"))){
                        SwitchRole();
                    }
                }
            }
        }
    }

    public void SwitchRole()
    {
        if (partition.CurrentRole == Personnage.AvailableRole[0])
        {
            partition.CurrentRole = Personnage.AvailableRole[1];
            partition.ChangeRole(Personnage.AvailableRole[0]);
        }
        else
        {
            partition.CurrentRole = Personnage.AvailableRole[0];
            partition.ChangeRole(Personnage.AvailableRole[1]);
        }
    }

    public void SetPartition(Partition partition)
    {
        if(partition != null)
            Debug.Log("partition loaded");
        this.partition = partition;
    }

    public Partition GetPartition()
    {
        return this.partition;
    }
}
