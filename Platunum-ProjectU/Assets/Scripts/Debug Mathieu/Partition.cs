using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public SpriteRenderer RoleSprite;
    private Role currentRole;
    public Role CurrentRole
    {
        get { return currentRole; }
        set
        {
            currentRole = value;
            RoleSprite.sprite = currentRole.RoleSprite;
            Debug.Log("changement de rôle pour le " + idplayer + " en : " + currentRole);
        }
    }

    private void Start()
    {
        TrackCount = SongInfoCustom.Instance.currentSong.partitions[partitionId - 1].tracks.Length;
        
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
        tracksNode = songInfo.partitions[partitionId-1].tracks; //keep a reference of the tracks
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
                    Debug.Log("next note is stone");
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
            if(offsetY < partitionManager.badOffsetY)
            {
                if (frontNode.isStone)
                    BossManager.Instance.TriggerNextAttackStone();
                if (offsetY < partitionManager.perfectOffsetY) //perfect hit
                {
                    //fait quelque chose sur le gameplay selon la qualité du hit
                    frontNode.PerfectHit();
                    BarManager.Instance.GetImpact(currentRole, PartitionManager.Rank.PERFECT);
                    //print("Perfect");

                    //SendBeatHit to particle
                    tracks[trackNumber].PlayParticle(PartitionManager.Rank.PERFECT);

                    //Remove node
                    queueForTracks[trackNumber].Dequeue();
                }
                else if (offsetY < partitionManager.goodOffsetY) //good hit
                {
                    //fait quelque chose sur le gameplay selon la qualité du hit
                    frontNode.GoodHit();
                    BarManager.Instance.GetImpact(currentRole, PartitionManager.Rank.GOOD);
                    //print("Good");

                    //SendBeatHit to particle
                    tracks[trackNumber].PlayParticle(PartitionManager.Rank.GOOD);

                    //Remove node
                    queueForTracks[trackNumber].Dequeue();
                }
                else if (offsetY < partitionManager.badOffsetY) //bad hit
                {
                    //fait quelque chose sur le gameplay selon la qualité du hit
                    frontNode.BadHit();
                    BarManager.Instance.GetImpact(currentRole, PartitionManager.Rank.BAD);
                    //print("Bad");

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
                frontNode.MissHit();
                BarManager.Instance.GetImpact(currentRole, PartitionManager.Rank.MISS);
            }
        }
        else
        {
            BarManager.Instance.GetImpact(currentRole, PartitionManager.Rank.MISS);
        }
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
}