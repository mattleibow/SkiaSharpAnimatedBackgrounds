using System;
using Xamarin.Forms;

namespace SkiaSharp.Views.Forms.AnimatedBackground
{
	public class SpotParticleRenderer : ParticleRenderer<SpotParticle, SpotParticleOptions>
	{
		private SKImage particleImage;
		private SKPaint _paint;

		private readonly Random random = new Random();

		public SpotParticleRenderer()
		{
			_paint = CreateDefaultPaint();
		}

		public SKPaint ParticlePaint
		{
			get => _paint;
			set
			{
				_paint = value ?? CreateDefaultPaint();

				if (_paint.StrokeWidth <= 0)
					_paint.StrokeWidth = 1;
			}
		}

		protected override void InitParticle(SpotParticle particle)
		{
			// position
			particle.Center = new Point(
				random.NextDouble() * CanvasBounds.Width,
				random.NextDouble() * CanvasBounds.Height);

			// size / radius
			var deltaRadius = Options.SpawnMaxRadius - Options.SpawnMinRadius;
			particle.Radius = random.NextDouble() * deltaRadius + Options.SpawnMinRadius;

			// direction & speed
			var deltaSpeed = Options.SpawnMaxSpeed - Options.SpawnMinSpeed;
			var speed = random.NextDouble() * deltaSpeed + Options.SpawnMinSpeed;
			var dirX = random.NextDouble() - 0.5;
			var dirY = random.NextDouble() - 0.5;
			var magSq = dirX * dirX + dirY * dirY;
			var mag = magSq <= 0 ? 1 : Math.Sqrt(magSq);
			particle.DirectionX = dirX / mag * speed;
			particle.DirectionY = dirY / mag * speed;

			// opacity
			var deltaOpacity = Options.MaxOpacity - Options.MinOpacity;
			particle.Opacity = Options.SpawnOpacity;
			particle.TargetOpacity = random.NextDouble() * deltaOpacity + Options.MinOpacity;
		}

		protected override void PaintParticle(SKCanvas canvas, SpotParticle particle)
		{
			if (particle.Opacity <= double.Epsilon)
				return;

			if (particleImage != null)
			{
				var dst = new SKRect(
					(float)(particle.Center.X - particle.Radius),
					(float)(particle.Center.Y - particle.Radius),
					(float)(particle.Center.X + particle.Radius),
					(float)(particle.Center.Y + particle.Radius));

				canvas.DrawImage(particleImage, dst, ParticlePaint);
			}
			else
			{
				ParticlePaint.Color = Options.BaseColor.ToSKColor().WithAlpha((byte)(particle.Opacity * 255));

				canvas.DrawCircle(
					particle.Center.ToSKPoint(),
					(float)particle.Radius,
					ParticlePaint);
			}
		}

		protected override void OnOptionsUpdate(SpotParticleOptions oldOptions, SpotParticleOptions newOptions)
		{
			base.OnOptionsUpdate(oldOptions, newOptions);

			if (!IsInitialized)
				return;

			particleImage = _options.Image;

			var minSpeedSqr = Options.SpawnMinSpeed * Options.SpawnMinSpeed;
			var maxSpeedSqr = Options.SpawnMaxSpeed * Options.SpawnMaxSpeed;
			foreach (var p in Particles)
			{
				// size / radius
				if (p.Radius < Options.SpawnMinRadius)
					p.Radius = Options.SpawnMinRadius;
				if (p.Radius > Options.SpawnMaxRadius)
					p.Radius = Options.SpawnMaxRadius;

				// speed
				var speedSqr = p.SpeedSquared;
				if (speedSqr > maxSpeedSqr)
					p.Speed = Options.SpawnMaxSpeed;
				else if (speedSqr < minSpeedSqr)
					p.Speed = Options.SpawnMinSpeed;

				// opacity
				if (p.Opacity > Options.MaxOpacity)
					p.Opacity = Options.MaxOpacity;
				else if (p.Opacity < Options.MinOpacity)
					p.Opacity = Options.MinOpacity;
			}
		}

		protected override void UpdateParticle(SpotParticle particle, TimeSpan delta)
		{
			if (!IsInitialized)
				return;

			var secondsDelta = delta.TotalSeconds;

			particle.Center = new Point(
				particle.Center.X + particle.DirectionX * secondsDelta,
				particle.Center.Y + particle.DirectionY * secondsDelta);

			if ((Options.OpacityChangeRate > 0 && particle.Opacity < particle.TargetOpacity) ||
				(Options.OpacityChangeRate < 0 && particle.Opacity > particle.TargetOpacity))
			{
				particle.Opacity += secondsDelta * Options.OpacityChangeRate;

				if ((Options.OpacityChangeRate > 0 && particle.Opacity > particle.TargetOpacity) ||
					(Options.OpacityChangeRate < 0 && particle.Opacity < particle.TargetOpacity))
					particle.Opacity = particle.TargetOpacity;
			}
		}

		private static SKPaint CreateDefaultPaint() =>
			new SKPaint
			{
				StrokeCap = SKStrokeCap.Round,
				Style = SKPaintStyle.Fill,
				StrokeWidth = 1
			};
	}
}
