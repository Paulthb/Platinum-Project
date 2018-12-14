using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO.Ports;

public class editor : MonoBehaviour {
    private bool PlayMode = false;

    //Song
    private float songLength;
    AudioSource audios;
    SongInfo songInfo;
    AudioClip song;

    //UI
    [Header("UI")]
    public Slider sliderTime;
    public Slider SliderPitch;
    public Slider TrackLengthSlider;
    public Text txtTps, txtPch;
    public Text txtNom;
    public Image cursorEditor;
    public Image cursorPlay;
    public RectTransform tracks;
    public Transform LecteurFolder;
    public Text UIPartitionId;
    private Vector3 LecteurPlayPos = new Vector3(-315, -25);
    private Vector3 LecteurEditorPos;



    //Crotchet
    private int crotchetCount;
    private PrimaryBeat[] beats;
    //time between each bpm
    private float crotchet;
    private float crotchetSize;
    public int BeatDividing;
    public float TrackLength;

    //Prefabs
    [Header("Prefabs & Parent")]
    //Beats
    public GameObject prefabsBeats;
    public GameObject prefabsSubBeat;
    //Node
    public GameObject prefabsNode;
    //Parent
    public Transform parentBeats;
    public Transform parentNode;

    //Camera
    public Transform cameraTransform;
    public float offsetX = 7;

    //Partitions
    private SongInfo.Partition[] PlayersPartition;
    private int CurrentIdPartition;
    //Tracks
    public int trackNb;
    public float[] OffsetYTracks;


    //PADS
    private gamepads pads;

    //Get Instance
    private static editor instance;
    public static editor Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<editor>();
            if (instance == null)
                Debug.Log("No tutorial found");
            return instance;
        }
    }

    void Start () {

        if(SerialPort.GetPortNames().Length >0)
            pads = new gamepads((int)char.GetNumericValue(SerialPort.GetPortNames()[0][SerialPort.GetPortNames()[0].Length-1]));
        //Load Song
        audios = GetComponent<AudioSource>();
        songInfo = SongInfoCustom.Instance.currentSong;
        song = songInfo.song;
        audios.clip = song;

        //UI
        //load Slider
        sliderTime.minValue = 0;
        sliderTime.maxValue = song.length;
        txtPch.text = audios.pitch.ToString();
        txtNom.text = song.name;


        //TrackLength
        TrackLengthSlider.value = TrackLength;

        //initialize fields
        crotchet = 60f / songInfo.bpm;
        songLength = songInfo.song.length;
        crotchetCount = (int)(songLength / crotchet);
        //Init BPM
        beats = new PrimaryBeat[crotchetCount];
        for (int i = 0; i < crotchetCount; i++)
        {
            beats[i] = Instantiate(prefabsBeats, parentBeats).GetComponent<PrimaryBeat>();
            //Init a 1 car le premier subbeat est le beat lui même
            for (int j = 1; j < BeatDividing; j++)
            {
                Instantiate(prefabsSubBeat, beats[i].transform).GetComponent<subbeat>().subNode = j;
            }
            beats[i].beatId = i + 1;
            beats[i].initSubNode(trackNb, BeatDividing);
        }

        //Partition
        PlayersPartition = songInfo.partitions;
        CurrentIdPartition = 0;
        UIPartitionId.text = (CurrentIdPartition+1).ToString();
        LoadPartition();

        //Resize BPM space
        ResizeTracks();
    }


    void Update () {
        //UI
        //Slider
        sliderTime.value = audios.time;
        txtTps.text = (audios.time).ToString("0") + "/" + song.length.ToString("0");

        cameraTransform.transform.position = new Vector3(Mathf.Lerp(0, TrackLength, audios.time / audios.clip.length) + offsetX, 0, -10);

        //if TrackLength Change -> resizeTracks
        if(System.Math.Round(crotchetSize, 4) != System.Math.Round(TrackLength / crotchetCount, 4))
        {
            ResizeTracks();
        }
        //Key Event
        //Click Add/Destroy
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {

                //Debug.Log(OffsetYTracks[0] - hit.point.y);
                float closest = Mathf.Infinity;
                int TrackIdClosest = -1;
                for (int i = 0; i < OffsetYTracks.Length; i++)
                {
                    if (Mathf.Abs(OffsetYTracks[i] - hit.point.y) < closest)
                    {
                        TrackIdClosest = i;
                        closest = Mathf.Abs(OffsetYTracks[i] - hit.point.y);
                    }
                }
                if(hit.transform.tag == "PrimaryBeat")
                {
                    hit.transform.GetComponent<PrimaryBeat>().OnClick(TrackIdClosest, 0);
                }
                else
                {
                    hit.transform.GetComponent<subbeat>().OnClick(TrackIdClosest);
                }
            }
        }

        //PlayPause
        if (Input.GetKeyDown(KeyCode.Space))
            play();

        //PlayMode input
        if (PlayMode)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.A))
            {
                LookForNode(0);
            }
            if (Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.Z))
            {
                LookForNode(1);
            }
            if (Input.GetKeyDown(KeyCode.Joystick1Button2) || Input.GetKeyDown(KeyCode.E))
            {
                LookForNode(2);
            }
            if (Input.GetKeyDown(KeyCode.Joystick1Button3) || Input.GetKeyDown(KeyCode.R))
            {
                LookForNode(3);
            }

            pads.Update();
            for (int i = 0; i < 4; i++)
            {
                if (pads.GetKeyDown(i))
                {
                    //Send TrackKey input
                    LookForNode(i);
                }
            }
        }
    }

    public void LookForNode(int track)
    {
        int initialBeat = (int)Mathf.Floor(audios.time / crotchet);
        int beatId = initialBeat;
        int nextSubBeatId = (int)Mathf.Floor(audios.time % crotchet / (crotchet / BeatDividing));
        do
        {
            nextSubBeatId++;
            if (nextSubBeatId == 4)
            {
                beatId++;
                nextSubBeatId = 0;
            }
        } while (!beats[beatId].nodes[track][nextSubBeatId] && beatId < initialBeat+5);

        if (beats[beatId].nodes[track][nextSubBeatId])
        {
            float CursorPosX = cameraTransform.position.x - offsetX;
            float distance = Mathf.Abs(beats[beatId].nodes[track][nextSubBeatId].position.x - CursorPosX);
            if (distance < (crotchetSize / BeatDividing)*2)
            {
                beats[beatId].nodes[track][nextSubBeatId].GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
    }
 
    public void MovePositionTime()
    {
        if(sliderTime.value < song.length)
        {
            //ConductorCustom.Instance.SetDspTime(ConductorCustom.Instance.GetdspTime() + (audios.time - slider.value));
            audios.time = sliderTime.value;
        }
        else
        {
            audios.Play();
        }
    }

    public void MovePositionPitch()
    {
        audios.pitch = SliderPitch.value;
        txtPch.text = System.Math.Round(SliderPitch.value,2).ToString();
    }

    public void GoToNextBeat()
    {
        float TimeInCrotchet = audios.time / crotchet;
        float newTime = Mathf.Ceil(TimeInCrotchet) * crotchet;

        //Debug.Log("currentTime:" + System.Math.Round(audios.time, 4) + "/newTime:" + System.Math.Round(newTime, 4));
        if (System.Math.Round(audios.time, 3) == System.Math.Round(newTime, 3)) {
            audios.time = newTime + crotchet;
        }
        else
        {
            audios.time = newTime;
        }
    }
    public void GoToPreviousBeat()
    {
        float TimeInCrotchet = audios.time / crotchet;
        float newTime = Mathf.Floor(TimeInCrotchet) * crotchet;
        //Debug.Log("currentTime:" + System.Math.Round(audios.time, 4) + "/newTime:" + System.Math.Round(newTime, 4));
        if (System.Math.Round(audios.time, 3) == System.Math.Round(newTime, 3))
        {
            audios.time = newTime - crotchet;
        }
        else
        {
            audios.time = newTime;
        }
    }

    public void play()
    {
        if (audios.isPlaying)
            audios.Pause();
        else
            audios.Play();
    }

    private void ResizeTracks()
    {
        crotchetSize = TrackLength / crotchetCount;
        Vector3 beatPos = new Vector3(0, 0, -1);
        for (int i =  0; i < parentBeats.childCount; i++)
        {
            Transform beat = parentBeats.GetChild(i);
            beat.position = beatPos;
            beatPos.x += crotchetSize;
            for (int j = 1; j < BeatDividing; j++)
            {
                beat.GetChild(j).localPosition = new Vector3(crotchetSize / BeatDividing * (j), 0, -1);
            }
        }
        resetNodePosition();
    }

    public void resetNodePosition()
    {
        for (int j = 0; j < parentBeats.childCount; j++)
        {
            PrimaryBeat beat = parentBeats.GetChild(j).GetComponent<PrimaryBeat>();
            for (int i = 0; i < trackNb; i++)
            {
                for (int k = 0; k < beat.nodes[i].Length; k++)
                {
                    if (beat.nodes[i][k])
                    {
                        beat.nodes[i][k].position = new Vector3(j * crotchetSize + GetSubNodePosX(k), OffsetYTracks[i], -2);
                    }
                }
            }
        }
    }

    public float GetSubNodePosX(int subNode)
    {
        return (crotchetSize / BeatDividing) * subNode;
    }

    public void EnterPlayMode()
    {
        if (PlayMode)
        {
            LecteurFolder.GetComponent<RectTransform>().anchoredPosition = LecteurEditorPos;
            PlayMode = false;
            cursorEditor.enabled = true;
            cursorPlay.enabled = false;
            tracks.Rotate(-90 * Vector3.forward);
            cameraTransform.Rotate(90 * Vector3.forward);
            offsetX *= 2;
            TrackLength *= 2;
        }
        else
        {
            LecteurEditorPos = LecteurFolder.GetComponent<RectTransform>().anchoredPosition;
            LecteurFolder.GetComponent<RectTransform>().anchoredPosition = LecteurPlayPos;
            //UIFolder.gameObject.SetActive(false);
            PlayMode = true;
            cursorEditor.enabled = false;
            cursorPlay.enabled = true;
            tracks.Rotate(90 * Vector3.forward);
            cameraTransform.Rotate(-90 * Vector3.forward);
            offsetX /= 2;
            TrackLength /= 2;
        }
    }

    public void ResetColorNode()
    {

        //reset nodes
        foreach (PrimaryBeat beat in beats)
        {
            for (int i = 0; i < beat.nodes.Length; i++)
            {
                for (int j = 0; j < beat.nodes[i].Length; j++)
                {
                    if (beat.nodes[i][j])
                    {
                        beat.nodes[i][j].GetComponent<SpriteRenderer>().color = Color.white;
                    }
                }
            }
        }
    }
    public void NextPartition()
    {
        Save();
        CurrentIdPartition = (CurrentIdPartition+1) % songInfo.partitions.Length;
        UIPartitionId.text = (CurrentIdPartition+1).ToString();
        CleanPartition();
        LoadPartition();
    }
    public void PreviousPartition()
    {
        Save();
        CurrentIdPartition--;
        if(CurrentIdPartition == -1)
        {
            CurrentIdPartition = songInfo.partitions.Length - 1;
        }
        UIPartitionId.text = (CurrentIdPartition+1).ToString();
        CleanPartition();
        LoadPartition();
    }
    public void CleanPartition()
    {
        //Delete Node
        foreach (PrimaryBeat beat in beats)
        {
            for (int i = 0; i < beat.nodes.Length; i++)
            {
                for (int j = 0; j < beat.nodes[i].Length; j++)
                {
                    if (beat.nodes[i][j])
                    {
                        Destroy(beat.nodes[i][j].gameObject);
                    }
                }
            }
        }
    }
    public void LoadPartition()
    {
        PlayersPartition = songInfo.partitions;
        //Load New Partition
        for (int j = 0; j < PlayersPartition[CurrentIdPartition].tracks.Length; j++)
        {
            for (int i = 0; i < PlayersPartition[CurrentIdPartition].tracks[j].notes.Length; i++)
            {
                //Get Beat of node
                int beatId = (int)Mathf.Floor(PlayersPartition[CurrentIdPartition].tracks[j].notes[i].note);
                //Get Subbeat id
                int subBeatId;
                if (beatId >= 1)
                    subBeatId = (int)(PlayersPartition[CurrentIdPartition].tracks[j].notes[i].note % beatId / 0.25f);
                else
                    subBeatId = (int)(PlayersPartition[CurrentIdPartition].tracks[j].notes[i].note / 0.25f);
                //Instantiate new node
                GameObject newNode = Instantiate(prefabsNode, Vector3.zero, Quaternion.identity, parentBeats.GetChild((int)PlayersPartition[CurrentIdPartition].tracks[j].notes[i].note));
                //link node to beat
                beats[beatId].SetNode(j, subBeatId, newNode.transform);
            }
        }
        resetNodePosition();
    }

    public void Save()
    {
        int[] NodePerTrack = new int[trackNb];
        for (int i = 0; i < NodePerTrack.Length; i++)
        {
            NodePerTrack[i] = 0;
        }

        //Save
        for (int j = 0; j < parentBeats.childCount; j++)
        {
            PrimaryBeat beat = parentBeats.GetChild(j).GetComponent<PrimaryBeat>();
            for (int i = 0; i < beat.nodes.Length; i++)
            {
                for (int k = 0; k < beat.nodes[i].Length; k++)
                {
                    if(beat.nodes[i][k])
                            NodePerTrack[i]++;
                }
            }
        }
        SongInfo.Partition newPartition = new SongInfo.Partition();
        newPartition.tracks = new SongInfo.Track[trackNb];
        for (int i = 0; i < trackNb; i++)
        {
            newPartition.tracks[i] = new SongInfo.Track();
            newPartition.tracks[i].notes = new SongInfo.Note[NodePerTrack[i]];
        }

        //Indice note
        for (int i = 0; i < NodePerTrack.Length; i++)
        {
            NodePerTrack[i] = 0;
        }
        //Transfert des notes
        for (int j = 0; j < parentBeats.childCount; j++)
        {
            PrimaryBeat beat = parentBeats.GetChild(j).GetComponent<PrimaryBeat>();
            for (int i = 0; i < beat.nodes.Length; i++)
            {
                for (int k = 0; k < beat.nodes[i].Length; k++)
                {
                    if (beat.nodes[i][k])
                    {
                        newPartition.tracks[i].notes[NodePerTrack[i]] = new SongInfo.Note(j + k * 0.25f, 0);
                        NodePerTrack[i]++;
                    }
                }
            }
        }
        PlayersPartition[CurrentIdPartition] = newPartition;
        SongInfoCustom.Instance.currentSong.partitions[CurrentIdPartition] = newPartition;
        /*
        SongInfo newSong = new SongInfo();
        string path = AssetDatabase.GetAssetPath(SongInfoCustom.Instance.currentSong);
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.CreateAsset(newSong, path);
        */
        EditorUtility.CopySerialized(SongInfoCustom.Instance.currentSong, SongInfoCustom.Instance.currentSong);
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        //AssetDatabase.GetAssetPath(SongInfoCustom.Instance.currentSong);
    }

    public void TrackLengthChange()
    {
        TrackLength = TrackLengthSlider.value;
    }
}
