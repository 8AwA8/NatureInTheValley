﻿using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace NatureInTheValley
{
	// Token: 0x0200000F RID: 15
	[XmlType("Mods_NatInValeyGoldenNet")]
	[XmlRoot(ElementName = "natInValeyGoldenNet", Namespace = "")]
	[KnownType(typeof(NatInValeyGoldenNet))]
	[XmlInclude(typeof(NatInValeyGoldenNet))]
	public class NatInValeyGoldenNet : Tool
	{
		// Token: 0x060000C2 RID: 194 RVA: 0x00002550 File Offset: 0x00000750
		protected override Item GetOneNew()
		{
			return new NatInValeyGoldenNet();
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00002557 File Offset: 0x00000757
		protected override string loadDisplayName()
		{
			return this.helper.Translation.Get("NetName");
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00002573 File Offset: 0x00000773
		public override string getDescription()
		{
			return this.helper.Translation.Get("NetDescript");
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x0000B4B0 File Offset: 0x000096B0
		public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
		{
			spriteBatch.Draw(NatInValeyGoldenNet.texture, location + new Vector2(32f, 32f), new Rectangle?(new Rectangle(0, 0, 16, 16)), Color.White * transparency, 0f, new Vector2(8f, 8f), (float)(4.0 * (double)scaleSize), SpriteEffects.None, layerDepth);
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x0000B520 File Offset: 0x00009720
		public override void actionWhenStopBeingHeld(Farmer who)
		{
			base.actionWhenStopBeingHeld(who);
			who.canMove = true;
			this.isHeld = false;
			if (who.UsingTool)
			{
				who.UsingTool = false;
				if (who.FarmerSprite.PauseForSingleAnimation)
				{
					who.FarmerSprite.PauseForSingleAnimation = false;
				}
			}
			if (who.hasBuff("NatCSpeedN"))
			{
				who.buffs.Remove("NatCSpeedN");
			}
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x0000B588 File Offset: 0x00009788
		public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
		{
			who.jitterStrength = 1f;
			who.canMove = true;
			this.save = 0;
			this.isHeld = true;
			who.forceCanMove();
			if (!who.hasBuff("NatCSpeedN"))
			{
				who.applyBuff(new Buff("NatCSpeedN", null, null, 20000, null, -1, null, null, null, null)
				{
					effects = 
					{
						Speed = 
						{
							Value = -((float)who.Speed + who.buffs.Speed - 1.5f)
						}
					},
					millisecondsDuration = 20000
				});
			}
			return false;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x000024C3 File Offset: 0x000006C3
		public virtual int salePrice()
		{
			return -1;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x0000B62C File Offset: 0x0000982C
		public override bool onRelease(GameLocation location, int x, int y, Farmer who)
		{
			who.stopJittering();
			this.isHeld = false;
			who.canMove = false;
			who.jitterStrength = 0f;
			who.UsingTool = false;
			if (who.hasBuff("NatCSpeedN"))
			{
				who.buffs.Remove("NatCSpeedN");
			}
			who.canReleaseTool = false;
			switch (who.FacingDirection)
			{
			case 0:
				((FarmerSprite)who.Sprite).animateOnce(295, 27f, 8, delegate(Farmer f)
				{
					this.EndOfUse(f);
				});
				this.Update(0, 0, who);
				break;
			case 1:
				((FarmerSprite)who.Sprite).animateOnce(296, 27f, 8, delegate(Farmer f)
				{
					this.EndOfUse(f);
				});
				this.Update(1, 0, who);
				break;
			case 2:
				((FarmerSprite)who.Sprite).animateOnce(297, 27f, 8, delegate(Farmer f)
				{
					this.EndOfUse(f);
				});
				this.Update(2, 0, who);
				break;
			case 3:
				((FarmerSprite)who.Sprite).animateOnce(298, 27f, 8, delegate(Farmer f)
				{
					this.EndOfUse(f);
				});
				this.Update(3, 0, who);
				break;
			}
			return false;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x0000B780 File Offset: 0x00009980
		public override void tickUpdate(GameTime time, Farmer who)
		{
			if (!this.isHeld || !who.hasBuff("NatCSpeedN"))
			{
				who.stopJittering();
				return;
			}
			who.jitterStrength = 0.5f;
			switch (who.FacingDirection)
			{
			case 0:
				who.FarmerSprite.setCurrentSingleFrame(36, 32000, false, false);
				return;
			case 1:
				who.FarmerSprite.setCurrentSingleFrame(48, 100, false, false);
				return;
			case 2:
				who.FarmerSprite.setCurrentSingleFrame(66, 32000, false, false);
				return;
			case 3:
				who.FarmerSprite.setCurrentSingleFrame(48, 100, false, true);
				return;
			default:
				return;
			}
		}

		// Token: 0x060000CB RID: 203 RVA: 0x0000258F File Offset: 0x0000078F
		private void EndOfUse(Farmer who)
		{
			who.canMove = true;
			this.isHeld = false;
			who.jitterStrength = 0f;
			NatureInTheValleyEntry.TryCatch(who);
		}

		// Token: 0x060000CC RID: 204 RVA: 0x000024E7 File Offset: 0x000006E7
		public override bool canBeTrashed()
		{
			return true;
		}

		// Token: 0x060000CD RID: 205 RVA: 0x000024E7 File Offset: 0x000006E7
		public override bool canBeDropped()
		{
			return true;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x0000B820 File Offset: 0x00009A20
		public NatInValeyGoldenNet()
		{
			base.ItemId = "NIVGoldNet";
			this.helper = NatureInTheValleyEntry.staticHelper;
			NatInValeyGoldenNet.texture = this.helper.ModContent.Load<Texture2D>("PNGs\\GoldenNet");
			base.Category = -99;
			this.Name = "NatValleyGoldNet";
			this.Stack = 1;
		}

		// Token: 0x04000061 RID: 97
		public static Texture2D texture;

		// Token: 0x04000062 RID: 98
		private IModHelper helper;

		// Token: 0x04000063 RID: 99
		public bool isHeld;

		// Token: 0x04000064 RID: 100
		private int save;

		// Token: 0x04000065 RID: 101
		public int variant;
	}
}
