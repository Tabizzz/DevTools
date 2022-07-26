global using static DevTools.Extensions;
global using Vector2 = Microsoft.Xna.Framework.Vector2;
global using ImVect2 = System.Numerics.Vector2;
global using ImVect4 = System.Numerics.Vector4;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Terraria;

namespace DevTools;

internal static class Extensions
{
	public static ImVect2 Convert(this Vector2 vect)
	{
		return new ImVect2(vect.X, vect.Y);
	}

	public static ImVect4 Convert(this Vector4 vect)
	{
		return new ImVect4(vect.X, vect.Y, vect.Z, vect.W);
	}

	public static Vector2 Convert(this ImVect2 vect)
	{
		return new Vector2(vect.X, vect.Y);
	}

	public static void AddHitBox(this ImDrawListPtr ptr, Rectangle rectangle, Color red, Color yellow)
	{
		yellow.A /= 3;
		var rect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);

		if (rect.Intersects(rectangle))
		{
			var orig = new Vector2(rectangle.X, rectangle.Y) - Main.screenPosition;
			var end = orig + new Vector2(rectangle.Width, rectangle.Height);

			ptr.AddRectFilled(orig.Convert(), end.Convert(), yellow.PackedValue);
			ptr.AddRect(orig.Convert(), end.Convert(), red.PackedValue);
		}

	}

	internal static void VectorWrapped(string v, Vector2 position, bool Length = false)
	{
		TextWrapped($"{v}: ");
		Indent();

		TextWrapped("X: ");
		SameLine();
		TextWrapped(position.X.ToString());

		TextWrapped("Y: ");
		SameLine();
		TextWrapped(position.Y.ToString());

		if (Length)
		{
			TextWrapped("Length: ");
			SameLine();
			TextWrapped(position.Length().ToString());
		}

		Unindent();
	}
}
