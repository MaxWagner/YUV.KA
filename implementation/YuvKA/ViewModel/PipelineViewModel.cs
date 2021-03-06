﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using YuvKA.Pipeline;

namespace YuvKA.ViewModel
{
	/// <summary>
	/// View model that contains the nodes and edges shown to the user
	/// </summary>
	public class PipelineViewModel : ViewAware
	{
		NodeViewModel draggedNode;
		EdgeViewModel draggedEdge;
		NodeViewModel draggedEdgeEndNodeVM;
		Vector dragMouseOffset;
		IEnumerable<EdgeViewModel> edges = Enumerable.Empty<EdgeViewModel>();
		int maxZValue;

		public PipelineViewModel(MainViewModel parent)
		{
			Parent = parent;
			Nodes = new ObservableCollection<NodeViewModel>(Parent.Model.Graph.Nodes.Select(n => new NodeViewModel(n, this)));
		}

		public MainViewModel Parent { get; private set; }
		public IList<NodeViewModel> Nodes { get; private set; }
		public IEnumerable<EdgeViewModel> Edges
		{
			get
			{
				foreach (EdgeViewModel edge in edges)
					edge.Dispose();

				// Linq query... OF DEATH
				var newEdges =
					from node in Nodes
					from input in node.Inputs
					where !input.IsFake
					let iModel = ((Node.Input)input.Model)
					where iModel.Source != null
					let output = GetOutputViewModel(iModel.Source)
					select new EdgeViewModel { StartViewModel = input, EndViewModel = output };

				return edges = newEdges.ToList();
			}
		}

		public bool PipelineIsEmpty
		{
			get { return !Nodes.Any(); }
		}

		public EdgeViewModel DraggedEdge
		{
			get { return draggedEdge; }
			private set
			{
				draggedEdge = value;
				NotifyOfPropertyChange(() => DraggedEdge);
			}
		}

		/// <summary>
		/// Drops a NodeType on the pipeline by creating a new node of that type.
		/// </summary>
		public void Drop(IDragEventInfo e)
		{
			/* Only allow this if pipeline is not rendering */
			if (!Parent.Model.IsPlaying) {
				var type = e.GetData<NodeType>();
				var node = (Node)Activator.CreateInstance(type.Type);
				var nodeModel = new NodeViewModel(node, this);
				nodeModel.Position = e.GetPosition(relativeTo: this);
				nodeModel.ZIndex = maxZValue++;

				Parent.Model.Graph.AddNode(node);
				Nodes.Add(nodeModel);
				Parent.SaveSnapshot();
				NotifyOfPropertyChange("PipelineIsEmpty");
			}
		}

		/// <summary>
		/// Begins dragging the clicked node and pushes it in front of other nodes.
		/// </summary>
		public void NodeMouseDown(NodeViewModel node, IMouseEventInfo e)
		{
			draggedNode = node;
			dragMouseOffset = e.GetPosition(relativeTo: this) - draggedNode.Position;

			draggedNode.ZIndex = maxZValue++;
			draggedNode.NotifyOfPropertyChange(() => draggedNode.ZIndex);
		}

		/// <summary>
		/// Starts dragging the connected edge, if any, or a new one.
		/// </summary>
		public void InOutputMouseDown(InOutputViewModel inOut)
		{
			/* Only allow this if pipeline is not rendering */
			if (!Parent.Model.IsPlaying) {
				InOutputViewModel start = inOut;
				// If the input is already connected, drag the existing edge
				if (inOut.Model is Node.Input) {
					start = GetOutputViewModel(((Node.Input)inOut.Model).Source) ?? start;
					((Node.Input)inOut.Model).Source = null;
					NotifyOfPropertyChange(() => Edges);
					// remember the end of the grabbed edge
					draggedEdgeEndNodeVM = inOut.Parent;
				}
				DraggedEdge = new EdgeViewModel { StartViewModel = start, EndViewModel = inOut };
			}
		}

		/// <summary>
		/// Updates the position of the dragged edge or node.
		/// </summary>
		public void MouseMove(IMouseEventInfo e)
		{
			if (e.LeftButton != MouseButtonState.Pressed) {
				draggedNode = null;
				DraggedEdge = null;
				// removes unnecessary inputs
				CullInputs();
				return;
			}

			if (draggedNode != null)
				draggedNode.Position = e.GetPosition(relativeTo: this) - dragMouseOffset;
			else if (DraggedEdge != null) {
				if (DraggedEdge.Status != EdgeStatus.Indeterminate)
					DraggedEdge.Status = EdgeStatus.Indeterminate;
				DraggedEdge.EndPoint = e.GetPosition(relativeTo: this);
			}
		}

		/// <summary>
		/// Lets the dragged edge snap to the in-/output hovered over.
		/// </summary>
		public void InOutputMouseMove(InOutputViewModel inOut, RoutedEventArgs e)
		{
			if (DraggedEdge == null)
				return;

			InOutputViewModel inputVM, outputVM;
			DraggedEdge.EndViewModel = inOut;
			DraggedEdge.Status =
				DraggedEdge.GetInOut(out inputVM, out outputVM) && Parent.Model.Graph.CanAddEdge(outputVM.Parent.Model, inputVM.Parent.Model) ?
				EdgeStatus.Valid : EdgeStatus.Invalid;
			e.Handled = true; // don't bubble up into MouseMove
		}

		/// <summary>
		/// Cancels any drag operations.
		/// </summary>
		public void MouseUp()
		{
			CullInputs();
			draggedNode = null;
			DraggedEdge = null;
		}

		/// <summary>
		/// Connects the dragged edge to the in-/output, if valid.
		/// </summary>
		public void InOutputMouseUp(InOutputViewModel inOut)
		{
			if (DraggedEdge == null)
				return;

			DraggedEdge.EndViewModel = inOut;
			InOutputViewModel inputVM, outputVM;


			if (DraggedEdge.GetInOut(out inputVM, out outputVM)) {
				var output = (Node.Output)outputVM.Model;
				if (Parent.Model.Graph.CanAddEdge(output.Node, inputVM.Parent.Model)) {
					if (inputVM.IsFake)
						inputVM.Parent.AddInput(output);
					else
						((Node.Input)inputVM.Model).Source = output;

					CullInputs();
					NotifyOfPropertyChange(() => Edges);
					Parent.SaveSnapshot();
				}
			}
			DraggedEdge = null;
		}

		/// <summary>
		/// Culls inputs in all nodes.
		/// </summary>
		public void CullAllInputs()
		{
			foreach (NodeViewModel nodeViewModel in Nodes) {
				nodeViewModel.CullInputs();
			}
		}

		/// <summary>
		/// Prevents dropping nodes while the pipeline is playing.
		/// </summary>
		public void CheckClearance(IDragEventInfo e)
		{
			if (Parent.Model.IsPlaying) {
				e.Effects = DragDropEffects.None;
			}
		}

		InOutputViewModel GetOutputViewModel(Node.Output output)
		{
			return output == null ? null : Nodes.SelectMany(n => n.Outputs).Single(o => o.Model == output);
		}

		private void CullInputs()
		{
			if (draggedEdgeEndNodeVM != null)
				draggedEdgeEndNodeVM.CullInputs();
			draggedEdgeEndNodeVM = null;
		}
	}
}
