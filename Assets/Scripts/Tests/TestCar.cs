using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Tests
{
    class TestCar
    {
        public TestCar()
        {
            speed = 0;
            steering = 0;
            gear = 0;

            mOri = new FMatrix();
            mPos = new FVector();
            BOOST_EFFECT = 3.5f;
            boostOn = false;
        }

        CPOChassis mPOChassis;

        float speed;
        float steering;
        int gear;
        FVector velocity;

        float BOOST_EFFECT;
        bool boostOn;

        public FVector mPos;
        public FMatrix mOri;

        public void Start()
        {
            mPOChassis = new CPOChassis();
            mPOChassis.Init();
            TestLandscape tl = new TestLandscape();

            mPOChassis.testLandscape = tl;


            mPOChassis.SetOrientation(mOri);
            mPOChassis.SetPos(mPos);
        }

        public void Update()
        {
            float h = 0;
            float v = 0;

            Turn(h);
            if (v > 0)
            {
                Accelerate(v);
            }
            else
            {
                Brake(-v);
            }

            //if (PhysicsReady())
            Drive();

            // Copy data back from the Physics object to the Car Thing.

            FMatrix orientation = mPOChassis.GetOrientation();
            FVector localPosition = orientation.Multiply(mPOChassis.GetMeshOffset());
            FVector worldPosition = mPOChassis.GetPos().Add(localPosition);

            mPos = worldPosition;
            mOri = orientation;


            // velocity = mPOChassis.GetLinearVelocity() / 30.0f; ;

            //speed = mPOChassis.GetSpeedoSpeed() / 1000.0f * 3600.0f * (0.62137f); // JCL - is now MPH!!
            //steering = 1.0f - (mPOChassis->GetSteeringWheelPos() / 2.0f + 0.5f);
            //gear = mPOChassis->GetGear();
        }

        //******************************************************************************************
        public void Drive()
        {
            mPOChassis.ClearDebugArrows();

            /*
            if (mAI)
            {
        #if TARGET == XBOX
                    mPOChassis.SetTimeStep(100, 20);
        #else
                mPOChassis.SetTimeStep(60, 20);
        #endif
                }
                else
                */

            mPOChassis.SetTimeStep(200, 20);

            // JCL - think....
            // this may need to happen per physics tick...
            SPOInputParams chassisParams = new SPOInputParams();


            chassisParams.mAnglesValid = true;

            chassisParams.mTrackYaw = 0.0f;
            chassisParams.mTrackPitch = 0.0f;
            chassisParams.mTrackRoll = 0.0f;

            chassisParams.mIsAI = false;

            //CalculateSlipStream();
            chassisParams.mSlipStream = 0.0f;

            //   if (SCR_WORLD->GetRateOfChange(mSplinePosition, angles))
            //  {
            //      chassisParams.mYawChange = 0.0f;
            //    chassisParams.mPitchChange = 0.0f;
            //   }

            mPOChassis.SetInputParams(chassisParams);

            mPOChassis.Process();

            //!JCL hm.....

            mPOChassis.SetDesiredThrottle(0);
            mPOChassis.SetDesiredBraking(0);
        }

        //******************************************************************************************
        public void Accelerate(float val)
        {
            //   if ((Ready()) && (!Wrecked()))
            {
                if (val > 1.0f)
                    val = 1.0f;

                // if ((val > 0.0f) && (val < 1.0f))
                //   val = val;

                if (boostOn == true)
                    val *= BOOST_EFFECT;

                mPOChassis.SetDesiredThrottle(val);

                //mBools.SetBit(BRAKE_LIGHTS, FALSE);

                //		ApplyDamage(.01f);			// TM-TEST
            }
        }

        //******************************************************************************************
        public void Turn(float inValue)
        {
            if (inValue > 1.0f)
                inValue = 1.0f;
            else if (inValue < -1.0f)
                inValue = -1.0f;

            //   mTurn = inValue;
            mPOChassis.SetSteeringAngle(-inValue);
        }


        //******************************************************************************************
        public void Brake(float val)
        {
            //  if (Ready())
            {
                if (val > 1.0f)
                    val = 1.0f;

                mPOChassis.SetDesiredBraking(val);

                // if (val > 0.f)
                // {
                //    mBools.SetBit(BRAKE_LIGHTS, TRUE);

                //			ApplyDamage(.3f);			// TM-TEST
                //}
                //else
                //   mBools.SetBit(BRAKE_LIGHTS, FALSE);
            }
        }

        public void ApplyDamage(float amount)
        {

        }
    }
}
