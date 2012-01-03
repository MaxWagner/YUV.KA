﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using Caliburn.Micro;

namespace YuvKA.Pipeline
{
	[DataContract]
	public class PipelineState
	{
		[DataMember]
		public int CurrentTick { get; set; }

		/// <summary>
		/// Replay speed in frames per second
		/// </summary>
		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public PipelineGraph Graph { get; private set; }

		[Import(typeof(IEventAggregator))]
		public IEventAggregator Events { get; set; }

		public void Start(IEnumerable<Node> outputNodes)
		{
			//PipelineDriver.RenderTicks(outputNodes, CurrentTick, null);
		}

		public void Stop()
		{
			throw new System.NotImplementedException();
		}

		public void RenderTick(IEnumerable<Node> outputNodes)
		{
			//PipelineDriver.RenderTicks(outputNodes, CurrentTick, null);
		}
	}
}
