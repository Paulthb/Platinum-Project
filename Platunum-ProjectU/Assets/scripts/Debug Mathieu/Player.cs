﻿using System.Collections;
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
        }
        else if(Controller >=0)
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
        if(ControllerId >= 0)
        {
            KeyCode[] keyCode = new KeyCode[4];
            for (int i = 0; i < keyCode.Length; i++)
            {
                keyCode[i] = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerId + "Button" + i);
            }
            return keyCode;
        }
        return null;
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
                if(ControllerId >= 0)
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
                else
                {
                    if(Input.GetKeyDown(KeyCode.A))
                        partition.PlayerInputted(0);
                    if (Input.GetKeyDown(KeyCode.Z))
                        partition.PlayerInputted(1);
                    if (Input.GetKeyDown(KeyCode.E))
                        partition.PlayerInputted(2);
                    if (Input.GetKeyDown(KeyCode.R))
                        partition.PlayerInputted(3);
                }
            }


            if (partition != null && Personnage.AvailableRole.Length > 1)
            {
                if(pads != null)
                {
                    if (pads.GetKeyDown(4))
                    {
                        if (!BossManager.Instance.goHurlement)
                            SwitchRole();
                        else
                            SoundMgr.Instance.PlaySound("Snd_Cant_Switch");
                    }
                }
                else if(ControllerId >= 0)
                {
                    if (Input.GetKeyDown(KeyCodeUtils.GetKeyCode("Joystick" + ControllerId + "Button5")))
                    {
                        if (!BossManager.Instance.goHurlement)
                            SwitchRole();
                        else
                            SoundMgr.Instance.PlaySound("Snd_Cant_Switch");
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        if (!BossManager.Instance.goHurlement)
                            SwitchRole();
                        else
                            SoundMgr.Instance.PlaySound("Snd_Cant_Switch");
                    }
                }
            }
        }
    }

    public void SwitchRole()
    {
        if (partition.CurrentRole.RoleState == BossManager.Instance.randomRoleState && BossManager.Instance.goMalediction)
        {
            partition.CadreMaudit.gameObject.SetActive(false);
        }
        if (partition.CurrentRole == Personnage.AvailableRole[0])
        {
            partition.ChangeRole(Personnage.AvailableRole[0]);
            partition.CurrentRole = Personnage.AvailableRole[1];
            if(Personnage.AvailableRole[1].RoleState == BossManager.Instance.randomRoleState && BossManager.Instance.goMalediction)
                partition.CadreMaudit.gameObject.SetActive(true);
        }
        else
        {
            partition.ChangeRole(Personnage.AvailableRole[1]);
            partition.CurrentRole = Personnage.AvailableRole[0];
            if (Personnage.AvailableRole[0].RoleState == BossManager.Instance.randomRoleState && BossManager.Instance.goMalediction)
                partition.CadreMaudit.gameObject.SetActive(true);
        }
    }

    public void SetPartition(Partition partition)
    {
        if (partition != null)
            this.partition = partition;
        else
            Debug.Log("Error Partition loading");
    }

    public Partition GetPartition()
    {
        return this.partition;
    }
}
