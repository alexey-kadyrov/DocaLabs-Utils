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
    /// A map that stores the occurrences of values and computes various statistical data.
    /// </summary>
    public class DistributionStore<TValue>
    {
        const int MinimumHashCount = 2;

        object Locker { get; set; }
        HashSet<TValue> One { get; set; }
        HashSet<TValue> Two { get; set; }
        IDictionary<TValue, int> More { get; set; }

        public int Count { get; private set; }

        public DistributionStore()
        {
            Locker = new object();
            One = new HashSet<TValue>();
            Two = new HashSet<TValue>();
            More = new Dictionary<TValue, int>();
        }

        public void Add(TValue value)
        {
            lock (Locker)
            {
                if (One.Contains(value))
                {
                    One.Remove(value);
                    Two.Add(value);
                }
                else if (Two.Contains(value))
                {
                    Two.Remove(value);
                    More.Add(value, 3);
                }
                else
                {
                    int n;

                    if (More.TryGetValue(value, out n))
                    {
                        More[value] = 1 + n;
                    }
                    else
                    {
                        One.Add(value);
                    }
                }

                Count++;
            }
        }

        public DistributionStoreResult CalculateResults()
        {
            // ReSharper disable ForCanBeConvertedToForeach
            lock (Locker)
            {
                if (Count < MinimumHashCount)
                    throw new InvalidOperationException(string.Format("Not Enough Hashes: minimal {0}, actual {1}", MinimumHashCount, Count));

                var bucketSize = GetBucketSize();
                var bucket = new double[bucketSize];
                double collisionProbability = 0;
                var index = 0;

                for (var i = 0; i < One.Count; i++)
                {
                    bucket[index++%bucketSize] += 1;
                }

                for (int i = 0; i < Two.Count; i++)
                {
                    bucket[index++%bucketSize] += 2;
                    collisionProbability += 2.0/(Count*(Count - 1));
                }

                foreach (var pair in More)
                {
                    bucket[index++%bucketSize] += pair.Value;
                    collisionProbability += (double) pair.Value/Count*(pair.Value - 1)/(Count - 1);
                }

                var chiSquareTest = new ChiSquareTest((double) Count/bucketSize, bucket, 1);

                var uniformDistributionDeviationProbability = 1.0 - chiSquareTest.TwoTailedPValue;

                return new DistributionStoreResult(Count, collisionProbability, uniformDistributionDeviationProbability);
            }
            // ReSharper restore ForCanBeConvertedToForeach
        }

        int GetBucketSize()
        {
            const int threshold = 10;

            var primes = new[] { 14813, 3613, 223, 17 };

            var count = One.Count + Two.Count + More.Count;

            foreach (var prime in primes)
            {
                if (count >= prime * threshold)
                    return prime;
            }

            return count;
        }
    }
}
