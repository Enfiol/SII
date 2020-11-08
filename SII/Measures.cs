using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SII
{
    public class Measures
    {
        const double RatingWeight = 1.5;
        const double PagesWeight = 0.5;
        const double ThemesCountWeight = 2;
        const double YearWeight = 1;


        public double EuqlidDistance(Lection lection1, Lection lection2)
        {
            double distance = 0;

            distance += Math.Sqrt(Math.Abs(Math.Pow(lection1.Rating, 2) - Math.Pow(lection2.Rating, 2))) * RatingWeight;
            distance += Math.Sqrt(Math.Abs(Math.Pow(lection1.Pages, 2) - Math.Pow(lection2.Pages, 2))) * PagesWeight;
            distance += Math.Sqrt(Math.Abs(Math.Pow(lection1.ThemesCount, 2) - Math.Pow(lection2.ThemesCount, 2))) * ThemesCountWeight;
            distance += Math.Sqrt(Math.Abs(Math.Pow(DateTime.Now.Year-lection1.Year, 2) - Math.Pow(DateTime.Now.Year - lection2.Year, 2))) * YearWeight;

            return distance;
        }

        public double ManhattanDistance(Lection lection1, Lection lection2)
        {
            double distance = 0;

            distance += Math.Abs(lection1.Rating - lection2.Rating) * RatingWeight;
            distance += Math.Abs(lection1.Pages - lection2.Pages) * PagesWeight;
            distance += Math.Abs(lection1.ThemesCount - lection2.ThemesCount) * ThemesCountWeight;
            distance += Math.Abs(lection1.Year - lection2.Year) * YearWeight;

            return distance;
        }

        public double CorrelationDistance(Lection lection1, Lection lection2)
        {
            var values1 = new double[] { DateTime.Now.Year - lection1.Year, lection1.Rating, lection1.ThemesCount, lection1.Pages };
            var values2 = new double[] { DateTime.Now.Year - lection2.Year, lection2.Rating, lection2.ThemesCount, lection2.Pages };

            if (values1.Length != values2.Length)
                throw new ArgumentException("values must be the same length");

            var avg1 = values1.Average();
            var avg2 = values2.Average();

            var sum1 = values1.Zip(values2, (x1, y1) => (x1 - avg1) * (y1 - avg2)).Sum();

            var sumSqr1 = values1.Sum(x => Math.Pow((x - avg1), 2.0));
            var sumSqr2 = values2.Sum(y => Math.Pow((y - avg2), 2.0));

            var result = sum1 / Math.Sqrt(sumSqr1 * sumSqr2);

            return result;
        }

        public int EqualValues(Lection lection1, Lection lection2)
        {
            int result = 0;

            if(lection1.Pages==lection2.Pages)
            {
                result++;
            }

            if (lection1.ThemesCount == lection2.ThemesCount)
            {
                result++;
            }

            if (lection1.Subject == lection2.Subject)
            {
                result++;
            }

            if (lection1.Author == lection2.Author)
            {
                result++;
            }

            //if (lection1.Country == lection2.Country)
            //{
            //    result++;
            //}
            if (lection1.Rating == lection2.Rating)
            {
                result++;
            }
            if (lection1.University == lection2.University) ;
            {
                result++;
            }
            if (lection1.Language == lection2.Language) ;
            {
                result++;
            }
            if (lection1.Year==lection2.Year)
            {
                result++;
            }
            return result;
        }
    }
}
