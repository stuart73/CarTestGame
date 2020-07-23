using Assets.Scripts;
using Assets.Scripts.Tests;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;
using System;
//using ClassLibrary1;
//using System.Runtime.InteropServices;


public class Car2 : MonoBehaviour
{
    public Mesh mesh;
    public GameObject featureTrack;
    // Start is called before the first frame update

    public int updateCount = 0;

    public CPOChassis mPOChassis;

    float speed;
    float steering;
    int gear;
    Vector3 velocity = new Vector3();

    float BOOST_EFFECT = 3.5f;
    bool boostOn = false;

    bool ranTests = false;

    float f1 = 180.0f;
    float nextActionTime;
    const float gamefps = 100.0f;

  //  [DllImport("Phy2.dll", CallingConvention = CallingConvention.Cdecl)] public static extern int somecode(int g);


    void Start()
    {

       // Application.targetFrameRate = 60;

       // QualitySettings.vSyncCount = 1;

        if (ranTests == false)
        {
            TestRunner testRunner = new TestRunner();
            testRunner.RunAll();
            ranTests = true;
        }
        mPOChassis = new CPOChassis();
        mPOChassis.Init();
        mPOChassis.SetCar(this);
        mPOChassis.featureTrack = featureTrack;

        Vector3 ea = this.transform.eulerAngles;
        FMatrix m = new FMatrix(ea.x, ea.y, ea.z);

        Vector3 pos = this.transform.position;

        mPOChassis.SetOrientation(m);
        mPOChassis.SetPos(new FVector(pos.x, pos.y, pos.z));
        nextActionTime = Time.time;

        /*
        Fred f = new Fred();
        int result = f.doCode(3);

        int g = result;

        int gg = somecode(102);

        g = gg;
        */

    }

    // Update is called once per frame
    void Update()
    {
        // To try and keep it to original (updated called 20fps), only process 1/3 updates (i.e. update is called 60fps)

        if (Input.GetKeyDown("joystick 1 button " + 7))
        {
            boostOn = true;
        }

        if (Input.GetKeyUp("joystick 1 button " + 7))
        {
            boostOn = false;
        }


        if (Time.time < nextActionTime)
        {
            return;
        }
        nextActionTime += 1 / gamefps;
        // execute block of code here
        
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical2");

        Turn(h);
        if (v>0)
        {
            Accelerate(v);
        }
        else
        {
            Brake(-v);
        }

        Collider collider = featureTrack.GetComponent<MeshCollider>();

        RaycastHit cast;

        Vector3 thisPos = this.transform.position;

        Vector3 down = new Vector3(0, -1, 0);

        //if (PhysicsReady())
        Drive();

        // Copy data back from the Physics object to the Car Thing.

        FMatrix orientation = mPOChassis.GetOrientation();
        FVector localPosition = orientation.Multiply(mPOChassis.GetMeshOffset());
        FVector worldPosition = mPOChassis.GetPos().Add(localPosition);

        this.transform.position = new Vector3(worldPosition.X, worldPosition.Y, worldPosition.Z);
        CEulerAngles angles = new CEulerAngles(orientation);

        Quaternion q = new Quaternion();
        q.eulerAngles = new Vector3(angles.mPitch * (float)(180 / Math.PI), -angles.mYaw*(float)(180/Math.PI), -angles.mRoll * (float)(180 / Math.PI));
        this.transform.rotation = q;

        FVector chassisVelocity = PhysicsParameters.PHYSICS_TO_WORLD(mPOChassis.GetLinearVelocity()).Divide(gamefps);

        velocity = new Vector3(chassisVelocity.X, chassisVelocity.Y, chassisVelocity.Z);
        speed = mPOChassis.GetSpeedoSpeed() / 1000.0f * 3600.0f * (0.62137f); // JCL - is now MPH!!
        steering = 1.0f - (mPOChassis.GetSteeringWheelPos() / 2.0f + 0.5f);
        gear = mPOChassis.GetGear();
    }

    //******************************************************************************************
    void Drive()
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

            mPOChassis.SetTimeStep(200, (int)gamefps);

        // JCL - think....
        // this may need to happen per physics tick...
        SPOInputParams chassisParams = new SPOInputParams();


        chassisParams.mAnglesValid = false;
  
        chassisParams.mTrackYaw = 0.0f;
        chassisParams.mTrackPitch = 0.0f;
        chassisParams.mTrackRoll = 0.0f;

        chassisParams.mIsAI = false;

        //CalculateSlipStream();
        chassisParams.mSlipStream = 0.0f ;

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
    void Accelerate(float val)
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
    void Turn(float inValue)
    {
        if (inValue > 1.0f)
            inValue = 1.0f;
        else if (inValue < -1.0f)
            inValue = -1.0f;

     //   mTurn = inValue;
        mPOChassis.SetSteeringAngle(-inValue);
    }


    //******************************************************************************************
    void Brake(float val)
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
