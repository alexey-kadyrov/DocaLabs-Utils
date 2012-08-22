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

namespace DocaLabs.Testing.Common.Mathematics
{

    /// <summary>
    /// A list of common limits for the uniform distribution significance level.
    /// </summary>
    public static class UniformDistributionQuality
    {
        /// <summary>
        /// Nearly uniform (less than 1% of deviation probability)
        /// </summary>
        public static readonly double Excellent = 0.01;

        /// <summary>
        /// Characterizes a roughly uniform distribution (less than 5% of deviation probability)
        /// </summary>
        public static readonly double Good = 0.05;

        /// <summary>
        /// Characterizes a reasonably uniform distribution (less than 10% of deviation probability)
        /// </summary>
        public static readonly double Fair = 0.1;

        /// <summary>
        /// Characterizes a poor uniform distribution (less than 30% of deviation probability) 
        /// </summary>
        public static readonly double Mediocre = 0.3;
    }
}
