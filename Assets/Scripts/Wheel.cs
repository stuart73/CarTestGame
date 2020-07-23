using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{	

	public class Wheel
	{

		public Wheel() { }

		//*****************************************
		public void Init(FVector pos, FVector size, float mass, CPOChassis chassis, int number)
		{
			// Copy initialisation data
			mChassis = chassis;
			mNumber = number;

			mPos = pos;
			mSize = size;
			mMass = mass;

			// Clear other data
			Reset();

			// Moment of inertia of a cylinder
			// from http://scienceworld.wolfram.com/physics/MomentofInertiaCylinder.html
			FMatrix m = new FMatrix();

			float radius_sq = (mSize.X * mSize.X) / 4.0f;
			float height_sq = (mSize.Y * mSize.Y);

			m.Row[0].X = 0.5f * mMass * radius_sq;
			m.Row[1].Y = (1.0f / 12.0f * mMass * height_sq) + (0.25f * mMass * radius_sq);
			m.Row[2].Z = m.Row[1].Y;

			mMomentOfInertia = m;
			mMomentOfInertiaInverse = m.Inverse();

			// although, as it turns out, I'm only ever using the top left term of this matrix...
			// because it's the MoI about the axle

			mLastSlipSpeed = 0.0f;

			mCurrentLongitudinalForce = 0.0f;

			// Init debug stuff for graphs
			mDGRPM = 0.0f;
			mDGSlipAngle = 0.0f;
			mDGSlipRatio = 0.0f;
			mDGLoad = 0.0f;
			mDGSuspensionOffset = 0.0f;
			mDGSuspensionSpeed = 0.0f;
		}
	
		public void Reset()
		{
			mSpeed = 0.0f;
			mYaw = 0.0f;
			mPitch = 0.0f;
			mSuspensionTravel = 0.0f;
			mSuspensionSpeed = 0.0f;
			mPreviousSuspensionSpeed = 0.0f;
			mIsSlipping = false;
			mIsOnGround = false;
		}

		//*****************************************
		public FVector GetBasePos()
		{
			// the point at the bottom of the tyre where it would hit the ground
			return mPos.Minus(new FVector(0, mSuspensionTravel, 0)).Minus( new FVector(0, mSize.Y * 0.5f, 0));
		}
		public FVector GetMinBasePos()
		{
			return mPos.Minus(new FVector(0, mSize.Y * 0.5f, 0));
		}
		public FVector GetMaxBasePos()
		{
			FVector a = new FVector(0, mSize.Y * 0.5f, 0);
			FVector b = new FVector(0, PhysicsParameters.PC_MAX_SUSPENSION_TRAVEL, 0);
			FVector c = mPos.Minus(b);
			FVector d = c.Minus(a);

			FVector f = mPos.Minus(new FVector(0, PhysicsParameters.PC_MAX_SUSPENSION_TRAVEL, 0)).Minus(new FVector(0, mSize.Y * 0.5f, 0));
			return d;
		}
		public float GetSuspensionForce()
		{
			// Damping
			float suspension_offset = PhysicsParameters.PC_MAX_SUSPENSION_TRAVEL - mSuspensionTravel;
			//	float	average_speed = (mSuspensionSpeed + mPreviousSuspensionSpeed) * 0.5f;
			float average_speed = mSuspensionSpeed;

			float force = POFunctions.DampedSuspensionForce(suspension_offset, average_speed);

			if (force < 0)
				force = 0;

			mDGSuspensionOffset = suspension_offset;
			mDGSuspensionSpeed = average_speed;

			return force;
		}
		public float GetTorqueFromCar()
		{
			// Amalgamate engine and braking forces
			// No engine braking as yet

			float torque = 0.0f;

			//****************************************************
			// Acceleration
			if (mChassis.IsWheelDriving(mNumber) && (!mChassis.mChangingGear))
			{
				float rpm = mChassis.GetEngineRPM();

				torque += mChassis.GetThrottle() *
						  POFunctions.MaxEngineTorquePercentageFromRPM(rpm) *
						  PhysicsParameters.PC_ENGINE_MAX_TORQUE *
						  (1.0f / mChassis.GetGearRatio()) * // Divide. not multiply!!
						  ((mChassis.mDriveType == EDriveType.DT_4WD) ? (0.25f) : 0.5f);

				mDGRPM = rpm; // store for graphs
			}

			//****************************************************
			// Braking
			float braking = mChassis.GetBraking() * PhysicsParameters.PC_BRAKING_TORQUE;

			if (IsFront())
			{
				braking *= PhysicsParameters.PC_BRAKE_BALANCE * 2.0f;
			}
			else
			{
				braking *= (1.0f - PhysicsParameters.PC_BRAKE_BALANCE) * 2.0f;
			}

			// calculate how much force would actually cause the wheel to lock up
			float torque_to_stop = (float) Math.Abs(mSpeed * mMomentOfInertia.Row[0].X) / mChassis.mTimeStep;

			// and clip - we can't brake more than this!
			if (braking > torque_to_stop)
				braking = torque_to_stop;

			// braking can go either way
			if (mSpeed >= 0)
				torque -= braking;
			else
				torque += braking;

			//****************************************************
			return torque;
		}

		public FMatrix GetOri()
		{
			FMatrix foo = new FMatrix();
			foo.MakeRotationYawF(mYaw);

			return foo;
		}
		public FVector GetPerp()
		{
			// direction along the axle
			return GetOri().Multiply(new FVector(1.0f, 0, 0));

		}
		public FVector GetDir(FMatrix ori,  FVector normal)
		{
			// "forwards" in the plane of the ground, perpendicular to the normal
			return (ori.Multiply(GetPerp())).Cross(normal);
		}

		public float GetLastSlipSpeed() { return mLastSlipSpeed; }

		public bool IsFront() { return mNumber < 2; }

		// Wheel spin speed code - lifted from Simergy


		//*****************************************************************************
		bool SAME_SIGN(float a, float b) { return (a * b) >= 0.0f; }

		//*****************************************************************************
		float DRIVE_FILTER_GAIN(int n)
		{
			switch (n)
			{
				default:
				case 0:
				case 1:
					return 0.375f;
				case 2:
				case 3:
					return 0.25f;
			};
		}

		//*****************************************************************************
		public void UpdateWheelSpinSpeed()
		{
			// update wheel speeds
			// code *would* look like this:

			//	float	wheel_torque = GetTorqueFromCar() - (mCurrentLongitudinalForce * mSize.Y * 0.5f);
			//	mSpeed += wheel_torque / mMomentOfInertia.Row[0].X * mChassis->mTimeStep;

			// except it introduces instabilities at low dt.  So - various special case hacks come into play

			float hub_speed = mCurrentHubSpeed;
			float wheel_speed = mSpeed - (mCurrentHubSpeed / (mSize.Y * 0.5f)); // is relative
			float old_wheel_speed = wheel_speed;

			float engine_torque = GetTorqueFromCar();                           // +ve = forwards  ie +yaw
			float traction_torque = mCurrentLongitudinalForce * mSize.Y * 0.5f; // +ve = backwards ie -yaw
			float Dt = mChassis.mTimeStep;
			float inertia = mMomentOfInertia.Row[0].X;
			float DSpeed = (engine_torque - traction_torque) / inertia * Dt;

			bool supporting = true;

			// if driving wheel and off ground, always do opposing
			if ((!mIsOnGround) && mChassis.IsWheelDriving(mNumber))
			{
				supporting = false;
			}
			else
			{
				// see if the traction force is supporting or opposing the resultant force
				// (ie they have different sign)
				// slightly different hack - multiply them together
				if (!SAME_SIGN(engine_torque, traction_torque))  // remember - they have opposite signs
					supporting = true;
				else
					supporting = false;
			}

			if (supporting)
			{
				// traction is acting in the same direction as non tractive forces
				// like braking.  Therefore  the wheel is spinning in the "wrong direction"
				// relative to the non slip velocity and can be allowed to move toward
				// zero (relative) unfiltered

				// assume that spin is opposite to traction_torque - should check
				//		ASSERT((!SAME_SIGN(mSpeed, DSpeed)) || (mSpeed * DSpeed == 0));

				// if we apply the full force, do we cross 0?
				if (SAME_SIGN(wheel_speed, wheel_speed + DSpeed) || wheel_speed == 0)
				{
					// nope - all's well
					wheel_speed += DSpeed;
				}
				else
				{
					// yes - bad things happen
					float perc_before = wheel_speed / (-DSpeed);
					float perc_after = 1.0f - perc_before;
					float Dt_remaining = Dt * perc_after;

					// so - business as usual to the zero crossing
					wheel_speed = 0;

					// then we want to continue (with the 'opposing' code below with what's left)
					DSpeed = DSpeed * perc_after;
					Dt = Dt_remaining;

					// pull out the traction force
					DSpeed += traction_torque / inertia * Dt; // remember sign convention
					traction_torque = 0;
					supporting = false;
				}

			}

			if (!supporting)
			{
				// if the traction force dominates then may want speedy qeul.
				// it depends on how much differential there is between opposing forces.
				// This may just require a slight reduction in slip ratio rather than a
				// complete sign reversal.  DVMAX is calculated to limit the quel
				// between no change and a zero slip velocity...

				if ((float)Math.Abs(traction_torque) > (float)Math.Abs(engine_torque))
				{
					// traction force dominates
					float DVMax = (float)Math.Abs(wheel_speed * (1.0f - (float)Math.Abs((engine_torque / traction_torque))));

					// clip DSpeed
					if ((float)Math.Abs(DSpeed) > (float)Math.Abs(DVMax))
					{
						if (SAME_SIGN(DSpeed, DVMax))
							DSpeed = DVMax;
						else
							DSpeed = -DVMax;
					}

					// and change the speed - but don't cross zero
					float new_speed = wheel_speed + DSpeed;

					if (!SAME_SIGN(wheel_speed, new_speed))
						wheel_speed = 0;
					else
						wheel_speed = new_speed;
				}
				else
				{
					// traction force doesn't dominate

					// use this random 'drive filter gain' thing...
					DSpeed = DSpeed * DRIVE_FILTER_GAIN(mNumber);


					//!!!!!!!!!!!!

					wheel_speed += DSpeed;

				}
			}

			// actually change the speed
			mSpeed += (wheel_speed - old_wheel_speed);
		}



		//*****************************************
		CPOChassis mChassis;
		int mNumber;

		public FVector mPos;
		public FVector mSize;
		public float mSpeed;
		public float mYaw;
		public float mPitch;
		public float mSuspensionTravel;
		public float mSuspensionSpeed, mPreviousSuspensionSpeed;
		bool mIsSlipping;
		public bool mIsOnGround;
		public FVector mGroundNormal;

		public float mLastSlipSpeed;

		public float mCurrentLongitudinalForce;
		public float mCurrentHubSpeed;


		float mMass;
		FMatrix mMomentOfInertia;
		FMatrix mMomentOfInertiaInverse;

		public FMatrix GetMomentOfInertia()
        {
			return mMomentOfInertia;

		}

		public FMatrix GetMomentOfInertiaInverse()
		{
			return mMomentOfInertiaInverse;

		}

		// Debug stuff for graphs
		float mDGRPM;

		public float mDGSlipAngle;
		public float mDGSlipRatio;
		public float mDGLoad;
		public float mDGSuspensionOffset;
		public float mDGSuspensionSpeed;
	};
}
