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
using System.Collections.Generic;

namespace DocaLabs.Testing.Common.Mathematics
{
    /// <summary>
    /// Implementation of a Chi-square test (http://en.wikipedia.org/wiki/Chi_square_test)
    /// that calculates the "difference" between an expected and an actual statistical distribution.
    /// </summary>
    public class ChiSquareTest
    {
        /// <summary>
        /// Gets the resulting number of degrees of freedom.
        /// </summary>
        public int DegreesOfFreedom { get; private set; }

        /// <summary>
        /// Gets the resulting Chi-square value.
        /// </summary>
        public double ChiSquareValue { get; private set; }

        /// <summary>
        /// Gets the resulting significance probability.
        /// </summary>
        public double TwoTailedPValue { get; private set; }

        /// <summary>
        /// Runs a Chi-square test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Given the observed events (<paramref name="actual"/>) and the expected events (<paramref name="expected"/>), 
        /// calculates the number of degrees of freedom, the chi-square value, and the significance probability.
        /// </para>
        /// <para>
        /// A small value of the significance probability indicates a significant difference between the distributions.
        /// </para>
        /// </remarks>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="numberOfConstraints"></param>
        public ChiSquareTest(double expected, ICollection<double> actual, int numberOfConstraints)
        {
            if (expected <= 0)
                throw new ArgumentOutOfRangeException("expected", @"The expected value is negative.");
            if (actual == null)
                throw new ArgumentNullException("actual");

            DegreesOfFreedom = actual.Count - numberOfConstraints;

            foreach (var value in actual)
            {
                var delta = value - expected;
                ChiSquareValue += (delta * delta) / expected;
            }

            TwoTailedPValue = Gamma.IncompleteGamma(DegreesOfFreedom / 2d, ChiSquareValue / 2d);
        }
    }
}
