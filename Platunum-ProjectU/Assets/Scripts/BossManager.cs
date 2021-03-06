﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour {

    private bool goAttack1, goAttack2, goAttack3, goAttack4, goAttack5;
    [System.NonSerialized] public bool goMalediction, goHurlement, goInvincibilite, goBloc, goUltralaser, goUltrason;
    public GameObject Conductor;
    private AudioSource myAudio;
    private Animator animatorBoss;
    public GameObject brouillardObject;
    public Animator animatorBrouillard;
    public GameObject brouillardStele;
    public GameObject brouillardSteleFin;

    public Role.RoleStates randomRoleState;
    public GameObject MaledictionStelePrefab;
    public GameObject MaledictionCadre;
    public List<int> partitionsMaudites;

    public GameObject HurlementStelePrefab;
    private GameObject HurlementSteleArriere;

    public Transform targetBoss;
    public float StackDmg;
    public int damageCoupDeQueue = 20;
    public float volumeDownUltrason = 0.16f;
    public float maledictionTime = 10f;
    public float hurlementTime = 10f;
    public float ultrasonTime = 10f;
    public float invincibiliteTime = 4f;
    public float beforeInvincibilite = 1.5f;
    public float brouillardTime = 10f;
    public float blocGiveHarmony = 10f;
    public int damageLanceFlamme = 100;
    public float damageLanceFlammeTime = 2f;
    //Ultralaser
    [Header("Ultralaser Settings")]
    public float ultralaserTime = 20f;
    public float ultralaserDamageTime = 2f;
    public int resistUltralaser = 3500;
    public int ultralaserDamage = 250;
    private float ultralaserTimer = 0;
    //Stone
    private int StoneLife = 5;

    //Ultrason
    private float Timer;
    private float soundVolume;
    [Header("Ultrason Settings")]
    public float fadeOut = 1f;

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
        soundVolume = myAudio.volume;
        QueueAttack = new Queue<AttackBoss>(ListAttack);
        nextAttack = QueueAttack.Peek();
        QueueAttack.Dequeue();
    }

    void Update () {
        if (QueueAttack.Count > 0)
        {
            if (ConductorCustom.Instance.audioSource.time >= nextAttack.Time && BarManager.Instance.endGame == false)
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
                animatorBoss.SetTrigger("UltralaserBreak");
                SoundMgr.Instance.StopSound("AttqUltralaser");
                SoundMgr.Instance.PlaySound("AttqUltralaserBreak");
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
                StartCoroutine(UltralaserDamageTime());
                //ShieldBar.Instance.TakeDamage(ultralaserDamage);
                //lancer animation de réduction de cast
                goUltralaser = false;
                StackDmg = 0;
                ultralaserTimer = 0;
            }
        }
        animatorBoss.SetFloat("BossLife", BossBar.Instance.GetValue());

        if (!goUltrason)
        {
            if(myAudio.volume < soundVolume)
            {
                Timer += fadeOut * Time.deltaTime;
                myAudio.volume = Mathf.Lerp(volumeDownUltrason, soundVolume, Timer);
            }
            else
            {
                myAudio.volume = soundVolume;
                Timer = 0;
            }
        }
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
                SoundMgr.Instance.PlaySound("AttqPoison");
                StartCoroutine(MaledictionTime());
                break;
            case BossAttack.HURLEMENT:
                //changer de role et les bloquer sur ce role pdt 10 sec
                animatorBoss.SetTrigger("HurlementTrigger");
                goHurlement = true;
                SoundMgr.Instance.PlaySound("AttqHurlement");
                int count = PlayerManager.Instance.GetPlayersCount();
                for (int i = 0; i < count; i++)
                {
                    Player player = PlayerManager.Instance.GetPlayer(i + 1);
                    Vector3 SteleArrierePosition = player.GetPartition().BackgroundSteleSprite.transform.position;
                    if (PlayerManager.Instance.GetPlayer(i + 1).Personnage.id != 0)
                    {
                        PlayerManager.Instance.GetPlayer(i + 1).SwitchRole();
                        HurlementSteleArriere = Instantiate(HurlementStelePrefab, SteleArrierePosition + new Vector3(0.1f,0,0), Quaternion.identity) as GameObject;
                        HurlementSteleArriere.transform.parent = player.GetPartition().BackgroundSteleSprite.transform;
                    }
                        
                }
                StartCoroutine(HurlementTime());
                break;
            case BossAttack.ULTRASON:
                //Etouffer le son de la musique pdt x sec
                goUltrason = true;
                soundVolume = myAudio.volume;
                animatorBoss.SetTrigger("UltrasonTrigger");
                SoundMgr.Instance.PlaySound("AttqUltrason");
                StartCoroutine(UltrasonTime());
                break;
            case BossAttack.ULTRALASER:
                //le boss prepare une boule grandissante
                //Les joueurs doivent réussir tant de notes sinon ils se prennent la boule
                //Sinon l'attaque du boss est annulée
                animatorBoss.SetTrigger("ultralaserStart");
                animatorBoss.SetBool("UltralaserLoop", true);
                SoundMgr.Instance.PlaySound("AttqUltralaser");
                goUltralaser = true;
                //créer un compeur de dégats
                //lancer une coroutine
                //activer le compteur dans le temps de la coroutine
                //à l'issue du temps, checker si la team a dépasser telle valeur de notes
                //si oui, nothing happen, si non hit player
                break;
            case BossAttack.BLOC:
                animatorBoss.SetTrigger("BlocPierreTrigger");
                SoundMgr.Instance.PlaySound("AttqBloc");
                goBloc = true;
                InitStoneAttack();
                TriggerNextAttackStone();
                break;
            case BossAttack.BROUILLARD:
                //La partie basse des partitions est cachée
                animatorBoss.SetTrigger("BrouillardTrigger");
                SoundMgr.Instance.PlaySound("AttqBrouillard");
                StartCoroutine(BrouillardTime());
                //foreach (Player player in PlayerManager.Instance.GetPlayers())
                //    player.GetPartition().ShowBrouillard(brouillardTime);
                break;
            case BossAttack.INVINCIBLITE:
                //Le boss est invincible pdt x sec
                animatorBoss.SetTrigger("invincibilité");
                StartCoroutine(TimeBeforeInvincibilite());
                //animatorBoss.SetBool("invincibilitéLoop", true);
                //goInvincibilite = true;
                //StartCoroutine(InvincibiliteTime());
                break;
            case BossAttack.LANCEFLAMME:
                //Lance flamme qui fait des dégâts à l'équipe
                animatorBoss.SetTrigger("LanceFlamme");
                SoundMgr.Instance.PlaySound("AttqFlamme");
                StartCoroutine(LanceflammeDamageTime());
                //ShieldBar.Instance.TakeDamage(damageLanceFlamme);
                break;
        }
    }

    IEnumerator MaledictionTime()
    {
        yield return new WaitForSeconds(2f);
        LoopAnimMalediction();
        yield return new WaitForSeconds(maledictionTime);
        foreach(Player player in PlayerManager.Instance.GetPlayers())
        {
            player.GetPartition().CadreMaudit.gameObject.SetActive(false);
        }
        goMalediction = false;
    }
    IEnumerator HurlementTime()
    {
        yield return new WaitForSeconds(hurlementTime);
        foreach(Player player in PlayerManager.Instance.GetPlayers())
        {
            if(player.Personnage.id != 0)
                Destroy(player.GetPartition().BackgroundSteleSprite.transform.GetChild(0).gameObject);
        }
        goHurlement = false;
    }
    IEnumerator UltrasonTime()
    {
        myAudio.volume = volumeDownUltrason;
        yield return new WaitForSeconds(ultrasonTime);
        goUltrason = false;
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

    IEnumerator UltralaserDamageTime()
    {
        yield return new WaitForSeconds(ultralaserDamageTime);
        ShieldBar.Instance.TakeDamage(ultralaserDamage);
    }

    IEnumerator LanceflammeDamageTime()
    {
        yield return new WaitForSeconds(damageLanceFlammeTime);
        ShieldBar.Instance.TakeDamage(damageLanceFlamme);
    }

    IEnumerator TimeBeforeInvincibilite()
    {
        yield return new WaitForSeconds(beforeInvincibilite);
        animatorBoss.SetBool("invincibilitéLoop", true);
        goInvincibilite = true;
        StartCoroutine(InvincibiliteTime());
    }

    IEnumerator BrouillardTime()
    {
        yield return new WaitForSeconds(2f);
        brouillardObject.SetActive(true);
        brouillardStele.SetActive(true);

        yield return new WaitForSeconds(brouillardTime);
        animatorBrouillard.SetTrigger("BrouillardFinTrigger");
        
        yield return new WaitForSeconds(1f);
        brouillardSteleFin.SetActive(true);
        brouillardObject.SetActive(false);
        brouillardStele.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        brouillardSteleFin.SetActive(false);
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
        // choisit un role à maudir
        randomRoleState = (Role.RoleStates)Random.Range(0, 3);

        // on regarde si les joueurs sont actuellement sur ce role
        foreach (Player playerMaudit in PlayerManager.Instance.GetPlayers())
        {
            Partition partition = playerMaudit.GetPartition();
            Role.RoleStates currentRole = partition.CurrentRole.RoleState;
            Vector3 CadrePosition = partition.BackgroundSprite.transform.position;

            if (currentRole == randomRoleState)
            {
                partition.CadreMaudit.gameObject.SetActive(true);
                //partitionsMaudites.Add(partition.partitionId);
            }
        }
    }

    public int GetStoneLife()
    {
        return StoneLife;
    }

    public void GameOverBoss()
    {
        animatorBoss.SetTrigger("GameOverTrigger");
    }

    public void PersonnageDead()
    {
        if(PartitionManager.Instance.Rodeur)
            PartitionManager.Instance.Rodeur.SetActive(false);
        if (PartitionManager.Instance.Assassin)
            PartitionManager.Instance.Assassin.SetActive(false);
        if (PartitionManager.Instance.Demoniste)
            PartitionManager.Instance.Demoniste.SetActive(false);
        if (PartitionManager.Instance.Druide)
            PartitionManager.Instance.Druide.SetActive(false);
    }
}

[System.Serializable]
public class AttackBoss
{
    public float Time;
    public BossManager.BossAttack Attack;

}
