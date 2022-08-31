using System.Globalization;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Terraria;

namespace DevTools;

internal static class Extensions
{

	public static void AddHitBox(this ImDrawListPtr ptr, Rectangle rectangle, Color red, Color yellow)
	{
		yellow.A /= 3;
		var rect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
		
		if (rect.Intersects(rectangle))
		{
			var orig = new Vector2(rectangle.X, rectangle.Y);
			var end = orig + new Vector2(rectangle.Width, rectangle.Height);
			orig = orig.ToScreen();
			end = end.ToScreen();
			ptr.AddRectFilled(orig, end, yellow.PackedValue);
			ptr.AddRect(orig, end, red.PackedValue);
		}
	}

	public static Vector2 ToScreen(this Vector2 worldPos)
	{
		return Vector2.Transform(worldPos - Main.screenPosition, Main.GameViewMatrix.ZoomMatrix);
	}

	internal static void VectorWrapped(string v, Vector2 position, bool length = false)
	{
		TextWrapped($"{v}: ");
		Indent();

		TextWrapped("X: ");
		SameLine();
		TextWrapped(position.X.ToString(CultureInfo.InvariantCulture));

		TextWrapped("Y: ");
		SameLine();
		TextWrapped(position.Y.ToString(CultureInfo.InvariantCulture));

		if (length)
		{
			TextWrapped("Length: ");
			SameLine();
			TextWrapped(position.Length().ToString(CultureInfo.InvariantCulture));
		}

		Unindent();
	}
}
