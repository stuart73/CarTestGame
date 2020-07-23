using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
	public class FMatrix
	{

		// Public data
		public FVector[] Row = new FVector[3] { new FVector(1, 0, 0), new FVector(0, 1, 0), new FVector(0, 0, 1) };

		// Construction
		public FMatrix()
		{
		}
		public FMatrix(FVector row0, FVector row1, FVector row2)
		{
			Row[0] = row0;
			Row[1] = row1;
			Row[2] = row2;
		}

		public FMatrix(float yaw, float pitch, float roll)
		{
			float cy = (float)Math.Cos(yaw);
			float sy = (float)Math.Sin(yaw);
			float cr = (float)Math.Cos(roll);
			float sr = (float)Math.Sin(roll);
			float cp = (float)Math.Cos(pitch);
			float sp = (float)Math.Sin(pitch);

			// Fast version
			Row[0].X = cy * cr + sr * sy * sp;
			Row[0].Y = cy * sr - sy * sp * cr;
			Row[0].Z = -sy * cp;
			Row[1].X = -sr * cp;
			Row[1].Y = cr * cp;
			Row[1].Z = -sp;
			Row[2].X = sy * cr - sr * cy * sp;
			Row[2].Y = sy * sr + cr * cy * sp;
			Row[2].Z = cy * cp;

			// Slow version
			/*	Row[0] = FVector( (float)cos(roll) , (float)sin(roll), 0.f );
				Row[1] = FVector( -(float)sin(roll) , (float)cos(roll), 0.f );
				Row[2] = FVector( 0.0f , 0.0f , 1.0f );

				FMatrix	temp;
				temp.Row[0] = FVector( 1.0f , 0.0f , 0.0f );
				temp.Row[1] = FVector( 0.0f , (float)cos(pitch) , -(float)sin(pitch) );
				temp.Row[2] = FVector( 0.0f , (float)sin(pitch) , (float)cos(pitch) );

				*this = temp*(*this);

				temp.Row[0] = FVector( (float)cos(yaw) , 0.f, -(float)sin(yaw));
				temp.Row[1] = FVector( 0.0f , 1.0f , 0.0f );
				temp.Row[2] = FVector( (float)sin(yaw) , 0.f, (float)cos(yaw));

				*this = temp*(*this);*/
		}
		public void MakeRotationYawF(float yaw)
		{
			Row[0] = new FVector((float)Math.Cos(yaw), 0.0f, -(float)Math.Sin(yaw));
			Row[1] = new FVector(0.0f, 1.0f, 0.0f);
			Row[2] = new FVector((float)Math.Sin(yaw), 0.0f, (float)Math.Cos(yaw));
		}

		float Determinant() 
		{
			float result;

			result =  Row[0].X* ((Row[1].Y* Row[2].Z) - (Row[1].Z* Row[2].Y));
			result -= Row[0].Y* ((Row[1].X* Row[2].Z) - (Row[1].Z* Row[2].X));
			result += Row[0].Z* ((Row[1].X* Row[2].Y) - (Row[1].Y* Row[2].X));

			return result;
		}

	public FMatrix Adjoint() 
		{
			FMatrix result = new FMatrix();

			result.Row[0].X =  ((Row[1].Y* Row[2].Z) - (Row[1].Z* Row[2].Y));
			result.Row[0].Y = -((Row[1].X* Row[2].Z) - (Row[1].Z* Row[2].X));
			result.Row[0].Z =  ((Row[1].X* Row[2].Y) - (Row[1].Y* Row[2].X));

			result.Row[1].X = -((Row[0].Y* Row[2].Z) - (Row[0].Z* Row[2].Y));
			result.Row[1].Y =  ((Row[0].X* Row[2].Z) - (Row[0].Z* Row[2].X));
			result.Row[1].Z = -((Row[0].X* Row[2].Y) - (Row[0].Y* Row[2].X));

			result.Row[2].X =  ((Row[0].Y* Row[1].Z) - (Row[0].Z* Row[1].Y));
			result.Row[2].Y = -((Row[0].X* Row[1].Z) - (Row[0].Z* Row[1].X));
			result.Row[2].Z =  ((Row[0].X* Row[1].Y) - (Row[0].Y* Row[1].X));

			return result;
		}

		FVector Column0() 
		{ 
			return new FVector(Row[0].X, Row[1].X, Row[2].X);
		}
		FVector Column1() 
		{
				return new FVector(Row[0].Y, Row[1].Y, Row[2].Y);
		}
		public FVector Column2() 
		{ 
			return new FVector(Row[0].Z, Row[1].Z, Row[2].Z);
		}

		// Transpose
		public FMatrix Transpose()
		{
			return new FMatrix(Column0(), Column1(), Column2());
		}
	
		public void TransposeInPlace()
		{
			float t;
			t = Row[0].Y; Row[0].Y = Row[1].X; Row[1].X = t;
			t = Row[0].Z; Row[0].Z = Row[2].X; Row[2].X = t;
			t = Row[1].Z; Row[1].Z = Row[2].Y; Row[2].Y = t;
		}


		public FMatrix Inverse()
		{
			FMatrix result = Adjoint();
			result.TransposeInPlace();
			result.DivideInPlace(Determinant());

			return result;
		}


		// Basic arithmetic and scalar multiples
		public FMatrix  Add(  FMatrix a ) 
		{ 
			return new FMatrix(Row[0].Add(a.Row[0]), Row[1].Add(a.Row[1]), Row[2].Add(a.Row[2]));
		}

		public FMatrix  Minus( FMatrix a ) 
		{ 
			return new FMatrix(Row[0].Minus(a.Row[0]), Row[1].Minus(a.Row[1]), Row[2].Minus(a.Row[2]));
		}

		public FMatrix Multiply(float a) 
		{
			return new FMatrix(Row[0].Multiply(a), Row[1].Multiply(a), Row[2].Multiply(a)); 
		}

		public FMatrix Divide(float a) 
		{ 
			return new FMatrix(Row[0].Divide(a), Row[1].Divide(a), Row[2].Divide(a)); 
		}

		public void AddInPlace(  FMatrix a )
		{ 
			Row[0].AddInPlace( a.Row[0]); Row[1].AddInPlace(a.Row[1]); Row[2].AddInPlace(a.Row[2]);
		}

		public void MinusInPlace (  FMatrix a )
		{
			Row[0].MinusInPlace(a.Row[0]); Row[1].MinusInPlace( a.Row[1]); Row[2].MinusInPlace( a.Row[2]); 
		}

		public void MultiplyInPlace(float a )
		{ 
			Row[0].MultiplyInPlace(a); Row[1].MultiplyInPlace(a); Row[2].MultiplyInPlace(a); 
		}

		public void DivideInPlace(float a )
		{ 
			Row[0].DivideInPlace(a); Row[1].DivideInPlace(a); Row[2].DivideInPlace(a); 
		}

		// Multiplication with vector
		public FVector Multiply(FVector a) 
		{
			return	new FVector((Row[0].X* a.X) + (Row[0].Y* a.Y) + (Row[0].Z* a.Z) ,
			(Row[1].X* a.X) + (Row[1].Y* a.Y) + (Row[1].Z* a.Z) ,
			(Row[2].X* a.X) + (Row[2].Y* a.Y) + (Row[2].Z* a.Z) );
		}

		// Matrix multiply
		public FMatrix Multiply(FMatrix a) 
		{
			return new FMatrix(
					new FVector(Row[0].X* a.Row[0].X+Row[0].Y* a.Row[1].X+Row[0].Z* a.Row[2].X,
								Row[0].X* a.Row[0].Y+Row[0].Y* a.Row[1].Y+Row[0].Z* a.Row[2].Y,
								Row[0].X* a.Row[0].Z+Row[0].Y* a.Row[1].Z+Row[0].Z* a.Row[2].Z),
					new FVector(Row[1].X* a.Row[0].X+Row[1].Y* a.Row[1].X+Row[1].Z* a.Row[2].X,
								Row[1].X* a.Row[0].Y+Row[1].Y* a.Row[1].Y+Row[1].Z* a.Row[2].Y,
								Row[1].X* a.Row[0].Z+Row[1].Y* a.Row[1].Z+Row[1].Z* a.Row[2].Z),
					new FVector(Row[2].X* a.Row[0].X+Row[2].Y* a.Row[1].X+Row[2].Z* a.Row[2].X,
								Row[2].X* a.Row[0].Y+Row[2].Y* a.Row[1].Y+Row[2].Z* a.Row[2].Y,
								Row[2].X* a.Row[0].Z+Row[2].Y* a.Row[1].Z+Row[2].Z* a.Row[2].Z) );
		}

		// JCL 20/6 - fix up matrix errors by normalising each row.
		// (This is, of course, complete bollocks.)
		public void Normalise()
		{
			// 0 most dominant
			Row[0].Normalise();
			Row[1].Normalise();
			//		Row[2].Normalise();
			Row[2] = Row[0].Cross(Row[1]);
			Row[2].Normalise();
			Row[1] = Row[2].Cross(Row[0]);
		}


	}
}
