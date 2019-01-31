using Xamarin.Forms;

namespace SkiaSharp.Views.Forms.AnimatedBackground
{
	public class Bubble : IParticle
	{
		public Rectangle Bounds =>
			new Rectangle(Center.X - Radius, Center.Y - Radius, Center.X + Radius, Center.Y + Radius);

		public Point Center { get; set; }

		public double Radius { get; set; }

		public double TargetRadius { get; set; }

		public Color Color { get; set; }

		public bool Popping { get; set; }
	}
}
