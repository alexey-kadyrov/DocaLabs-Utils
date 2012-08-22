using System;
using System.Diagnostics;
using DocaLabs.Testing.Common.Mathematics;
using Machine.Specifications;

namespace DocaLabs.Testing.Common.MSpec
{
    public static class DistributionExtensions
    {
        public static void ShouldHaveUniformDistributionDeviationProbability<TValue>(this DistributionStore<TValue> distribution, double uniformDistributionQuality)
        {
            var result = distribution.CalculateResults();

            var probability = result.UniformDistributionDeviationProbability;

            Debug.WriteLine("Statistical Population = {0}", result.StatisticalPopulation);
            Debug.WriteLine("Deviation Probability = {0} {1} {2}", probability, ((probability <= uniformDistributionQuality) ? "=" : ">"), uniformDistributionQuality);

            if (probability > uniformDistributionQuality)
            {
                throw new SpecificationException(
                    String.Format("The distribution is not considered uniform because the actual probability of deviation {0} is higher than the specified limit of {1}.", probability, uniformDistributionQuality));
            }
        }

        public static void ShouldHaveCollisionProbability<TValue>(this DistributionStore<TValue> distribution, double collisionProbabilityLimit)
        {
            var result = distribution.CalculateResults();

            var probability = result.CollisionProbability;

            Debug.WriteLine("Statistical Population = {0}", result.StatisticalPopulation);
            Debug.WriteLine("Actual Collision Probability = {0} {1} {2}", probability, ((probability <= collisionProbabilityLimit) ? "=" : ">"), collisionProbabilityLimit);

            if (probability <= collisionProbabilityLimit)
                return;

            throw new SpecificationException(
                string.Format("The actual collision probability {0} is higher than the specified limit of {1}.", probability, collisionProbabilityLimit));
        }
    }
}
