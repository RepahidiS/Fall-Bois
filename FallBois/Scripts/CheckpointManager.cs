using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public GameObject goSpawnPoint;
    public List<GameObject> goCheckpoints;
    public GameObject goFinish;

    public Vector3 GetSpawnablePos(GameObject checkpoint)
    {
        Vector3 pos = checkpoint.transform.position;

        // TODO : check bot poisitons to find an available spawn pos

        return pos;
    }
}