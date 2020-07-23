using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Tests
{
	class WheelTest : Test
	{

		public void Run()
		{
			DeclareRunningTest("******* wheel testsv********");

			CPOChassis chassis = new CPOChassis();

			// set chassis to 4WD
			chassis.SetDriveType(EDriveType.DT_4WD)  ;


			chassis.Init();
			chassis.SetPos(new FVector(0, 0, 0));
			chassis.SetOrientation(new FMatrix());


			int testNumber = 1;
			Wheel wheel1 = chassis.GetWheel(0);

			FVector basepos = wheel1.GetBasePos();
			FVector basepos1ExpectedResukt = new FVector(-1.00000000f, -0.474999994f, 1.51650000f);
			CheckResult(basepos, basepos1ExpectedResukt, testNumber++);

			FVector minbasepos = wheel1.GetMinBasePos();
			FVector minbaseposExpectedResult = new FVector(-1.00000000f, -0.474999994f, 1.51650000f);
			CheckResult(minbasepos, minbaseposExpectedResult, testNumber++);

			FVector maxbasepos = wheel1.GetMaxBasePos();
			FVector maxbaseposExpectedResult = new FVector(-1.00000000f, -0.675000012f, 1.51650000f);


			FMatrix momentsOfIntetia = wheel1.GetMomentOfInertia();
			FMatrix momentsOfIntetiaExpectedResult = new FMatrix();

			momentsOfIntetiaExpectedResult.Row[0] = new FVector(2.80000019f, 0.000000000f, 0.000000000f);
			momentsOfIntetiaExpectedResult.Row[1] = new FVector(0.000000000f, 11.9291668f, 0.000000000f);
			momentsOfIntetiaExpectedResult.Row[2] = new FVector(0.000000000f, 0.000000000f, 11.9291668f);

			CheckResult(momentsOfIntetia, momentsOfIntetiaExpectedResult, testNumber++);

			FMatrix momentsOfIntetiaInverse = wheel1.GetMomentOfInertiaInverse();
			FMatrix momentsOfIntetiaInverseExpectedResult = new FMatrix();

			momentsOfIntetiaInverseExpectedResult.Row[0] = new FVector(0.357142836f, -0.000000000f, 0.000000000f);
			momentsOfIntetiaInverseExpectedResult.Row[1] = new FVector(-0.000000000f, 0.0838281438f, -0.000000000f);
			momentsOfIntetiaInverseExpectedResult.Row[2] = new FVector(0.000000000f, -0.000000000f, 0.0838281438f);

			CheckResult(momentsOfIntetiaInverse, momentsOfIntetiaInverseExpectedResult, testNumber++);

			float suspensioForce = wheel1.GetSuspensionForce();
			CheckResult(suspensioForce, 7200.000f, testNumber++);

			FMatrix ori = wheel1.GetOri();
			FMatrix oriExpectedResult = new FMatrix();
			CheckResult(ori, oriExpectedResult, testNumber++);

			FVector perp = wheel1.GetPerp();
			FVector perpExpectedResult = new FVector(1.0f, 0.0f, 0.0f);
			CheckResult(perp, perpExpectedResult, testNumber++);

			FMatrix ori1 = new FMatrix(); ;
			FVector groundNormal = new FVector(0.0f, 1.0f, 0.0f);
			FVector direction = wheel1.GetDir(ori1, groundNormal);
			FVector directionExpectedResult = new FVector(0.0f, 0.0f, 1.0f);
			CheckResult(direction, directionExpectedResult, testNumber++);

			float torqueFromCar = wheel1.GetTorqueFromCar();
			CheckResult(torqueFromCar, 0.0f, testNumber++);

			// set wheen hub speed to 11.34 and re-calculater torque
			chassis.SetRawThrottle(1.0f);
			chassis.SetEngineRPM(4500);
			wheel1.mCurrentLongitudinalForce = 45.23f;
			wheel1.mLastSlipSpeed = 11.2f;
			wheel1.mCurrentHubSpeed = 11.34f;
			wheel1.UpdateWheelSpinSpeed();

			float torqueFromCar2 = wheel1.GetTorqueFromCar();
			CheckResult(torqueFromCar2, 1429.76599f, testNumber++);

			chassis.SetChangingGear(true);
			float torqueFromCar3 = wheel1.GetTorqueFromCar();
			CheckResult(torqueFromCar3, 0.0f, testNumber++);

			chassis.SetChangingGear(false);
			chassis.SetRawBraking(0.4f);
			float torqueFromCar4 = wheel1.GetTorqueFromCar();
			CheckResult(torqueFromCar4, 901.660339f, testNumber++);

			chassis.SetRawBraking(0.1f);
			float torqueFromCar5 = wheel1.GetTorqueFromCar();
			CheckResult(torqueFromCar5, 973.765991f, testNumber++);

			// test with a rear wheel
			Wheel wheel2 = chassis.GetWheel(2);
			wheel2.mCurrentLongitudinalForce = 45.23f;
			wheel2.mLastSlipSpeed = 11.2f;
			wheel2.mCurrentHubSpeed = 11.34f;
			wheel2.UpdateWheelSpinSpeed();

			chassis.SetRawBraking(1.0f);

			float torqueFromCar6 = wheel2.GetTorqueFromCar();
			CheckResult(torqueFromCar6, 1077.69556f, testNumber++);

			chassis.SetRawBraking(0.1f);
			float torqueFromCar7 = wheel2.GetTorqueFromCar();
			CheckResult(torqueFromCar7, 1085.76599f, testNumber++);

			// test with neagtive speed;
			wheel2.mSpeed = -11;

			float torqueFromCar8 = wheel2.GetTorqueFromCar();
			CheckResult(torqueFromCar8, 1773.76599f, testNumber++);


			// test on ground speed calculation

			wheel2.mIsOnGround = true;
			wheel2.UpdateWheelSpinSpeed();

			float resultSpeed1 = wheel2.mSpeed;
			CheckResult(resultSpeed1, -8.39243698f, testNumber++);

			wheel2.mCurrentLongitudinalForce = -45.23f;
			wheel2.UpdateWheelSpinSpeed();
			float resultSpeed2 = wheel2.mSpeed;
			CheckResult(resultSpeed2, 2.293575f, testNumber++);

			wheel2.mSpeed = -25.34f;
			wheel2.mCurrentHubSpeed = -11.34f;
			wheel2.UpdateWheelSpinSpeed();
			float resultSpeed3 = wheel2.mSpeed;
			CheckResult(resultSpeed3, -21.5963440f, testNumber++);

			wheel1.mCurrentLongitudinalForce = 4045.23f;
			wheel1.UpdateWheelSpinSpeed();
			float resultSpeed4 = wheel1.mSpeed;
			CheckResult(resultSpeed4, -2.49769402f, testNumber++);

			wheel1.mCurrentHubSpeed = 0.1f;
			wheel1.mSpeed = 0;
			wheel1.mCurrentLongitudinalForce = -8834.89844f;

			wheel1.UpdateWheelSpinSpeed();
			float resultSpeed5 = wheel1.mSpeed;
			CheckResult(resultSpeed5, 0.138800f, testNumber++);


			wheel1.mCurrentHubSpeed = 0.1f;
			wheel1.mSpeed = 0.217457563f;
			wheel1.mCurrentLongitudinalForce = 7867.93750f;

			wheel1.UpdateWheelSpinSpeed();
			float resultSpeed6 = wheel1.mSpeed;
			CheckResult(resultSpeed6, 0.213110253f, testNumber++);

		}
	}
}
