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
using Assets.Scripts.CarAI;

public class Car2 : MonoBehaviour
{
    public GameObject featureTrack;
    public GameObject featureTrack2;
    public GameObject featureTrack3;

    // Start is called before the first frame update

    public int updateCount = 0;
    public bool isAI = false;

    private CarControlParameters lastPar = new CarControlParameters();
     CPOChassis mPOChassis;

    float speed;
    float steering;
    int gear;
    Vector3 velocity = new Vector3();

    float BOOST_EFFECT = 3.5f;
    bool boostButtonOn = false; // when controled by controller only

    bool ranTests = false;

    float nextActionTime;
    const float gamefps = 100.0f;

    CarAI carAI = null;

    public CarAI GetCarAI()
    {
        return carAI;
    }

    // for debug
    public void MoveToWaypoint(int index)
    {
        Track track = featureTrack.GetComponent<Track>();

        Route r = track.Route;

        Waypoint wp = r.Waypoints[index];

        mPOChassis.SetPos(new FVector(wp.position.x, wp.position.y+3, wp.position.z));

        Vector3 ea = wp.transform.eulerAngles;

        float yaw = -ea.y * 0.0174533f;
        float pitch = ea.x * 0.0174533f;
        float roll = ea.z * 0.0174533f;

        FMatrix m = new FMatrix(yaw, pitch, roll);
        mPOChassis.SetOrientation(m);
        mPOChassis.Reset();

        // if we have ai, update this as well
        if (carAI != null)
        {
            carAI.MoveToWaypoint(index);
        }
    }

    public float GetSpeed()
    {
        return speed;
    }
    public float GetSteering()
    {
        return steering;
    }
    public int GetGear()
    {
        return gear;
    }

    public CarControlParameters GetLastCarControlParameters()
    {
        return lastPar;
    }

    public CPOChassis GetChassis()
    {
        return mPOChassis;
    }



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
        mPOChassis.featureTrack2 = featureTrack2;
        mPOChassis.featureTrack3 = featureTrack3;

        Vector3 ea = this.transform.eulerAngles;

        // ea = pitch, yaw and roll

        float yaw = -ea.y * 0.0174533f;
        float pitch = ea.x * 0.0174533f;
        float roll = ea.z * 0.0174533f;

        FMatrix m = new FMatrix(yaw, pitch, roll);

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
        // flush buttons if player on every call to Update
        if (isAI == false)
        {

            if (Input.GetKeyDown("joystick 1 button " + 7))
            {
                boostButtonOn = true;
            }

            if (Input.GetKeyUp("joystick 1 button " + 7))
            {
                boostButtonOn = false;
            }
        }

        //
        // Limit numner times per second this is called
        //
        if (Time.time >= nextActionTime)
        {

            nextActionTime += 1 / gamefps;



            SPOInputParams chassisParams = new SPOInputParams();
            chassisParams.mAnglesValid = false;
            chassisParams.mTrackYaw = 0.0f;
            chassisParams.mTrackPitch = 0.0f;
            chassisParams.mTrackRoll = 0.0f;
            chassisParams.mIsAI = false;
            //CalculateSlipStream();
            chassisParams.mSlipStream = 0.0f;


            CarControlParameters par = null;

            //
            // All cars have a carai, for player this is used as an steering assist
            //

            if (carAI == null)
            {
                Track track = featureTrack.GetComponent<Track>();
                if (track)
                {
                    carAI = new CarAI(this, track.Route);
                }
            }

            if (isAI == true)
            {
                par = carAI.Update(this.transform, speed);
            }
            else
            {
                par = new CarControlParameters();

                //
                // Assume the player
                //

                par.steer = CrossPlatformInputManager.GetAxis("Horizontal");
                float v = CrossPlatformInputManager.GetAxis("Vertical2");

                if (v > 0)
                {
                    par.acceletation = v;
                }
                else
                {
                    par.brake = -v;
                }

                par.boost = boostButtonOn;


                //track assist 
                CarControlParameters tapar = this.carAI.Update(this.transform, speed);

                //chassisParams.mAnglesValid = true;
                //chassisParams.mTrackYaw = tapar.angleToDesiredPoint;
            }

            //   if (SCR_WORLD->GetRateOfChange(mSplinePosition, angles))
            //  {
            //      chassisParams.mYawChange = 0.0f;
            //    chassisParams.mPitchChange = 0.0f;
            //   }

            mPOChassis.SetInputParams(chassisParams);


            lastPar = par;

            Turn(par.steer);

            if (par.acceletation > 0)
            {
                Accelerate(par.acceletation, par.boost);
            }

            if (par.brake > 0)
            {
                Brake(par.brake);
            }

            //if (PhysicsReady())
            Drive();
        }


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

        Rigidbody rb  =  this.GetComponentInChildren<Rigidbody>();

        if (rb == null)
        {
            int ff = 3;
            ff++;
        }
        else
        {
            rb.MovePosition(this.transform.position);
            rb.MoveRotation(q);
        }

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

       

        mPOChassis.Process();

        //!JCL hm.....

        mPOChassis.SetDesiredThrottle(0);
        mPOChassis.SetDesiredBraking(0);
    }

    //******************************************************************************************
    void Accelerate(float val, bool boost)
    {
     //   if ((Ready()) && (!Wrecked()))
        {
            if (val > 1.0f)
                val = 1.0f;

           // if ((val > 0.0f) && (val < 1.0f))
             //   val = val;

            if (boost == true)
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
        mPOChassis.SetDesiredSteeringAngle(-inValue);
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
