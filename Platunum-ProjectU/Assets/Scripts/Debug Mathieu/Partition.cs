using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Partition : MonoBehaviour {

    [System.NonSerialized] public bool nextNoteIsStone = false;

    public int idplayer;
    public int partitionId;
    private Player player;

    //Tracks Color
    private Color[] TracksColors;
    //Gameplay
    //ADD Role
    //Public Role CurrentRole;

    private SongInfo.Track[] tracksNode;
    public Track[] tracks;
    //Tracks queue
    private Queue<MusicNode>[] queueForTracks;
    private MusicNode[] previousMusicNodes;
    private int[] trackNextIndices;
    //public KeyCode[] activatorKey;

    //beatEvent
    public delegate void BeatOnHitAction(int trackNumber, PartitionManager.Rank rank);
    public static event BeatOnHitAction beatOnHitEvent;
    //input
    //public delegate void InputtedAction(int partitionId, int trackNumber);
    //àpublic static event InputtedAction inputtedEvent;

    //public ParticleSystemManagerCustom ParticleManager;

    //layer each music node, so that the first one would be at the front
    private const float LayerOffsetZ = 0.001f;
    private const float FirstLayerZ = -6f;
    private float[] nextLayerZ;

    private PartitionManager partitionManager;
    private SongInfo songInfo;

    private int TrackCount; 

    public GameObject brouillard;

    //Sprite Partition
    public Image roleSprite;
    public Image backgroundRoleSprite;
    public SpriteRenderer BackgroundSteleSprite;
    public SpriteRenderer BackgroundSprite;

    //Count & Stack
    private float CountNote = 0;
    public float maxNbNote;
    public int powerStack = 0;
    public int attackHitNumber = 0;

    //GD
    [Header("DamageTaken")]
    public int MissDamage;
    public int MaledictionDamage;
    [Header("Mana Conso")]
    public int ManaBurnPerAttack;
    public int AttackCountBurnMana;
    [Header("Attack")]
    public int ATKPerfectPowerToStack;
    public int ATKGoodPowerToStack;
    public int ATKBadPowerToStack;

    [Header("Mana")]
    public int MANAPerfectPowerToStack;
    public int MANAGoodPowerToStack;
    public int MANABadPowerToStack;

    [Header("Shield")]
    public int SHIELDPerfectPowerToStack;
    public int SHIELDGoodPowerToStack;
    public int SHIELDBadPowerToStack;

    [Header("Harmonie")]
    public int HARMONIEPerfect;
    public int HARMONIEGood;
    public int HARMONIEBad;

    private Role currentRole;
    public Role CurrentRole
    {
        get { return currentRole; }
        set
        {
            if(currentRole != null)
                RoleFire();
            currentRole = value;
            roleSprite.sprite = currentRole.RoleSprite;
            backgroundRoleSprite.sprite = currentRole.RoleSpriteVide;
        }
    }

    private void Start()
    {
        TrackCount = SongInfoCustom.Instance.currentSong.partitions[partitionId].tracks.Length;
        
        trackNextIndices = new int[TrackCount];
        TracksColors = new Color[TrackCount];
        nextLayerZ = new float[TrackCount];
        queueForTracks = new Queue<MusicNode>[TrackCount];
        previousMusicNodes = new MusicNode[TrackCount];
        for (int i = 0; i < TrackCount; i++)
        {
            //Init Variable for each track
            trackNextIndices[i] = 0;
            queueForTracks[i] = new Queue<MusicNode>();
            previousMusicNodes[i] = null;
            nextLayerZ[i] = FirstLayerZ;
            TracksColors[i] = PartitionManager.Instance.trackColor[i];
        }

        partitionManager = PartitionManager.Instance;
        songInfo = SongInfoCustom.Instance.currentSong;
        //initialize arrays
        //TrackCount = SpawnOffset.Length;
        for (int i = 0; i < TrackCount; i++)
        {
        }
        tracksNode = songInfo.partitions[partitionId].tracks; //keep a reference of the tracks

        roleSprite.fillAmount = 0;
    }

    private void Update()
    {
        //check if need to instantiate new nodes
        float beatToShow = ConductorCustom.songposition / ConductorCustom.crotchet + ConductorCustom.BeatsShownOnScreen;
        for (int i = 0; i < queueForTracks.Length; i++)
        {
            int nextIndex = trackNextIndices[i];
            SongInfo.Track currTrack = tracksNode[i];

            if (nextIndex < currTrack.notes.Length && currTrack.notes[nextIndex].note < beatToShow)
            {
                SongInfo.Note currNote = currTrack.notes[nextIndex];

                //set z position
                float layerZ = nextLayerZ[i];
                nextLayerZ[i] += LayerOffsetZ;
                //get a new node
                MusicNode musicNode = MusicNodePool.instance.GetNode(tracks[i].offsetX, partitionManager.startLineY, partitionManager.finishLineY, partitionManager.removeLineY, layerZ, currNote.note, currNote.times, TracksColors[i], idplayer);
                if (nextNoteIsStone)
                {
                    
                    musicNode.isStone = true;
                    nextNoteIsStone = false;
                }
                //enqueue
                queueForTracks[i].Enqueue(musicNode);

                //update the next index
                trackNextIndices[i]++;
            }
        }

        for (int i = 0; i < queueForTracks.Length; i++)
        {
            //empty queue, continue
            if (queueForTracks[i].Count == 0) continue;

            MusicNode currNode = queueForTracks[i].Peek();

            //multi-times note
            if (currNode.transform.position.y <= partitionManager.finishLineY - partitionManager.goodOffsetY)   //single time note
            {
                //have previous note stuck on the finish line
                if (previousMusicNodes[i] != null)
                {
                    previousMusicNodes[i].MultiTimesFailed();
                    previousMusicNodes[i] = null;

                    //dispatch miss event
                    if (beatOnHitEvent != null) beatOnHitEvent(i, PartitionManager.Rank.MISS);
                }

                //deque
                queueForTracks[i].Dequeue();

                //dispatch miss event (if a multi-times note is missed, its next single note would also be missed)
                if (beatOnHitEvent != null) beatOnHitEvent(i, PartitionManager.Rank.MISS);
            }
        }
    }

    public void PlayerInputted(int trackNumber)
    {
        //Add Animation to the track
        
        if (queueForTracks[trackNumber].Count != 0)
        {
            //peek the node in the queue
            MusicNode frontNode = queueForTracks[trackNumber].Peek();

            if (frontNode.times > 0) return; //multi-times node should be handled in the Update() func

            float offsetY = Mathf.Abs(frontNode.gameObject.transform.position.y - partitionManager.finishLineY);
            //Note porté minimum
            if(offsetY < partitionManager.badOffsetY)
            {
                if (frontNode.isStone)
                {
                    BossManager.Instance.TriggerNextAttackStone();
                    frontNode.updateSprite(BossManager.Instance.GetStoneLife());
                }
                if (offsetY < partitionManager.perfectOffsetY) //perfect hit
                {
                    //fait quelque chose sur le gameplay selon la qualité du hit
                    frontNode.PerfectHit();
                        ChargeRole(PartitionManager.Rank.PERFECT);

                    //SendBeatHit to particle
                    tracks[trackNumber].PlayParticle(PartitionManager.Rank.PERFECT);

                    //Harmonie
                    if (PlayerManager.Instance.PlayersByRole(currentRole.RoleState) > 1)
                    {
                        HarmonieBar.Instance.GiveHarmonie(HARMONIEPerfect);
                    }

                    //Remove node
                    queueForTracks[trackNumber].Dequeue();
                }
                else if (offsetY < partitionManager.goodOffsetY) //good hit
                {
                    //fait quelque chose sur le gameplay selon la qualité du hit
                    frontNode.GoodHit();
                    ChargeRole(PartitionManager.Rank.GOOD);
                    //SendBeatHit to particle
                    tracks[trackNumber].PlayParticle(PartitionManager.Rank.GOOD);

                    //Harmonie
                    if (PlayerManager.Instance.PlayersByRole(currentRole.RoleState) > 1)
                    {
                        HarmonieBar.Instance.GiveHarmonie(HARMONIEGood);
                    }

                    //Remove node
                    queueForTracks[trackNumber].Dequeue();
                }
                else if (offsetY < partitionManager.badOffsetY) //bad hit
                {
                    //fait quelque chose sur le gameplay selon la qualité du hit
                    frontNode.BadHit();
                    ChargeRole(PartitionManager.Rank.BAD);
                    //SendBeatHit to particle
                    tracks[trackNumber].PlayParticle(PartitionManager.Rank.BAD);

                    //Harmonie
                    if (PlayerManager.Instance.PlayersByRole(currentRole.RoleState) > 1)
                    {
                        HarmonieBar.Instance.GiveHarmonie(HARMONIEBad);
                    }

                    //Remove node
                    queueForTracks[trackNumber].Dequeue();
                }
            }
            else
            {   
                // ---> MISS : trop tot / trop tard
                ShieldBar.Instance.TakeDamage(8);

                player = PlayerManager.Instance.GetPlayer(idplayer);
                // Son en fct du perso
                if (player.Personnage.id == 0)
                {
                    int i = Random.Range(1, 5);
                    switch(i)
                    {
                        case 1:
                            SoundMgr.Instance.PlaySound("FailPiano1");
                            break;
                        case 2:
                            SoundMgr.Instance.PlaySound("FailPiano2");
                            break;
                        case 3:
                            SoundMgr.Instance.PlaySound("FailPiano3");
                            break;
                        case 4:
                            SoundMgr.Instance.PlaySound("FailPiano4");
                            break;
                    } 
                }
                else if (player.Personnage.id == 1)
                {
                    int i = Random.Range(1, 5);
                    switch (i)
                    {
                        case 1:
                            SoundMgr.Instance.PlaySound("FailBass1");
                            break;
                        case 2:
                            SoundMgr.Instance.PlaySound("FailBass2");
                            break;
                        case 3:
                            SoundMgr.Instance.PlaySound("FailBass3");
                            break;
                        case 4:
                            SoundMgr.Instance.PlaySound("FailBass4");
                            break;
                    }
                }
                else if (player.Personnage.id == 2)
                {
                    int i = Random.Range(1, 5);
                    switch (i)
                    {
                        case 1:
                            SoundMgr.Instance.PlaySound("FailGuitar1");
                            break;
                        case 2:
                            SoundMgr.Instance.PlaySound("FailGuitar2");
                            break;
                        case 3:
                            SoundMgr.Instance.PlaySound("FailGuitar3");
                            break;
                        case 4:
                            SoundMgr.Instance.PlaySound("FailGuitar4");
                            break;
                    }
                }
                else if (player.Personnage.id == 3)
                {
                    int i = Random.Range(1, 5);
                    switch (i)
                    {
                        case 1:
                            SoundMgr.Instance.PlaySound("FailBell1");
                            break;
                        case 2:
                            SoundMgr.Instance.PlaySound("FailBell2");
                            break;
                        case 3:
                            SoundMgr.Instance.PlaySound("FailBell3");
                            break;
                        case 4:
                            SoundMgr.Instance.PlaySound("FailBell4");
                            break;
                    }
                }
            }
        }
        /*
        else
        {
            //BarManager.Instance.GetImpact(currentRole, PartitionManager.Rank.MISS);
            ChargeRole(PartitionManager.Rank.MISS);
        }*/
    }

    public void ShowBrouillard(float duration)
    {
        brouillard.SetActive(true);
        StartCoroutine(BrouillardTime(duration));
    }

    IEnumerator BrouillardTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        brouillard.SetActive(false);
    }

    public void ChangeRole(Role role)
    {
        BackgroundSteleSprite.sprite = role.BackgroundStele;
        RoleFire();
    }

    public void ChargeRole(PartitionManager.Rank rank)
    {
        Debug.Log("test");
        CountNote++;
        attackHitNumber++;
        roleSprite.fillAmount = (CountNote / maxNbNote);
        if (CountNote < maxNbNote)
        {
            switch (rank)
            {
                case PartitionManager.Rank.PERFECT:
                    if (BossManager.Instance.goMalediction)
                        ShieldBar.Instance.TakeDamage(MaledictionDamage);
                    if (currentRole.RoleState == Role.RoleStates.Attack && ManaBar.Instance.GetValue() >= ManaBurnPerAttack)
                    {
                        powerStack += ATKPerfectPowerToStack;
                        if (attackHitNumber >= AttackCountBurnMana)
                        {
                            ManaBar.Instance.Attack();
                            attackHitNumber = 0;
                        }
                    }
                    else if (currentRole.RoleState == Role.RoleStates.Mana)      // si la note est Perfect et que le role est mana
                    {
                        powerStack += MANAPerfectPowerToStack;
                        ManaBar.Instance.WinMana(MANAPerfectPowerToStack);
                    }
                    else if (currentRole.RoleState == Role.RoleStates.Defence)     // si la note est Perfect et que le role est defense
                    {
                        powerStack += SHIELDPerfectPowerToStack;
                        ShieldBar.Instance.WinArmor(SHIELDPerfectPowerToStack);
                    }
                    break;

                case PartitionManager.Rank.GOOD:
                    if (BossManager.Instance.goMalediction)
                        ShieldBar.Instance.TakeDamage(MaledictionDamage);
                    if (currentRole.RoleState == Role.RoleStates.Attack && ManaBar.Instance.GetValue() >= ManaBurnPerAttack)
                    {
                        powerStack += ATKGoodPowerToStack;
                        if (attackHitNumber == AttackCountBurnMana)
                        {
                            ManaBar.Instance.Attack();
                            attackHitNumber = 0;
                        }
                    }
                    else if (currentRole.RoleState == Role.RoleStates.Mana)      // si la note est Good et que le role est mana
                    {
                        powerStack += MANAGoodPowerToStack;
                        ManaBar.Instance.WinMana(MANAGoodPowerToStack);
                    }
                    else if (currentRole.RoleState == Role.RoleStates.Defence)     // si la note est Good et que le role est defense
                    {
                        powerStack += SHIELDGoodPowerToStack;
                        ShieldBar.Instance.WinArmor(SHIELDGoodPowerToStack);
                    }
                    break;

                case PartitionManager.Rank.BAD:
                    if (BossManager.Instance.goMalediction)
                        ShieldBar.Instance.TakeDamage(MaledictionDamage);
                    if (currentRole.RoleState == Role.RoleStates.Attack && ManaBar.Instance.GetValue() >= ManaBurnPerAttack)
                    {
                        powerStack += ATKBadPowerToStack;
                        if (attackHitNumber == AttackCountBurnMana)
                        {
                            ManaBar.Instance.Attack();
                            attackHitNumber = 0;
                        }
                    }
                    else if (currentRole.RoleState == Role.RoleStates.Mana)      // si la note est Bad et que le role est mana
                    {
                        powerStack += MANABadPowerToStack;
                        ManaBar.Instance.WinMana(MANABadPowerToStack);
                    }
                    else if (currentRole.RoleState == Role.RoleStates.Defence)     // si la note est Bad et que le role est defense
                    {
                        powerStack += SHIELDBadPowerToStack;
                        ShieldBar.Instance.WinArmor(SHIELDBadPowerToStack);
                    }
                    break;

                case PartitionManager.Rank.MISS:    // si la note est Miss
                    ShieldBar.Instance.TakeDamage(MissDamage);
                    break;
            }
        }
        else
            RoleFire();
    }

    public void RoleFire()
    {
        if (currentRole.RoleState == Role.RoleStates.Attack)
            BossBar.Instance.TakeDamage(powerStack);
        else if (currentRole.RoleState == Role.RoleStates.Mana)
            ManaBar.Instance.WinMana(powerStack);
        else if (currentRole.RoleState == Role.RoleStates.Defence)
            ShieldBar.Instance.WinArmor(powerStack);

        powerStack = 0;
        CountNote = 0;
        roleSprite.fillAmount = 0;
    }
}