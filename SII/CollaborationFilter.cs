using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SII
{
    public static class CollaborationFilter
    {
        public static double Correlation(double[] values1, double[] values2)
        {
            if(values1.Length == 0 || values2.Length == 0)
            {
                return 0;
            }

            var avg1 = values1.Average();
            var avg2 = values2.Average();

            var sum1 = values1.Zip(values2, (x1, y1) => (x1 - avg1) * (y1 - avg2)).Sum();

            var sumSqr1 = values1.Sum(x => Math.Pow((x - avg1), 2.0));
            var sumSqr2 = values2.Sum(y => Math.Pow((y - avg2), 2.0));

            double result;

            if (sumSqr1 == 0 || sumSqr2 == 0)
            {
                if (sumSqr1 == sumSqr2)
                {
                    result = 1 - (Math.Abs(avg1 - avg2) / 5);
                }
                else
                {
                    result = 0;
                }
            }
            else
            {
                result = sum1 / Math.Sqrt(sumSqr1 * sumSqr2);
            }

            return result;
        }


    }
}
