using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace NatureInTheValley
{
	// Token: 0x02000015 RID: 21
	[XmlType("Mods_NatInValeyJadeNet")]
	[XmlRoot(ElementName = "natInValeyJadeNet", Namespace = "")]
	[KnownType(typeof(NatInValeyJadeNet))]
	[XmlInclude(typeof(NatInValeyJadeNet))]
	public class NatInValeyJadeNet : Tool
	{
		// Token: 0x060000FF RID: 255 RVA: 0x000026C9 File Offset: 0x000008C9
		protected override Item GetOneNew()
		{
			return new NatInValeyJadeNet();
		}

		// Token: 0x06000100 RID: 256 RVA: 0x000026D0 File Offset: 0x000008D0
		protected override string loadDisplayName()
		{
			return "=" + this.helper.Translation.Get("NetName");
		}

		// Token: 0x06000101 RID: 257 RVA: 0x000026F6 File Offset: 0x000008F6
		public override string getDescription()
		{
			return this.helper.Translation.Get("NetDescript");
		}

		// Token: 0x06000102 RID: 258 RVA: 0x0000B8D0 File Offset: 0x00009AD0
		public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
		{
			spriteBatch.Draw(NatInValeyJadeNet.texture, location + new Vector2(32f, 32f), new Rectangle?(new Rectangle(0, 0, 16, 16)), Color.White * transparency, 0f, new Vector2(8f, 8f), (float)(4.0 * (double)scaleSize), SpriteEffects.None, layerDepth);
		}

		// Token: 0x06000103 RID: 259 RVA: 0x0000B940 File Offset: 0x00009B40
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

		// Token: 0x06000104 RID: 260 RVA: 0x0000B9A8 File Offset: 0x00009BA8
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
							Value = -0.3f
						}
					},
					millisecondsDuration = 20000
				});
			}
			return false;
		}

		// Token: 0x06000105 RID: 261 RVA: 0x000024C3 File Offset: 0x000006C3
		public virtual int salePrice()
		{
			return -1;
		}

		// Token: 0x06000106 RID: 262 RVA: 0x0000BA34 File Offset: 0x00009C34
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

		// Token: 0x06000107 RID: 263 RVA: 0x0000BB88 File Offset: 0x00009D88
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

		// Token: 0x06000108 RID: 264 RVA: 0x00002712 File Offset: 0x00000912
		private void EndOfUse(Farmer who)
		{
			who.canMove = true;
			this.isHeld = false;
			who.jitterStrength = 0f;
			NatureInTheValleyEntry.TryCatch(who);
		}

		// Token: 0x06000109 RID: 265 RVA: 0x000024E7 File Offset: 0x000006E7
		public override bool canBeTrashed()
		{
			return true;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x000024E7 File Offset: 0x000006E7
		public override bool canBeDropped()
		{
			return true;
		}

		// Token: 0x0600010B RID: 267 RVA: 0x0000BC28 File Offset: 0x00009E28
		public NatInValeyJadeNet()
		{
			base.ItemId = "NIVjadeNet";
			this.helper = NatureInTheValleyEntry.staticHelper;
			NatInValeyJadeNet.texture = this.helper.ModContent.Load<Texture2D>("PNGs\\JadeNet");
			base.Category = -99;
			this.Name = "NatValleyJadeNet";
			this.Stack = 1;
		}

		// Token: 0x04000076 RID: 118
		public static Texture2D texture;

		// Token: 0x04000077 RID: 119
		private IModHelper helper;

		// Token: 0x04000078 RID: 120
		public bool isHeld;

		// Token: 0x04000079 RID: 121
		private int save;

		// Token: 0x0400007A RID: 122
		public int variant;
	}
}
