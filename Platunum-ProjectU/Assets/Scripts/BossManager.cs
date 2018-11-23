using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour {

    private bool goAttack1, goAttack2, goAttack3, goAttack4, goAttack5;
    public bool goMalediction, goHurlement, goInvincibilite;
    public GameObject Conductor;
    private AudioSource myAudio;

    public int damageCoupDeQueue = 20;
    public float volumeDownUltrason = 0.16f;
    public float maledictionTime = 10f;
    public float hurlementTime = 10f;
    public float ultrasonTime = 10f;
    public float invincibiliteTime = 10f;

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

    void Start()
    {
        myAudio = Conductor.GetComponent<AudioSource>();
    }

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
            attack = BossAttack.HURLEMENT;
            Attack(attack);
            goAttack2 = true;
            return;
        }
        if (!goAttack1 && BossBar.Instance.bossPoint <= 850)
        {
            attack = BossAttack.ULTRASON;
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
                goHurlement = true;
                StartCoroutine(HurlementTime());
                break;
            case BossAttack.ULTRASON:
                //Etouffer le son de la musique pdt x sec
                StartCoroutine(UltrasonTime());
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
                //La partie basse des partitions est cachée
                Debug.Log(AttackBoss);
                break;
            case BossAttack.INVINCIBLITE:
                //Le boss est invincible pdt x sec
                goInvincibilite = true;
                StartCoroutine(InvincibiliteTime());
                break;
            case BossAttack.QUEUE:
                //Coup de queue qui fait des dégâts à l'équipe
                BarManager.Instance.HitPlayer(damageCoupDeQueue);
                break;
        }
    }

    IEnumerator MaledictionTime()
    {
        yield return new WaitForSeconds(maledictionTime);
        goMalediction = false;
    }
    IEnumerator HurlementTime()
    {
        yield return new WaitForSeconds(hurlementTime);
        goHurlement = false;
    }
    IEnumerator UltrasonTime()
    {
        myAudio.volume = volumeDownUltrason;
        yield return new WaitForSeconds(ultrasonTime);
        myAudio.volume = 0.6f;
    }
    IEnumerator InvincibiliteTime()
    {
        yield return new WaitForSeconds(invincibiliteTime);
        goInvincibilite = false;
    }
}
