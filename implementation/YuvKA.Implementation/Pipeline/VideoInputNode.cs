﻿using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using YuvKA.VideoModel;

namespace YuvKA.Pipeline.Implementation
{
	/// <summary>
	/// This class can stream videos from YUV-files and annotate the resultframes
	/// with Data from logfiles and motionvectorfiles.
	/// </summary>
	[DataContract]
	[Description("This Node can stream videos from YUV-files and read out additional Data with Log- and Motionfiles.")]
	public class VideoInputNode : InputNode
	{
		static readonly Size Cif = new Size(352, 288);
		static readonly Size Sif = new Size(352, 240);
		static readonly Size Qcif = new Size(176, 144);

		FilePath fileName = new FilePath(null);
		FilePath logFileName = new FilePath(null);
		FilePath motionVectorFileName = new FilePath(null);
		YuvEncoder.Video input;

		public VideoInputNode()
			: base(outputCount: 1)
		{
			Name = "Video";
		}

		/// <summary>
		/// The path of the YUV-file which shall be streamed
		/// </summary>
		[DisplayName("Video File")]
		[FilePath.ExtensionFilter("YUV-Video|*.yuv|All files (*.*)|*")]
		[DataMember]
		[Browsable(true)]
		public FilePath FileName
		{
			get { return fileName; }
			set
			{
				fileName = value;
				input = null;

				if (value.Path != null) {
					// Try and guess the video resolution
					Match m = Regex.Match(fileName.Path, @"(\d+)x(\d+)");
					if (m.Success)
						Size = new Size(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value));
					else if (Regex.IsMatch(fileName.Path, @"[_.]cif[_.]"))
						Size = Cif;
					else if (Regex.IsMatch(fileName.Path, @"[_.]sif[_.]"))
						Size = Sif;
					else if (Regex.IsMatch(fileName.Path, @"[_.]qcif[_.]"))
						Size = Qcif;
					NotifyOfPropertyChange(() => FileName);
					NotifyOfPropertyChange(() => Size);
					NotifyOfPropertyChange(() => TickCount);
				}
			}
		}

		/// <summary>
		/// The path of the Logfile
		/// </summary>
		[DisplayName("(Log File)")]
		[FilePath.ExtensionFilter("Log File|*.dat|All files (*.*)|*")]
		[DataMember]
		[Browsable(true)]
		public FilePath LogFileName
		{
			get { return logFileName; }
			set
			{
				logFileName = value;
				input = null;
			}
		}

		public override bool OutputHasLogfile
		{
			get { return File.Exists(LogFileName.Path); }
		}

		/// <summary>
		/// The path of the motionvectorfile
		/// </summary>
		[DisplayName("(Motion Vector File)")]
		[FilePath.ExtensionFilter("Motion Vector File|*.csv|All files (*.*)|*")]
		[DataMember]
		[Browsable(true)]
		public FilePath MotionVectorFileName
		{
			get { return motionVectorFileName; }
			set
			{
				motionVectorFileName = value;
				input = null;
			}
		}

		public override bool OutputHasMotionVectors
		{
			get { return File.Exists(MotionVectorFileName.Path); }
		}

		public override int? TickCount
		{
			get
			{
				if (InputIsValid) {
					EnsureInputLoaded();
					return input.FrameCount;
				}
				return null;
			}
		}

		public override bool InputIsValid
		{
			get
			{
				return File.Exists(fileName.Path) && (logFileName.Path == null || File.Exists(logFileName.Path));
			}
		}

		public override Frame OutputFrame(int tick)
		{
			EnsureInputLoaded();

			if (tick < 0 || tick >= input.FrameCount)
				return new Frame(Size);

			return input[tick];
		}

		protected override void OnSizeChanged()
		{
			base.OnSizeChanged();
			input = null;
		}

		private void EnsureInputLoaded()
		{
			if (input == null)
				input = YuvEncoder.Decode(Size.Width, Size.Height, FileName.Path, LogFileName.Path, MotionVectorFileName.Path);
		}
	}
}
