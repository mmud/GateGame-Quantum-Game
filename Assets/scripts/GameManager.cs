using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int score;
    public int winscore;

    public bool lose = false;

    public GameObject winparticle;

    static public GameManager instance;

    void Awake() => instance = this;

    void Update()
    {
        if(!lose && score >= winscore)
        {
            lose = true;
            Instantiate(winparticle, Vector3.zero-Vector3.forward*5, Quaternion.identity);
            SceneManagerScript.instance.WinGame();
        }
    }
}
