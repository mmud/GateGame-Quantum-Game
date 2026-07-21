using System.Collections.Generic;
using UnityEngine;

public class QuantumCircuitManager : MonoBehaviour
{
    public static QuantumCircuitManager Instance;

    public Transform circuitStart;

    public Wire[] wires;
    public float columnSpacing = 1.5f;

    public Transform[] Ppos;
    List<GameObject> particles = new List<GameObject>();

    private List<Gate[]> columns = new List<Gate[]>();

    void Awake() => Instance = this;

    public void PlaceGate(Gate gate, int droppedWireIndex, float dropWorldX)
    {
        if (gate.gateType == GateType.CNOT && wires.Length < 2)
        {
            Destroy(gate.gameObject);
            return;
        }
        int targetColumn = ResolveColumnIndex(droppedWireIndex, dropWorldX);
        RemoveGateFromCircuit(gate);
        EnsureColumnExists(targetColumn);

        if (gate.gateType == GateType.CNOT)
        {
            int control = Mathf.Clamp(droppedWireIndex, 0, wires.Length - 2);
            int target = control + 1;

            bool occupied = IsCellOccupied(targetColumn, control) || IsCellOccupied(targetColumn, target);
            if (occupied)
            {
                InsertColumnAt(targetColumn);
            }

            columns[targetColumn][control] = gate;
            columns[targetColumn][target] = gate;
            gate.wireIndex = control;
            gate.targetWireIndex = target;
            gate.columnIndex = targetColumn;
        }
        else
        {
            bool occupied = IsCellOccupied(targetColumn, droppedWireIndex);
            if (occupied)
            {
                InsertColumnAt(targetColumn);
            }

            columns[targetColumn][droppedWireIndex] = gate;
            gate.wireIndex = droppedWireIndex;
            gate.targetWireIndex = -1;
            gate.columnIndex = targetColumn;
        }

        RepositionAllGates();
        PrintCircuit();
    }

    private bool IsCellOccupied(int column, int wire)
    {
        return columns[column][wire] != null && columns[column][wire].gateType != GateType.Empty;
    }

   private int ResolveColumnIndex(int wireIndex, float dropWorldX)
    {
        float startX = circuitStart.position.x;
        int approx = Mathf.RoundToInt((dropWorldX - startX) / columnSpacing);

        int maxAllowed = columns.Count;
        return Mathf.Clamp(approx, 0, maxAllowed);
    }

    private void EnsureColumnExists(int index)
    {
        while (columns.Count <= index)
            columns.Add(new Gate[wires.Length]);
    }

    private void InsertColumnAt(int index)
    {
        columns.Insert(index, new Gate[wires.Length]);
        for (int c = index + 1; c < columns.Count; c++)
            for (int w = 0; w < wires.Length; w++)
                if (columns[c][w] != null)
                    columns[c][w].columnIndex = c;
    }

    private void RemoveGateFromCircuit(Gate gate)
    {
        if (gate.wireIndex < 0 || gate.columnIndex < 0) return;
        if (gate.columnIndex >= columns.Count) return;

        columns[gate.columnIndex][gate.wireIndex] = null;
        if (gate.targetWireIndex >= 0)
            columns[gate.columnIndex][gate.targetWireIndex] = null;
    }

    public void RepositionGate(Gate gate)
    {
        if (gate.wireIndex < 0) return;

        float x = circuitStart.position.x + gate.columnIndex * columnSpacing;

        Vector3 centerPos;
        if (gate.targetWireIndex >= 0)
        {
            float y1 = wires[gate.wireIndex].transform.position.y;
            float y2 = wires[gate.targetWireIndex].transform.position.y;
            centerPos = new Vector3(x, (y1 + y2) / 2f, 0f);
        }
        else
        {
            float y = wires[gate.wireIndex].transform.position.y;
            centerPos = new Vector3(x, y, 0f);
        }

        gate.PositionSelf(centerPos);
    }

    private void RepositionAllGates()
    {
        var seen = new HashSet<Gate>();
        for (int c = 0; c < columns.Count; c++)
        {
            for (int w = 0; w < wires.Length; w++)
            {
                Gate g = columns[c][w];
                if (g == null || seen.Contains(g)) continue;
                seen.Add(g);
                RepositionGate(g);
            }
        }
    }

    public void PrintCircuit()
    {
        for (int c = 0; c < columns.Count; c++)
        {
            string row = $"index {c}: ";
            for (int w = 0; w < wires.Length; w++)
            {
                Gate g = columns[c][w];
                string label = "I";

                if (g != null)
                {
                    if (g.gateType == GateType.CNOT)
                        label = (w == g.wireIndex) ? "C" : "NOT";
                    else
                        label = g.gateType.ToString();
                }

                row += label + (w < wires.Length - 1 ? "," : "");
            }
            Debug.Log(row);
        }
    }

    public void RunCircuit()
    {
        GameManager.instance.score = 0;
        GameManager.instance.lose = false;

        Complex[] state = CreateInitialState();

        foreach(Gate[] column in columns)
        {
            Complex[,] columnMatrix = BuildColumnMatrix(column);

            state = QuantumCalculator.Multiply(columnMatrix, state);
        }

        instantiateParticles(state);

    }

   Complex[,] BuildColumnMatrix(Gate[] column)
    {
        Complex[,] result = null;
        int wireCount = column.Length;

        for (int w = 0; w < wireCount; w++)
        {
            Gate gate = column[w];
            Complex[,] gateMatrix;

            if (gate == null)
            {
                gateMatrix = Identity(2);
            }
            else if (gate.gateType == GateType.CNOT)
            {
                if (gate.wireIndex == w)
                {
                    gateMatrix = QuantumMatrices.CNOT();
                    result = (result == null) ? gateMatrix : TensorProduct(result, gateMatrix);
                    w++;
                    continue;
                }
                else
                {
                    gateMatrix = Identity(2);
                }
            }
            else
            {
                gateMatrix = QuantumMatrices.GetMatrix(gate.gateType);
            }

            result = (result == null) ? gateMatrix : TensorProduct(result, gateMatrix);
        }

        return result;
    }

    Complex[,] Identity(int size)
    {
        Complex[,] m = new Complex[size,size];

        for(int i=0;i<size;i++)
        {
            m[i,i] = new Complex(1);
        }

        return m;
    }

    Complex[,] TensorProduct(Complex[,] A, Complex[,] B)
    {
        int rows = A.GetLength(0) * B.GetLength(0);

        int cols = A.GetLength(1) * B.GetLength(1);

        Complex[,] result = new Complex[rows,cols];

        for(int i=0;i<A.GetLength(0);i++)
        {
            for(int j=0;j<A.GetLength(1);j++)
            {
                for(int k=0;k<B.GetLength(0);k++)
                {
                    for(int l=0;l<B.GetLength(1);l++)
                    {
                        result[i*B.GetLength(0)+k, j*B.GetLength(1)+l] = A[i,j] * B[k,l];
                    }
                }
            }
        }

        return result;
    }


    Complex[] CreateInitialState()
    {
        Complex[] state = new Complex[(int)Mathf.Pow(2, wires.Length)];

        state[0] = new Complex(1,0);

        return state;
    }

    void instantiateParticles(Complex[] state)
    {
        particles.ForEach(p => Destroy(p));
        particles.Clear();
        for(int i=0;i<state.Length;i++)
        {
            Complex amp = state[i];

            if(amp.Probability() < 0.001f)
                continue;

            GameObject g = Instantiate(Resources.Load<GameObject>("Particle"), Ppos[i].position, Quaternion.identity);
            g.GetComponent<Particle>().real = amp.real;
            g.GetComponent<Particle>().imaginary = amp.imaginary;
            particles.Add(g);
            string basis = GetBasisName(i);

            Debug.Log($"State {basis} has amplitude {amp} and probability {amp.Probability()}");

            if (amp.real > 0.0001f)
                Debug.Log("Direction Right");
            else if (amp.real < -0.0001f)
                Debug.Log("Direction Left");

            if (amp.imaginary > 0.0001f)
                Debug.Log("Direction Up (+i)");
            else if (amp.imaginary < -0.0001f)
                Debug.Log("Direction Down (-i)");

        }
    }

    string GetBasisName(int index)
    {
        int qubits = wires.Length;

        string result="";

        for(int i=qubits-1;i>=0;i--)
        {
            int bit = (index >> i) & 1;

            result += bit;
        }

        return "|" + result + ">";
    }
}