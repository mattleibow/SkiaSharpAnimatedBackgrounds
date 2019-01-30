using System;

namespace SkiaSharp.Views.Forms.AnimatedBackground
{
	public class Particle
	{
		public double CenterX { get; set; } = 0.0;

		public double CenterY { get; set; } = 0.0;

		public double DirectionX { get; set; } = 0.0;

		public double DirectionY { get; set; } = 1.0;

		public double Radius { get; set; } = 0.0;

		public double Opacity { get; set; } = 0.0;

		public double TargetOpacity { get; set; } = 0.0;

		public object? UserState { get; set; }

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
