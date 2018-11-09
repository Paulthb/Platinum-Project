using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Partition : MonoBehaviour {

    public enum Rank { PERFECT, GOOD, BAD, MISS };
    public int idplayer;
    public int partitionId;
    public float[] SpawnOffset;

    private SongInfo.Track[] tracks;
    //Tracks queue
    private Queue<MusicNode>[] queueForTracks;
    private MusicNode[] previousMusicNodes;
    private int[] trackNextIndices;
    //public KeyCode[] activatorKey;

    //beatEvent
    public delegate void BeatOnHitAction(int trackNumber, Rank rank);
    public static event BeatOnHitAction beatOnHitEvent;
    //input
    public delegate void InputtedAction(int partitionId, int trackNumber);
    public static event InputtedAction inputtedEvent;

    //layer each music node, so that the first one would be at the front
    private const float LayerOffsetZ = 0.001f;
    private const float FirstLayerZ = -6f;
    private float[] nextLayerZ;

    private LevelManager levelManager;
    private SongInfo songInfo;

    private int TrackCount;

    private void Start()
    {
        levelManager = LevelManager.Instance;
        songInfo = SongInfoCustom.Instance.currentSong;
        Debug.Log(songInfo.bpm);
        //initialize arrays
        TrackCount = SpawnOffset.Length;
        trackNextIndices = new int[TrackCount];
        nextLayerZ = new float[TrackCount];
        queueForTracks = new Queue<MusicNode>[TrackCount];
        previousMusicNodes = new MusicNode[TrackCount];
        for (int i = 0; i < TrackCount; i++)
        {
            trackNextIndices[i] = 0;
            queueForTracks[i] = new Queue<MusicNode>();
            previousMusicNodes[i] = null;
            nextLayerZ[i] = FirstLayerZ;
        }
        tracks = songInfo.partitions[partitionId-1].tracks; //keep a reference of the tracks
    }

    private void Update()
    {
        //check if need to instantiate new nodes
        float beatToShow = Conductor.songposition / Conductor.crotchet + Conductor.Instance.GetBeatToShowOnScreen();
        for (int i = 0; i < queueForTracks.Length; i++)
        {
            int nextIndex = trackNextIndices[i];
            SongInfo.Track currTrack = tracks[i];

            if (nextIndex < currTrack.notes.Length && currTrack.notes[nextIndex].note < beatToShow)
            {
                SongInfo.Note currNote = currTrack.notes[nextIndex];

                //set z position
                float layerZ = nextLayerZ[i];
                nextLayerZ[i] += LayerOffsetZ;
                Debug.Log(levelManager.startLineY);
                //get a new node
                MusicNode musicNode = MusicNodePool.instance.GetNode(SpawnOffset[i], levelManager.startLineY, levelManager.finishLineY, levelManager.removeLineY, layerZ, currNote.note, currNote.times, levelManager.trackColor[i]);

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
            if (currNode.transform.position.y <= levelManager.finishLineY - levelManager.goodOffsetY)   //single time note
            {
                //have previous note stuck on the finish line
                if (previousMusicNodes[i] != null)
                {
                    previousMusicNodes[i].MultiTimesFailed();
                    previousMusicNodes[i] = null;

                    //dispatch miss event
                    if (beatOnHitEvent != null) beatOnHitEvent(i, Rank.MISS);
                }

                //deque
                queueForTracks[i].Dequeue();

                //dispatch miss event (if a multi-times note is missed, its next single note would also be missed)
                if (beatOnHitEvent != null) beatOnHitEvent(i, Rank.MISS);
            }
        }
    }

    void Inputted(int i)
    {
        //inform Conductor and other interested classes
        if (inputtedEvent != null) inputtedEvent(partitionId, i);
    }

    public void PlayerInputted(int trackNumber)
    {
        if (queueForTracks[trackNumber].Count != 0)
        {
            //peek the node in the queue
            MusicNode frontNode = queueForTracks[trackNumber].Peek();

            if (frontNode.times > 0) return; //multi-times node should be handled in the Update() func

            float offsetY = Mathf.Abs(frontNode.gameObject.transform.position.y - levelManager.finishLineY);

            if (offsetY < levelManager.perfectOffsetY) //perfect hit
            {
                frontNode.PerfectHit();
                //print("Perfect");

                //dispatch beat on hit event
                if (beatOnHitEvent != null) beatOnHitEvent(trackNumber, Rank.PERFECT);

                queueForTracks[trackNumber].Dequeue();
            }
            else if (offsetY < levelManager.goodOffsetY) //good hit
            {
                frontNode.GoodHit();
                //print("Good");

                //dispatch beat on hit event
                    if (beatOnHitEvent != null) beatOnHitEvent(trackNumber, Rank.GOOD);

                queueForTracks[trackNumber].Dequeue();
            }
            else if (offsetY < levelManager.badOffsetY) //bad hit
            {
                frontNode.BadHit();

                //dispatch beat on hit event
                if (beatOnHitEvent != null) beatOnHitEvent(trackNumber, Rank.BAD);

                queueForTracks[trackNumber].Dequeue();
            }
        }
    }
}