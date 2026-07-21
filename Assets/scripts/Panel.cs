using UnityEngine;
using System.Collections.Generic;

public class Panel : MonoBehaviour
{
    public static Panel Instance;

    public List<GameObject> Instances;

    public float xposition = -6.5f;
    public float yposition = -4f;
    public float xspacing = 2.5f;

    void Awake() => Instance = this;

    void Start()
    {
        Instances = new List<GameObject>();
        for (int i = 0; i < Resources.LoadAll<GameObject>("Gates").Length; i++)
        {
            Instances.Add(null);
        }
    }

    void Update()
    {
        for (int i = 0; i < Resources.LoadAll<GameObject>("Gates").Length; i++)
        {
            if (Instances[i] == null)
            {
                Instances[i] = Instantiate(Resources.LoadAll<GameObject>("Gates")[i], new Vector3(xposition + i * xspacing, yposition, transform.position.z), Quaternion.identity);
            }
        }
        
    }
}