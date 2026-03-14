namespace MiniChatGPT.Sampling.Processing
{
    public static class ProbabilityNormalizer
    {
        public static void Normalize(float[] probs)
        {
            float sum = 0f;

            for(int i = 0; i < probs.Length; i++)
            {
                sum += probs[i];
            }

            if(sum >  0f)
            {
                for(int i = 0; i < probs.Length;i++)
                {
                    probs[i] /= sum;
                }
            }
        }
    }
}
