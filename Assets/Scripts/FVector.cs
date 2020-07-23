using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class FVector
    {
        public float X, Y, Z, W;

        public FVector()
        {
            X = 0; Y = 0; Z = 0; W = 0;

        }
        public FVector(float x, float y, float z)
        {
            X = x; Y = y; Z = z; W = 0;
        }
        public FVector Add(FVector a)
        {
            return new FVector(X + a.X, Y + a.Y, Z + a.Z);
        }
        public FVector Minus(FVector a)
        {
            return new FVector(X - a.X, Y - a.Y, Z - a.Z);
        }
        public FVector Multiply(float a)
        {
            return new FVector(X * a, Y * a, Z * a);
        }

        public void AddInPlace(FVector a)
        {
            X += a.X; Y += a.Y; Z += a.Z;
        }
        public void MinusInPlace(FVector a)
        {
            X -= a.X; Y -= a.Y; Z -= a.Z;
        }

        public void MultiplyInPlace(float a)
        {
            X *= a; Y *= a; Z *= a;
        }

        public void DivideInPlace(float a)
        {
            X /= a; Y /= a; Z /= a;
        }

        public FVector Divide(float a)
        {
            return new FVector(X / a, Y / a, Z / a);
        }

        // Vector (cross) product - NB: Careful of operator precedence!
        public FVector Cross(FVector a)
        {
            return new FVector(Y * a.Z - Z * a.Y, Z * a.X - X * a.Z, X * a.Y - Y * a.X);
        }

        // Normalisation
        public void Normalise()
        {
            float scale = (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
            if (scale != 0.0f)
            {
                scale = 1.0f / scale;
                X *= scale;
                Y *= scale;
                Z *= scale;
            }
        }


        public void NormaliseAndScale(float scale)
        {
	        Normalise();
            Multiply(scale);
        }


        public float Magnitude()
        {
            return (float)Math.Sqrt(((X * X) + (Y * Y) + (Z * Z)));
        }

        public float DotProduct(FVector a)
        {
            return ((X * a.X) + (Y * a.Y) + (Z * a.Z));

        }

        public FVector Negative()
        {
            return new FVector(-X, -Y, -Z);
        }
    }
}
