using UnityEngine;

public class Wire : MonoBehaviour, IDropArea
{
    public int wireIndex;

    public void OnDrop(Gate gate)
    {
        if (gate.gameObject == Panel.Instance.Instances[(int)gate.gateType - 1])
        {
            Panel.Instance.Instances[(int)gate.gateType-1] = null;
        }
        QuantumCircuitManager.Instance.PlaceGate(gate, wireIndex, gate.transform.position.x);
    }
    
    void OnMouseDown()
    {
        print(transform.name + " mouse down");
    }
}