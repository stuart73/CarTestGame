using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Tests
{
    public class POFunctionsTest : Test
    {

		public void Run()
		{

			//	test 1 Failed(2788.804443, 45.560001)
			//		Test 2 Passed(-0.000000)
			//		Test 3 Passed(0.000000)
			//		Test 4 Passed(4719.899414)
			//		Test 5 Passed(-0.000000)
			//		test 6 Failed(15093.388672, 45.560001)
			//		test 7 Failed(0.000000, 45.560001)
			//		test 8 Failed(0.000000, 45.560001)
			//		Test 9 Passed(15660.238281)
			//		Test 10 Passed(0.000000)
			//		Test 11 Passed(1660.500000)
			//		Test 12 Passed(1012143.187500)
			//		Test 13 Passed(0.000000)

			int test = 1;
			DeclareRunningTest("POFunctions");

			float result = POFunctions.LateralForceFromSlipAngleSlipRatioAndLoad(0.34f, 1.4f, 23234.0f);
			CheckResult(result, 2788.804443f, test++);
			result = POFunctions.LateralForceFromSlipAngleSlipRatioAndLoad(-0.977f, 0.4f, -13432);
			CheckResult(result, 0.0f, test++);
			result = POFunctions.LateralForceFromSlipAngleSlipRatioAndLoad(0.0f, 0.0f, 0.0f);
			CheckResult(result, 0.0f, test++);
			result = POFunctions.LateralForceFromSlipAngleSlipRatioAndLoad(0.24f, 0.56f, 20456.0f);
			CheckResult(result, 4719.899414f, test++);
			result = POFunctions.LateralForceFromSlipAngleSlipRatioAndLoad(-0.834f, 0.354f, -5643.35f);
			CheckResult(result, 0.0f, test++);

			result = POFunctions.LongitudinalForceFromSlipAngleSlipRatioAndLoad(0.74f, 1.1f, 33234.0f);
			CheckResult(result, 15093.388672f, test++);
			result = POFunctions.LongitudinalForceFromSlipAngleSlipRatioAndLoad(-0.877f, 0.23f, -23432);
			CheckResult(result, 0.0f, test++);
			result = POFunctions.LongitudinalForceFromSlipAngleSlipRatioAndLoad(0.0f, 0.0f, 0.0f);
			CheckResult(result, 0.0f, test++);
			result = POFunctions.LongitudinalForceFromSlipAngleSlipRatioAndLoad(0.34f, 0.46f, 204526.0f);
			CheckResult(result, 15660.238281f, test++);
			result = POFunctions.LongitudinalForceFromSlipAngleSlipRatioAndLoad(-0.534f, 0.154f, -31643.35f);
			CheckResult(result, 0.0f, test++);


			result = POFunctions.DragFromSpeed(45.0f);
			CheckResult(result, 1660.500000f, test++);
			result = POFunctions.DragFromSpeed(-1111.0f);
			CheckResult(result, 1012143.187500f, test++);
			result = POFunctions.DragFromSpeed(0.0f);
			CheckResult(result, 0.0f, test++);
		}
	}
}
