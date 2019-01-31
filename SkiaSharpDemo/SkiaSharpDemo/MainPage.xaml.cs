using System.Collections.Generic;
using System.Linq;
using SkiaSharp.Views.Forms.AnimatedBackground;
using Xamarin.Forms;

namespace SkiaSharpDemo
{
	public partial class MainPage : ContentPage
	{
		private readonly Dictionary<string, Renderer> _renderers = new Dictionary<string, Renderer>
		{
			{ "Spots", new SpotParticleRenderer() },
			{ "Bubbles", new BubbleRenderer() }
		};

		private int _index;
		private Thickness _safeAreaInsets;

		public MainPage()
		{
			InitializeComponent();

			Renderers = _renderers.Keys.ToArray();
			SelectedIndex = 0;

			BindingContext = this;
		}

		public string[] Renderers { get; }

		public int SelectedIndex
		{
			get => _index;
			set
			{
				_index = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(SelectedRenderer));
			}
		}

        public Thickness SafeAreaInsets
		{
			get => _safeAreaInsets;
			set
			{
				_safeAreaInsets = value;
				OnPropertyChanged();
			}
		}

		public Renderer SelectedRenderer =>
			_renderers.Values.Skip(_index).FirstOrDefault();
	}
}
