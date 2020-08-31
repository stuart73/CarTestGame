using Assets.Scripts.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
	public enum EKeyPointType
	{
		KPT_COLLIDABLE,
		KPT_NON_COLLIDABLE,
		KPT_SUSPENSION,
	}

	public struct SKeyPoint
	{
		public FVector mPos;
		public EKeyPointType mType;
	};


	public class PhysicsObject
    {
		
		public PhysicsObject()
		{
			mPos = new FVector() ;
			mOrientation = new FMatrix() ;

			mLinearMomentum = new FVector();
			mAngularMomentum = new FVector();

			mMass = PhysicsParameters.PC_DEFAULT_MASS;
			mMomentOfInertia = new FMatrix(); ;
			mMomentOfInertiaInverse = new FMatrix(); ;

			//mCoefficientOfRestitution = PhysicsParameters.PC_DEFAULT_COEFFICIENT_OF_RESTITUTION;
			//mCoefficientOfFriction = PhysicsParameters.PC_DEFAULT_COEFFICIENT_OF_FRICTION;

			mTimeStep = 1 / 60.0f;
			mIterations = 20;

			mCachedImpactPointsValid = false;

			// debug
			mNumDebugCuboids = 0;
		}
				// Exposed Functions
		public void Init()
		{
			// nada

		}
		public void Process()
		{
			int c0;
			for (c0 = 0; c0 < mIterations; c0++)
				Model();
		}

		public virtual void Reset()
		{
			mLinearMomentum = new FVector();
			mAngularMomentum = new FVector();
		}

		public FVector GetPos()  
		{
			return PhysicsParameters.PHYSICS_TO_WORLD(mPos);
		}
		public FMatrix GetOrientation() 
		{
			return mOrientation;
		}

		public void SetPos( FVector pos) 
		{
			mPos = PhysicsParameters.WORLD_TO_PHYSICS(pos);
			mCachedImpactPointsValid = false; 
		}
		public void SetOrientation( FMatrix ori) 
		{ 
			mOrientation = ori; 
			mCachedImpactPointsValid = false; 
		}

		// Velocity
		public FVector GetLinearVelocity()
		{
			return mLinearMomentum.Divide(mMass);
		}
		public FVector GetAngularVelocity()
		{
			return GetRotatedMoIInverse().Multiply(mAngularMomentum);
		}
		public FMatrix GetAngularVelocityStar()
		{
			// see Barraf page D8
			FVector am = GetAngularVelocity();

			return new FMatrix(new FVector(0, -am.Z, am.Y), new FVector(am.Z, 0, -am.X), new FVector(-am.Y, am.X, 0));
		}
		public FVector GetPointVelocity( FVector pos)
		{
			return GetLinearVelocity().Add((GetAngularVelocity().Cross(pos)));
		}

		// Momentum
		public void SetLinearMomentum( FVector v) 
		{
			mLinearMomentum = v;
		}

		public void SetAngularMomentum( FVector v) 
		{ 
			mAngularMomentum = v;
		}

		// Momentum
		public void SetLinearVelocity( FVector v)
		{ 
			mLinearMomentum = v.Multiply(mMass); 
		}


		// Force & Impulse
		public void ApplyImpulse(FVector pos, FVector impulse, int point, int type)
		{
			// assumes pos is in worldspace
			FVector impact_offset = pos.Minus(mPos);

			// apply force
			mLinearMomentum.AddInPlace(impulse);

			// torque 
			FVector torque = impact_offset.Cross(impulse);

			// apply torque
			mAngularMomentum.AddInPlace(torque);

			// and flag for debug
			DebugNotifyImpulse(point, type, impulse);

		}
		public void ApplyForce(FVector pos, FVector force, int point, int type)
		{
			ApplyImpulse(pos, force.Multiply(mTimeStep), point, type);
		}

		// Timestep
		public virtual void SetTimeStep(int ticks_per_second, int calls_per_second)
		{
			mTimeStep = 1.0f / ((float)ticks_per_second);
			mIterations = ticks_per_second / calls_per_second;
		}

		public float GetTimeStep()
		{
			return mTimeStep;
		}

		// Collision
		//void SetTrack(CSCRTrack* track)
		//{
		//	mTrack = track;
		//}
		//void SetLandscape(CLandscape* l) { mLandscape = l; };

		// Debug
		public virtual void RenderDebug()
		{

		}

		public virtual void Model()
		{
			//Rigidbody r = new Rigidbody();

			// Apply Gravity
			ApplyForce(mPos, new FVector(0, -Gravity() * mMass, 0), 8, 0);

			// change in position
			mPos.AddInPlace(GetLinearVelocity().Multiply(mTimeStep));

			// change in rotation
			mOrientation.AddInPlace((GetAngularVelocityStar().Multiply(mTimeStep)).Multiply(mOrientation));
			mOrientation.Normalise();

			HandleCollision();

		}

		// Property functions
		public float Gravity()
		{
			return PhysicsParameters.PC_GRAVITY;
		}

		public FMatrix GetRotatedMoI()
		{
			FMatrix inv_ori = mOrientation.Inverse();

			FMatrix step1 = mMomentOfInertia.Multiply(inv_ori);
			return mOrientation.Multiply(step1);
		}
		public FMatrix GetRotatedMoIInverse()
		{
			FMatrix inv_ori = mOrientation.Inverse();
			FMatrix step1 = mMomentOfInertiaInverse.Multiply(inv_ori);

			return mOrientation.Multiply(step1);
		}

		public int WhatDoICollideWith()
		{
			if (testLandscape!=null)
            {
				return 3;

            }

			return 2;

			/*
			if (SCR_WORLD->TrackCollision())
				return 2;
			else
				return 1;
			*/

		}

		//***************************************************************
		FVector remove_component_in_direction( FVector v,  FVector dir)
		{
			// assume dir is normalised
			return (v.Minus((dir.Multiply((v.DotProduct(dir))))));
		}


		public virtual bool ShouldIFixInterpenetration(int num)
		{ 
			// SCR SRG always true from c++
			return true;
		}

		public bool LineIntersectionTest(GameObject forObject, FVector start, FVector end, FVector ip, FVector normal)
		{
			if (forObject)
			{
				Collider collider = forObject.GetComponent<MeshCollider>();

				RaycastHit cast;

				Vector3 startPos = new Vector3(start.X, start.Y, start.Z);

				FVector dir = end.Minus(start);
				dir.Normalise();

				Vector3 direction = new Vector3(dir.X, dir.Y, dir.Z);

				Ray myRay = new Ray(startPos, direction);
				bool result = collider.Raycast(myRay, out cast, 10.0f);

				if (result == true)
				{
					Vector3 ip2 = myRay.GetPoint(cast.distance);
					ip.X = ip2.x; ip.Y = ip2.y; ip.Z = ip2.z;
					normal.X = cast.normal.x; normal.Y = cast.normal.y; normal.Z = cast.normal.z;

					return true;
				}
			}
			return false;
		}

		public bool LineIntersectionTest(FVector start, FVector end, FVector ip, FVector normal)
		{
			bool result = LineIntersectionTest(featureTrack3, start, end, ip, normal);
			if (result == true) return true;

			result = LineIntersectionTest(featureTrack2, start, end, ip, normal);
			if (result == true) return true;

			result = LineIntersectionTest(featureTrack, start, end, ip, normal);

			return result;
		}


		public virtual void HandleCollision()
		{
			// Fairly temporary collision code for now - using an array of points

			if (mCachedImpactPointsValid)
			{
				SKeyPoint[] impact_points= new SKeyPoint[100];
				int num_points = 0;
				int c0 = 0;

				// Get the points
				FillOutKeyPoints(impact_points, out num_points);

				// rotate the points into world space
				for (c0 = 0; c0 < num_points; c0++)
					impact_points[c0].mPos = mOrientation.Multiply(impact_points[c0].mPos);

				// fixup value for collision with heightfield (not polygonal track)
				float min_dist_to_move_up = 0.0f;

				for (c0 = 0; c0 < num_points; c0++)
				{
					if (impact_points[c0].mType == EKeyPointType.KPT_NON_COLLIDABLE)
						continue;  // don't collide with this point...

					int point = c0;

					switch (WhatDoICollideWith())
					{
						case 2:
							{
								//************************************
								// collide with mesh
								FVector np = impact_points[point].mPos.Add(mPos);
								FVector op = mCachedImpactPoints[point].mPos;

								//back it up a bit
								//					op = op + ((op - np) * 2.f);

								/*					FVector delta = op - np;
													delta.Normalise();
													op += delta * 1.f;*/

								// ok something different again!
								op = mPos;

								FVector ip = new FVector(op.X, 0, op.Z);
								FVector normal = new FVector(0, 1, 0);

								bool hit = LineIntersectionTest(op, np, ip, normal);

								// need to backface now...
								//					if (normal * delta < 0)
								//						break;

								if (hit)
								{
									//ip = PhysicsParameters.WORLD_TO_PHYSICS(ip);

									/*
#if 0	// rigid body

						// prevent car from moving through landscape
						if (ShouldIFixInterpenetration(point))
						{
							FVector	penetration = ip - np;
							FVector correction = normal * (normal * penetration) + (normal * 0.001f);
							mPos += correction;
						}

						// and do the rigid body thing
						float	impulse = ProcessCollisionWithUnmovableObject(impact_points[point].mPos, normal, point);
						*/
//#else  // ground = spring
									float force = ProcessCollisionWithUnmovableObject2(impact_points[point].mPos, normal, point, np.Minus(ip), (impact_points[c0].mType == EKeyPointType.KPT_SUSPENSION));
									float impulse = force * mTimeStep;
//#endif
									float damage = impulse / PhysicsParameters.PC_DAMAGE_SCALAR;

									if (damage > 0.01f)
										DeclareDamage(damage);
								}
								break;
							}

						case 1:
							{
								// SCR SRG NOT USED
								/*
								//************************************
								// collide with Landscape
								FVector pt = impact_points[point].mPos + mPos;
								float hgt = mLandscape->GetHeight(pt.X, pt.Z);

								if (pt.Y < hgt)
								{
									// Hit Ground

									// Move up?
									if (ShouldIFixInterpenetration(point))
										if (pt.Y - hgt < min_dist_to_move_up)
											min_dist_to_move_up = pt.Y - hgt;

									// guess normal...
									FVector normal = -FVector(1.f, mLandscape->GetHeight(pt.X + 1.f, pt.Z) - hgt, 0) ^
													FVector(0.f, mLandscape->GetHeight(pt.X, pt.Z + 1.f) - hgt, 1.f);
									normal.Normalise();

									// Apply Force
									ProcessCollisionWithUnmovableObject(impact_points[point].mPos, normal, point);
								}
								*/
							}
							break;

						case 0:
							{
								// SCR SRG NOT USED

								/*
								//************************************
								// collide with notional ground plane
								FVector pt = impact_points[point].mPos + mPos;

								if (pt.Y < 0)
								{
									// Hit Ground

									// Move up?
									if (ShouldIFixInterpenetration(point))
										if (pt.Y < min_dist_to_move_up)
											min_dist_to_move_up = pt.Y;

									// Apply Force
									ProcessCollisionWithUnmovableObject(impact_points[point].mPos, FVector(0, 1.f, 0), point);
								}
								*/

							};
							break;
					};
				}

				// and move to avoid clipping
				mPos.Y -= min_dist_to_move_up;
			}

			// cache points for next time

			int num_points2 = 0;
			FillOutKeyPoints(mCachedImpactPoints, out num_points2);

			// rotate the points into world space & add position
			int c02 = 0;

			for (c02 = 0; c02 < num_points2; c02++)
				mCachedImpactPoints[c02].mPos = mOrientation.Multiply(mCachedImpactPoints[c02].mPos).Add(mPos);


			mCachedImpactPointsValid = true;
		}
		public virtual void FillOutKeyPoints(SKeyPoint[] points, out int num) 
		{
			// overrided
			num = 0; 
		}

		public virtual float ProcessCollisionWithUnmovableObject( FVector pos,  FVector normal, int collision_point)
		{
			return 1;
		}
		public virtual float ProcessCollisionWithUnmovableObject2( FVector pos,  FVector normal, int collision_point,  FVector intersection, bool is_suspension)
		{
			// OK - completely different take on this than the above code.
			// Assume that the ground itself is a damped spring.

			// How deep in are we?
			float dist = (normal.Negative()).DotProduct(intersection);  // hope normal is normalised!

			//	ASSERT (dist >= 0.f);	// better be!
			if (dist < 0.0f)
				return 0.0f;  // can happen due to Glenn's approximate track collision....

			// what's the point velocity at this point?
			FVector point_velocity = GetPointVelocity(pos);

			// ...in the direction of the collision/
			float speed = (normal.Negative()).DotProduct(point_velocity);

			// and a simple damped thing
			float BODY_FORCE = 1000000.0f;
			float BODY_DAMPING = 20000.0f;

			float force_mag = (dist * BODY_FORCE) + (speed * BODY_DAMPING);
			FVector force = normal.Multiply(force_mag);

			// cancel out component if suspension
			if (is_suspension)
				force = remove_component_in_direction(force, mOrientation.Multiply( new FVector(0.0f, 1.0f, 0.0f)));

			// and apply
			ApplyForce(pos.Add(mPos), force, collision_point, 0);

			// OK - think about friction
			if (!is_suspension)
			{
				//	#define	COF		0.6f
				float COF = 0.3f;
				//	FVector	lateral_velocity = (point_velocity - (normal * (point_velocity * normal)));
				FVector lateral_velocity = remove_component_in_direction(point_velocity, normal);
				FVector friction_direction = lateral_velocity.Negative();
				friction_direction.Normalise();

				//hm...
				float friction_mag = COF * force_mag;
				FVector friction = friction_direction.Multiply(friction_mag);

				// cancel out component if suspension
				if (is_suspension)
					force = remove_component_in_direction(friction, mOrientation.Multiply(new FVector(0.0f, 1.0f, 0.0f)));

				// and apply
				ApplyForce(pos.Add(mPos), friction, collision_point, 3);
			}

			return force_mag;
		}
		public virtual void DeclareDamage(float amount)
		{ 
			// overridden
		}

		// Helper Functions
		public float CalculateRigidBodyImpulse(float mass,  FMatrix inv_moi,  FVector point_velocity,  FVector pos,  FVector normal, float coefficient_of_restitution)
		{
			// From Baraff et al.
			float j = 0;

			if (normal.Magnitude() > 0.001f)
			{
				if (point_velocity.DotProduct(normal) < 0)
				{
					FVector step1 = point_velocity.Multiply(-(1.0f + coefficient_of_restitution));
					float normal_component_of_desired_change = step1.DotProduct(normal);

					FVector change_in_linear_velocity_for_unit_force = normal.Divide(mass);
					FVector change_in_av_for_unit_force = inv_moi.Multiply(normal.Cross(pos).Negative());
					FVector change_in_velocity_by_rotation_for_unit_force = change_in_av_for_unit_force.Cross(pos);
					FVector total_velocity_change_for_unit_force = change_in_linear_velocity_for_unit_force.Add(change_in_velocity_by_rotation_for_unit_force);
					float normal_component_of_change_for_unit_force = total_velocity_change_for_unit_force.DotProduct(normal);

					j = normal_component_of_desired_change / normal_component_of_change_for_unit_force;
				}
			}

			if (j < -0.001f)
			{
				int messedup = 1;
			}

			return j;

		}
		public FMatrix MakeCuboidMOITensor(float mass, FVector size, FVector offset)
		{
			return new FMatrix();
		}

		// State
		protected FVector mPos;
		protected FMatrix mOrientation;

		FVector mLinearMomentum;
		public FVector mAngularMomentum;

		public float mMass;
		public FMatrix mMomentOfInertia;
		public FMatrix mMomentOfInertiaInverse;

		// properties
		// float mCoefficientOfRestitution;
		// float mCoefficientOfFriction;

		public float mTimeStep;
		int mIterations;

		public GameObject featureTrack;
		public GameObject featureTrack2;
		public GameObject featureTrack3;


		public TestLandscape testLandscape;


		// Collision
		//CSCRTrack* mTrack;
		//CLandscape* mLandscape;

		// Debug
		//CDebugCuboid2* mDebugCuboids[MAX_DEBUG_CUBOIDS];
		int mNumDebugCuboids;

		void AddDebugCuboid( FVector pos,  FVector size)
		{

		}
		public virtual void SetupDebugCuboids()
		{

		}

		public virtual FVector GetDebugCuboidPosOffset(int num) 
		{ 
			return new FVector(0, 0, 0); 
		}
		public virtual FMatrix GetDebugCuboidOriOffset(int num)
		{
			return new FMatrix();
		}

		public virtual void DebugNotifyImpulse(int point, int type, FVector impulse) 
		{ 

		}

		SKeyPoint[] mCachedImpactPoints = new SKeyPoint[100];
		bool mCachedImpactPointsValid;
    }
}
