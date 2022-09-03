using DevTools.Utils;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;
using ImGuiNET;
using ImGuiNET.Extras;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria.ModLoader;

namespace DevTools.Tools;

internal class ModAssembly : IGui
{
	public static bool Open ;
	public static Mod selected_mod;
	private FullTypeName seleted_item;
	TextEditor editor;

	public void Gui()
	{
		if (!Open) return;
		if(Begin("Mod Assembly", ref Open))
		{
			if (BeginChild("tree", new Vector2(250, 0), true, ImGuiWindowFlags.HorizontalScrollbar))
			{
				foreach (var cmod in ModLoader.Mods)
				{
					Show(cmod);
				}

				EndChild();
			}
			
			SameLine();
			PushFont(GetIO().Fonts.Fonts[0]);
			editor.Render("code", new System.Numerics.Vector2(0, -1), border: true);
			PopFont();
		}
		End();
	}

	private void Show(Mod mod)
	{
		if(TreeNode(mod.Name))
		{
			var dec = Decompiler.DecompileMod(mod);
			INamespace ns;
			if(mod.Name == "ModLoader")
			{
				ns = dec.TypeSystem.RootNamespace;
			}
			else
			{
				ns = dec.TypeSystem.RootNamespace.GetChildNamespace(mod.Name) ?? dec.TypeSystem.RootNamespace.ChildNamespaces.First();
			}

			if (ns != null)
			{
				foreach (var cns in ns.ChildNamespaces)
				{
					Show(cns, dec, mod);
				}
				foreach (var item in ns.Types)
				{
					Show(item, dec, mod);
				}
			}
			TreePop();
		}
	}

	private void Show(ITypeDefinition item, CSharpDecompiler dec, Mod mod)
	{
		if(Selectable(item.MetadataName, mod.Name == selected_mod?.Name && item.ReflectionName == seleted_item.ReflectionName))
		{
			selected_mod = mod;
			seleted_item = new FullTypeName(item.ReflectionName);

			editor.Text = dec.DecompileTypeAsString(seleted_item);
		}
	}

	private void Show(INamespace ns, CSharpDecompiler dec, Mod mod)
	{
		if (TreeNode(ns.Name))
		{
			foreach (var cns in ns.ChildNamespaces)
			{
				Show(cns, dec, mod);
			}
			foreach (var item in ns.Types)
			{
				Show(item, dec, mod);
			}
			TreePop();
		}
	}

	public void Load(Mod mod)
	{
		InfoWindow.Guis.Add(this);
		editor = new()
		{
			ReadOnly = true,
			Text = "Choose an item from the left pane to see the code"
		};
	}

	public void Unload()
	{
		InfoWindow.Guis.Remove(this);
		editor.Dispose();
		selected_mod = null;
	}
}
