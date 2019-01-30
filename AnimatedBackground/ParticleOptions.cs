using Xamarin.Forms;

namespace SkiaSharp.Views.Forms.AnimatedBackground
{
	public class ParticleOptions
	{
		public SKImage? Image { get; set; }

		public Color BaseColor { get; set; } = Color.Black;

		public double SpawnMinRadius { get; set; } = 1.0;

		public double SpawnMaxRadius { get; set; } = 10.0;

		public double SpawnMinSpeed { get; set; } = 150.0;

		public double SpawnMaxSpeed { get; set; } = 300.0;

		public double SpawnOpacity { get; set; } = 0.0;

		public double MinOpacity { get; set; } = 0.1;

		public double MaxOpacity { get; set; } = 0.4;

		public double OpacityChangeRate { get; set; } = 0.25;

		public int ParticleCount { get; set; } = 100;
	}
}
