﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace YuvKA.ViewModel
{
	public class ToolboxViewModel
	{
		public ObservableCollection<NodeTypeViewModel> NodeTypes { get; private set; }
	}
}