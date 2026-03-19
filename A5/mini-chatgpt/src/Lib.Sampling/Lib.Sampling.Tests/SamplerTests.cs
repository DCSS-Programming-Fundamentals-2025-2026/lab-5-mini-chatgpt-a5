using System;
using System.Timers;
using Lib.MathCore;
using MiniChatGPT.Sampling;
using MiniChatGPT.Sampling.Interfaces;
using Moq;
using NUnit.Framework;

namespace Lib.Sampling.Tests
{
	[TestFixture]
	public class SamplerTests
	{
		private Mock<IMathOps> _mathOpsMock;
		private Sampler _sampler;

		[SetUp]
		public void SetUp()
		{
			_mathOpsMock = new Mock<IMathOps>();
			_sampler = new Sampler(_mathOpsMock.Object);
		}

		[Test]
		public void Constructor_NullMathOps_ThrowsArgumentNullException()
		{
			var ex = Assert.Throws<ArgumentNullException>(() => new Sampler(null));
			Assert.That(ex.ParamName, Is.EqualTo("mathOps"));
		}

		[Test]
		public void Sample_NullProbs_ThrowsArgumentException()
		{
			float[] probs = null;

			var ex = Assert.Throws<ArgumentException>(() => _sampler.Sample(probs, 1.0f, 10));
			Assert.That(ex.Message, Does.Contain("Масив ймовірностей є порожнім!"));
		}

		[Test]
		public void Sample_EmptyProbs_ThrowsArgumentException()
		{
			float[] probs = Array.Empty<float>();

			// Act & Assert
			var ex = Assert.Throws<ArgumentException>(() => _sampler.Sample(probs, 1.0f, 10));
			Assert.That(ex.Message, Does.Contain("Масив ймовірностей є порожнім!"));
		}

		[Test]
		public void Sample_ValidInputs_CallsMathOpsAndReturnsExpectedResult()
		{
			float[] probs = { 0.1f, 0.2f, 0.7f, 0.05f };
			float temperature = 1.0f;
			int topK = 2;

			int expectedSampledIndex = 0;
			_mathOpsMock
				.Setup(m => m.SampleFromProbs(It.IsAny<float[]>(), It.IsAny<Random>()))
				.Returns(expectedSampledIndex);

			int result = _sampler.Sample(probs, temperature, topK);

			_mathOpsMock.Verify(m => m.SampleFromProbs(It.IsAny<float[]>(), It.IsAny<Random>()), Times.Once);
			Assert.That(result, Is.GreaterThanOrEqualTo(0).And.LessThan(probs.Length));
		}

		[Test]
		public void Sample_WithSeed_UsesPredictableRandomAndCallsMathOps()
		{
			float[] probs = { 0.5f, 0.5f };
			float temperature = 0.8f;
			int topK = 2;
			int seed = 42;

			_mathOpsMock
				.Setup(m => m.SampleFromProbs(It.IsAny<float[]>(), It.IsAny<Random>()))
				.Returns(1);

			int result = _sampler.Sample(probs, temperature, topK, seed);

			_mathOpsMock.Verify(m => m.SampleFromProbs(It.IsAny<float[]>(), It.IsNotNull<Random>()), Times.Once);
		}
	}
}