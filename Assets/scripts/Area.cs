using UnityEngine;

public class Area : MonoBehaviour
{
    public bool winarea = true;
    public GameObject loseparticle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if(winarea)
                GameManager.instance.score++;
            else{
                GameObject g = Instantiate(loseparticle, other.transform.position, Quaternion.identity);
                Destroy(g, 2f);
                other.gameObject.SetActive(false);
                GameManager.instance.lose = true;
            }
        }
    }
}
