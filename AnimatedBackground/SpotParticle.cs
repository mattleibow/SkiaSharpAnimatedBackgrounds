using System;
using Xamarin.Forms;

namespace SkiaSharp.Views.Forms.AnimatedBackground
{
	public class SpotParticle : IParticle
	{
		public Rectangle Bounds => 
            new Rectangle(Center.X - Radius, Center.Y - Radius, Center.X + Radius, Center.Y + Radius);

		public Point Center { get; set; }

		public double DirectionX { get; set; }

		public double DirectionY { get; set; } = 1.0;

		public double Radius { get; set; }

		public double Opacity { get; set; }

		public double TargetOpacity { get; set; }

		public object UserState { get; set; }

		public double SpeedSquared
		{
			get => DirectionX * DirectionX + DirectionY * DirectionY;
			set => Speed = Math.Sqrt(Math.Abs(value) * Math.Sign(value));
		}

		public double Speed
		{
			get => Math.Sqrt(SpeedSquared);
			set
			{
				var mag = Speed;
				if (mag == 0)
				{
					DirectionX = 0.0;
					DirectionY = value;
				}
				else
				{
					DirectionX = DirectionX / mag * value;
					DirectionY = DirectionY / mag * value;
				}
			}
		}
	}
}
