using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nouveau Role", menuName = "Role")]
public class Role : ScriptableObject {
    public enum RoleStates { Attack, Defence, Mana};
    public RoleStates RoleState;
    public Sprite RoleSprite;
}
