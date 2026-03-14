namespace MiniChatGPT.Sampling.Interfaces
{
    public interface ISampler
    {
        int Sample(float[] probs, float temperature, int topK, Random? rng = null);

        int Sample(float[] probs, float temperature, int topK, int? seed);
    }
}
