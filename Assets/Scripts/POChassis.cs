using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
	//***************************************************************
	public enum EDriveType
	{
		DT_FWD,
		DT_RWD,
		DT_4WD,
	};

	//***************************************************************
	public class SPOInputParams
	{
		public SPOInputParams() 
		{
			mAnglesValid = false;
			mIsAI = false;
		}

		public float mTrackYaw;
		public float mTrackPitch;
		public float mTrackRoll;

		public float mYawChange, mPitchChange;

		public float mSlipStream;

		public bool mAnglesValid;

		public bool mIsAI;
	};

	//***************************************************************
	public class CPOChassis : PhysicsObject
	{
		const int NUM_POINTS		=	100;
		const int NUM_POINT_TYPES = 5;
		const int NUM_CCS = 2;


		const float COG_Z_OFFSET = 0.1685f;
		FVector MESH_OFFSET = new FVector(0, 0.3f, 0.235f);   // Hand-picked offset for centre of car
		FVector COG_OFFSET = new FVector(0, 0, COG_Z_OFFSET); // Hand-picked offset for Centre of Gravity

		public CPOChassis()
		{
			int c0;

			//****************************************************
			// Ensure the functions we'll use are initialised
			//CPOFunctions::Init();

			//****************************************************
			// Sort out Centre of Gravity and Moment of Inertia Tensor Matrix
			EvalutateCoGAndMoI();

			//****************************************************
			// Initialse collision shape (numbers chosen by hand)

			for (int i=0;i< NUM_CCS;i++)
			{
				mCCs[i] = new POCollisionCuboid();
			}

			mCCs[0].mPos = PhysicsParameters.WORLD_TO_PHYSICS(new FVector(0, 0.0f + 0.3f, 0)).Minus(mCoGOffset);
			mCCs[0].mSize = PhysicsParameters.WORLD_TO_PHYSICS(new FVector(2.20f, 0.40f, 4.2f));
			mCCs[0].mWeight = 4.0f;
			mCCs[1].mPos = PhysicsParameters.WORLD_TO_PHYSICS(new FVector(0, 0.3f + 0.3f, -0.8f)).Minus(mCoGOffset);
			mCCs[1].mSize = PhysicsParameters.WORLD_TO_PHYSICS(new FVector(1.8f, 1.0f, 2.4f));
			mCCs[1].mWeight = 1.0f;

			//****************************************************
			// Initialise wheels
			//FVector wheel_size = new FVector(0.4f, 0.95f, 0.95f);

			// SRH slight smaller wheels make the car feel more menevable
			FVector wheel_size = new FVector(0.4f, 0.68f, 0.68f);


			//	float	wheel_mass = mMass / 50.f;
			float wheel_mass = mMass / 50.0f * 5.0f;

			float wheel_x_offset = 1.0f;
			float wheel_z_offset = 1.685f;

		//	wheel_x_offset /= 2;
			//wheel_z_offset /= 2;



			mWheels[0].Init(PhysicsParameters.WORLD_TO_PHYSICS(new FVector(-wheel_x_offset, 0.0f, wheel_z_offset)).Minus(mCoGOffset), wheel_size, wheel_mass, this, 0);
			mWheels[1].Init(PhysicsParameters.WORLD_TO_PHYSICS(new FVector(wheel_x_offset, 0.0f, wheel_z_offset)).Minus(mCoGOffset), wheel_size, wheel_mass, this, 1);
			mWheels[2].Init(PhysicsParameters.WORLD_TO_PHYSICS(new FVector(-wheel_x_offset, 0.0f, -wheel_z_offset)).Minus(mCoGOffset), wheel_size, wheel_mass, this, 2);
			mWheels[3].Init(PhysicsParameters.WORLD_TO_PHYSICS(new FVector(wheel_x_offset, 0.0f, -wheel_z_offset)).Minus(mCoGOffset), wheel_size, wheel_mass, this, 3);

			for (c0 = 0; c0 < 4; c0++)
				mWheels[c0].Reset();

			//****************************************************
			// Reset Engine and Steering stuff
			mThrottle = 0;
			mBraking = 0;

			mDesiredSteeringAngle = 0;
			mSteeringWheelPos = 0;
			mDesiredThrottle = 0;
			mDesiredBraking = 0;
			mSteeringAssistOffset = 0;
			mLastPitchOnGround = 0;

			mDriveType = EDriveType.DT_4WD;

			mGear = 1;
			mChangingGear = false;
			mEngineRPM = 0;

			//****************************************************
			mCar = null ;

			//****************************************************
			// Clear debug stuff
			ClearDebugArrows();

			// and debug storage for graph markers
			mDGSpeed4Drag = 0.0f;
			mDGYaw4Correct = 0.0f;
			mDGOmegaY4Correct = 0.0f;
			mDGPitch4Correct = 0.0f;
			mDGOmegaP4Correct = 0.0f;

			for (c0 = 0; c0 < 4; c0++)
			{
				mDGTractionForcesLat[c0] = 0.0f;
				mDGTractionForcesLong[c0] = 0.0f;
			}
		}

		public override void Reset()
        {
			base.Reset();
			for (int c0 = 0; c0 < 4; c0++)
				mWheels[c0].Reset();

			//****************************************************
			// Reset Engine and Steering stuff
			mThrottle = 0;
			mBraking = 0;

			mDesiredSteeringAngle = 0;
			mSteeringWheelPos = 0;
			mDesiredThrottle = 0;
			mDesiredBraking = 0;
			mSteeringAssistOffset = 0;
			mLastPitchOnGround = 0;
			mGear = 1;
			mChangingGear = false;
			mEngineRPM = 0;

		}

		public void SetDriveType(EDriveType type)
        {
			mDriveType = type;
        }


		public void SetInputParams(SPOInputParams parms) 
		{
			mInputParams = parms;
		}

		public FVector GetMeshOffset() 
		{
			return PhysicsParameters.PHYSICS_TO_WORLD(mMeshOffset.Add(mCoGOffset)).Negative(); 
		}


		// Access Functions
		public void SetDesiredSteeringAngle(float v)
		{
			float angle = v * PhysicsParameters.PC_MAX_TURNING_CIRCLE * GetSteeringLock();

			//float angle = v;
			mDesiredSteeringAngle = angle;
			mSteeringWheelPos = v;
		}

		public void SetDesiredThrottle(float v) 
		{ 
			mDesiredThrottle = v; 
		}
		public void SetDesiredBraking(float v)
		{
			mDesiredBraking = v; 
		}

		public void SetRawThrottle(float v)	// for testing
		{
			mThrottle = v;
		}
		public void SetRawBraking(float v) // for tesing
		{
			mBraking = v;
		}

		public Wheel GetWheel(int i)
        {
			return mWheels[i];
        }

		public void SetChangingGear(bool value) // for testign
        {
			mChangingGear = value;
        }

		public void SetCar(Car2 inCar) { mCar = inCar; }

		public float GetEngineRPM()
		{ 
			return mEngineRPM;
		}
		public void SetEngineRPM(float inRPM)
		{
			mEngineRPM = inRPM;
		}
		public float GetThrottle() 
		{ 
			return mThrottle;
		}

		public float GetBraking()
		{
			return mBraking;
		}


		public float GetGearRatio()
		{
			return GetGearRatio(-1000);
		}
		public float GetGearRatio(int gear)
		{
			if (gear == -1000)
				gear = mGear;    // if not specified, use the current gear

			switch (gear)
			{
				default:
				case 1: return PhysicsParameters.PC_GEAR_RATIO_1;
				case 2: return PhysicsParameters.PC_GEAR_RATIO_2;
				case 3: return PhysicsParameters.PC_GEAR_RATIO_3;
				case 4: return PhysicsParameters.PC_GEAR_RATIO_4;
				case 5: return PhysicsParameters.PC_GEAR_RATIO_5;
				case 6: return PhysicsParameters.PC_GEAR_RATIO_6;
				case 7: return PhysicsParameters.PC_GEAR_RATIO_7;
				case 8: return PhysicsParameters.PC_GEAR_RATIO_8;
				case 9: return PhysicsParameters.PC_GEAR_RATIO_9;
				case 10: return PhysicsParameters.PC_GEAR_RATIO_10;
			};
		}
		public int GetGear()
		{ 
			return mGear;
		}
		public void SetGear(int inGear) 
		{ 
			mGear = inGear; 
		}
		public float GetSpeedoSpeed()
		{
			/*	// hmm.. just use the speed of the driven wheels
	float ws = GetAverageDrivingWheelSpeed();

	return ws * mWheels[0].mSize.Y * 0.5f;    */

			//	return (mOrientation.Inverse() * mVelocity).Z
			return GetLinearVelocity().Magnitude();
		}

		public float GetSteeringWheelPos()
		{
			float pos = mWheels[0].mYaw / PhysicsParameters.PC_MAX_TURNING_CIRCLE;

			Clamp(ref pos, -1.0f, 1.0f);

			return pos;
		}

		public FVector GetWheelBasePosWorld(int n)
		{
			return (mOrientation.Multiply(mWheels[n].GetMinBasePos())).Add(mPos);
		}


		public void ClearDebugArrows()
		{

		}

		public void Apply(CPOChassis inData)
		{
			/// SCR SRG (*this) = inData;
		}

		// Testbed methods
		public virtual void ApplyInitialOverrides()
		{ 
			// SCR SRG empty
		}

		public virtual void ApplyOverrides()
		{
			// SCR SRG empty
		}

		// Overloads from CPhysicsObject
		public override void Model()
		{
			int c0;

			//****************************************************
			// Update stuff from the editable constants
			mDriveType = EDriveType.DT_4WD;   // SCR SRG EDriveType(PC_DRIVE_TYPE);

			//****************************************************
			// Slightly amusing RPM calculation
			mEngineRPM = GetAverageDrivingWheelSpeed() / (2.0f * (float)Math.PI) * 60.0f / GetGearRatio();

			//****************************************************
			// Update Steering
			UpdateSteering();

			//****************************************************
			// Update Engine and Braking
			mThrottle = mDesiredThrottle;
			mBraking = mDesiredBraking;

			//****************************************************
			// Apply any overrides from the testbed
			ApplyOverrides();

			//****************************************************
			// Apply Downforce
			ApplyDownforce();

			//****************************************************
			// Apply Drag
			ApplyDrag();

			//****************************************************
			// Call the base class to move the object and handle any rigid body collision
			base.Model();

			//****************************************************
			// Move suspension - also handles collision with the wheels and the ground
			for (c0 = 0; c0 < 4; c0++)
				MoveSuspension(c0);

			//****************************************************
			// Process forces at the wheel - this is the basic driving model.
			ProcessWheelForces();

			//****************************************************
			// Update wheel spin speeds
			for (c0 = 0; c0 < 4; c0++)
				mWheels[c0].UpdateWheelSpinSpeed();

			//****************************************************
			// Limited slip Diff??
			ProcessDifferential();

			//****************************************************
			// Change Gears?
			ProcessAutoChanger();

			//****************************************************
			// Orientation controls
			ProcessOrientationDampingAndAssist();

			//****************************************************
			// Rotate wheels
			RotateWheels();

		}
		public override void FillOutKeyPoints(SKeyPoint[] points, out int num)
		{
			// Point positions for collision (and debugging)
			// This is a slightly silly way to do it, but it allows for movable things like suspension.
			// Will be rewritten when we get cuboid collision in 

			int n = 0, c0;

			// tyres first
			for (c0 = 0; c0 < 4; c0++)
			{
				points[n].mPos = mWheels[c0].GetBasePos();
				//		points[n].mType = SKeyPoint::KPT_SUSPENSION;  
				points[n].mType = EKeyPointType.KPT_NON_COLLIDABLE;
				n++;
			}
			// axle positions
			for (c0 = 0; c0 < 4; c0++)
			{
				points[n].mPos = mWheels[c0].mPos;
				points[n].mType = EKeyPointType.KPT_NON_COLLIDABLE;
				n++;
			}

			// Centre of Gravity
			points[n].mPos = new FVector(0, 0, 0) ;
			points[n].mType = EKeyPointType.KPT_NON_COLLIDABLE;
			n++;

			// then chassis
			for (c0 = 0; c0 < NUM_CCS; c0++)
				mCCs[c0].AddPoints(points, ref n, (mInputParams.mIsAI) ? false : true);

			num = n;
		}
		public override void DeclareDamage(float amount)
		{
			if (mCar)
				mCar.ApplyDamage(amount);
		}

		// Internal methods
		public void EvalutateCoGAndMoI()
		{
			//****************************************************
			mMeshOffset = MESH_OFFSET;
			mCoGOffset = COG_OFFSET;

			// Numbers from GC
			mMomentOfInertia = new FMatrix() ;
			mMomentOfInertia.Row[0].X = PhysicsParameters.PC_MOI_X;
			mMomentOfInertia.Row[1].Y = PhysicsParameters.PC_MOI_Y;
			mMomentOfInertia.Row[2].Z = PhysicsParameters.PC_MOI_Z;

			mMomentOfInertiaInverse = mMomentOfInertia.Inverse();
		}
		public int GetNumWheelsOnGround()
		{
			int c0, n = 0;

			for (c0 = 0; c0 < 4; c0++)
				if (mWheels[c0].mIsOnGround)
					n++;
			return n;
		}

		public float GetAverageDrivingWheelSpeed()
		{
			float speed = 0;
			int num_wheels = 0;
			int c0;

			for (c0 = 0; c0 < 4; c0++)
			{
				if (IsWheelDriving(c0))
				{
					speed += mWheels[c0].mSpeed;
					num_wheels++;
				}
			}

			if (num_wheels > 0)
				speed /= (float)(num_wheels);

			return speed;
		}
		public float GetSteeringLock()
		{
			// Reduce the maximum amount of steering with speed.
			// This is to help control of the car on a joypad - maybe not necessary if you're 
			// using a steering wheel - although I can't really see any way that turning the 
			// steering wheel to full lock at high speed (>150kph) won't result in total loss
			// of control of the vehicle

			// Simple log reduction - lock is halved at PC_TURNING_CIRCLE_HALF, 
			// and quartered at PC_TURNING_CIRCLE_HALF * 2 etc....

			float speed = GetLinearVelocity().Magnitude();
			float thelock  = PhysicsParameters.PC_MAX_TURNING_CIRCLE;

			thelock /= (speed / PhysicsParameters.PC_TURNING_CIRCLE_HALF) * 2.0f;

			if (thelock > PhysicsParameters.PC_MAX_TURNING_CIRCLE)
				thelock = PhysicsParameters.PC_MAX_TURNING_CIRCLE;

			return thelock;
		}
		public float GetSuspensionForce(int num)
		{
			// SCR SRG missing from c++ version
			return 0.0f;
		}


		

		public void MoveSuspension(int num)
		{
			// Collide the wheel with the track and adjust the suspension accordingly

			// Interestingly, I currently allow the suspension to move at an infinite speed
			// ie it adjusts so that the wheels remain on the track
			// this provides better grip at the moment with a bumpy polygonal track

			// If I restricted the maximum speed (or acceleration) of the suspension,
			// I'd need to pass the remaining force directly to the chassis, bypassing the suspension
			// I'm not sure exactly what effect this would have, but it would take some time fiddling
			// to prevent the car bouncing all over the place..

			Wheel w = mWheels[num];
			w.mPreviousSuspensionSpeed = w.mSuspensionSpeed;

			if (WhatDoICollideWith() == 2 || WhatDoICollideWith() == 3)
			{
				//***************************************************************
				// collide with mesh
				float prev_susp = w.mSuspensionTravel;

				// calculate 'start' and 'end' points representing the extent of the suspension travel
				FVector min_pos = (mOrientation.Multiply(w.GetMinBasePos())).Add(mPos);
				FVector max_pos = (mOrientation.Multiply(w.GetMaxBasePos())).Add(mPos);

				// and collide this line with the track 
				// the extra vector is to make sure we don't miss & fall through
				FVector ip = new FVector(0,0,0);
				FVector normal = new FVector(0, 1, 0);

				//bool hit = mTrack->LineIntersect(PHYSICS_TO_WORLD(min_pos) + (mOrientation * FVector(0, 15.f, 0)),
				//									PHYSICS_TO_WORLD(max_pos), &ip, &normal);

				FVector sp = min_pos.Add(mOrientation.Multiply(new FVector(0, 1.5f, 0)));
				FVector ep = max_pos;
				bool hit = false;

				if (WhatDoICollideWith() == 2)
				{
					hit = LineIntersectionTest(sp, ep, ip, normal);
				}
				else
                {
					hit = testLandscape.LineIntersect(sp, ep, ip, normal);
				}

				float hpy = -12.3979435f;

				/*
				bool hit = false;		
				float sl = min_pos.Y + 1.5f;
				float el = max_pos.Y;

				if ( sl > hpy && el < hpy)
				{
					hit = true;
					ip = new FVector(min_pos.X, hpy, min_pos.Z);
					normal = new FVector(0, 1, 0);
				}
				*/


				if (hit)
				{
					//ip = PhysicsParameters.WORLD_TO_PHYSICS(ip);
					FVector local_ip = mOrientation.Inverse().Multiply((ip.Minus(mPos)));
					FVector local_min_pos = w.GetMinBasePos();

					w.mSuspensionTravel = -(local_ip.Y - local_min_pos.Y) + 0.2f;

					// clip to maximum travel
					if (w.mSuspensionTravel > PhysicsParameters.PC_MAX_SUSPENSION_TRAVEL)
						w.mSuspensionTravel = PhysicsParameters.PC_MAX_SUSPENSION_TRAVEL;

					// OK - now can we actually use the tyre model at this point??
					// are we significantly upside down?
//#if PM_COLLISION_TYPE > PMCT_PARTIAL
					FVector	car_up = min_pos.Minus(max_pos);
					car_up.Normalise();

					float	cs = (float)Math.Cos(DEG2RAD(50.0f));

					if (normal.DotProduct(car_up) < cs)
					{
						// too far - rely on chassis
						w.mIsOnGround = false;
					}
					else
//#endif
					{
						w.mIsOnGround = true;
						w.mGroundNormal = normal;
					}

					// damage??
					// if suspension is very compressed, add damage
					if (w.mSuspensionTravel < -0.4f)
					{
						float dist = ((-0.4f) - (w.mSuspensionTravel));
						if (dist > 0.2f)
							dist = 0.2f;

						DeclareDamage(dist * 0.025f);
					}
				}
				else
				{
					// didn't hit anything
					w.mSuspensionTravel = PhysicsParameters.PC_MAX_SUSPENSION_TRAVEL;
					w.mIsOnGround = false;
				}

				w.mSuspensionSpeed = (w.mSuspensionTravel - prev_susp) / mTimeStep;
			}
			else
			{
				// SCR SRG NOT USED
				/*
				// JCL - Hacky code for collision with heightfield - largely irrelevant
				float dist_below_ground = 0;

				float prev_susp = w.mSuspensionTravel;

				w.mSuspensionTravel = PhysicsParameters.PC_MAX_SUSPENSION_TRAVEL;

				FVector wheel_pos = (mOrientation.Multiply(w.GetBasePos())).Add(mPos);

				float hgt = mLandscape->GetHeight(wheel_pos.X, wheel_pos.Z);
				//		float hgt = 0.f;  // uncomment for plane rather than heightfield

				if (wheel_pos.Y < hgt)
					dist_below_ground = hgt - (wheel_pos.Y);

				// Try to clip to ground
				float susp_delta = 0;

				if (dist_below_ground > 0)
				{
					FVector up = mOrientation * FVector(0, 1, 0);  //! Use normal!

					float change_per_unit_object_y = up.Y;

					if (change_per_unit_object_y > 0.1f)
						susp_delta = dist_below_ground / change_per_unit_object_y;

					// Flag that this wheel is on the ground
					w->mIsOnGround = TRUE;

					// approximate vnormal...
					FVector normal = -FVector(1.f, mLandscape->GetHeight(wheel_pos.X + 1.f, wheel_pos.Z) - hgt, 0) ^
									  FVector(0.f, mLandscape->GetHeight(wheel_pos.X, wheel_pos.Z + 1.f) - hgt, 1.f);
					normal.Normalise();

					w->mGroundNormal = normal;
				}
				else
					w->mIsOnGround = FALSE;

				w->mSuspensionTravel -= susp_delta - 0.2f;


				if (w->mSuspensionTravel > PC_MAX_SUSPENSION_TRAVEL)
					w->mSuspensionTravel = PC_MAX_SUSPENSION_TRAVEL;

				w->mSuspensionSpeed = (w->mSuspensionTravel - prev_susp) / mTimeStep;
				*/

			}
		}

		public struct SCachedForce
		{
			public FVector mForce;
			public FVector mPos;
			public int mPoint;
			public int mType;
		};

		public void AddForce(FVector f, FVector ps, int pt, int tp, SCachedForce[] forces, ref int num_forces)
		{ 
			forces[num_forces].mForce = f; 
			forces[num_forces].mPos = ps; 
			forces[num_forces].mPoint = pt;
			forces[num_forces].mType = tp;
			num_forces++; 
		}


		public void ProcessWheelForces()
		{
			// This is the heart of the driving model
			// For each wheel, calculate longitudinal and lateral forces based on
			// slip ratio and slip angle, and resolve using traction angle

			SCachedForce[] forces = new SCachedForce[100];
			int num_forces = 0;

			int c0;

			//*************************************************************
			for (c0 = 0; c0< 4; c0 ++)
			{
				Wheel w = mWheels[c0];

				if (mWheels[c0].mIsOnGround)
				{
					//*************************************************************
					// Calculate some useful information
					FVector pos = mOrientation.Multiply(w.GetBasePos());
					FVector normal = w.mGroundNormal;
					FVector wheel_direction = w.GetDir(mOrientation, normal);
					FVector hub_velocity = GetPointVelocity(pos);
					FVector hub_direction = hub_velocity;
					hub_direction.Normalise();
					FMatrix invori = mOrientation.Inverse();
					float hub_speed = hub_velocity.DotProduct(wheel_direction);
					float wheel_speed = w.mSpeed * mWheels[c0].mSize.Y * 0.5f;
					FVector wheel_velocity = hub_velocity.Minus((wheel_direction.Multiply(wheel_speed)));

					float traction_bias = (c0 < 2) ? (2.0f - PhysicsParameters.PC_TRACTION_BIAS_REAR) : PhysicsParameters.PC_TRACTION_BIAS_REAR;

					//*************************************************************
					// Suspension Force
					float upward_force = w.GetSuspensionForce();
					FVector suspension_force = normal.Multiply(upward_force);
					AddForce(suspension_force, pos.Add(mPos), c0, 0, forces, ref num_forces);

					//*************************************************************
					// Slip Angle

					// work out the slip angle - this is the angle between the wheel direction and the
					// direction of the chassis at that point.

					// rotate back into frame of reference of car;
					FVector local_wheel_direction = invori.Multiply(wheel_direction);
					FVector local_hub_direction = invori.Multiply(hub_direction);

					// force into plane of car
					local_wheel_direction.Y = 0;
					local_hub_direction.Y = 0;

					float wheel_angle = (float)Math.Atan2(local_wheel_direction.X, local_wheel_direction.Z);
					float hub_angle = (float)Math.Atan2(local_hub_direction.X, local_hub_direction.Z);
					float slip_angle = wheel_angle - hub_angle;

					while (slip_angle< (float)-Math.PI)
						slip_angle += (float)Math.PI* 2.0f;
					while (slip_angle > (float)Math.PI)
						slip_angle -= (float)Math.PI* 2.0f;

					//*************************************************************
					// Slip Ratio

					if (hub_speed< 0.1f)   // 32mph ??
						hub_speed = 0.1f;
					float slip_ratio = (wheel_speed - hub_speed) / hub_speed;

					//*************************************************************
					// Longitudinal Force

					float lateral_force_magnitude = POFunctions.LateralForceFromSlipAngleSlipRatioAndLoad(slip_angle, slip_ratio, upward_force) * traction_bias;
					FVector lateral_direction = (wheel_direction.Cross(normal)).Negative();

					FVector lateral_force = lateral_direction.Multiply(lateral_force_magnitude);

					//*************************************************************
					// Longitudinal force

					float longitudinal_force_magnitude = POFunctions.LongitudinalForceFromSlipAngleSlipRatioAndLoad(slip_angle, slip_ratio, upward_force) * traction_bias;
					FVector longitudinal_force = wheel_direction.Multiply(longitudinal_force_magnitude);

					//*************************************************************
					//hack!!
					bool sign_changed = false;

					if ((slip_ratio > 0.0f && w.mDGSlipRatio< 0.0f) ||
						(slip_ratio > 0.0f && w.mDGSlipRatio< 0.0f))
						sign_changed = true;

					if (sign_changed)
					{
						longitudinal_force.DivideInPlace( 2.0f);
						longitudinal_force_magnitude /= 2.0f;
					}

					//*************************************************************
					// Apply the force

					AddForce(lateral_force.Add(longitudinal_force), pos.Add(mPos), c0, 1, forces, ref num_forces);
					//*************************************************************
					// Store force to move wheels later
					w.mCurrentLongitudinalForce = longitudinal_force_magnitude;
					w.mCurrentHubSpeed = hub_speed;
					//			float	wheel_force = engine_torque - (longitudinal_force_magnitude * w->mSize.Y * 0.5f);
					//			w->mSpeed += wheel_force / w->mMomentOfInertia.Row[0].X * mTimeStep;

					//*************************************************************
					// and store some debug stuff for the graphs
					w.mDGSlipAngle = slip_angle;
					w.mDGSlipRatio = slip_ratio;
					w.mDGLoad		= upward_force;

					mDGTractionForcesLat[c0] = lateral_force_magnitude;
					mDGTractionForcesLong[c0] = longitudinal_force_magnitude;

					w.mLastSlipSpeed = wheel_velocity.Magnitude();
				}
				else
				{
					w.mDGSlipAngle = 0.0f;
					w.mDGSlipRatio = 0.0f;
					w.mDGLoad		= 0.0f;
					mDGTractionForcesLat[c0] = 0.0f;
					mDGTractionForcesLong[c0] = 0.0f;

					w.mLastSlipSpeed = 0.0f;
					w.mCurrentLongitudinalForce = 0.0f;
					w.mCurrentHubSpeed = 0.0f;
				}
			}

			for (c0 = 0; c0<num_forces; c0 ++)
			{
				ApplyForce(forces[c0].mPos, forces[c0].mForce, forces[c0].mPoint, forces[c0].mType);
			}
		}

		public float NonSlipRPM()
		{
			// calculate the RPM the engine *would* have if the wheels weren't slipping

			float speed = (mOrientation.Inverse().Multiply( GetLinearVelocity())).Z;
			float axle_speed = speed / (mWheels[0].mSize.Y * (float)Math.PI) * 60.0f;

			return axle_speed / GetGearRatio();
		}
		public void ProcessAutoChanger()
		{
			// Assess whether or not we need to change gear
			if (mChangingGear)
			{
				// advance the timer
				mGearChangeTime += mTimeStep;

				if (mGearChangeTime > PhysicsParameters.PC_GEAR_CHANGE_TIME)
				{
					mChangingGear = false;
					mGear = mDestGear;
				}
			}
			else
			{
				if (GetNumWheelsOnGround() > 0)     // hmmm!
				{
					float non_slip_rpm = NonSlipRPM();

					// Change up?
					if ((non_slip_rpm > PhysicsParameters.PC_GEAR_CHANGE_UP_RPM) && (mGear < PhysicsParameters.PC_GEARS_NUM))
					{
						// yes
						mChangingGear = true;
						mDestGear = mGear + 1;
						mGearChangeTime = 0.0f;
					}
					else
					{
						// change down?
						if (mGear > 1 && (mThrottle == 0.0f))
						{
							float rpm_in_lower_gear = non_slip_rpm * GetGearRatio(mGear) / GetGearRatio(mGear - 1);

							if (rpm_in_lower_gear < PhysicsParameters.PC_GEAR_CHANGE_DOWN_RPM)
							{
								// yes
								mChangingGear = true;
								mDestGear = mGear - 1;
								mGearChangeTime = 0.0f;
							}
						}
					}
				}
			}

		}

		float DEG2RAD(float v) { return v * (float)(Math.PI / 180.0); }

		float RangeTransition(float tr, float start, float end)
		{
			tr -= start;
			tr /= (end - start);
			if (tr < 0.0f) tr = 0.0f;
			if (tr > 1.0f) tr = 1.0f;
			return tr;
		}

		void Clamp(ref float v, float min, float max)
		{
			if (v < min) v = min;
			if (v > max) v = max;
		}



		public void ProcessSteeringAssist()
		{
			if (mInputParams.mAnglesValid)
			{
				// get car yaw;
				//float yaw = (float)-Math.Atan2(mOrientation.Row[0].Z, mOrientation.Row[0].X);

				//CEulerAngles angles = new CEulerAngles(mOrientation);

				//mSteeringAssistOffset = (mInputParams.mTrackYaw - angles.mYaw);

				mSteeringAssistOffset = mInputParams.mTrackYaw; // - angles.mYaw);


				while (mSteeringAssistOffset > (float)Math.PI)
					mSteeringAssistOffset -= 2.0f * (float)Math.PI;
				while (mSteeringAssistOffset < (float)-Math.PI)
					mSteeringAssistOffset += 2.0f * (float)Math.PI;

				// Ok - now the steering assist should only really be there if you're pointing vaguely in the right direction
				float sa_cap = DEG2RAD(30.0f);
				float sa_max = DEG2RAD(50.0f);

				// try and fade out the angle through sa_cap to sa_max
				if ((mSteeringAssistOffset > sa_cap) || (mSteeringAssistOffset < -sa_cap))
				{
					float perc = 1.0f - (((mSteeringAssistOffset - sa_cap) / (sa_max - sa_cap)) * (mSteeringAssistOffset > 0 ? 1.0f : -1.0f));
					if (perc < 0)
						perc = 0.0f;

					mSteeringAssistOffset *= perc;
				}

				mSteeringAssistOffset *= PhysicsParameters.PC_STEERING_ASSIST;

				// also - fade out if you're actually steering
				mSteeringAssistOffset *= 1.0f - RangeTransition((float)Math.Abs(mSteeringWheelPos), 0.3f, 0.6f);

				// also - fade out if you're going slowly
				mSteeringAssistOffset *= RangeTransition(GetLinearVelocity().Magnitude(), PhysicsParameters.PC_STEERING_ASSIST_MIN_SPEED, PhysicsParameters.PC_STEERING_ASSIST_MAX_SPEED);
			}
			else mSteeringAssistOffset = 0.0f;
		}

		public void ProcessYawAssist()
		{
			// try to reorient the car so that it's facing the way it's moving
		/*
			mDGYaw4Correct		= 0.f;
			mDGOmegaY4Correct	= 0.f;
	
			if (GetNumWheelsOnGround() == 0)
			{

				// rotate velocity into frame of reference of car
				FVector	local_velocity = mOrientation.Inverse() * GetLinearVelocity();

				// Don't care about up or down motion
				local_velocity.Y = 0; 

				if (local_velocity.Z <= 0.f)
				{
					// oh dear - going backwards!!  
					// Just do nothing for now...
				}
				else
				{
					// get angular difference
					float speed = local_velocity.Magnitude();
					if (speed > 0.1f)
					{
						local_velocity.Normalise();
						float	yaw_offset = atan2f(local_velocity.X, local_velocity.Z);

						// we also need the angular velocity about the Y axis for damping
						float	omega = (mOrientation.Inverse() * GetAngularVelocity()).Y;

						float	torque = CPOFunctions::YawCorrectingTorque(yaw_offset, omega);

						torque *= speed;	// Could be the square of the speed, I guess

						// Torque is about the Y axis (yaw)
						mAngularMomentum += mOrientation * FVector(0, torque * mTimeStep, 0);  // should probably write an 'apply torque' function...

						// and store for graphs
						mDGYaw4Correct		= yaw_offset;
						mDGOmegaY4Correct	= omega;
					}
				}
			}
		    */
		}
		public void ProcessPitchAssist()
		{
			mDGPitch4Correct = 0.0f;
			mDGOmegaP4Correct = 0.0f;

			if (GetNumWheelsOnGround() == 0)
			{

				// rotate velocity into frame of reference of car
				FVector local_velocity = mOrientation.Inverse().Multiply(GetLinearVelocity());

				// Don't care about left or right motion
				local_velocity.X = 0;

				if (local_velocity.Z <= 0.0f)
				{
					// oh dear - going backwards!!  
					// Just do nothing for now...
				}
				else
				{
					// get angular difference
					float speed = local_velocity.Magnitude();
					if (speed > 0.1f)
					{
						local_velocity.Normalise();
						float pitch_offset = (float)-Math.Atan2(local_velocity.Y, local_velocity.Z);

						float actual_pitch = new CEulerAngles(mOrientation).mPitch;
						// OK - take #2 at bodging the pitch.
						// we now know the take off pitch - assuming that the landing pitch is approximately the same
						// but negative, add in an offset that means that we're looking down +10degrees (for a 30degree takeoff)
						// at the apex
						float max_offset = (-mLastPitchOnGround);// / 3.f;
						Clamp(ref max_offset, -DEG2RAD(30.0f), DEG2RAD(30.0f));

						// how far through the jump are we?
						float at_apex = 0.0f;

						if (mLastPitchOnGround != 0.0f)
							at_apex = 1.0f - ((float)Math.Abs(actual_pitch) / (float)Math.Abs(mLastPitchOnGround)) + 1.0f;

						Clamp(ref at_apex, 0.0f, 1.0f);

						pitch_offset += max_offset * at_apex;

						// we also need the angular velocity about the X axis for damping
						float omega = (mOrientation.Inverse().Multiply(GetAngularVelocity())).X;

						float torque = POFunctions.PitchCorrectingTorque(pitch_offset, omega);

						torque *= speed;    // Could be the square of the speed, I guess

						// Torque is about the X axis (yaw)
						mAngularMomentum.AddInPlace(mOrientation.Multiply( new FVector(torque * mTimeStep, 0, 0)));  // should probably write an 'apply torque' function...

						// and store for graphs
						mDGPitch4Correct = pitch_offset;
						mDGOmegaP4Correct = omega;
					}
				}
			}
			else
			{
				mLastPitchOnGround = new CEulerAngles(mOrientation).mPitch;
			}
		}
		public void ProcessRollAssist()
		{
			// nothing exciting here yet...

			//hm.. - consider just applying a damping value to the roll....
		}
		public void ProcessOrientationDampingAndAssist()
		{
			ProcessYawAssist();
			ProcessPitchAssist();
			ProcessRollAssist();

			/*	if (GetNumWheelsOnGround() == 0)
				{
					// damp the angular momentum a bit
			//#define	AM_DAMPING	0.96f
			#define	AM_DAMPING	0.7f

					float	damping = (1.f - AM_DAMPING) * mTimeStep;

					mAngularMomentum -= mAngularMomentum * damping;
				}*/
		}

		public void UpdateSteering()
		{
			int c0;

			ProcessSteeringAssist();

			float steering_delta = GetSteeringLock() * mTimeStep / PhysicsParameters.PC_STEERING_SPEED; // takes time to go to full lock

			float desired_steering_angle = mDesiredSteeringAngle + mSteeringAssistOffset;

			for (c0 = 0; c0 < 2; c0++)
			{
				if (mWheels[c0].mYaw < desired_steering_angle)
					mWheels[c0].mYaw += steering_delta;
				else if (mWheels[c0].mYaw > desired_steering_angle)
					mWheels[c0].mYaw -= steering_delta;

				if ((float)Math.Abs(mWheels[c0].mYaw - desired_steering_angle) <= steering_delta)
					mWheels[c0].mYaw = desired_steering_angle;


				// HACK
				//		mWheels[c0].mYaw = 0.033;
			}

		}
		public void ApplyDownforce()
		{
			int c0;

			if (GetNumWheelsOnGround() > 0)     // hmmm!
			{
				// apply force at the wheels (so I can shift the balance front to rear)
				float downforce = GetLinearVelocity().Magnitude() * PhysicsParameters.PC_DOWNFORCE / 4.0f;   // (4 wheels)
				FVector force = new FVector(0, -downforce, 0);

				for (c0 = 0; c0 < 4; c0++)
				{
					float modifier = 1.0f;
					if (c0 < 2)
						modifier = (1.0f - PhysicsParameters.PC_DOWNFORCE_BALANCE);
					else
						modifier = PhysicsParameters.PC_DOWNFORCE_BALANCE;

					modifier *= (1.0f - mInputParams.mSlipStream); // Account for Slipstream

					ApplyForce((mOrientation.Multiply(mWheels[c0].GetBasePos())).Add(mPos), mOrientation.Multiply((force.Multiply(modifier))), c0, 4);
				}
			}

		}
		public void ApplyDrag()
		{
			FVector drag_force = GetLinearVelocity();
			float speed = drag_force.Magnitude();
			float drag = POFunctions.DragFromSpeed(speed);

			drag *= (1.0f - mInputParams.mSlipStream); // Account for Slipstream

			drag_force.NormaliseAndScale(-drag);

			FVector ZERO_FVECTOR = new FVector(0, 0, 0);

			ApplyForce(ZERO_FVECTOR.Add(mPos), drag_force, 8, 4);		// SCR SRG add mpos to a zero vector ??!!
			mDGSpeed4Drag = speed;

		}
		public void ProcessDifferential()
		{
			//#define	DIFF	0.f
			float DIFF	=5.0f;

			if ((mDriveType == EDriveType.DT_FWD) || (mDriveType == EDriveType.DT_4WD))
			{
				// average out wheel speeds?
				float average_speed = (mWheels[0].mSpeed + mWheels[1].mSpeed) * 0.5f;

				mWheels[0].mSpeed += (average_speed - mWheels[0].mSpeed) * DIFF * mTimeStep;
				mWheels[1].mSpeed += (average_speed - mWheels[1].mSpeed) * DIFF * mTimeStep;
			}

			if ((mDriveType == EDriveType.DT_RWD) || (mDriveType == EDriveType.DT_4WD))
			{
				// average out wheel speeds?
				float average_speed = (mWheels[2].mSpeed + mWheels[3].mSpeed) * 0.5f;

				mWheels[2].mSpeed += (average_speed - mWheels[2].mSpeed) * DIFF * mTimeStep;
				mWheels[3].mSpeed += (average_speed - mWheels[3].mSpeed) * DIFF * mTimeStep;
			}
		}
		public void RotateWheels()
		{
			// note - this is a one-way process - the actual orientation of the wheels isn't used by
			// the physics code anywhere
			int c0;

			for (c0 = 0; c0 < 4; c0++)
			{
				float speed = mWheels[c0].mSpeed;
				float theta = speed * mTimeStep;

				mWheels[c0].mPitch -= theta;

				while (mWheels[c0].mPitch > (float)Math.PI * 2.0f)
					mWheels[c0].mPitch -= (float)Math.PI * 2.0f;

				while (mWheels[c0].mPitch < (float)-Math.PI * 2.0f)
					mWheels[c0].mPitch += (float)Math.PI * 2.0f;
			}
		}

		// Debug methods
		public override void SetupDebugCuboids()
		{

		}
		public override FVector GetDebugCuboidPosOffset(int num)
		{
			return new FVector(1, 1, 1);

		}
		public override FMatrix GetDebugCuboidOriOffset(int num)
		{
			return new FMatrix(); //
		}
		public override void DebugNotifyImpulse(int point, int type, FVector impulse)
		{

		}

		//******************************
		// Data

		Car2 mCar;         // Parent car in game
		POCollisionCuboid[] mCCs = new POCollisionCuboid[NUM_CCS];   // Collision Information
		FVector mMeshOffset;    // hard coded mesh offset to 'visual' centre
		FVector mCoGOffset;     // offset from centre of car to actual centre of gravity
		Wheel[] mWheels = new Wheel[4] { new Wheel(), new Wheel(), new Wheel(), new Wheel() };   // Robin Reliants in a later version!
		float mEngineRPM;       // now actual engine RPM!!
		int mGear;

		// Automatic gear changer
		public bool mChangingGear;
		int mDestGear;
		float mGearChangeTime;

		// Engine and Steering
		float mThrottle;
		float mBraking;
		float mDesiredThrottle;
		float mDesiredBraking;
		float mDesiredSteeringAngle;
		float mSteeringWheelPos;
		float mSteeringAssistOffset;
		public EDriveType mDriveType;

		// flight control
		float mLastPitchOnGround;

		public bool IsWheelDriving(int n)
		{
			switch (mDriveType)
			{
				case EDriveType.DT_FWD: return n < 2;
				case EDriveType.DT_RWD: return n > 1;
				default:
				case EDriveType.DT_4WD: return true;
			};
		}

		// Input data from game
		SPOInputParams mInputParams;



		// Debugging
		FVector[,] mDebugImpacts = new FVector[NUM_POINTS,NUM_POINT_TYPES];

		float mDGSpeed4Drag;
		float mDGYaw4Correct;
		float mDGPitch4Correct;
		float mDGOmegaY4Correct;
		float mDGOmegaP4Correct;
		float[] mDGTractionForcesLat = new float[4];
		float[] mDGTractionForcesLong = new float[4];
	}
}
