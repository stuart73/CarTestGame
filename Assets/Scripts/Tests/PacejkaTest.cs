using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Tests
{
    public class PacejkaTest : Test
    {
		//******************************************************************************************
		public void Run()
		{
			//	test 1 Failed(410.904022, 45.560001)
			//	test 2 Failed(-0.000000, 45.560001)
			//	test 3 Failed(-4302.196289, 45.560001)
			//	test 4 Failed(-0.000000, 45.560001)
			//	test 5 Failed(-2708.695068, 45.560001)
			//	test 6 Failed(16683.208984, 45.560001)
			//	test 7 Failed(-1922.217529, 45.560001)
			//	test 8 Failed(0.000000, 45.560001)

			// slip=degrees range=+/- 90.0
			// slipratio= percent range +/- 1600.0
			// loadkN range= 0 to 15.0 kiloNewtons  
			// result = force in Newtons
			int test = 1;

			DeclareRunningTest("Pacejka");

			float result = Pacejka.Flateral_combined(-23.0f, 1000.0f, 5.6f);
			CheckResult(result, 410.904022f, test++);
			result = Pacejka.Flateral_combined(0.0f, 40.0f, 15.6f);
			CheckResult(result, 0.0f, test++);
			result = Pacejka.Flateral_combined(33.0f, -2.0f, 2.6f);
			CheckResult(result, -4302.196289f, test++);
			result = Pacejka.Flateral_combined(2.3f, 20.0f, 0.0f);
			CheckResult(result, 0.0f, test++);

			result = Pacejka.Flongitudinal_combined(87.0f, -700.0f, 2.23f);
			CheckResult(result, -2708.695068f, test++);
			result = Pacejka.Flongitudinal_combined(0.0f, 40.0f, 13.1f);
			CheckResult(result, 16683.208984f, test++);
			result = Pacejka.Flongitudinal_combined(92.0f, -500.0f, 1.6f);
			CheckResult(result, -1922.217529f, test++);
			result = Pacejka.Flongitudinal_combined(-12.3f, 1201.0f, 0.0f);
			CheckResult(result, 0.0f, test++);
		}


	}
}
