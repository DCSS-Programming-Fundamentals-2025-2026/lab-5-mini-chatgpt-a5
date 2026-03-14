using MiniChatGPT.Sampling.Interfaces;

namespace MiniChatGPT.Sampling
{
    public class Sampler : ISampler
    {
        public int Sample(float[] probs, float temperature, int topK, Random? rng = null)
        {

        }

        public int Sample(float[] probs, float temperature, int topK, int? seed)
        {

        }
    }
}
