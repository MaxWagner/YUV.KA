﻿using System;
using System.Runtime.Serialization;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	[DataContract]
	public class ColorInputNode : InputNode
	{
		public ColorInputNode() : base(outputCount: 1)
		{
		}

		[DataMember]
		public Rgb Color
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		#region INode Members

		public override Frame OutputFrame(int tick)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}