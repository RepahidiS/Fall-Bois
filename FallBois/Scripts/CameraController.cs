using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraController : MonoBehaviour
{
    public float followSpeed = 1.0f;
    public Transform target;
    public Material mtDefault;
    public Material mtTransparent;
    public List<GameObject> currentHitObjects = new List<GameObject>();

    void Start()
    {
        if(target == null)
        {
            Debug.LogError("Camera follow target is not set!");
            Debug.Break();
        }
    }

    void FixedUpdate()
    {
        float speed = Time.deltaTime * followSpeed;
        Vector3 followPos = target.position;
        followPos.y = 7.0f;
        followPos.z += 7.0f; // stay behind the character
        Vector3 targetPos = Vector3.Lerp(transform.position, followPos, speed);
        transform.position = targetPos;

        // sometimes some objects/obstacles coming between camera and character.
        // we dont want that, so let's make these objects/obstacles transparent.
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, new Vector3(0.0f, -0.5f, -1.0f), 7.0f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

        // check current raycast hits. if there is a new hit, add it to our list and make it transparent.
        foreach (RaycastHit hit in hits)
        {
            GameObject go = currentHitObjects.Where(i => i.transform.gameObject == hit.transform.gameObject).FirstOrDefault();

            if(go == null)
            {
                if(hit.transform.gameObject.GetComponent<MeshRenderer>() != null)
                {
                    hit.transform.gameObject.GetComponent<MeshRenderer>().material = mtTransparent;
                    currentHitObjects.Add(hit.transform.gameObject);
                }
            }
        }

        // check our list. if there is a hit no more hitting then remove it from list and make it opaque again.
        foreach (GameObject go in currentHitObjects.ToArray())
        {
            bool found = false;
            for(int i = 0; i < hits.Length; i++)
            {
                if(go == hits[i].transform.gameObject)
                {
                    found = true;
                    break;
                }
            }

            if(!found)
            {
                if(go.GetComponent<MeshRenderer>() != null)
                {
                    go.GetComponent<MeshRenderer>().material = mtDefault;
                    currentHitObjects.Remove(go);
                }
            }
        }
    }
}