using UnityEngine;

public class Trash : MonoBehaviour, IDropArea
{

    public void OnDrop(Gate gate)
    {
        Destroy(gate.gameObject);
    }
    
}