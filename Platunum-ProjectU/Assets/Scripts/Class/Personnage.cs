﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nouveau Personnage", menuName = "Personnage")]
public class Personnage : ScriptableObject {
    public int id;
    public new string name;
    public Sprite Sprite;
    public float HP;
    public float Mana;
    public float Shield;
    public Role[] AvailableRole;
    public int idPartition;
    public Sprite cadreSprite;
    public Sprite spriteReady;
    public int PartitionJaugeMax;
}
