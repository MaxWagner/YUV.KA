﻿using System.Windows;
using System.Windows.Input;

namespace Caliburn.Micro
{
	/// <summary>
	/// Provides window screen positions relative to a view model without depending on its view.
	/// </summary>
	public interface IGetPosition
	{
		Point GetMousePosition(MouseEventArgs e, IViewAware relativeTo);
		Point GetDropPosition(DragEventArgs e, IViewAware relativeTo);
	}
}
