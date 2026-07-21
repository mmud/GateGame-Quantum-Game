using UnityEngine;

public struct Complex
{
    public float real;
    public float imaginary;

    public Complex(float r, float i = 0)
    {
        real = r;
        imaginary = i;
    }

    public float Abs()
    {
        return Mathf.Sqrt(real * real + imaginary * imaginary);
    }

    public float Probability()
    {
        return real * real + imaginary * imaginary;
    }

    public bool HasImaginary()
    {
        return Mathf.Abs(imaginary) > 0.0001f;
    }

    public static Complex operator +(Complex a, Complex b)
    {
        return new Complex(a.real + b.real, a.imaginary + b.imaginary);
    }

    public static Complex operator *(Complex a, Complex b)
    {
        return new Complex(
            a.real * b.real - a.imaginary * b.imaginary,
            a.real * b.imaginary + a.imaginary * b.real
        );
    }

    public static implicit operator Complex(float value)
    {
        return new Complex(value, 0);
    }

    public override string ToString()
    {
        return $"{real} + {imaginary}i";
    }
}