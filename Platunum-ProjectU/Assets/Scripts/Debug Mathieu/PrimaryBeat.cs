using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryBeat : MonoBehaviour {
    public int beatId;
    public Transform[][] nodes;

    public void initSubNode(int trackNb, int BeatDividing)
    {
        nodes = new Transform[trackNb][];
        for (int i = 0; i < trackNb; i++)
        {
            nodes[i] = new Transform[BeatDividing];
        }
    }

    public void SetNode(int trackId, int subNode, Transform node)
    {
        nodes[trackId][subNode] = node;
    }

    public void OnClick(int trackId, int subNode)
    {
        if (nodes[trackId][subNode])
        {
            Destroy(nodes[trackId][subNode].gameObject);
            nodes[trackId][subNode] = null;
        }
        else
        {
            nodes[trackId][subNode] = Instantiate(editor.Instance.prefabsNode, new Vector3(transform.position.x + editor.Instance.GetSubNodePosX(subNode), editor.Instance.OffsetYTracks[trackId], -2), Quaternion.identity, transform).transform;
        }
    }
}
