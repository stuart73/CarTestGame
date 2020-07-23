using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class POCollisionCuboid
	{

		//***************************************************************
		FVector vmult(FVector a, FVector b)
		{
			return new FVector(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
		}

		//***************************************************************

		public FVector mPos;
		public FVector mSize;
		public float mWeight;

		public void AddPoints(SKeyPoint[] points, ref int n, bool collidable )// = TRUE)
		{
			FVector[] o = new FVector[8] 
			{
				new FVector(-1, -1, -1),
				new FVector(-1, -1,  1),
				new FVector( 1, -1, -1),
				new FVector( 1, -1,  1),
				new FVector(-1,  1, -1),
				new FVector(-1,  1,  1),
				new FVector( 1,  1, -1),
				new FVector( 1,  1,  1)
			};


			EKeyPointType collide_type;

//#if PM_COLLISION_TYPE > PMCT_PARTIAL
			if (collidable)
				collide_type = EKeyPointType.KPT_COLLIDABLE;
			else
//#endif
			collide_type = EKeyPointType.KPT_NON_COLLIDABLE;

			// extra at front & back for bullbars
			FVector foo;

			foo = new FVector(0, 1.0f, 1.1f);
			points[n].mPos = mPos.Add((vmult(mSize, foo).Multiply(0.5f))); points[n++].mType = collide_type;
			foo = new FVector(0, 1.0f, -1.1f);
			points[n].mPos = mPos.Add((vmult(mSize, foo).Multiply(0.5f))); points[n++].mType = collide_type;

			// top only....
			/*		points[n].mPos = mPos + vmult(mSize, o[0]) * 0.5f;  points[n++].mType = collide_type;
					points[n].mPos = mPos + vmult(mSize, o[1]) * 0.5f;  points[n++].mType = collide_type;
					points[n].mPos = mPos + vmult(mSize, o[2]) * 0.5f;  points[n++].mType = collide_type;
					points[n].mPos = mPos + vmult(mSize, o[3]) * 0.5f;  points[n++].mType = collide_type;
			*/
			points[n].mPos = mPos.Add((vmult(mSize, o[4]).Multiply(0.5f))); points[n++].mType = collide_type;
			points[n].mPos = mPos.Add((vmult(mSize, o[5]).Multiply(0.5f))); points[n++].mType = collide_type;
			points[n].mPos = mPos.Add((vmult(mSize, o[6]).Multiply(0.5f))); points[n++].mType = collide_type;
			points[n].mPos = mPos.Add((vmult(mSize, o[7]).Multiply(0.5f))); points[n++].mType = collide_type;
		}
	}
}
