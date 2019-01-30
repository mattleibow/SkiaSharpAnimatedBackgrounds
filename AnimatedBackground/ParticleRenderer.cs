using System;
using System.Collections.Generic;

namespace SkiaSharp.Views.Forms.AnimatedBackground
{
	public abstract class ParticleRenderer : Renderer
	{
		private SKImage? particleImage;

		private SKPaint _paint;
		private ParticleOptions _options;
		private readonly List<Particle> _particles;

		public ParticleRenderer()
		{
			// initialize with defaults
			_options = CreateDefaultParticleOptions();
			_paint = CreateDefaultPaint();
			_particles = new List<Particle>();
		}

		public override bool IsInitialized =>
			Options != null && (_particles.Count > 0 || Options.ParticleCount == 0);

		public IEnumerable<Particle> Particles => _particles;

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

		public ParticleOptions Options
		{
			get => _options;
			set
			{
				var oldOptions = _options;

				_options = value ?? CreateDefaultParticleOptions();

				particleImage = _options.Image;

				OnOptionsUpdate(oldOptions, _options);
			}
		}

		public override void Init()
		{
			Options = CreateDefaultParticleOptions();
			_particles.AddRange(GenerateParticles(Options!.ParticleCount));
		}

		public override void Paint(SKCanvas canvas)
		{
			if (!IsInitialized)
				return;

			foreach (var particle in Particles!)
			{
				if (particle.Opacity <= double.Epsilon)
					continue;

				if (particleImage != null)
				{
					var dst = new SKRect(
						(float)(particle.CenterX - particle.Radius),
						(float)(particle.CenterY - particle.Radius),
						(float)(particle.CenterX + particle.Radius),
						(float)(particle.CenterY + particle.Radius));

					canvas.DrawImage(particleImage, dst, ParticlePaint);
				}
				else
				{
					ParticlePaint!.Color = Options!.BaseColor.ToSKColor().WithAlpha((byte)(particle.Opacity * 255));

					canvas.DrawCircle(
						new SKPoint((float)particle.CenterX, (float)particle.CenterY),
						(float)particle.Radius,
						ParticlePaint);
				}
			}
		}

		public override bool Tick(TimeSpan delta)
		{
			if (!IsInitialized)
				return false;

			foreach (var particle in Particles!)
			{
				if (!CanvasBounds.Contains(new SKPoint((float)particle.CenterX, (float)particle.CenterY)))
					InitParticle(particle);
				else
					UpdateParticle(particle, delta);
			}

			return true;
		}

		protected abstract void InitParticle(Particle particle);

		protected virtual void OnOptionsUpdate(ParticleOptions? oldOptions, ParticleOptions newOptions)
		{
			if (!IsInitialized)
				return;

			if (_particles.Count > newOptions.ParticleCount)
			{
				var extra = _particles.Count - newOptions.ParticleCount;
				_particles.RemoveRange(0, extra);
			}
			else if (_particles.Count < newOptions.ParticleCount)
			{
				var extra = newOptions.ParticleCount - _particles.Count;
				_particles.AddRange(GenerateParticles(extra));
			}
		}

		private IEnumerable<Particle> GenerateParticles(int particleCount)
		{
			for (int i = 0; i < particleCount; i++)
			{
				var particle = new Particle();
				InitParticle(particle);
				yield return particle;
			}
		}

		private void UpdateParticle(Particle particle, TimeSpan delta)
		{
			if (!IsInitialized)
				return;

			var secondsDelta = delta.TotalSeconds;

			particle.CenterX += particle.DirectionX * secondsDelta;
			particle.CenterY += particle.DirectionY * secondsDelta;

			if ((Options!.OpacityChangeRate > 0 && particle.Opacity < particle.TargetOpacity) ||
				(Options!.OpacityChangeRate < 0 && particle.Opacity > particle.TargetOpacity))
			{
				particle.Opacity += secondsDelta * Options.OpacityChangeRate;

				if ((Options.OpacityChangeRate > 0 && particle.Opacity > particle.TargetOpacity) ||
					(Options.OpacityChangeRate < 0 && particle.Opacity < particle.TargetOpacity))
					particle.Opacity = particle.TargetOpacity;
			}
		}

		private static ParticleOptions CreateDefaultParticleOptions() =>
			new ParticleOptions();

		private static SKPaint CreateDefaultPaint() =>
			new SKPaint
			{
				StrokeCap = SKStrokeCap.Round,
				Style = SKPaintStyle.Fill,
				StrokeWidth = 1
			};
	}
}
