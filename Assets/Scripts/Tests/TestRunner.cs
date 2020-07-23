using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Tests
{
    public class TestRunner
    {
        public void RunAll()
        {
            PacejkaTest pacejkaTest = new PacejkaTest(); ;
            pacejkaTest.Run();

            POFunctionsTest pofunctionTest = new POFunctionsTest();
            pofunctionTest.Run();

            //RunClassTest1 crt = new RunClassTest1();
            // crt.Run();

            WheelTest wheelTest = new WheelTest();
            wheelTest.Run();
        }
    }
}
