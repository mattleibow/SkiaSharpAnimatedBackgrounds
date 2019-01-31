using System;
using Xamarin.Forms;

namespace SkiaSharp.Views.Forms.AnimatedBackground
{
	public abstract class Renderer : BindableObject
	{
		public abstract bool IsInitialized { get; }

		public SKRect CanvasBounds { get; internal set; }

		public abstract void Init();

		public abstract bool Tick(TimeSpan delta);

		public abstract void Paint(SKCanvas canvas);

		public abstract void Touch(Point location);
	}
}
