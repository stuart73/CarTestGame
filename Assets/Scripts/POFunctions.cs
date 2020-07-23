using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class POFunctions
    {
		//******************************************************************************************
		static public float LateralForceFromSlipAngleSlipRatioAndLoad(float slip_angle, float slip_ratio, float load)
		{
			return -Pacejka.Flateral_combined(slip_angle * 180.0f / (float)Math.PI, slip_ratio * 100.0f, load / 1000.0f);
		}

		//******************************************************************************************
		static public float LongitudinalForceFromSlipAngleSlipRatioAndLoad(float slip_angle, float slip_ratio, float load)
		{
			return Pacejka.Flongitudinal_combined(slip_angle * 180.0f / (float)Math.PI, slip_ratio * 100.0f, load / 1000.0f);
		}
		//******************************************************************************************
		static public float LongitudinalForceFromSlipAngleSlipRatioAndLoad2(float slip_ratio, float slip_angle, float load)
		{
			return LongitudinalForceFromSlipAngleSlipRatioAndLoad(slip_angle, slip_ratio, load);
		}

		//******************************************************************************************
		static public float DragFromSpeed(float speed)
		{
			return speed * speed * PhysicsParameters.PC_DRAG;
		}

		static public  float Range(float tr, float bottom, float top)
		{
			return (bottom + (tr * (top - bottom)));
		}

		static public  float RangeTransition(float tr, float start, float end)
		{
			tr -= start;
			tr /= (end - start);
			if (tr < 0.0f) tr = 0.0f;
			if (tr > 1.0f) tr = 1.0f;
			return tr;
		}

		static public void Clamp(ref float v, float min, float max)
		{
			if (v < min) v = min;
			if (v > max) v = max;
		}


		//******************************************************************************************
		static public float MaxEngineTorquePercentageFromRPM(float rpm)
		{
			float torque_percentage;

			/*	if (rpm < PC_MAX_RPM1)
					torque_percentage = 1.f;
				else
				{
					torque_percentage = (PC_MAX_RPM2 - rpm) / (PC_MAX_RPM2 - PC_MAX_RPM1);
					Clamp(torque_percentage, 0.f, 1.f);
				}	*/

			if (rpm < PhysicsParameters.PC_RPM_PRE_PEAK)
				torque_percentage = PhysicsParameters.PC_RPM_CUTOFF;
			else if (rpm < PhysicsParameters.PC_RPM_PEAK)
				torque_percentage = Range(RangeTransition(rpm, PhysicsParameters.PC_RPM_PRE_PEAK, PhysicsParameters.PC_RPM_PEAK), PhysicsParameters.PC_RPM_CUTOFF, 1.0f);
			else if (rpm < PhysicsParameters.PC_RPM_LIMIT)
				torque_percentage = Range(RangeTransition(rpm, PhysicsParameters.PC_RPM_PEAK, PhysicsParameters.PC_RPM_LIMIT), 1.0f, PhysicsParameters.PC_RPM_CUTOFF);
			else
				torque_percentage = Range(RangeTransition(rpm, PhysicsParameters.PC_RPM_LIMIT, PhysicsParameters.PC_RPM_MAX), PhysicsParameters.PC_RPM_CUTOFF, 0.0f);

			return torque_percentage;
		}

		//******************************************************************************************
		static public float YawCorrectingTorque(float yaw_offset, float omega)
		{
			// well, this could be anything, really

			// start by trying just a linear offset, like a spring, with damping to prevent oscillation

			Clamp(ref yaw_offset, -0.3f, 0.3f); // prevent silly values

			return (yaw_offset * PhysicsParameters.PC_YAW_CORRECTING_TORQUE) - (omega * PhysicsParameters.PC_YAW_CORRECTING_DAMPING);
		}

		//******************************************************************************************
		static public float PitchCorrectingTorque(float yaw_offset, float omega)
		{
			// well, this could be anything, really

			// start by trying just a linear offset, like a spring, with damping to prevent oscillation

			Clamp(ref yaw_offset, -0.3f, 0.3f); // prevent silly values

			return (yaw_offset * PhysicsParameters.PC_PITCH_CORRECTING_TORQUE) - (omega * PhysicsParameters.PC_PITCH_CORRECTING_DAMPING);
		}


		//******************************************************************************************
		static public float DampedSuspensionForce(float suspension_offset, float suspension_speed)
		{
			// damped spring mass system

			// JCL - this almost certainly needs more thought...
			// 1) as the suspension can move very fast, this force can get large, which affects
			//    the load on the tyre, and its grip
			// 2) I need to think if I need to integrating this so that it generates similar results
			//    for different time steps

			float force = (suspension_offset * PhysicsParameters.PC_SUSPENSION_FORCE_1);
			float damping = (suspension_speed * PhysicsParameters.PC_SUSPENSION_DAMPING_FORCE_1);

			if (suspension_offset > PhysicsParameters.PC_MAX_SUSPENSION_TRAVEL)
			{
				force += ((suspension_offset - PhysicsParameters.PC_MAX_SUSPENSION_TRAVEL) * PhysicsParameters.PC_SUSPENSION_FORCE_2);
				damping += (suspension_speed * PhysicsParameters.PC_SUSPENSION_DAMPING_FORCE_2);
			}

			/*	if (suspension_offset > PC_MAX_SUSPENSION_TRAVEL + PC_SUSPENSION_3_CUT_POINT)
				{
					force	+= ((suspension_offset - PC_MAX_SUSPENSION_TRAVEL - PC_SUSPENSION_3_CUT_POINT) * PC_SUSPENSION_FORCE_3);
					damping	+= (suspension_speed * PC_SUSPENSION_DAMPING_FORCE_3);
				}*/

			return force - damping;
		}


		//******************************************************************************************
		static public float Line(float f)
		{
			// silly function necessary for graphs

			return f;
		}

	}
}
