using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class editor : MonoBehaviour {
    //Song
    AudioSource audios;
    SongInfo songInfo;
    AudioClip song;
    public int crotchetCount;

    //UI
    public Text txtNom;
    public Slider sliderTime, SliderPitch;
    public Text txtTps, txtPch;



    //Crotchet
    public beat[] beats;
    public float crotchet;
    public float songLength;

    public float crotchetSize;
    public float TrackLength;

    //Beats
    public GameObject prefabsBeats;
    public Transform parentBeats;

    //Camera
    public Transform camera;
    public float offsetX = 7;

    //Partitions
    private SongInfo.Partition[] PlayersPartition;
    private int CurrentIdPartition;
    //Tracks
    public float[] OffsetYTracks;
    //Node
    public GameObject prefabsNode;
    public Transform parentNode;

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




        //initialize fields
        crotchet = 60f / songInfo.bpm;

        songLength = songInfo.song.length;
        crotchetCount = (int)(songLength / crotchet);
        //Init BPM
        beats = new beat[crotchetCount];
        for (int i = 0; i < crotchetCount; i++)
        {
            beats[i] = Instantiate(prefabsBeats, parentBeats).GetComponent<beat>();
            beats[i].beatId = i + 1;
            //beats[i].nodes = new Transform[4];  
        }

        //Partition
        PlayersPartition = songInfo.partitions;
        CurrentIdPartition = 0;
        for (int j = 0; j < PlayersPartition[CurrentIdPartition].tracks.Length; j++)
        {
            Transform trackPos = parentNode.GetChild(j);
            for (int i = 0; i < PlayersPartition[CurrentIdPartition].tracks[j].notes.Length; i++)
            {
                //Calculate PosX
                float posX = PlayersPartition[CurrentIdPartition].tracks[j].notes[i].note * crotchetSize;
                //Instantiate new node
                GameObject newNode = Instantiate(prefabsNode, new Vector3(posX, trackPos.position.y, -2), Quaternion.identity, parentBeats.GetChild((int)PlayersPartition[CurrentIdPartition].tracks[j].notes[i].note));
                //Get Beat of node
                int beatId = (int)PlayersPartition[CurrentIdPartition].tracks[j].notes[i].note;
                //link node to beat
                beats[beatId].SetNode(j, newNode.transform);
            }
        }

        //Resize BPM space
        ResizeTracks();
    }


    void Update () {
        //UI
        //Slider
        sliderTime.value = audios.time;
        txtTps.text = (audios.time).ToString("0") + "/" + song.length.ToString("0");

        camera.transform.position = new Vector3(Mathf.Lerp(0, TrackLength, audios.time / audios.clip.length) + offsetX, 0, -10);

        //if TrackLength Change -> resizeTracks
        if(System.Math.Round(crotchetSize, 4) != System.Math.Round(TrackLength / crotchetCount, 4))
        {
            ResizeTracks();
        }

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
                hit.transform.GetComponent<beat>().OnClick(TrackIdClosest);
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
        audios.time = Mathf.Floor(TimeInCrotchet) * crotchet;
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
            parentBeats.GetChild(i).position = beatPos;
            beatPos.x += crotchetSize;
        }

        resetNodePosition();

    }

    public void resetNodePosition()
    {
        /*for (int j = 0; j < PlayersPartition[CurrentIdPartition].tracks.Length; j++)
        {
            Transform trackPos = parentNode.GetChild(j);
            for (int i = 0; i < PlayersPartition[CurrentIdPartition].tracks[j].notes.Length; i++)
            {
                int beatId = (int)PlayersPartition[CurrentIdPartition].tracks[j].notes[i].note;
                trackPos.GetChild(i).position = new Vector3(beatId*crotchetSize, trackPos.position.y, -2);
            }
        }*/
        for (int j = 0; j < parentBeats.childCount; j++)
        {
            beat beat = parentBeats.GetChild(j).GetComponent<beat>();
            for (int i = 0; i < beat.nodes.Length; i++)
            {
                if (beat.nodes[i])
                {
                    beat.nodes[i].position = new Vector3(j * crotchetSize, OffsetYTracks[i], -2);
                }
            }
        }
    }
}
