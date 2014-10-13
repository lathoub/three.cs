using System;
using SMath = System.Math;

namespace ThreeCs.Math
{

    public class Color : IEquatable<Color>
    {

        public int R = 1;
        public int G = 1;
        public int B = 1;

        public Color() { }

        public Color(int Hex)
        {
            this.SetHex(Hex);
        }

        public void Set(Color Color)
        {
            this.Copy(Color);
        }

        public Color SetRGB(int R, int G, int B)
        {
            this.R = R;
            this.G = G;
            this.B = B;
            return this;
        }

        public Color SetHex(int Hex)
        {
            this.R = (Hex >> 16 & 255) / 255;
            this.G = (Hex >> 8 & 255) / 255;
            this.B = (Hex & 255) / 255;
            return this;
        }

        public Color SetHSL(int H, int S, int L)
        {
            if (S == 0)
            {
                this.R = this.G = this.B = L;
            }
            else
            {
                Func<int, int, int, int> Hue2RGB = (P, Q, T) =>
                {
                    if (T < 0) T += 1;
                    if (T > 1) T -= 1;
                    if (T < 1 / 6) return P + (Q - P) * 6 * T;
                    if (T < 1 / 2) return Q;
                    if (T < 2 / 3) return P + (Q - P) * 6 * (2 / 3 - T);
                    return P;
                };
                int P1 = L <= 0.5 ? L * (1 + S) : L + S - (L * S);
                int Q1 = (2 * L) - P1;

                this.R = Hue2RGB(Q1, P1, H + 1 / 3);
                this.G = Hue2RGB(Q1, P1, H);
                this.B = Hue2RGB(Q1, P1, H - 1 / 3);
            }
            return this;
        }

        public Color Copy(Color Color)
        {
            this.R = Color.R;
            this.G = Color.G;
            this.B = Color.B;
            return Color;
        }

        public Color CopyGammaToLinear(Color Color)
        {
            this.R = Color.R * Color.R;
            this.G = Color.G * Color.G;
            this.B = Color.B * Color.B;
            return this;
        }

        public Color CopyLinearToGamma(Color Color)
        {
            this.R = (int)SMath.Sqrt(Color.R);
            this.G = (int)SMath.Sqrt(Color.G);
            this.B = (int)SMath.Sqrt(Color.B);
            return this;
        }

        public Color ConvertGammaToLinear()
        {
            this.R = this.R * this.R;
            this.G = this.G * this.G;
            this.B = this.B * this.B;
            return this;
        }

        public Color ConvertLinearToGamma()
        {
            this.R = (int)SMath.Sqrt(this.R);
            this.G = (int)SMath.Sqrt(this.G);
            this.B = (int)SMath.Sqrt(this.B);
            return this;
        }

        public int Hex
        {
            get { return (this.R * 255) << 16 ^ (this.G * 255) << 8 ^ (this.B * 255) << 0; }
        }

        public static Color operator +(Color C1, Color C2)
        {
            C1.R += C2.R;
            C1.G += C2.G;
            C1.B += C2.B;
            return C1;
        }

        public static Color operator *(Color C1, Color C2)
        {
            C1.R *= C2.R;
            C1.G *= C2.G;
            C1.B *= C2.B;
            return C1;
        }

        public Color AddColors(Color C1, Color C2)
        {
            this.R = C1.R + C2.R;
            this.G = C1.G + C2.G;
            this.B = C1.B + C2.B;
            return this;
        }

        public Color AddScalar(int Scalar)
        {
            this.R += Scalar;
            this.G += Scalar;
            this.B += Scalar;
            return this;
        }

        public Color MultiplyScalar(int Scalar)
        {
            this.R *= Scalar;
            this.G *= Scalar;
            this.B *= Scalar;
            return this;
        }

        public Color Lerp(Color Color, int Alpha)
        {
            this.R += (Color.R - this.R) * Alpha;
            this.G += (Color.G - this.G) * Alpha;
            this.B += (Color.B - this.B) * Alpha;
            return this;
        }

        public bool Equals(Color Color)
        {
            return (Color.R == this.R) && (Color.G == this.G) && (Color.B == this.B);
        }

        public int[] Array
        {
            get { return new[] { this.R, this.G, this.B }; }
        }

        public Color Clone()
        {
            return new Color().SetRGB(this.R, this.G, this.B);
        }

        public Color FromArray(int[] Array)
        {
            if (Array.Length < 3)
            {
                throw new ArgumentOutOfRangeException();
            }
            this.R = Array[0];
            this.G = Array[1];
            this.B = Array[2];
            return this;
        }

    }

}
