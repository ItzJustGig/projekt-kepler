using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuffList : MonoBehaviour
{
    [SerializeField] List<ScriptableObject> list;

    public List<ScriptableObject> returnStuff()
    {
        return list;
    }
}
