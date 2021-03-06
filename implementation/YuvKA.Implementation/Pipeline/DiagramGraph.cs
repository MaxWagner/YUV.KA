﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using Caliburn.Micro;

namespace YuvKA.Pipeline.Implementation
{
	/// <summary>
	/// Represents one graph for in the diagram of a DiagramNode
	/// </summary>
	[DataContract]
	public class DiagramGraph
	{
		/// <summary>
		/// Creates a new diagram graph with an empty list of values
		/// </summary>
		public DiagramGraph()
		{
			Data = new BindableCollection<KeyValuePair<int, double>>();
		}

		/// <summary>
		/// Gets or sets the video source of the data for the graph
		/// </summary>
		[DataMember]
		public YuvKA.Pipeline.Node.Input Video { get; set; }

		/// <summary>
		/// Gets the type of the diagram graph
		/// </summary>
		[DataMember]
		public IGraphType Type { get; set; }

		/// <summary>
		/// Gets the values of the graph. The X-values are the frames and
		/// the Y-values are calculated by according to the Type.
		/// </summary>
		public BindableCollection<KeyValuePair<int, double>> Data { get; private set; }

		[OnDeserialized]
		public void OnDeserialized(StreamingContext ctx)
		{
			Data = new BindableCollection<KeyValuePair<int, double>>();
		}
	}
}
