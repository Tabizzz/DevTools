using DevTools.Editors;
using DevTools.Explorers;
using DevTools.Tools;
using Newtonsoft.Json;

namespace DevTools;

[JsonObject(MemberSerialization.OptOut)]
public class Preferences
{
	public HitboxesPref hitboxes { get; set; } = new();
	public EditorsPref editors { get; set; } = new();
	public ExplorersPref explorers { get; set; } = new();
	public ToolsPref tools { get; set; } = new();
}

public class ToolsPref
{
	public TileFinderPref tileFinder { get; set; } = new();
	public WorldInfoPref worldInfo { get; set; } = new();
}

public class WorldInfoPref
{
	public bool Open { get => WorldInfo.Open; set => WorldInfo.Open = value; }
}

public class TileFinderPref
{
	public bool Open { get => TileFinder.Open; set => TileFinder.Open = value; }
	public bool closest { get => TileFinder.closest; set => TileFinder.closest = value; }
	public bool TileFrameX { get => TileFinder.TileFrameX; set => TileFinder.TileFrameX = value; }
	public bool TileFrameY { get => TileFinder.TileFrameY; set => TileFinder.TileFrameY = value; }
	public int tileId { get => TileFinder.tileId; set => TileFinder.tileId = value; }
	public int iTileFrameX { get => TileFinder.iTileFrameX; set => TileFinder.iTileFrameX = value; }
	public int iTileFrameY { get => TileFinder.iTileFrameY; set => TileFinder.iTileFrameY = value; }
}

public class ExplorersPref
{
	public bool Open { get => NpcExplorer.Open; set => NpcExplorer.Open = value; }
	public bool f_private { get => NpcExplorer.f_private; set => NpcExplorer.f_private = value; }
	public bool f_instance { get => NpcExplorer.f_instance; set => NpcExplorer.f_instance = value; }
	public bool f_readonly { get => NpcExplorer.f_readonly; set => NpcExplorer.f_readonly = value; }
	public bool f_editable { get => NpcExplorer.f_editable; set => NpcExplorer.f_editable = value; }
	public bool f_public { get => NpcExplorer.f_public; set => NpcExplorer.f_public = value; }
	public bool AnimateNpcTexture { get => NpcExplorer.AnimateNpcTexture; set => NpcExplorer.AnimateNpcTexture = value; }
	public int current_add_buff_time { get => NpcExplorer.current_add_buff_time; set => NpcExplorer.current_add_buff_time = value; }
	public bool NpcSelector { get => NpcExplorerSelector.NpcSelector; set => NpcExplorerSelector.NpcSelector = value; }
}

public class EditorsPref
{
	public ItemEditorPref itemEditor { get; set; } = new();
}

public class ItemEditorPref
{
	public bool Open { get => ItemEditor.Open; set => ItemEditor.Open = value; }
	public bool f_private { get => ItemEditor.f_private; set => ItemEditor.f_private = value; }
	public bool f_instance { get => ItemEditor.f_instance; set => ItemEditor.f_instance = value; }
	public bool f_readonly { get => ItemEditor.f_readonly; set => ItemEditor.f_readonly = value; }
	public bool f_editable { get => ItemEditor.f_editable; set => ItemEditor.f_editable = value; }
	public bool f_public { get => ItemEditor.f_public; set => ItemEditor.f_public = value; }
}


public class HitboxesPref
{
	public bool Open { get => Hitboxes.Open; set => Hitboxes.Open = value; }
	public bool Npc { get => Hitboxes.Npc; set => Hitboxes.Npc = value; }
	public bool Projectiles { get => Hitboxes.Projectiles; set => Hitboxes.Projectiles = value; }
	public bool Players { get => Hitboxes.Players; set => Hitboxes.Players = value; }
}