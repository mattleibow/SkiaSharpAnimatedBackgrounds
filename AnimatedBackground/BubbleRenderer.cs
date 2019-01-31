using System;
using Xamarin.Forms;

namespace SkiaSharp.Views.Forms.AnimatedBackground
{
	public class BubbleRenderer : ParticleRenderer<Bubble, BubbleOptions>
	{
		private const double SqrtInverse = 0.707;

		private readonly Random random = new Random();

		private SKPaint _paint;

		public BubbleRenderer()
		{
			_paint = CreateDefaultPaint();
		}

		public event EventHandler BubblePopped;

		protected override void PaintParticle(SKCanvas canvas, Bubble particle)
		{
			_paint.Color = particle.Color.ToSKColor();

			if (!particle.Popping)
			{
				canvas.DrawCircle(particle.Center.ToSKPoint(), (float)particle.Radius, _paint);
			}
			else
			{
				var radiusSqrt = (float)(particle.Radius * SqrtInverse);
				var targetRadiusSqrt = (float)(particle.TargetRadius * SqrtInverse);
				var center = particle.Center.ToSKPoint();

				canvas.DrawLine(
					center + new SKPoint(radiusSqrt, radiusSqrt),
					center + new SKPoint(targetRadiusSqrt, targetRadiusSqrt),
					_paint);
				canvas.DrawLine(
					center + new SKPoint(radiusSqrt, -radiusSqrt),
					center + new SKPoint(targetRadiusSqrt, -targetRadiusSqrt),
					_paint);
				canvas.DrawLine(
					center + new SKPoint(-radiusSqrt, radiusSqrt),
					center + new SKPoint(-targetRadiusSqrt, targetRadiusSqrt),
					_paint);
				canvas.DrawLine(
					center + new SKPoint(-radiusSqrt, -radiusSqrt),
					center + new SKPoint(-targetRadiusSqrt, -targetRadiusSqrt),
					_paint);

				canvas.DrawLine(
					center + new SKPoint(0, radiusSqrt),
					center + new SKPoint(0, targetRadiusSqrt),
					_paint);
				canvas.DrawLine(
					center + new SKPoint(0, -radiusSqrt),
					center + new SKPoint(0, -targetRadiusSqrt),
					_paint);
				canvas.DrawLine(
					center + new SKPoint(-radiusSqrt, 0),
					center + new SKPoint(-targetRadiusSqrt, 0),
					_paint);
				canvas.DrawLine(
					center + new SKPoint(-radiusSqrt, -0),
					center + new SKPoint(-targetRadiusSqrt, -0),
					_paint);
			}
		}

		protected override void UpdateParticle(Bubble particle, TimeSpan delta)
		{
			var rate = particle.Popping ? Options.PopRate : Options.GrowthRate;
			particle.Radius += delta.TotalSeconds * rate;

			if (particle.Radius >= particle.TargetRadius)
			{
				if (particle.Popping)
					InitParticle(particle);
				else
					PopBubble(particle);
			}
		}

		protected override void InitParticle(Bubble particle)
		{
			// position
			particle.Center = new Point(
			  random.NextDouble() * CanvasBounds.Width,
			  random.NextDouble() * CanvasBounds.Height);

			// radius / size
			var deltaTargetRadius = Options.MaxTargetRadius - Options.MinTargetRadius;
			particle.TargetRadius = random.NextDouble() * deltaTargetRadius + Options.MinTargetRadius;
			particle.Radius = 0.0;

			// color
			particle.Color = new Color(
				random.NextDouble(),
				random.NextDouble(),
				random.NextDouble(),
				random.NextDouble() * 0.3 + 0.2);

			particle.Popping = false;
		}

		protected override void TouchParticle(Bubble particle, Point location)
		{
			var dx = location.X - particle.Center.X;
			var dy = location.Y - particle.Center.Y;
			var r = particle.Radius;

			// only pop if inside the bubble circle
			if (dx * dx + dy * dy < r * r)
				PopBubble(particle);
		}

		private void PopBubble(Bubble bubble)
		{
			bubble.Popping = true;
			bubble.Radius = 0.2 * bubble.TargetRadius;
			bubble.TargetRadius *= 0.5;

			BubblePopped?.Invoke(this, EventArgs.Empty);
		}

		private static SKPaint CreateDefaultPaint() =>
			new SKPaint
			{
				StrokeCap = SKStrokeCap.Round,
				Style = SKPaintStyle.Stroke,
				StrokeWidth = 3
			};
	}
}
