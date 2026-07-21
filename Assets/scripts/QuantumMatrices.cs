using UnityEngine;

public static class QuantumMatrices
{

    public static Complex[,] GetMatrix(GateType type)
    {
        switch(type)
        {
            case GateType.X:
                return X();

            case GateType.Y:
                return Y();

            case GateType.Z:
                return Z();

            case GateType.H:
                return H();
        }


        return null;
    }



    static Complex[,] X()
    {
        return new Complex[,]
        {
            {0,1},
            {1,0}
        };
    }



    static Complex[,] Y()
    {
        return new Complex[,]
        {
            {0,new Complex(0,-1)},
            {new Complex(0,1),0}
        };
    }



    static Complex[,] Z()
    {
        return new Complex[,]
        {
            {1,0},
            {0,-1}
        };
    }



    static Complex[,] H()
    {
        float v = 1 / Mathf.Sqrt(2);

        return new Complex[,]
        {
            {v,v},
            {v,-v}
        };
    }



    public static Complex[,] CNOT()
    {
        return new Complex[,]
        {
            {1,0,0,0},
            {0,1,0,0},
            {0,0,0,1},
            {0,0,1,0}
        };
    }
}