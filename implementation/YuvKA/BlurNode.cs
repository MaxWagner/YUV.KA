﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuvKA
{
	public class BlurNode : Node
	{
		public BlurType Type
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		public int Radius
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

		public override void ProcessFrame()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
