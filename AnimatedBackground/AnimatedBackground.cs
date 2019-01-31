using System;
using Xamarin.Forms;

namespace SkiaSharp.Views.Forms.AnimatedBackground
{
	public class AnimatedBackground : SKCanvasView
	{
		public static readonly BindableProperty RendererProperty = BindableProperty.Create(
			nameof(Renderer), typeof(Renderer), typeof(AnimatedBackground), propertyChanged: OnRendererChanged);

		private long lastTicks = -1;
		private double scale = 1.0;

		public AnimatedBackground()
		{
			EnableTouchEvents = true;
			SizeChanged += OnSizeChanged;

			Device.StartTimer(TimeSpan.FromMilliseconds(1000.0 / 60.0), OnTimerTick);
		}

		protected override void OnTouch(SKTouchEventArgs e)
		{
			base.OnTouch(e);

			Renderer?.Touch(new Point(e.Location.X / scale, e.Location.Y / scale));
		}

		private void OnSizeChanged(object sender, EventArgs e)
		{
			if (Renderer != null)
				Renderer.CanvasBounds = new SKRect(0, 0, (float)Width, (float)Height);
		}

		public Renderer Renderer
		{
			get => (Renderer)GetValue(RendererProperty);
			set => SetValue(RendererProperty, value);
		}

		protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
		{
			var canvas = e.Surface.Canvas;

			canvas.Clear(SKColors.Transparent);

			if (Renderer?.IsInitialized == false)
				Renderer?.Init();

			scale = e.Info.Width / Width;
			canvas.Scale((float)scale);

			Renderer?.Paint(canvas);
		}

		private bool OnTimerTick()
		{
			if (Renderer?.IsInitialized == true)
			{
				var currentTicks = DateTime.Now.Ticks;

				// first time
				if (lastTicks == -1)
					lastTicks = currentTicks;

				var delta = TimeSpan.FromTicks(currentTicks - lastTicks);

				if (Renderer?.Tick(delta) == true)
					InvalidateSurface();

				lastTicks = currentTicks;
			}

			return true;
		}

		private static void OnRendererChanged(BindableObject bindable, object oldValue, object newValue)
		{
			if (bindable is AnimatedBackground background)
				background.InvalidateSurface();
		}
	}
}
