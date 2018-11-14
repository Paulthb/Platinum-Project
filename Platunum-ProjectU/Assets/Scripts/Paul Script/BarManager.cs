using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarManager : MonoBehaviour {

    public void GiveMana(int mana)
    {
        ManaBar.Instance.WinMana(mana);
    }

    public void GiveArmor(int armorPoint)
    {
        HealthBar.Instance.WinArmor(armorPoint);
    }

    public void HitBoss(int damage)
    {
        BossBar.Instance.TakeDamage(damage);
    }

    public void GetImpact(int role, PartitionManager.Rank rank)
    {
        switch (rank)
        {
            case PartitionManager.Rank.PERFECT :
                if (role == 1)      // si la note est Perfect et que le role est mana
                    GiveMana(30);
                else if (role == 2)     // si la note est Perfect et que le role est defense
                    GiveArmor(30);
                else HitBoss(30);        // si la note est Perfect et que le role est attack
                break;

            case PartitionManager.Rank.GOOD:
                if (role == 1)      // si la note est Good et que le role est mana
                    GiveMana(20);
                else if (role == 2)     // si la note est Good et que le role est defense
                    GiveArmor(20);
                else HitBoss(20);        // si la note est Good et que le role est attack
                break;

            case PartitionManager.Rank.BAD:
                if (role == 1)      // si la note est Bad et que le role est mana
                    GiveMana(10);
                else if (role == 2)     // si la note est Bad et que le role est defense
                    GiveArmor(10);
                else HitBoss(10);        // si la note est Bad et que le role est attack
                break;

            case PartitionManager.Rank.MISS:
                if (role == 2)     // si la note est Miss et que le role est defense
                    GiveArmor(-10);
                break;
        }
    }
}
