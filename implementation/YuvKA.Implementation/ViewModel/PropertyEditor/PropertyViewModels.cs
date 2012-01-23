﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Media;
using Caliburn.Micro;
using YuvKA.VideoModel;

namespace YuvKA.ViewModel.PropertyEditor.Implementation
{
	public class BooleanPropertyViewModel : PropertyViewModel<bool>
	{
	}

	public class PathPropertyViewModel : PropertyViewModel<Pipeline.FilePath>
	{
		public IEnumerable<IResult> OpenDialog()
		{
			var file = new ChooseFileResult { Filter = "YUV-Video|*.yuv|All files (*.*)|*" };
			yield return file;
			Value = new YuvKA.Pipeline.FilePath(file.FileName);
		}
	}

	public class RgbPropertyViewModel : PropertyViewModel<VideoModel.Rgb>
	{
		public Color ChosenColor
		{
			get { return Color.FromRgb(Value.R, Value.G, Value.B); }
			set { Value = new Rgb(value.R, value.G, value.B); }
		}
	}

	public class SizePropertyViewModel : PropertyViewModel<Size>
	{
		public int Width
		{
			get { return Value.Width; }
			set { Value = new Size(value, Height); }
		}

		public int Height
		{
			get { return Value.Height; }
			set { Value = new Size(Width, value); }
		}

		protected override void OnValueChanged()
		{
			base.OnValueChanged();
			NotifyOfPropertyChange(() => Width);
			NotifyOfPropertyChange(() => Height);
		}
	}

	public abstract class NumericalPropertyViewModel<T> : PropertyViewModel<T>
	{
		public double Minimum
		{
			get { return (double)(Property.Attributes.OfType<RangeAttribute>().First().Minimum); }
			private set { }
		}
		public double Maximum
		{
			get { return (double)(Property.Attributes.OfType<RangeAttribute>().First().Maximum); }
			private set { }
		}
	}

	public class IntPropertyViewModel : NumericalPropertyViewModel<int>
	{
	}

	public class DoublePropertyViewModel : NumericalPropertyViewModel<double>
	{
	}

	public class ObservableCollectionOfDoublePropertyViewModel : PropertyViewModel<ObservableCollection<double>>
	{
	}

	public class EnumerationPropertyViewModel : PropertyViewModel<Enum>
	{
		public System.Array Choices
		{
			get { return Enum.GetValues(Property.PropertyType); }
			private set { }
		}
	}

	public class OutputWindowViewModelPropertyViewModel : PropertyViewModel<OutputWindowViewModel> { }
}
