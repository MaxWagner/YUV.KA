using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using YuvKA.Pipeline;
using YuvKA.Pipeline.Implementation;

namespace YuvKA.Implementation
{
	public class GraphControl : INotifyPropertyChanged
	{
		
		private RelayCommand delete;
		private Tuple<string, IGraphType> chosenType;
		private List<System.Drawing.Color> lineColors;
		private bool referenceSet;

		public event PropertyChangedEventHandler PropertyChanged;
		
		public GraphControl()
		{
			Events = IoC.Get<IEventAggregator>();
		}
		
		[Import]
		IEventAggregator Events { get; set; }

		public ICommand Delete
		{
			get { return delete ?? (delete = new RelayCommand(param => Del(), param => true)); }
		}

		public Tuple<string, Node.Input> Video { get; set; }

		public DiagramGraph Graph { get; set; }

		public ObservableCollection<Tuple<string, IGraphType>> Types { get; set; }

		public ObservableCollection<Tuple<string, IGraphType>> DisplayTypes { get; set; }

		public Tuple<string, IGraphType> ChosenType
		{
			get { return chosenType; }
			set
			{
				if (chosenType == null) {
					chosenType = value;
					Graph.Type = value.Item2;
					SetLineColor();
					Events.Publish(new GraphTypeChosenMessage(this));
				} else {
					chosenType = value;
					Graph.Type = value.Item2;
					SetLineColor();
				}
			}
		}

		public List<Color> TypeColors { get; set; }

		public List<System.Drawing.Color> LineColors 
		{
 			get { return lineColors ?? (lineColors = new List<System.Drawing.Color>()); }
			set { lineColors = value; }
		} 

		public Color LineColor { get; set; }

		public bool ReferenceSet
		{
			get { return referenceSet; }
			set
			{
				referenceSet = value;
				SetDisplayTypes();
			}
		}

		public SolidColorBrush GraphColor { get; set; }
		
		protected virtual void OnPropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler == null)
				return;
			var e = new PropertyChangedEventArgs(propertyName);
			handler(this, e);
		}
		
		public void SetLineColor()
		{
			LineColor = NewColor(ChosenType);
			GraphColor = new SolidColorBrush(LineColor);
			OnPropertyChanged("LineColor");
			OnPropertyChanged("GraphColor");
		}

		public Color NewColor(Tuple<string, IGraphType> type)
		{
			System.Drawing.Color newColor;
			var typeCollection = new ObservableCollection<Tuple<string, IGraphType>>(Types);
			var baseColor = System.Drawing.Color.FromArgb(TypeColors[typeCollection.IndexOf(type)].A, TypeColors[typeCollection.IndexOf(type)].R, TypeColors[typeCollection.IndexOf(type)].G, TypeColors[typeCollection.IndexOf(type)].B);

			var h = baseColor.GetHue();

			var random = new Random();
			do {
				newColor = HslHelper.HslToRgb(h, random.NextDouble(), random.NextDouble());
			} while (LineColors.Contains(newColor));
			LineColors.Add(newColor);

			return Color.FromArgb(newColor.A, newColor.R, newColor.G, newColor.B);
		}

		public void SetDisplayTypes()
		{
			var newTypes = new ObservableCollection<Tuple<string, IGraphType>>();
			if (ReferenceSet) {
				foreach (var t in Types.Where(type => type.Item2.DependsOnReference))
					newTypes.Add(t);
			}
			if (Video.Item2.Source.Node.OutputHasLogfile) {
				foreach (var t in Types.Where(type => type.Item2.DependsOnLogfile))
					newTypes.Add(t);
			}
			DisplayTypes = newTypes;
			OnPropertyChanged("DisplayTypes");
		}

		public void Del()
		{
			Events.Publish(new DeleteGraphControlMessage(this));
		}
	}
}