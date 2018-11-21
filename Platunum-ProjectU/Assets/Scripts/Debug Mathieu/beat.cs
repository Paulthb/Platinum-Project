using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beat : MonoBehaviour {
    public int beatId;
    public Transform[] nodes;

    private void Awake()
    {
        nodes = new Transform[4];
    }

    public void SetNode(int trackId, Transform node)
    {
        nodes[trackId] = node;
    }

    public void OnClick(int trackId)
    {
        if (nodes[trackId])
        {
            Debug.Log("node delete");
            Destroy(nodes[trackId].gameObject);
            nodes[trackId] = null;
        }
        else
        {
            nodes[trackId] = Instantiate(editor.Instance.prefabsNode, new Vector3(transform.position.x, editor.Instance.OffsetYTracks[trackId], -2), Quaternion.identity, transform).transform;
        }
    }
}
