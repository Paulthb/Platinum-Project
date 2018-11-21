using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour {

    private bool goAttack1, goAttack2, goAttack3, goAttack4, goAttack5;
    public bool goMalediction;

    private static BossManager instance;
    public static BossManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<BossManager>();
            if (instance == null)
                Debug.Log("No BossManager found");
            return instance;
        }
    }

    public enum BossAttack { MALEDICTION = 0, HURLEMENT, ULTRASON, ULTRALASER, BLOC, BROUILLARD, INVINCIBLITE, QUEUE };

    private BossAttack attack;

    void Update () {
        ChooseAttack();        
    }
	
    public void ChooseAttack()
    {
        if (!goAttack5 && BossBar.Instance.bossPoint <= 100)
        {
            attack = (BossAttack)Random.Range(0, 7);
            Attack(attack);
            goAttack5 = true;
            return;
        }
        if (!goAttack4 && BossBar.Instance.bossPoint <= 250)
        {
            attack = (BossAttack)Random.Range(0, 7);
            Attack(attack);
            goAttack4 = true;
            return;
        }
        if (!goAttack3 && BossBar.Instance.bossPoint <= 450)
        {
            attack = (BossAttack)Random.Range(0, 7);
            Attack(attack);
            goAttack3 = true;
            return;
        }
        if (!goAttack2 && BossBar.Instance.bossPoint <= 650)
        {
            attack = (BossAttack)Random.Range(0, 7);
            Attack(attack);
            goAttack2 = true;
            return;
        }
        if (!goAttack1 && BossBar.Instance.bossPoint <= 850)
        {
            attack = BossAttack.MALEDICTION;
            //attack = (BossAttack)Random.Range(0, 7);
            Attack(attack);
            goAttack1 = true;
            return;
        }
        else
            return;
    }

    public void Attack (BossAttack AttackBoss)
    {
        switch(AttackBoss)
        {
            case BossAttack.MALEDICTION:
                //attendre 10sec en faisant scintiller la piste
                //les notes réussies font perdre des hp pendant x sec
                goMalediction = true;
                StartCoroutine(MaledictionTime());
                break;
            case BossAttack.HURLEMENT:
                //changer de role et les bloquer sur ce role pdt 10 sec
                break;
            case BossAttack.ULTRASON:
                //Etouffer le son de la musique pdt x sec
                break;
            case BossAttack.ULTRALASER:
                //le boss prepare une boule grandissante
                //Les joueurs doivent réussir tant de notes sinon ils se prennent la boule 
                //Sinon l'attaque du boss est annulée
                break;
            case BossAttack.BLOC:
                Debug.Log(AttackBoss);
                break;
            case BossAttack.BROUILLARD:
                Debug.Log(AttackBoss);
                break;
            case BossAttack.INVINCIBLITE:
                Debug.Log(AttackBoss);
                break;
            case BossAttack.QUEUE:
                Debug.Log(AttackBoss);
                break;
        }
    }

    IEnumerator MaledictionTime()
    {
        yield return new WaitForSeconds(10f);
        goMalediction = false;
    }
}
