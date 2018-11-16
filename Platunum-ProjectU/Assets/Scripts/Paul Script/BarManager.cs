using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarManager : MonoBehaviour {

    private int manaSomme;
    private int manaHitNumber;
    private int attackHitNumber;

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
        manaHitNumber += 1;
        manaSomme += mana;
        if (manaHitNumber == 3)
        {
            Debug.Log("On gagne " + manaSomme + " pts de mana");
            ManaBar.Instance.WinMana(manaSomme);
            manaSomme = 0;
            manaHitNumber = 0;
        }
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
        BossBar.Instance.TakeDamage(damage);
        Debug.Log("Le boss a perdu " + damage + " pts de vie");
        if (attackHitNumber == 3)
        {
            Debug.Log("On a perdu 50 mana");
            ManaBar.Instance.Attack();
            attackHitNumber = 0;
        }
    }

    public void GetImpact(Personnage.Role role, PartitionManager.Rank rank)
    {
        switch (rank)
        {
            case PartitionManager.Rank.PERFECT :
                if (role == Personnage.Role.Mana)      // si la note est Perfect et que le role est mana
                    GiveMana(30);
                else if (role == Personnage.Role.Defense)     // si la note est Perfect et que le role est defense
                    GiveArmor(30);
                else HitBoss(30);        // si la note est Perfect et que le role est attack
                break;

            case PartitionManager.Rank.GOOD:
                if (role == Personnage.Role.Mana)      // si la note est Good et que le role est mana
                    GiveMana(20);
                else if (role == Personnage.Role.Defense)     // si la note est Good et que le role est defense
                    GiveArmor(20);
                else HitBoss(20);        // si la note est Good et que le role est attack
                break;

            case PartitionManager.Rank.BAD:
                if (role == Personnage.Role.Mana)      // si la note est Bad et que le role est mana
                    GiveMana(10);
                else if (role == Personnage.Role.Defense)     // si la note est Bad et que le role est defense
                    GiveArmor(10);
                else HitBoss(10);        // si la note est Bad et que le role est attack
                break;

            case PartitionManager.Rank.MISS:    // si la note est Miss et que le role est defense
                    HitPlayer(8);
                break;
        }
    }
}
