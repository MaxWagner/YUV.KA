﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA.Pipeline.Implementation
{
	public class NoiseInputNode : InputNode
	{
		#region INode Members

		public override void ProcessFrame(int frameIndex)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}