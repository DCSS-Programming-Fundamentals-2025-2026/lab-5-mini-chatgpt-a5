using Lib.MathCore;
using MiniChatGPT.Sampling;

namespace Lib.Sampling.Tests
{
    public class SamplerTests
    {
        private FakeMathOps _fakeMathOps;
        private Sampler _sampler;

        [SetUp]
        public void SetUp()
        {
            _fakeMathOps = new FakeMathOps();
            _sampler = new Sampler(_fakeMathOps);
        }

        [Test]
        public void Constructor_NullMathOps_ThrowsArgumentNullException()
        {
            IMathOps mathOps = null;

            var ex = Assert.Throws<ArgumentNullException>(() => new Sampler(mathOps));
            Assert.That(ex.ParamName, Is.EqualTo("mathOps"));
        }

        [Test]
        public void Sample_NullProbs_ThrowsArgumentException()
        {
            float[] probs = null;

            var ex = Assert.Throws<ArgumentException>(() => _sampler.Sample(probs, 1.0f, 10));
            Assert.That(ex.Message, Is.EqualTo("Масив ймовірностей є порожнім!"));
        }

        [Test]
        public void Sample_EmptyProbs_ThrowsArgumentException()
        {
            float[] probs = Array.Empty<float>();

            var ex = Assert.Throws<ArgumentException>(() => _sampler.Sample(probs, 1.0f, 10));
            Assert.That(ex.Message, Is.EqualTo("Масив ймовірностей є порожнім!"));
            Assert.That(probs.Length, Is.EqualTo(0));
        }

        [Test]
        public void Sample_TopK_ProperlyTrimsProbabilities()
        {
            float[] probs = { 0.1f, 0.6f, 0.2f, 0.1f };
            float temperature = 1.0f;
            int topK = 2; 

            _fakeMathOps.ReturnIndex = 0;

            int result = _sampler.Sample(probs, temperature, topK);

            Assert.That(_fakeMathOps.LastReceivedProbs, Is.Not.Null);
            Assert.That(_fakeMathOps.LastReceivedProbs.Length, Is.EqualTo(topK));

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void Sample_Temperature_AffectsProbabilitiesDistribution()
        {
            float[] probs = { 0.1f, 0.9f };
            int topK = 2;

            _sampler.Sample(probs, 1.0f, topK);
            float[] probsTempNormal = _fakeMathOps.LastReceivedProbs;
            float diffNormal = Math.Abs(probsTempNormal[0] - probsTempNormal[1]);

            _sampler.Sample(probs, 5.0f, topK);
            float[] probsTempHigh = _fakeMathOps.LastReceivedProbs;
            float diffHigh = Math.Abs(probsTempHigh[0] - probsTempHigh[1]);

            Assert.That(diffHigh, Is.LessThan(diffNormal), "Висока температура повинна зближувати ймовірності між собою!");
        }

        [Test]
        public void Sample_WithSeed_IsDeterministic()
        {
            float[] probs = { 0.2f, 0.3f, 0.5f };
            float temperature = 1.0f;
            int topK = 3;
            int seed = 42;

            _sampler.Sample(probs, temperature, topK, seed);
            var rng1 = _fakeMathOps.LastRng;
            int randomValue1 = rng1.Next();

            _sampler.Sample(probs, temperature, topK, seed);
            var rng2 = _fakeMathOps.LastRng;
            int randomValue2 = rng2.Next();

            Assert.That(randomValue1, Is.EqualTo(randomValue2), "Однаковий seed має генерувати однакову послідовність чисел");
        }
    }

    public class FakeMathOps : IMathOps
    {
        public float[] LastReceivedProbs { get; private set; }
        public Random LastRng { get; private set; }
        public int ReturnIndex { get; set; } = 0;

        public int SampleFromProbs(ReadOnlySpan<float> probs, Random rng)
        {
            LastReceivedProbs = probs.ToArray();
            LastRng = rng;

            return ReturnIndex;
        }

        public float[] Softmax(ReadOnlySpan<float> logits) => throw new NotImplementedException();
        public float CrossEntropyLoss(ReadOnlySpan<float> logits, int target) => throw new NotImplementedException();
        public int ArgMax(ReadOnlySpan<float> scores) => throw new NotImplementedException();
    }
}