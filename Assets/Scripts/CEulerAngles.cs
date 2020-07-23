using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class CEulerAngles
    {

		public float mYaw;
		public float mPitch;
		public float mRoll;


		//*********************************************************************************
		public CEulerAngles( FMatrix m)
		{
			mPitch = (float)Math.Asin( -m.Row[1].Z );

			// singularity occurs when pitch == 90 degs or pitch == - 90 degs ( make heading and roll zero )

			if ((mPitch >= (float)Math.PI) || (mPitch <= (float)-Math.PI))
			{
				mYaw = 0.0f;
				mRoll = 0.0f;
			}
			else
			{
				mYaw = (float)Math.Atan2(-m.Row[0].Z, m.Row[2].Z);  // i.e ( atan2( (cos p * sin y), (cos p * cos y) ) )
				mRoll = (float)Math.Atan2(-m.Row[1].X, m.Row[1].Y);  // i.e.( atan2( (cos p * sin r), (cos p * cos r) ) ) 
			}
		}
    }
}
