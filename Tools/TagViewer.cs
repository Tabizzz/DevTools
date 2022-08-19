using ImGuiNET;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DevTools.Tools;

internal class TagViewer : IGui
{
	public static bool Open;

	private static TagCompound tag;
	static bool needfocus;

	public static void OpenTag(TagCompound tagcompound)
	{
		Open = true;
		needfocus = true;
		tag = tagcompound;
		ConvertListToTag(tag);
	}

	private static void ConvertListToTag(TagCompound rtag)
	{
		foreach (var item in rtag)
		{
			if (item.Value is TagCompound sub)
				ConvertListToTag(sub);
			else if (item.Value is List<TagCompound> tags)
			{
				var subtag = new TagCompound();
				var l = 0;
				foreach (var subt in tags)
				{
					subtag.Add((l++).ToString(), subt);
				}
				rtag.Set(item.Key, subtag, true);
			}
		}
	}

	public static void OpenTag(IEnumerable<TagCompound> tagcompound, Func<TagCompound, string> nameSelector)
	{
		var ttag = new TagCompound();
		foreach (var sub in tagcompound)
		{
			tag.Add(nameSelector(sub), sub);
		}
		OpenTag(ttag);
	}

	public void Gui()
	{
		SetNextWindowSize(new ImVect2(430, 450), ImGuiCond.FirstUseEver);
		if(needfocus)
		{
			needfocus = false;
			SetNextWindowFocus();
		}

		if (!Open || !Begin("Tag Viewer", ref Open))
		{
			End();
			return;
		}

		if(tag == null)
		{
			TextWrapped("Use other tools to open a TagCompund here.");

		}
		else
		{
			PushStyleVar(ImGuiStyleVar.FramePadding, new ImVect2(2, 2));

			if(BeginTable("slit", 2, ImGuiTableFlags.BordersOuter | ImGuiTableFlags.Resizable))
			{
				foreach (var item in tag)
				{
					Show(item);
				}
				EndTable();
			}

			PopStyleVar();
		}

		End();
	}

	private void ShowItem(string key, object value)
	{
		TableNextRow();
		TableSetColumnIndex(0);
		AlignTextToFramePadding();
		var flags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.Bullet;
		TreeNodeEx(key, flags);
		TableSetColumnIndex(1);
		Text(value?.ToString() ?? "null");
		NextColumn();

	}

	private void ShowPlaceholder(string key, TagCompound value)
	{
		TableNextRow();
		TableSetColumnIndex(0);
		AlignTextToFramePadding();
		var nodeopen = TreeNode(key);

		TableSetColumnIndex(1);
		Text("TagCompound");

		if(nodeopen)
		{
			foreach (var item in value)
			{
				Show(item);
			}

			TreePop();
		}

	}

	private void Show(KeyValuePair<string, object> item)
	{
		if (item.Value is TagCompound pairs)
			ShowPlaceholder(item.Key, pairs);
		else
			ShowItem(item.Key, item.Value);
	}

	public void Load(Mod mod)
	{
		InfoWindow.Guis.Add(this);
	}

	public void Unload()
	{
		InfoWindow.Guis.Add(this);
	}
}

class SavePreview : GlobalItem
{

}