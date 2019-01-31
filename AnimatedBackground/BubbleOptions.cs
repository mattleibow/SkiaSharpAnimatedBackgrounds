namespace SkiaSharp.Views.Forms.AnimatedBackground
{
	public class BubbleOptions : IParticleOptions
	{
		public int ParticleCount { get; set; } = 20;

		public double MinTargetRadius { get; set; } = 15.0;

		public double MaxTargetRadius { get; set; } = 50.0;

		public double GrowthRate { get; set; } = 10.0;

		public double PopRate { get; set; } = 150.0;
	}
}
