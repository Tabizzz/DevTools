using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Terraria.ModLoader;
namespace DevTools.Debugs;

internal class Framerate : IGui
{
	readonly List<float> framerates = new(61);

	int plot_frames = 30;

	public void Gui()
	{
		if (!TreeNodeEx("Framerate Plot", ImGuiNET.ImGuiTreeNodeFlags.DefaultOpen)) return;
		
		var io = GetIO();
		framerates.Add(io.Framerate);

		if (framerates.Count > plot_frames)
		{
			framerates.RemoveAt(0);
		}
		var len = Math.Min(plot_frames, framerates.Count);
		var framespan = new Span<float>(framerates.ToArray(), 0, len);
		PlotLines("Framerate", ref MemoryMarshal.GetReference(framespan), len, default, default, Math.Clamp(framerates.Min(), 0, 50), 60);
		framespan.Clear();

		SliderInt("Plot Frames", ref plot_frames, 10, 60);

		TreePop();
	}

	public void Load(Mod mod)
	{
		InfoWindow.Debugs.Add(this);
	}

	public void Unload()
	{
		InfoWindow.Debugs.Remove(this);
		framerates.Clear();
	}
}
