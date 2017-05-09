using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateTrack : MonoBehaviour {
    public GameObject node;
    public int numberOfNodes;
    public int minX, maxX, minY, maxY;
    public int x = 0;

    void Start()
    {
       // PlaceNodes();
    }

    public void PlaceNodes()
    {
        for (int i = 0; i < numberOfNodes; i++)
        {
            GameObject myNode = Instantiate(node, GeneratedPosition(), Quaternion.identity);
            myNode.transform.parent = GameObject.Find("rollercoaster").transform;
        }
    }
    Vector3 GeneratedPosition()
    {
        int y, z;
        x = UnityEngine.Random.Range(minX, maxX) + x;
        y = UnityEngine.Random.Range(minY, maxY);
        z = UnityEngine.Random.Range(0, 250);
        return new Vector3(x, y, z);
    }
}