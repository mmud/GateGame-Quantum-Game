using UnityEngine;


public static class QuantumCalculator
{
    public static Complex[] Multiply(Complex[,] matrix, Complex[] state)
    {
        int size = state.Length;

        Complex[] result =
            new Complex[size];

        for(int row = 0; row < size; row++)
        {
            Complex sum = new Complex(0);

            for(int col = 0; col < size; col++)
            {
                sum += matrix[row,col] * state[col];
            }

            result[row] = sum;
        }

        return result;
    }

}