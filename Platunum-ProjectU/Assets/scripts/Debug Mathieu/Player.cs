using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    public int id;
    public Color color;
    public Personnage Personnage;
    public int ControllerId { get; set; }
    private KeyCode[] trackKey;
    private Partition partition;
    public bool hasChanged;

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
            if (Input.GetKeyDown(trackKey[i]) && partition != null)
            {
                //Send TrackKey input
                partition.PlayerInputted(i);
            }
                    
        }

        if (partition != null)
            RoleSwitch();
    }

    private void RoleSwitch()
    {
        if (Personnage.AvailableRole.Length > 1)
        {
            if (!BossManager.Instance.goHurlement && Input.GetKeyDown(KeyCodeUtils.GetKeyCode("Joystick" + ControllerId + "Button5")))
            {
                hasChanged = false;
                if(partition.CurrentRole == Personnage.AvailableRole[0])
                    partition.CurrentRole = Personnage.AvailableRole[1];
                else
                    partition.CurrentRole = Personnage.AvailableRole[0];
            }
            if (BossManager.Instance.goHurlement && !hasChanged)
            {
                if (partition.CurrentRole == Personnage.AvailableRole[0])
                    partition.CurrentRole = Personnage.AvailableRole[1];
                else
                    partition.CurrentRole = Personnage.AvailableRole[0];
                hasChanged = true;
            }
        }
    }

    public void SetPartition(Partition partition)
    {
        this.partition = partition;
    }
}
