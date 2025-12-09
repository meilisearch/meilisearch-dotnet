using System;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Embedder distribution.
    /// </summary>
    public class EmbedderDistribution
    {
        private double _mean;
        private double _sigma;

        /// <summary>
        /// Creates a new instance of <see cref="EmbedderDistribution"/>.
        /// </summary>
        /// <param name="mean">Mean value between 0 and 1.</param>
        /// <param name="sigma">Sigma value between 0 and 1.</param>
        public EmbedderDistribution(double mean, double sigma)
        {
            Mean = mean;
            Sigma = sigma;
        }

        /// <summary>
        /// Gets or sets the mean.
        /// </summary>
        [JsonPropertyName("mean")]
        public double Mean
        {
            get => _mean;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(Mean), "Mean must be between 0 and 1.");
                }

                _mean = value;
            }
        }

        /// <summary>
        /// Gets or sets the sigma.
        /// </summary>
        [JsonPropertyName("sigma")]
        public double Sigma
        {
            get => _sigma;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(Sigma), "Sigma must be between 0 and 1.");
                }

                _sigma = value;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="EmbedderDistribution"/> with a uniform distribution.
        /// </summary>
        /// <returns></returns>
        public static EmbedderDistribution Uniform() => new EmbedderDistribution(0.5, 0.5);
    }
}
