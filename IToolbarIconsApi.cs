using System;
using Microsoft.Xna.Framework;

namespace NatureInTheValley
{
	// Token: 0x02000014 RID: 20
	public interface IToolbarIconsApi
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060000FB RID: 251
		// (remove) Token: 0x060000FC RID: 252
		event EventHandler<string> ToolbarIconPressed;

		// Token: 0x060000FD RID: 253
		void AddToolbarIcon(string id, string texturePath, Rectangle? sourceRect, string hoverText);

		// Token: 0x060000FE RID: 254
		void RemoveToolbarIcon(string id);
	}
}
