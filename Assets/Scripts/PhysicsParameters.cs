using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class PhysicsParameters
    {

		static public float WORLD_TO_PHYSICS(float input) { return input; }
		static public FVector WORLD_TO_PHYSICS(FVector input)
		{ 
			return new FVector(input.X, input.Y, input.Z); 
		}

		static public FVector PHYSICS_TO_WORLD(FVector input)
		{
			return new FVector(input.X, input.Y, input.Z);
		}


		//***************************************************************
		//** From CPhysicsObject
		static public float PC_GRAVITY = 9.81f;
		static public float PC_DEFAULT_COEFFICIENT_OF_RESTITUTION = 0.25f;
		static public float PC_DEFAULT_COEFFICIENT_OF_FRICTION = 0.75f;
		//float		PC_DEFAULT_MASS							= 1000.0f;
		static public float PC_DEFAULT_MASS = 1400.0f;
		static public float PC_DAMAGE_SCALAR = 300000.0f;

		//***************************************************************
		//** From CPOChassis
		static public float PC_MAX_SUSPENSION_TRAVEL = 0.2f;
		static public float PC_MAX_TURNING_CIRCLE = 0.5f; //0.8f;
		static public float PC_TURNING_CIRCLE_HALF = ((70.0f /*kph */ * 1000.0f /*metres/h*/ / 3600.0f * 10.0f) * 2.0f) /10.0f;
		static public float PC_STEERING_SPEED = 0.1f;
		static public float PC_STEERING_ASSIST = 0.2f;
		static public float PC_STEERING_ASSIST_MIN_SPEED = 27.0f;
		static public float PC_STEERING_ASSIST_MAX_SPEED = 45.0f;
		static public float PC_DOWNFORCE_BALANCE = 0.56f;
		static public float PC_DOWNFORCE = 300.0f; // unrelaistic physics // 0.0f;       //10.0f * 60.0f;
		static public int PC_DRIVE_TYPE = 2; // == DT_4WD

		static public int PC_GEARS_NUM = 5;
		static public float PC_GEAR_RATIO_1 = 0.0897f;
		static public float PC_GEAR_RATIO_2 = 0.1435f;
		static public float PC_GEAR_RATIO_3 = 0.215f;
		static public float PC_GEAR_RATIO_4 = 0.287f;
		static public float PC_GEAR_RATIO_5 = 0.359f;
		static public float PC_GEAR_RATIO_6 = 0.4f;
		static public float PC_GEAR_RATIO_7 = 0.45f;
		static public float PC_GEAR_RATIO_8 = 0.5f;
		static public float PC_GEAR_RATIO_9 = 0.55f;
		static public float PC_GEAR_RATIO_10 = 0.6f;
		static public float PC_GEAR_CHANGE_TIME = 0.2f;
		static public float PC_GEAR_CHANGE_UP_RPM = 5500.0f;
		static public float PC_GEAR_CHANGE_DOWN_RPM = 5500.0f;
		static public float PC_MOI_X = 3500.0f;
		static public float PC_MOI_Y = 3776.0f;
		static public float PC_MOI_Z = 950.0f;
		static public float PC_TRACTION_BIAS_REAR = 1.05f;

		//******************************************************************************************
		//** From CPOFunctions
		static public float PC_RPM_PRE_PEAK = 4000.0f;
		static public float PC_RPM_PEAK = 5000.0f;
		static public float PC_RPM_LIMIT = 6000.0f;
		static public float PC_RPM_MAX = 8000.0f;
		static public float PC_RPM_CUTOFF = 0.8f;
		static public float PC_DRAG = 0.82f;
		static public float PC_MIN_TORQUE_PERCENTAGE = 0.4f;
		static public float PC_TRACTION_FALLOFF = 100000.0f;
		static public float PC_YAW_CORRECTING_TORQUE = 200.0f;
		static public float PC_YAW_CORRECTING_DAMPING = 100.0f;
		static public float PC_PITCH_CORRECTING_TORQUE = 10000.0f;
		static public float PC_PITCH_CORRECTING_DAMPING = 4000.0f;
		static public float PC_SUSPENSION_FORCE_1 = 36000.0f;
		static public float PC_SUSPENSION_DAMPING_FORCE_1 = 3000.0f;
		static public float PC_SUSPENSION_FORCE_2 = 100000.0f;
		static public float PC_SUSPENSION_DAMPING_FORCE_2 = 7000.0f;  // 2000.0f
		static public float PC_SUSPENSION_FORCE_3 = 2000000.0f;
		static public float PC_SUSPENSION_DAMPING_FORCE_3 = 10000.0f;
		static public float PC_SUSPENSION_3_CUT_POINT = 0.05f;

		//***************************************************************
		//** From CWheel
		static public float PC_ENGINE_MAX_TORQUE = 570.0f;
		static public float PC_BRAKING_TORQUE = 4000.0f;
		static public float PC_BRAKE_BALANCE = 0.57f;


		//***************************************************************
		//***************************************************************
		void INIT_PHYSICS_PARAMETERS()
		{
			//***************************************************************
			//** Set up base Nodes

			/*
			CSTRING NodeGears = CSTRING("Gearbox");
			CSTRING NodeSuspension = CSTRING("Suspension");
			CSTRING NodeEngine = CSTRING("Engine");
			CSTRING NodeSteering = CSTRING("Steering");
			CSTRING NodeAero = CSTRING("Aerodynamics");
			CSTRING NodeTraction = CSTRING("Traction");
			CSTRING NodeMaterial = CSTRING("Materials");
			CSTRING NodeMoI = CSTRING("Moments Of Inertia");
			CSTRING NodeRatios = CSTRING("Gear Ratios");
			CSTRING NodeChanger = CSTRING("Auto Changer");

			PHYSICS_CONSTANTS.RegisterNode(NodeGears);
			PHYSICS_CONSTANTS.RegisterNode(NodeSuspension);
			PHYSICS_CONSTANTS.RegisterNode(NodeEngine);
			PHYSICS_CONSTANTS.RegisterNode(NodeSteering);
			PHYSICS_CONSTANTS.RegisterNode(NodeAero);
			PHYSICS_CONSTANTS.RegisterNode(NodeTraction);
			PHYSICS_CONSTANTS.RegisterNode(NodeMaterial);
			PHYSICS_CONSTANTS.RegisterNode(NodeMoI, NodeMaterial);
			PHYSICS_CONSTANTS.RegisterNode(NodeRatios, NodeGears);
			PHYSICS_CONSTANTS.RegisterNode(NodeChanger, NodeGears);

			//***************************************************************
			//** From CPhysicsObject

			CSTRING StrGravity = CSTRING("Gravity");
			CSTRING StrDefCoRest = CSTRING("Default Coefficient of Restitution");
			CSTRING StrDefCoFri = CSTRING("*Default Coefficient of Friction");
			CSTRING StrDefMass = CSTRING("*Default Mass");


			PHYSICS_CONSTANTS.RegisterVariable(StrGravity, &PC_GRAVITY, 0.1f);
			PHYSICS_CONSTANTS.RegisterVariable(StrDefCoRest, &PC_DEFAULT_COEFFICIENT_OF_RESTITUTION, 0.01f, 0.0f, 2.0f, NodeMaterial);
			PHYSICS_CONSTANTS.RegisterVariable(StrDefCoFri, &PC_DEFAULT_COEFFICIENT_OF_FRICTION, 0.01f, 0.0f, 2.0f, NodeMaterial);
			PHYSICS_CONSTANTS.RegisterVariable(StrDefMass, &PC_DEFAULT_MASS, 10.0f, 0.0f, 20000.0f, NodeMaterial);

			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Damage Scalar (inverted)"), &PC_DAMAGE_SCALAR, 10000000.0f, 0, 20000.0f);

			//****************************************************
			//** From CPOChassis
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Suspension Travel"), &PC_MAX_SUSPENSION_TRAVEL, 0.01f, 0, 10.0f, NodeSuspension);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Max Turning Circle"), &PC_MAX_TURNING_CIRCLE, 0.01f, 0, 2.0f, NodeSteering);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Turning Circle Half Speed"), &PC_TURNING_CIRCLE_HALF, 10.0f, 1.0f, 200.0f, NodeSteering);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Steering Speed"), &PC_STEERING_SPEED, 0.01f, 0, 1.0f, NodeSteering);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Steering Assist"), &PC_STEERING_ASSIST, 0.05f, 0, 1.5f, NodeSteering);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Steering Assist Min Speed"), &PC_STEERING_ASSIST_MIN_SPEED, 200.0f, 0, 2.0f, NodeSteering);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Steering Assist Max Speed"), &PC_STEERING_ASSIST_MAX_SPEED, 200.0f, 0, 2.0f, NodeSteering);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Downforce Balance"), &PC_DOWNFORCE_BALANCE, 0.01f, 0.0f, 1.0f, NodeAero);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Added Downforce"), &PC_DOWNFORCE, 20.0f, 0, 10000.0f, NodeAero);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Drive Type"), (SINT*)(&PC_DRIVE_TYPE), 1, 0, 2, NodeEngine, "Front|Rear|4WD");

			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Gear Ratio 10"), &PC_GEAR_RATIO_10, 0.02f, 0, 10.0f, NodeRatios);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Gear Ratio 9"), &PC_GEAR_RATIO_9, 0.02f, 0, 10.0f, NodeRatios);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Gear Ratio 8"), &PC_GEAR_RATIO_8, 0.02f, 0, 10.0f, NodeRatios);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Gear Ratio 7"), &PC_GEAR_RATIO_7, 0.02f, 0, 10.0f, NodeRatios);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Gear Ratio 6"), &PC_GEAR_RATIO_6, 0.02f, 0, 10.0f, NodeRatios);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Gear Ratio 5"), &PC_GEAR_RATIO_5, 0.02f, 0, 10.0f, NodeRatios);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Gear Ratio 4"), &PC_GEAR_RATIO_4, 0.02f, 0, 10.0f, NodeRatios);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Gear Ratio 3"), &PC_GEAR_RATIO_3, 0.02f, 0, 10.0f, NodeRatios);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Gear Ratio 2"), &PC_GEAR_RATIO_2, 0.02f, 0, 10.0f, NodeRatios);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Gear Ratio 1"), &PC_GEAR_RATIO_1, 0.02f, 0, 10.0f, NodeRatios);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Num Gears"), &PC_GEARS_NUM, 1, 1, 10, NodeRatios);

			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Gear Change Time"), &PC_GEAR_CHANGE_TIME, 0.02f, 0, 1.0f, NodeChanger);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Gear Change Up RPM"), &PC_GEAR_CHANGE_UP_RPM, 100.0f, 0, 10000.0f, NodeChanger);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Gear Change Down RPM"), &PC_GEAR_CHANGE_DOWN_RPM, 100.0f, 0, 10000.0f, NodeChanger);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("MoI - X"), &PC_MOI_X, 100.0f, 0.0f, 10000.0f, NodeMoI);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("MoI - Y"), &PC_MOI_Y, 100.0f, 0.0f, 10000.0f, NodeMoI);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("MoI - Z"), &PC_MOI_Z, 100.0f, 0.0f, 10000.0f, NodeMoI);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Traction Rear Bias"), &PC_TRACTION_BIAS_REAR, 0.005f, 0.0f, 2.0f, NodeTraction);

			//****************************************************
			//** From CPOFunctions
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("RPM - Peak"), &PC_RPM_PEAK, 100.0f, 0, 10000.0f, NodeEngine);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("RPM - Limit"), &PC_RPM_LIMIT, 100.0f, 0, 10000.0f, NodeEngine);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("RPM - Max"), &PC_RPM_MAX, 100.0f, 0, 10000.0f, NodeEngine);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("RPM - Cutoff %age"), &PC_RPM_CUTOFF, 0.02f, 0, 10000.0f, NodeEngine);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Drag"), &PC_DRAG, 0.02f, 0, 10.0f, NodeAero);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Min Torque Percentage"), &PC_MIN_TORQUE_PERCENTAGE, 0.05f, 0.0f, 0.95f, NodeTraction);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Traction Falloff"), &PC_TRACTION_FALLOFF, 10000.0f, 0, 1000000.0f, NodeTraction);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Yaw Correcting Torque"), &PC_YAW_CORRECTING_TORQUE, 25.0f, 0, 100000.0f, NodeAero);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Yaw Correcting Damping"), &PC_YAW_CORRECTING_DAMPING, 25.0f, 0, 100000.0f, NodeAero);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Pitch Correcting Torque"), &PC_PITCH_CORRECTING_TORQUE, 1000.0f, 0, 100000.0f, NodeAero);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Pitch Correcting Damping"), &PC_PITCH_CORRECTING_DAMPING, 1000.0f, 0, 100000.0f, NodeAero);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Suspension Force 1 "), &PC_SUSPENSION_FORCE_1, 2000.0f, 0, 1000000.0f, NodeSuspension);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Suspension Damping Force 1"), &PC_SUSPENSION_DAMPING_FORCE_1, 1000.0f, 0, 1000000.0f, NodeSuspension);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Suspension Force 2"), &PC_SUSPENSION_FORCE_2, 10000.0f, 0, 1000000.0f, NodeSuspension);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Suspension Damping Force 2"), &PC_SUSPENSION_DAMPING_FORCE_2, 2000.0f, 0, 1000000.0f, NodeSuspension);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Suspension Force 3"), &PC_SUSPENSION_FORCE_3, 50000.0f, 0, 1000000.0f, NodeSuspension);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Suspension Damping Force 3"), &PC_SUSPENSION_DAMPING_FORCE_3, 5000.0f, 0, 1000000.0f, NodeSuspension);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Suspension 3 cut-in point"), &PC_SUSPENSION_3_CUT_POINT, 0.005f, 0, 1000000.0f, NodeSuspension);

			//****************************************************
			//** From CWheel
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Engine Max Torque"), &PC_ENGINE_MAX_TORQUE, 100.0f, 0, 10000.0f, NodeEngine);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Braking Torque"), &PC_BRAKING_TORQUE, 100.0f, 0, 10000.0f, NodeEngine);
			PHYSICS_CONSTANTS.RegisterVariable(CSTRING("Brake Balance"), &PC_BRAKE_BALANCE, 0.02f, 0, 1.0f, NodeEngine);

			//****************************************************
			//****************************************************
			//****************************************************
			//**
			//** Hacks to get a model that is drivable until the full thing is balanced

// ifdef UNREALISTIC_PHYSICS

			// whack up the MoI
			//	PC_MOI_X	= 1000.0f * MOI_SCALE;
			//	PC_MOI_Y	= 1200.0f * MOI_SCALE;
			//	PC_MOI_Z	=  200.0f * MOI_SCALE;

			// add a bit of downforce
			PC_DOWNFORCE = 300.0f;

//endif

	*/

		}

	}
}
