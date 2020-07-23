using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Tests
{
	public class Test
	{  
		public static bool VectorsSame(FVector a, FVector b)
		{
			float d1 = a.X - b.X;
			float d2 = a.Y - b.Y;
			float d3 = a.Z - b.Z;
			if ((Math.Abs(d1) < 0.0001) &&
				(Math.Abs(d2) < 0.0001) &&
				(Math.Abs(d3) < 0.0001))
			{
				return true;
			}
			return false;
		}

		static bool MatrixSame(FMatrix a, FMatrix b)
		{
			bool d1 = VectorsSame(a.Row[0], b.Row[0]);
			bool d2 = VectorsSame(a.Row[1], b.Row[1]);
			bool d3 = VectorsSame(a.Row[2], b.Row[2]);

			if (d1 && d2 && d3)
			{
				return true;
			}
			return false;
		}



		public void CheckResult(FVector result, FVector expectedResult, int test)
		{


			if (VectorsSame(result, expectedResult))
			{
				UnityEngine.Debug.Log("Test " + test + " Passed ");
			}
			else
			{
				UnityEngine.Debug.Log("Test " + test + " Failed ");
			}

		}


		public void CheckResult(FMatrix result, FMatrix expectedResult, int test)
		{
			if (MatrixSame(result, expectedResult) == true)
			{
				UnityEngine.Debug.Log("Test " + test + " Passed ");
			}
			else
			{
				UnityEngine.Debug.Log("Test " + test + " Failed ");
			}
		}


		public void CheckResult(float result, float expectedResult, int test)
		{

			if (Math.Abs(result - expectedResult) < 0.01)
			{
				UnityEngine.Debug.Log("Test " + test + " Passed (" + result + "(");
			}
			else
			{
				UnityEngine.Debug.Log("Test " + test + " Faled (" + result + "," + expectedResult + ")");
			}
		}

		public void DeclareRunningTest(string name)
		{
			UnityEngine.Debug.Log(" ******Running test: " + name + " *******");
		}


	}
}
