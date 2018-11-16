using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nouveau Personnage", menuName = "Personnage")]
public class Personnage : ScriptableObject {
    public int id;
    public string name;
    public Sprite Sprite;
    public Role[] AvailableRole;
}
