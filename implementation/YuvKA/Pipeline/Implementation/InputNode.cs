﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using YuvKA.VideoModel;
using System.Runtime.Serialization;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public abstract class InputNode : Node
	{
		[DataMember]
		public Size Size { get; set; }

		[Browsable(false)]
		public virtual int FrameCount { get { return 1; } }

		public sealed override Frame[] ProcessFrame(Frame[] inputs, int frameIndex)
		{
			return new[] { OutputFrame(frameIndex) };
		}

		public abstract Frame OutputFrame(int frameIndex);
	}
}
