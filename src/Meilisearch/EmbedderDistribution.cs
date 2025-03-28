using System;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Embedder distribution.
    /// </summary>
    public class EmbedderDistribution
    {
        /// <summary>
        /// Creates a new instance of <see cref="EmbedderDistribution"/>.
        /// </summary>
        /// <param name="mean">Mean value between 0 and 1.</param>
        /// <param name="sigma">Sigma value between 0 and 1.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public EmbedderDistribution(double mean, double sigma)
        {
            if (mean < 0 || mean > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(mean), "Mean must be between 0 and 1.");
            }

            if (sigma < 0 || sigma > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(sigma), "Sigma must be between 0 and 1.");
            }
        }

        /// <summary>
        /// Gets or sets the mean.
        /// </summary>
        [JsonPropertyName("mean")]
        public double Mean { get; set; }

        /// <summary>
        /// Gets or sets the sigma.
        /// </summary>
        [JsonPropertyName("sigma")]
        public double Sigma { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="EmbedderDistribution"/> with a uniform distribution.
        /// </summary>
        /// <returns></returns>
        public static EmbedderDistribution Uniform() => new EmbedderDistribution(0.5, 0.5);
    }
}
