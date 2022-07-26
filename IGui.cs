using Terraria.ModLoader;

namespace DevTools;

public interface IGui : ILoadable
{
	void Gui();
}

public interface IGui<T> : ILoadable
{
	void Gui(T tdata);
}
