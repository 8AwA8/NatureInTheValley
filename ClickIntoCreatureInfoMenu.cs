﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;

namespace NatureInTheValley
{
	// Token: 0x0200000A RID: 10
	internal class ClickIntoCreatureInfoMenu : IClickableMenu
	{
		// Token: 0x06000084 RID: 132 RVA: 0x00008D60 File Offset: 0x00006F60
		public ClickIntoCreatureInfoMenu()
		{
			this._title = "Creatures In The Valley";
			this.width = 700 + IClickableMenu.borderWidth * 2;
			this.height = (this.IsAndroid ? 550 : 600) + IClickableMenu.borderWidth * 2;
			base.initializeUpperRightCloseButton();
			this.exitFunction = delegate()
			{
				ClickIntoCreatureInfoMenu.ExitFunction();
			};
			this.xPositionOnScreen = Game1.viewport.Width / 2 - (800 + IClickableMenu.borderWidth * 2) / 2;
			this.yPositionOnScreen = Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2;
			this.xPositionOnScreen = (int)((double)((float)this.xPositionOnScreen) * Math.Pow((double)Game1.options.zoomLevel, 1.5) * Math.Pow((double)(1f / Game1.options.uiScale), 1.5));
			this.yPositionOnScreen = (int)((double)((float)this.yPositionOnScreen) * Math.Pow((double)Game1.options.zoomLevel, 1.5) * Math.Pow((double)(1f / Game1.options.uiScale), 1.5));
			int num = this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder;
			int num2 = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16;
			this.Collection = new List<ClickableTextureComponent>();
			int num3 = 0;
			foreach (ParsedItemData parsedItemData in from data in ItemRegistry.GetObjectTypeDefinition().GetAllData()
			where data.Category == -81 && data.ItemId.Contains("NatInValley.Creature.")
			select data)
			{
				int x = num + num3 % 12 * 56;
				int num4 = num2 + num3 / 12 * 56;
				if (num4 > this.yPositionOnScreen + this.height - 128)
				{
					num3 = 0;
					x = num;
					num4 = num2;
				}
				Texture2D texture = parsedItemData.GetTexture();
				ClickableTextureComponent item = new ClickableTextureComponent(parsedItemData.ItemId.ToString(), new Rectangle(x, num4, 64, 64), null, "", texture, Game1.getSourceRectForStandardTileSheet(texture, parsedItemData.SpriteIndex, 16, 16), 3f, false)
				{
					myID = this.Collection.Count,
					rightNeighborID = (((this.Collection.Count + 1) % 10 == 0) ? -1 : (this.Collection.Count + 1)),
					leftNeighborID = ((this.Collection.Count % 10 == 0) ? 7001 : (this.Collection.Count - 1)),
					downNeighborID = ((num4 + 68 > this.yPositionOnScreen + this.height - 128) ? -7777 : (this.Collection.Count + 10)),
					upNeighborID = ((this.Collection.Count < 10) ? 12345 : (this.Collection.Count - 10)),
					fullyImmutable = true
				};
				this.Collection.Add(item);
				num3++;
			}
			base.initializeUpperRightCloseButton();
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000090E0 File Offset: 0x000072E0
		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			base.receiveLeftClick(x, y, true);
			foreach (ClickableTextureComponent clickableTextureComponent in this.Collection)
			{
				if (clickableTextureComponent.containsPoint(x, y))
				{
					Game1.playSound("shwip", null);
					this.exitFunction = null;
					Game1.activeClickableMenu = new CreatureHighlightPage(ItemRegistry.Create(clickableTextureComponent.name, 1, 0, false));
				}
			}
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00009174 File Offset: 0x00007374
		public override void draw(SpriteBatch b)
		{
			if (!Game1.options.showMenuBackground)
			{
				b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
			}
			else
			{
				base.drawBackground(b);
			}
			base.draw(b);
			Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true, null, false, true, -1, -1, -1);
			SpriteText.drawStringWithScrollCenteredAt(b, this._title, this.xPositionOnScreen + this.width / 2, (int)(((double)Game1.viewport.Height / 2.0 - 310.0) * Math.Pow((double)Game1.options.zoomLevel, 1.5) * Math.Pow((double)(1f / Game1.options.uiScale), 1.5)), SpriteText.getWidthOfString(this._title, 999999) + 16, 1f, null, 0, 0.88f, false);
			b.End();
			b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
			foreach (ClickableTextureComponent clickableTextureComponent in this.Collection)
			{
				string text;
				clickableTextureComponent.draw(b, Game1.getLocationFromName("NIVInnerInsec").modData.TryGetValue("NatureInTheValley/Donated/" + clickableTextureComponent.name.Split("NatInValley.Creature.", StringSplitOptions.None)[1], out text) ? Color.White : (Color.Black * 0.2f), 0.86f, 0, 0, 0);
			}
			b.End();
			b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
			base.drawMouse(b, false, -1);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x0000937C File Offset: 0x0000757C
		public static void ExitFunction()
		{
			if (Game1.currentLocation.Name == "NIVInnerInsec")
			{
				Game1.DrawDialogue(new Dialogue(Game1.currentLocation.getCharacterFromName("IvyInsectarium"), "ENGL", NatureInTheValleyEntry.staticHelper.Translation.Get("LeaveEncyclo")));
			}
		}

		// Token: 0x04000046 RID: 70
		private string hoverText = "";

		// Token: 0x04000047 RID: 71
		public List<ClickableTextureComponent> Collection = new List<ClickableTextureComponent>();

		// Token: 0x04000048 RID: 72
		private int currentPage;

		// Token: 0x04000049 RID: 73
		private string _title;

		// Token: 0x0400004A RID: 74
		private readonly bool IsAndroid = Constants.TargetPlatform == GamePlatform.Android;
	}
}
