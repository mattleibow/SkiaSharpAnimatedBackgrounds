using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SkiaSharp.Views.Forms.AnimatedBackground
{
	public abstract class ParticleRenderer<TParticle, TOptions> : Renderer
		where TParticle : class, IParticle, new()
		where TOptions : class, IParticleOptions, new()
	{
		protected readonly List<TParticle> _particles = new List<TParticle>();
		protected TOptions _options;

		protected ParticleRenderer()
		{
		}

		public override bool IsInitialized =>
			Options != null && (_particles.Count > 0 || Options.ParticleCount == 0);

		public IEnumerable<TParticle> Particles => _particles;

		public TOptions Options
		{
			get => _options;
			set
			{
				var oldOptions = _options;
				_options = value ?? new TOptions();
				OnOptionsUpdate(oldOptions, _options);
			}
		}

		public override void Init()
		{
			if (IsInitialized)
				return;

			Options = new TOptions();
			_particles.AddRange(GenerateParticles(Options.ParticleCount));
		}

		public override void Paint(SKCanvas canvas)
		{
			if (!IsInitialized)
				return;

			foreach (var particle in Particles)
			{
				PaintParticle(canvas, particle);
			}
		}

		public override bool Tick(TimeSpan delta)
		{
			if (!IsInitialized)
				return false;

			foreach (var particle in Particles)
			{
				if (!CanvasBounds.IntersectsWith(particle.Bounds.ToSKRect()))
					InitParticle(particle);
				else
					UpdateParticle(particle, delta);
			}

			return true;
		}

		public override void Touch(Point location)
		{
			if (!IsInitialized)
				return;

			foreach (var particle in Particles)
			{
				if (particle.Bounds.Contains(location))
				{
					TouchParticle(particle, location);
				}
			}
		}

		protected abstract void PaintParticle(SKCanvas canvas, TParticle particle);

		protected abstract void InitParticle(TParticle particle);

		protected abstract void UpdateParticle(TParticle particle, TimeSpan delta);

		protected virtual void TouchParticle(TParticle particle, Point location)
		{
		}

		protected virtual void OnOptionsUpdate(TOptions oldOptions, TOptions newOptions)
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

		protected virtual IEnumerable<TParticle> GenerateParticles(int particleCount)
		{
			for (int i = 0; i < particleCount; i++)
			{
				var particle = new TParticle();
				InitParticle(particle);
				yield return particle;
			}
		}
	}
}
