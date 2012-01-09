﻿using System;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	public class IntraBlockFrequency : IGraphType
	{
		public IntraBlockFrequency()
		{
			DependsOnReference = false;
		}

		public bool DependsOnReference { get; private set; }

		public double Process(Frame frame, Frame reference)
		{
			double IntraBlocks = 0.0;
			if (frame is AnnotatedFrame) {
				AnnotatedFrame AnnFrame = (AnnotatedFrame)frame;
				foreach (MacroblockDecision d in AnnFrame.Decisions) {
					if (d.PartitioningDecision == MacroblockPartitioning.Intra16x16 | d.PartitioningDecision == MacroblockPartitioning.Intra4x4 | d.PartitioningDecision == MacroblockPartitioning.Intra8x8 | d.PartitioningDecision == MacroblockPartitioning.IntraPCM) {
						IntraBlocks++;
					}
				}
			}
			return IntraBlocks;
		}
	}

	public class PeakSignalNoiseRatio : IGraphType
	{
		public PeakSignalNoiseRatio()
		{
			DependsOnReference = true;
		}

		public bool DependsOnReference { get; private set; }

		public double Process(Frame frame, Frame reference)
		{
			double MSE = 0.0;
			for (int i = 0; i < frame.Size.Height; i++) {
				for (int j = 0; j < frame.Size.Width; j++) {
					MSE += Math.Pow(((frame[i, j].R + frame[i, j].G + frame[i, j].B) - (reference[i, j].R + reference[i, j].G + reference[i, j].B)), 2);
				}
			}
			MSE *= (double)1 / (3 * frame.Size.Height * frame.Size.Width);
			if (MSE == 0.0)
				return 0.0;
			double PSNR = 10 * Math.Log10((Math.Pow((Math.Pow(2, 24) - 1), 2)) / MSE);
			return PSNR;
		}
	}

	public class PixelDiff : IGraphType
	{
		public PixelDiff()
		{
			DependsOnReference = true;
		}

		public bool DependsOnReference { get; private set; }

		public double Process(Frame frame, Frame reference)
		{
			double difference = 0.0;
			for (int x = 0; x < frame.Size.Width; x++) {
				for (int y = 0; y < frame.Size.Height; y++) {
					difference += Math.Abs(frame[x, y].R - reference[x, y].R) + Math.Abs(frame[x, y].G - reference[x, y].G) +
						Math.Abs(frame[x, y].B - reference[x, y].B);
				}
			}
			return (double)difference / 3;
		}
	}
}
