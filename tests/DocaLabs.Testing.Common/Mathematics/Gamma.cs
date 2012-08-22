#region License
// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;

namespace DocaLabs.Testing.Common.Mathematics
{
    /// <summary>
    /// Helper methods to calculate the Gamma function (http://en.wikipedia.org/wiki/Gamma_function).
    /// </summary>
    public static class Gamma
    {
        const int MaxIterations = 1000; // Maximum allowed number of iterations.
        const double Accuracy = 3E-7; // Relative accuracy
        const double Epsilon = 1E-30;

        /// <summary>
        /// Returns the incomplete Gamma function Q(a,x).
        /// </summary>
        /// <param name="a"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double IncompleteGamma(double a, double x)
        {
            if (x < 0)
                throw new ArgumentOutOfRangeException("x");
            if (a <= 0)
                throw new ArgumentOutOfRangeException("a");

            if (x < a + 1)
            {
                // Use the series representation and take its complement.
                return 1 - IncompleteGammaSeries(a, x);
            }

            // Use the continued fraction representation.
            return IncompleteGammaContinuedFraction(a, x);
        }

        // Returns the incomplete Gamma function evaluated by its series representation.
        static double IncompleteGammaSeries(double a, double x)
        {
            if (x < 0)
                throw new ArgumentOutOfRangeException("x");

            var ap = a;
            var delta = 1 / a;
            var sum = delta;

            for (var n = 1; n < MaxIterations; n++)
            {
                ap++;
                delta *= x / ap;
                sum += delta;

                if (Math.Abs(delta) < Math.Abs(sum) * Accuracy)
                    return sum * Math.Exp(-x + a * Math.Log(x) - GammaLogarithm(a));
            }

            throw new ArgumentOutOfRangeException("a", @"Value too large.");
        }

        // Returns the incomplete Gamma function evaluated by its continued fraction representation.
        static double IncompleteGammaContinuedFraction(double a, double x)
        {
            var b = x + 1 - a;
            var c = 1 / Epsilon;
            var d = 1 / b;
            var h = d;

            for (var i = 1; i < MaxIterations; i++)
            {
                var an = -i * (i - a);
                b += 2;
                d = 1 / Math.Max(Epsilon, an * d + b);
                c = Math.Max(Epsilon, b + an / c);
                var delta = d * c;
                h *= delta;

                if (Math.Abs(delta - 1) < Accuracy)
                    return Math.Exp(-x + a * Math.Log(x) - GammaLogarithm(a)) * h;
            }

            throw new ArgumentOutOfRangeException("a", @"Value too large.");
        }

        // Returns the natural logarithm of the specified positive Gamma value.
        static double GammaLogarithm(double a)
        {
            // ReSharper disable ForCanBeConvertedToForeach
            // ReSharper disable LoopCanBeConvertedToQuery
            var coef = new[]
            {
               76.18009172947146,
               -86.50532032941677,
               24.01409824083091,
               -1.231739572450155,
               0.1208650973866179E-2,
               -0.5395239384953E-5
            };

            var x = a;
            var y = a;
            var t = x + 5.5;
            t -= (x + 0.5) * Math.Log(t);
            var s = 1.000000000190015;


            for (var j = 0; j < coef.Length; j++)
            {
                s += coef[j] / (++y);
            }

            return -t + Math.Log(2.5066282746310005 * s / x);
            // ReSharper restore LoopCanBeConvertedToQuery
            // ReSharper restore ForCanBeConvertedToForeach
        }
    }
}
