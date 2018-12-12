using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Partition : MonoBehaviour {

    [System.NonSerialized] public bool nextNoteIsStone = false;

    public int idplayer;
    public int partitionId;

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

    public Image roleSprite;
    public Image backgroundRoleSprite;
    public SpriteRenderer BackgroundSteleSprite;

    private float CountNote = 0;
    public float maxNbNote;
    public int powerStack = 0;
    private int attackHitNumber = 0;

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
            /*
            RoleProgress.emptyTex = RoleSprite.sprite.texture;
            RoleProgress.fullTex = RoleSprite.sprite.texture;
            RoleProgress.pos = RoleSprite.transform.position;
            RoleProgress.size = RoleSprite.transform.localScale;
            */
            //Debug.Log("changement de rôle pour le " + idplayer + " en : " + currentRole);
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
            //instantiate tracks
            //tracks[i] = Instantiate(PartitionManager.Instance.TrackPrefabs, new Vector3(transform.position.x + OffsetX, PartitionManager.Instance.finishLineY, transform.position.z), Quaternion.identity, transform).GetComponent<Track>();
            //tracks[i].offsetX = OffsetX;
            //tracks[i].SetTrackWidth(trackWidth)
            //OffsetX += SpaceBetweenTracks;

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
                MusicNode musicNode = MusicNodePool.instance.GetNode(tracks[i].offsetX, partitionManager.startLineY, partitionManager.finishLineY, partitionManager.removeLineY, layerZ, currNote.note, currNote.times, TracksColors[i]);
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

    /*void Inputted(int i)
    {
        //inform Conductor and other interested classes
        if (inputtedEvent != null) inputtedEvent(partitionId, i);
    }*/

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
                    BossManager.Instance.TriggerNextAttackStone();
                if (offsetY < partitionManager.perfectOffsetY) //perfect hit
                {
                    //fait quelque chose sur le gameplay selon la qualité du hit
                    frontNode.PerfectHit();
                    //BarManager.Instance.GetImpact(currentRole, PartitionManager.Rank.PERFECT);
                    //print("Perfect");
                    ChargeRole(PartitionManager.Rank.PERFECT);

                    //SendBeatHit to particle
                    tracks[trackNumber].PlayParticle(PartitionManager.Rank.PERFECT);

                    //Remove node
                    queueForTracks[trackNumber].Dequeue();
                }
                else if (offsetY < partitionManager.goodOffsetY) //good hit
                {
                    //fait quelque chose sur le gameplay selon la qualité du hit
                    frontNode.GoodHit();
                    //BarManager.Instance.GetImpact(currentRole, PartitionManager.Rank.GOOD);
                    //print("Good");
                    ChargeRole(PartitionManager.Rank.GOOD);
                    //SendBeatHit to particle
                    tracks[trackNumber].PlayParticle(PartitionManager.Rank.GOOD);

                    //Remove node
                    queueForTracks[trackNumber].Dequeue();
                }
                else if (offsetY < partitionManager.badOffsetY) //bad hit
                {
                    //fait quelque chose sur le gameplay selon la qualité du hit
                    frontNode.BadHit();
                    //BarManager.Instance.GetImpact(currentRole, PartitionManager.Rank.BAD);
                    //print("Bad");
                    ChargeRole(PartitionManager.Rank.BAD);
                    //SendBeatHit to particle
                    tracks[trackNumber].PlayParticle(PartitionManager.Rank.BAD);

                    //Remove node
                    queueForTracks[trackNumber].Dequeue();
                }
            }
            else
            {
                //trop tot / trop tard
                //Baisse d'unisson
                //BarManager.Instance.GetImpact(currentRole, PartitionManager.Rank.MISS);
                HealthBar.Instance.TakeDamage(8);
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
    }

    public void ChargeRole(PartitionManager.Rank rank)
    {
        CountNote++;
        attackHitNumber++;
        roleSprite.fillAmount = (CountNote / maxNbNote);
        if (CountNote <= maxNbNote)
        {
            switch (rank)
            {
                case PartitionManager.Rank.PERFECT:
                    if (BossManager.Instance.goMalediction)
                        HealthBar.Instance.TakeDamage(4);
                    if (currentRole.RoleState == Role.RoleStates.Attack && ManaBar.Instance.manaPoint > 50)
                    {
                        Debug.Log("powerStack up");
                        powerStack += 200;
                        if (attackHitNumber == 3)
                        {
                            ManaBar.Instance.Attack();
                            attackHitNumber = 0;
                        }
                    }
                    else if (currentRole.RoleState == Role.RoleStates.Mana)      // si la note est Perfect et que le role est mana
                    {
                        powerStack += 30;
                        ManaBar.Instance.WinMana(30);
                    }
                    else if (currentRole.RoleState == Role.RoleStates.Defence)     // si la note est Perfect et que le role est defense
                    {
                        powerStack += 15;
                        HealthBar.Instance.WinArmor(15);
                    }
                    break;

                case PartitionManager.Rank.GOOD:
                    if (BossManager.Instance.goMalediction)
                        HealthBar.Instance.TakeDamage(4);
                    if (currentRole.RoleState == Role.RoleStates.Attack && ManaBar.Instance.manaPoint > 50)
                    {
                        Debug.Log("powerStack up");
                        powerStack += 200;
                        if (attackHitNumber == 3)
                        {
                            ManaBar.Instance.Attack();
                            attackHitNumber = 0;
                        }
                    }
                    else if (currentRole.RoleState == Role.RoleStates.Mana)      // si la note est Good et que le role est mana
                    {
                        powerStack += 100;
                        ManaBar.Instance.WinMana(20);
                    }
                    else if (currentRole.RoleState == Role.RoleStates.Defence)     // si la note est Good et que le role est defense
                    {
                        powerStack += 10;
                        HealthBar.Instance.WinArmor(10);
                    }
                    break;

                case PartitionManager.Rank.BAD:
                    if (BossManager.Instance.goMalediction)
                        HealthBar.Instance.TakeDamage(4);
                    if (currentRole.RoleState == Role.RoleStates.Attack && ManaBar.Instance.manaPoint > 50)
                    {
                        Debug.Log("powerStack up");
                        powerStack += 50;
                        if (attackHitNumber == 3)
                        {
                            ManaBar.Instance.Attack();
                            attackHitNumber = 0;
                        }
                    }
                    else if (currentRole.RoleState == Role.RoleStates.Mana)      // si la note est Bad et que le role est mana
                    {
                        powerStack += 10;
                        ManaBar.Instance.WinMana(10);
                    }
                    else if (currentRole.RoleState == Role.RoleStates.Defence)     // si la note est Bad et que le role est defense
                    {
                        powerStack += 5;
                        HealthBar.Instance.WinArmor(5);
                    }
                    break;

                case PartitionManager.Rank.MISS:    // si la note est Miss
                    HealthBar.Instance.TakeDamage(8);
                    break;
            }
        }
        if (CountNote == maxNbNote)
            RoleFire();
    }

    public void RoleFire()
    {
        if (currentRole.RoleState == Role.RoleStates.Attack)
            BossBar.Instance.TakeDamage(powerStack);
        else if (currentRole.RoleState == Role.RoleStates.Mana)
            ManaBar.Instance.WinMana(powerStack);
        else if (currentRole.RoleState == Role.RoleStates.Defence)
            HealthBar.Instance.WinArmor(powerStack);

        powerStack = 0;
        CountNote = 0;
        roleSprite.fillAmount = 0;
    }
}