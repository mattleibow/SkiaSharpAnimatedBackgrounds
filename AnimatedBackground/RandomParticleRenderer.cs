using System;

namespace SkiaSharp.Views.Forms.AnimatedBackground
{
	public class RandomParticleRenderer : ParticleRenderer
	{
		private readonly Random random = new Random();

		protected override void InitParticle(Particle particle)
		{
			// position
			particle.CenterX = random.NextDouble() * CanvasBounds.Width;
			particle.CenterY = random.NextDouble() * CanvasBounds.Height;

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

		protected override void OnOptionsUpdate(ParticleOptions oldOptions, ParticleOptions newOptions)
		{
			base.OnOptionsUpdate(oldOptions, newOptions);

			if (!IsInitialized)
				return;

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
	}
}
