using UnityEngine;

public class Gate : MonoBehaviour
{
    public GateType gateType;

    public int wireIndex = -1;
    public int targetWireIndex = -1;
    public int columnIndex = -1;

    private Collider2D col;
    private Vector3 startDragWorldPosition;

    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnMouseDown()
    {
        print(transform.name + " mouse down");
        startDragWorldPosition = transform.position;
    }

    private void OnMouseDrag()
    {
        transform.position = GetMousePositionWorldSpace();
    }

    private void OnMouseUp()
    {
        col.enabled = false;
        Collider2D hit = Physics2D.OverlapPoint(GetMousePositionWorldSpace());
        col.enabled = true;

        if (hit != null && hit.TryGetComponent<IDropArea>(out IDropArea dropArea))
        {
            dropArea.OnDrop(this);
        }
        else
        {
            if (wireIndex >= 0)
                QuantumCircuitManager.Instance.RepositionGate(this);
            else
                transform.position = startDragWorldPosition;
        }
    }

    public void PositionSelf(Vector3 centerPos)
    {
        transform.position = centerPos;
    }

    private Vector3 GetMousePositionWorldSpace()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }
}