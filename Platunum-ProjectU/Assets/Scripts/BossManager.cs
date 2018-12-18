﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour {

    private bool goAttack1, goAttack2, goAttack3, goAttack4, goAttack5;
    [System.NonSerialized] public bool goMalediction, goHurlement, goInvincibilite, goBloc, goUltralaser;
    public GameObject Conductor;
    private AudioSource myAudio;
    private Animator animatorBoss;
    public GameObject brouillardObject;
    public Animator animatorBrouillard;

    public GameObject StelePrefab;
    public GameObject SteleArrierePrefab;
    private GameObject MaledictionCadre;
    private GameObject MaledictionArriere;

    public float StackDmg;
    public int damageCoupDeQueue = 20;
    public float volumeDownUltrason = 0.16f;
    public float maledictionTime = 10f;
    public float hurlementTime = 10f;
    public float ultrasonTime = 10f;
    public float invincibiliteTime = 4f;
    public float brouillardTime = 10f;
    public float blocGiveHarmony = 10f;
    public int damageLanceFlamme = 100;
    //Ultralaser
    [Header("Ultralaser Settings")]
    public float ultralaserTime = 20f;
    public int resistUltralaser = 3500;
    public int ultralaserDamage = 250;
    private float ultralaserTimer = 0;
    //Stone
    private int StoneLife = 5;

    public List<AttackBoss> ListAttack;
    public Queue<AttackBoss> QueueAttack;
    private AttackBoss nextAttack;

    private List<Partition> StoneRemainingPartitions;

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

    public enum BossAttack { MALEDICTION = 0, HURLEMENT, ULTRASON, ULTRALASER, BLOC, BROUILLARD, INVINCIBLITE, LANCEFLAMME };

    private BossAttack attack;

    void Start()
    {
        animatorBoss = GetComponent<Animator>();
        myAudio = Conductor.GetComponent<AudioSource>();
        QueueAttack = new Queue<AttackBoss>(ListAttack);
        nextAttack = QueueAttack.Peek();
        QueueAttack.Dequeue();
    }

    void Update () {
        if (QueueAttack.Count > 0)
        {
            if (ConductorCustom.Instance.audioSource.time >= nextAttack.Time)
            {
                Attack(nextAttack.Attack);
                nextAttack = QueueAttack.Peek();
                QueueAttack.Dequeue();
            }
        }

        if (goUltralaser)
        {
            ultralaserTimer += Time.deltaTime;
            if (StackDmg >= resistUltralaser)
            {
                Debug.Log("Ultralaser Cancel");
                animatorBoss.SetBool("UltralaserLoop", false);
                //lancer animation de réduction de cast
                goUltralaser = false;
                StackDmg = 0;
                ultralaserTimer = 0;
            }
            else if(ultralaserTimer >= ultralaserTime)
            {
                Debug.Log("Ultralaser Success");
                animatorBoss.SetBool("UltralaserLoop", false);
                animatorBoss.SetTrigger("UltralaserShoot");
                ShieldBar.Instance.TakeDamage(ultralaserDamage);
                //lancer animation de réduction de cast
                goUltralaser = false;
            }
        }

        animatorBoss.SetFloat("BossLife", BossBar.Instance.GetValue());
    }

    public void Attack (BossAttack AttackBoss)
    {
        //Debug.Log(AttackBoss);
        switch (AttackBoss)
        {
            case BossAttack.MALEDICTION:
                //attendre 10sec en faisant scintiller la piste
                //les notes réussies font perdre des hp pendant x sec
                goMalediction = true;
                animatorBoss.SetTrigger("MaledictionTrigger");
                StartCoroutine(MaledictionTime());
                break;
            case BossAttack.HURLEMENT:
                //changer de role et les bloquer sur ce role pdt 10 sec
                animatorBoss.SetTrigger("HurlementTrigger");
                goHurlement = true;
                int count = PlayerManager.Instance.GetPlayersCount();
                for (int i = 0; i < count; i++)
                {
                    if(PlayerManager.Instance.GetPlayer(i + 1).Personnage.id != 0)
                        PlayerManager.Instance.GetPlayer(i+1).SwitchRole();
                }
                StartCoroutine(HurlementTime());
                break;
            case BossAttack.ULTRASON:
                //Etouffer le son de la musique pdt x sec
                animatorBoss.SetTrigger("UltrasonTrigger");
                StartCoroutine(UltrasonTime());
                break;
            case BossAttack.ULTRALASER:
                //le boss prepare une boule grandissante
                //Les joueurs doivent réussir tant de notes sinon ils se prennent la boule
                //Sinon l'attaque du boss est annulée
                animatorBoss.SetTrigger("ultralaserStart");
                animatorBoss.SetBool("UltralaserLoop", true);
                goUltralaser = true;
                StartCoroutine(UltralaserTime());
                //créer un compeur de dégats
                //lancer une coroutine
                //activer le compteur dans le temps de la coroutine
                //à l'issue du temps, checker si la team a dépasser telle valeur de notes
                //si oui, nothing happen, si non hit player
                break;
            case BossAttack.BLOC:
                animatorBoss.SetTrigger("BlocPierreTrigger");
                goBloc = true;
                InitStoneAttack();
                TriggerNextAttackStone();
                break;
            case BossAttack.BROUILLARD:
                //La partie basse des partitions est cachée
                animatorBoss.SetTrigger("BrouillardTrigger");
                StartCoroutine(BrouillardTime());
                foreach (Player player in PlayerManager.Instance.GetPlayers())
                    player.GetPartition().ShowBrouillard(brouillardTime);
                    break;
            case BossAttack.INVINCIBLITE:
                //Le boss est invincible pdt x sec
                animatorBoss.SetTrigger("invincibilité");
                animatorBoss.SetBool("invincibilitéLoop", true);
                goInvincibilite = true;
                StartCoroutine(InvincibiliteTime());
                break;
            case BossAttack.LANCEFLAMME:
                //Lance flamme qui fait des dégâts à l'équipe
                animatorBoss.SetTrigger("LanceFlamme");
                ShieldBar.Instance.TakeDamage(damageLanceFlamme);
                break;
        }
    }

    IEnumerator MaledictionTime()
    {
        yield return new WaitForSeconds(2f);
        LoopAnimMalediction();
        yield return new WaitForSeconds(maledictionTime);
        Destroy(MaledictionCadre);
        Destroy(MaledictionArriere);
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
        animatorBoss.SetBool("invincibilitéLoop", false);
        goInvincibilite = false;
    }
    IEnumerator UltralaserTime()
    {
        yield return new WaitForSeconds(ultralaserTime);
        goUltralaser = false;
    }

    IEnumerator BrouillardTime()
    {
        yield return new WaitForSeconds(2f);
        brouillardObject.SetActive(true);
        yield return new WaitForSeconds(brouillardTime);
        animatorBrouillard.SetTrigger("BrouillardFinTrigger");
        yield return new WaitForSeconds(1f);
        brouillardObject.SetActive(false);
    }

    private void InitStoneAttack()
    {
        StoneRemainingPartitions = new List<Partition>();
        foreach (Player player in PlayerManager.Instance.GetPlayers())
        {
            StoneRemainingPartitions.Add(player.GetPartition());
        }
        StoneLife = 5;
    }

    public void TriggerNextAttackStone()
    {
        StoneLife--;
        //Debug.Log("nb de partitions " + StoneRemainingPartitions.Count);
        if(StoneLife == 0 || StoneRemainingPartitions.Count == 0)
        {
            //Augmenter l'unisson
            HarmonieBar.Instance.GiveHarmonie(blocGiveHarmony);
            goBloc = false;
        }
        else
        {
            Partition partitionSelected = StoneRemainingPartitions[Random.Range(0, StoneRemainingPartitions.Count)];
            partitionSelected.nextNoteIsStone = true;
            if (PlayerManager.Instance.GetPlayersCount() > 1)
            {
                StoneRemainingPartitions.Remove(partitionSelected);
            }
        }

    }

    public void CancelAttackStone()
    {
        goBloc = false;
        StoneRemainingPartitions.Clear();
    }

    public void LoopAnimMalediction()
    {
        Player playerMaudit = PlayerManager.Instance.GetPlayer(1);
        Vector3 CadrePosition = playerMaudit.GetPartition().BackgroundSprite.transform.position;
        Vector3 SteleArrierePosition = playerMaudit.GetPartition().BackgroundSteleSprite.transform.position;

        // Créé le cadre maudit
        MaledictionCadre = Instantiate(StelePrefab, CadrePosition, Quaternion.identity) as GameObject;
        MaledictionCadre.transform.parent = playerMaudit.GetPartition().BackgroundSprite.transform;

        //Créé le cadre arrière maudit si 2 roles sont possibles
        if (playerMaudit.Personnage.AvailableRole.Length > 1)
        {
            MaledictionArriere = Instantiate(SteleArrierePrefab, SteleArrierePosition, Quaternion.identity) as GameObject;
            MaledictionArriere.transform.parent = playerMaudit.GetPartition().BackgroundSteleSprite.transform;
        }
    }
}

[System.Serializable]
public class AttackBoss
{
    public float Time;
    public BossManager.BossAttack Attack;

}
