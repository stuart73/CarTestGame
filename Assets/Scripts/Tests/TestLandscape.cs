using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Tests
{
    public class TestLandscape
    {
        public bool LineIntersect(FVector inStart, FVector inEnd, FVector intersect_point, FVector normal)
        {
            if (inStart.Y > 0.0f && inEnd.Y <= 0.0f)
            {
                intersect_point = inStart;
                intersect_point.Y = 0;
                normal = new FVector(0.0f, 1.0f, 0.0f);

                return true;

            }
            return false;
        }


    }
}
