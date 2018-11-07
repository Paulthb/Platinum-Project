using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassManager : MonoBehaviour {
    public Class[] Classes;

    public Class GetClassById(int id)
    {
        return Classes[id];
    }

    public Class GetClassByName(string name)
    {
        for (int i = 0; i < Classes.Length; i++)
        {
            if (Classes[i].name == name)
                return Classes[i];
        }
        Debug.LogError("Class name not found");
        return null;
    }

    public int GetClassIdByName(string name)
    {
        for (int i = 0; i < Classes.Length; i++)
        {
            if (Classes[i].name == name)
                return i;
        }
        Debug.LogError("Class name not found");
        return -1;
    }

    public int GetClassCount()
    {
        return Classes.Length;
    }
}
