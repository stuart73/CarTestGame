using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class Pacejka
    {
		static public float Flateral_combined(float slip, float slipratio, float loadkN)
		{

			// slip=degrees range=+/- 90.0
			// slipratio= percent range +/- 1600.0
			// loadkN range= 0 to 15.0 kiloNewtons  
			// result = force in Newtons

			float B, C, D, E, BCD, G, y;

			float[] a = new float[10] {1.45E+00f, 9.00E+01f, 2.0E+03f, -2.84E+03f, -6.0E+00f,
					 5.80E-02f, 1.05E-01f, 9.8E-01f,  2.00E-01f, 4.5E-01f};

			if (loadkN < 0.0f) { loadkN = 0.0f; }
			if (loadkN > 15.0f) { loadkN = 15.0f; }
			if (slip > 90.0f) { slip = 90.0f; }
			if (slip < -90.0f) { slip = -90.0f; }
			if (slipratio > 1600.0f) { slipratio = 1600.0f; }
			if (slipratio < -1600.0f) { slipratio = -1600.0f; }

			D = -(-a[1] * loadkN + a[2]) * loadkN;
			C = a[0];
			BCD = a[3] * (float) Math.Sin(2.0f * Math.Atan(-loadkN / a[4]));
			if (D == 0.0f)
				B = 0.0f;
			else
				B = BCD / (C * D);
			E = -a[6] * loadkN + a[7];
			G = (float) Math.Cos(Math.Atan(slipratio * a[8] * Math.Cos(Math.Atan(a[9] * slip))));
			y = D * G * (float) Math.Sin(C * Math.Atan((B * slip) - E * ((B * slip) - Math.Atan(B * slip))));

			return y;
		}


		static public float Flongitudinal_combined(float slip, float slipratio, float loadkN)
		{
			// slip=degrees range=+/- 90.0
			// slipratio= percent range +/- 1600.0
			// loadkN range= 0 to 15.0 kiloNewtons
			// result = force in Newtons

			float B, C, D, E, BCD, G, y;
			float[] a= new float[11] { 1.58E+00f,  0.00E+00f, -1.82E+03f, 8.77E+01f, -6.25E+02f, -1.12E-01f,
				   -2.65E-02f, -1.53E-01f,  6.82E-01f, 2.68E-01f,  1.80E-01f};

			if (loadkN < 0.0f) { loadkN = 0.0f; }
			if (loadkN > 15.0f) { loadkN = 15.0f; }
			if (slip > 90.0f) { slip = 90.0f; }
			if (slip < -90.0f) { slip = -90.0f; }
			if (slipratio > 1600.0f) { slipratio = 1600.0f; }
			if (slipratio < -1600.0f) { slipratio = -1600.0f; }

			D = -a[2] * loadkN;
			C = a[0];
			BCD = -(-a[3] * loadkN + a[4]) * loadkN * (float) Math.Exp(a[5] * loadkN);
			if (D == 0.0f)
				B = 0.0f;
			else
				B = BCD / (C * D);
			E = -(((-a[6] * loadkN) + a[7]) * loadkN) + a[8];
			G = (float) Math.Cos(Math.Atan(slip * a[9] * (float) Math.Cos(Math.Atan(a[10] * slipratio))));
			y = D * G * (float) Math.Sin(C * (float) Math.Atan((B * slipratio) - (E * (B * slipratio - (float) Math.Atan(B * slipratio)))));

			return y;

		}

	}
}
