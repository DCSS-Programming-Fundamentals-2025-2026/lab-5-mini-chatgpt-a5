using MiniChatGPT.Sampling.Interfaces;
using MiniChatGPT.Sampling.Processing;
using Lib.MathCore;

namespace MiniChatGPT.Sampling
{
    public class Sampler : ISampler
    {
        private readonly IMathOps _mathOps;

        public Sampler(IMathOps mathOps)
        {
            if (mathOps == null)
            {
                throw new ArgumentNullException("mathOps");
            }

            _mathOps = mathOps;
        }

        public int Sample(float[] probs, float temperature, int topK, Random? rng = null)
        {
            if (probs == null || probs.Length == 0)
            {
                throw new ArgumentException("Масив ймовірностей є порожнім!");
            }

            if (rng == null)
            {
                rng = new Random();
            }

            float[] tempered = TemperatureScaler.Scale(probs, temperature);

            int[] idx = TopKSelector.Select(tempered, topK);

            float[] topKProbs = new float[idx.Length];

            for (int i = 0; i < idx.Length; i++)
            {
                int originalIdx = idx[i];
                topKProbs[i] = MathF.Exp(tempered[originalIdx]); 
            }

            ProbabilityNormalizer.Normalize(topKProbs);

            int selectedKIdx = _mathOps.SampleFromProbs(topKProbs, rng);

            return idx[selectedKIdx];
        }

        public int Sample(float[] probs, float temperature, int topK, int? seed)
        {
            Random? rng = null;

            if (seed.HasValue)
            {
                rng = new Random(seed.Value);
            }

            return Sample(probs, temperature, topK, rng);
        }
    }
}
