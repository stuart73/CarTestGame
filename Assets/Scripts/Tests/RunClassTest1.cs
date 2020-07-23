using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Tests
{
    class RunClassTest1 : Test
    {

		public FVector[] pos= {
				new FVector( 0.000000f, 9.686512f, -0.403500f ) ,
new FVector( 0.000000f, 9.648499f, -0.403500f ) ,
new FVector( 0.000000f, 9.585962f, -0.403500f ) ,
new FVector( 0.000000f, 9.498903f, -0.403500f ) ,
new FVector( 0.000000f, 9.387325f, -0.403500f ) ,
new FVector( 0.000000f, 9.251231f, -0.403500f ) ,
new FVector( 0.000000f, 9.090628f, -0.403500f ) ,
new FVector( 0.000000f, 8.905515f, -0.403500f ) ,
new FVector( 0.000000f, 8.695898f, -0.403500f ) ,
new FVector( 0.000000f, 8.461785f, -0.403500f ) ,
new FVector( 0.000000f, 8.203182f, -0.403500f ) ,
new FVector( 0.000000f, 7.920097f, -0.403500f ) ,
new FVector( 0.000000f, 7.612538f, -0.403500f ) ,
new FVector( 0.000000f, 7.280514f, -0.403500f ) ,
new FVector( 0.000000f, 6.924034f, -0.403500f ) ,
new FVector( 0.000000f, 6.543108f, -0.403500f ) ,
new FVector( 0.000000f, 6.137747f, -0.403500f ) ,
new FVector( 0.000000f, 5.707962f, -0.403500f ) ,
new FVector( 0.000000f, 5.253767f, -0.403500f ) ,
new FVector( 0.000000f, 4.775173f, -0.403500f ) ,
new FVector( 0.000000f, 4.272195f, -0.403500f ) ,
new FVector( 0.000000f, 3.744846f, -0.403500f ) ,
new FVector( 0.000000f, 3.193143f, -0.403500f ) ,
new FVector( 0.000000f, 2.617100f, -0.403500f ) ,
new FVector( 0.000000f, 2.016734f, -0.403500f ) ,
new FVector( 0.000000f, 1.392061f, -0.403500f ) ,
new FVector( 0.000000f, 0.743100f, -0.403500f ) ,
new FVector( 0.000000f, 0.071712f, -0.404877f ) ,
new FVector( -0.003247f, -0.266529f, -0.421988f ) ,
new FVector( -0.005971f, -0.206384f, -0.436183f ) ,
new FVector( -0.006132f, -0.089328f, -0.446403f ) ,
new FVector( -0.005429f, 0.006259f, -0.456456f ) ,
new FVector( -0.005924f, 0.081969f, -0.467096f ) ,
new FVector( -0.006355f, 0.133745f, -0.477816f ) ,
new FVector( -0.006826f, 0.161246f, -0.488564f ) ,
new FVector( -0.007446f, 0.166956f, -0.499776f ) ,
new FVector( -0.008156f, 0.155227f, -0.512220f ) ,
new FVector( -0.009015f, 0.131308f, -0.526317f ) ,
new FVector( -0.009837f, 0.105271f, -0.540912f ) ,
new FVector( -0.010510f, 0.083368f, -0.555360f ) ,
new FVector( -0.011068f, 0.068598f, -0.569330f ) ,
new FVector( -0.011555f, 0.061468f, -0.582687f ) ,
new FVector( -0.012014f, 0.060755f, -0.595456f ) ,
new FVector( -0.012474f, 0.064364f, -0.607767f ) ,
new FVector( -0.012950f, 0.070061f, -0.619795f ) ,
new FVector( -0.013448f, 0.075978f, -0.631717f ) ,
new FVector( -0.013964f, 0.080855f, -0.643675f ) ,
new FVector( -0.014492f, 0.084081f, -0.655760f ) ,
new FVector( -0.015025f, 0.085586f, -0.668009f ) ,
new FVector( -0.015560f, 0.085665f, -0.680414f ) ,
new FVector( -0.016092f, 0.084794f, -0.692937f ) ,
new FVector( -0.016621f, 0.083474f, -0.705530f ) ,
new FVector( -0.017147f, 0.082122f, -0.718146f ) ,
new FVector( -0.017672f, 0.081017f, -0.730749f ) ,
new FVector( -0.018195f, 0.080293f, -0.743318f ) ,
new FVector( -0.018718f, 0.079964f, -0.755844f ) ,
new FVector( -0.019241f, 0.079958f, -0.768332f ) ,
new FVector( -0.019765f, 0.080167f, -0.780791f ) ,
new FVector( -0.020289f, 0.080474f, -0.793234f ) ,
new FVector( -0.020814f, 0.080786f, -0.805672f ) ,
new FVector( -0.021339f, 0.081039f, -0.818115f ) ,
new FVector( -0.021865f, 0.081203f, -0.830566f ) ,
new FVector( -0.022390f, 0.081276f, -0.843028f ) ,
new FVector( -0.022915f, 0.081275f, -0.855499f ) ,
new FVector( -0.023440f, 0.081225f, -0.867977f ) ,
new FVector( -0.023965f, 0.081153f, -0.880459f ) ,
new FVector( -0.024489f, 0.081081f, -0.892942f ) ,
new FVector( -0.025014f, 0.081022f, -0.905424f ) ,
new FVector( -0.025539f, 0.080985f, -0.917903f ) ,
new FVector( -0.026064f, 0.080968f, -0.930380f ) ,
new FVector( -0.026589f, 0.080969f, -0.942855f ) ,
new FVector( -0.027114f, 0.080981f, -0.955328f ) ,
new FVector( -0.027639f, 0.080998f, -0.967800f ) ,
new FVector( -0.028164f, 0.081015f, -0.980273f ) ,
new FVector( -0.028689f, 0.081028f, -0.992745f ) ,
new FVector( -0.029213f, 0.081037f, -1.005218f ) ,
new FVector( -0.029738f, 0.081041f, -1.017692f ) ,
new FVector( -0.030263f, 0.081041f, -1.030167f ) ,
new FVector( -0.030787f, 0.081038f, -1.042642f ) ,
new FVector( -0.031312f, 0.081034f, -1.055117f ) ,
new FVector( -0.031837f, 0.081030f, -1.067592f ) ,
new FVector( -0.032361f, 0.081026f, -1.080067f ) ,
new FVector( -0.032886f, 0.081024f, -1.092542f ) ,
new FVector( -0.033411f, 0.081024f, -1.105017f ) ,
new FVector( -0.033936f, 0.081024f, -1.117492f ) ,
new FVector( -0.034461f, 0.081024f, -1.129967f ) ,
new FVector( -0.034986f, 0.081025f, -1.142442f ) ,
new FVector( -0.035511f, 0.081026f, -1.154916f ) ,
new FVector( -0.036036f, 0.081027f, -1.167391f ) ,
new FVector( -0.036561f, 0.081027f, -1.179866f ) ,
new FVector( -0.037086f, 0.081028f, -1.192341f ) ,
new FVector( -0.037611f, 0.081028f, -1.204816f ) ,
new FVector( -0.038136f, 0.081028f, -1.217291f ) ,
new FVector( -0.038662f, 0.081027f, -1.229767f ) ,
new FVector( -0.039187f, 0.081027f, -1.242242f ) ,
new FVector( -0.039713f, 0.081027f, -1.254717f ) ,
new FVector( -0.040238f, 0.081027f, -1.267192f ) ,
new FVector( -0.040763f, 0.081027f, -1.279668f ) ,
new FVector( -0.041289f, 0.081027f, -1.292143f ) ,
new FVector( -0.041814f, 0.081027f, -1.304619f ) ,

 };

		TestCar mTestCar;

		//******************************************************************************************
		//******************************************************************************************

		bool FVectorsSame(FVector a, FVector b)
		{
			float d1 = a.X - b.X;
			float d2 = a.Y - b.Y;
			float d3 = a.Z - b.Z;

			if ((Math.Abs(d1) > 0.1) ||
				(Math.Abs(d2) > 0.1) ||
				(Math.Abs(d3) > 0.1))
			{
				return false;
			}

			return true;

		}

		//******************************************************************************************
		public void Run()
		{

			DeclareRunningTest("Run car test 1");


			mTestCar = new TestCar();

			mTestCar.mPos = new FVector(0.0f, 10.0f, 0.0f);
			mTestCar.Start();

			int checkIndex = 0;

			//DeclareRunningTest("Car Run Test1");

			// 5 seconds
			for (int i = 0; i < 5 * 20; i++)
			{
				mTestCar.Update();

			///	UnityEngine.Debug.Log("Test " + mTestCar.mPos.X + "," + mTestCar.mPos.Y + "," + mTestCar.mPos.Z);

				if (FVectorsSame(mTestCar.mPos , pos[checkIndex]))
				{
					UnityEngine.Debug.Log("Test " + checkIndex + " Passed");

				}
				else
				{
					UnityEngine.Debug.Log("Test " + checkIndex + " Faled");
				}

				checkIndex++;
				

			}

		}

	}
}
