using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarManager : MonoBehaviour {

    private int manaSomme;
    private int manaHitNumber;
    private int attackHitNumber;

    public GameObject GameOverUI;
    public GameObject WinGameUI;
    public GameObject PartitionManagerUI;

    private static BarManager instance;
    public static BarManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<BarManager>();
            if (instance == null)
                Debug.Log("No BarManager found");
            return instance;
        }
    }

    public void GiveMana(int mana)
    {
        ManaBar.Instance.WinMana(mana);
    }

    public void GiveArmor(int armorPoint)
    {
        HealthBar.Instance.WinArmor(armorPoint);
    }

    public void HitPlayer(int damagePt)
    {
        HealthBar.Instance.TakeDamage(damagePt);
    }

    public void HitBoss(int damage)
    {
        attackHitNumber += 1;
        if (ManaBar.Instance.manaPoint > 50)
            BossBar.Instance.TakeDamage(damage);
        else Debug.Log("Pas assez de mana pour attaquer");
        //Debug.Log("Le boss a perdu " + damage + " pts de vie");
        if (attackHitNumber == 3)
        {
            //Debug.Log("On a perdu 50 mana");
            ManaBar.Instance.Attack();
            attackHitNumber = 0;
        }
    }

    public void GetImpact(Role role, PartitionManager.Rank rank)
    {
        switch (rank)
        {
            case PartitionManager.Rank.PERFECT :
                if (BossManager.Instance.goMalediction)
                    HitPlayer(4);
                if (role.RoleState == Role.RoleStates.Mana)      // si la note est Perfect et que le role est mana
                    GiveMana(30);
                else if (role.RoleState == Role.RoleStates.Defence)     // si la note est Perfect et que le role est defense
                    GiveArmor(30);
                else if(!BossManager.Instance.goInvincibilite)
                    HitBoss(30);        // si la note est Perfect et que le role est attack
                break;

            case PartitionManager.Rank.GOOD:
                if (BossManager.Instance.goMalediction)
                    HitPlayer(4);
                if (role.RoleState == Role.RoleStates.Mana)      // si la note est Good et que le role est mana
                    GiveMana(20);
                else if (role.RoleState == Role.RoleStates.Defence)     // si la note est Good et que le role est defense
                    GiveArmor(20);
                else if (!BossManager.Instance.goInvincibilite)
                    HitBoss(20);        // si la note est Good et que le role est attack
                break;

            case PartitionManager.Rank.BAD:
                if (BossManager.Instance.goMalediction)
                    HitPlayer(4);
                if (role.RoleState == Role.RoleStates.Mana)      // si la note est Bad et que le role est mana
                    GiveMana(10);
                else if (role.RoleState == Role.RoleStates.Defence)     // si la note est Bad et que le role est defense
                    GiveArmor(10);
                else if (!BossManager.Instance.goInvincibilite)
                    HitBoss(10);        // si la note est Bad et que le role est attack
                break;

            case PartitionManager.Rank.MISS:    // si la note est Miss
                    HitPlayer(8);
                break;
        }
    }

    public void EndGame()
    {
        GameOverUI.SetActive(true);
        PartitionManagerUI.SetActive(false);
    }

    public void WinGame()
    {
        WinGameUI.SetActive(true);
        PartitionManagerUI.SetActive(false);
    }
}
