using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace NatureInTheValley
{
	// Token: 0x0200000E RID: 14
	[XmlType("Mods_NatInValeyGoldenNet")]
	[XmlRoot(ElementName = "natInValeyGoldenNet", Namespace = "")]
	[KnownType(typeof(NatInValeyGoldenNet))]
	[XmlInclude(typeof(NatInValeyGoldenNet))]
	public class NatInValeyGoldenNet : Tool
	{
		// Token: 0x06000094 RID: 148 RVA: 0x00002506 File Offset: 0x00000706
		protected override Item GetOneNew()
		{
			return new NatInValeyGoldenNet();
		}

		// Token: 0x06000095 RID: 149 RVA: 0x0000250D File Offset: 0x0000070D
		protected override string loadDisplayName()
		{
			return this.helper.Translation.Get("NetName");
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00002529 File Offset: 0x00000729
		public override string getDescription()
		{
			return this.helper.Translation.Get("NetDescript");
		}

		// Token: 0x06000097 RID: 151 RVA: 0x0000A2B0 File Offset: 0x000084B0
		public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
		{
			spriteBatch.Draw(NatInValeyGoldenNet.texture, location + new Vector2(32f, 32f), new Rectangle?(new Rectangle(0, 0, 16, 16)), Color.White * transparency, 0f, new Vector2(8f, 8f), (float)(4.0 * (double)scaleSize), SpriteEffects.None, layerDepth);
		}

		// Token: 0x06000098 RID: 152 RVA: 0x0000A320 File Offset: 0x00008520
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

		// Token: 0x06000099 RID: 153 RVA: 0x0000A388 File Offset: 0x00008588
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

		// Token: 0x0600009A RID: 154 RVA: 0x00002479 File Offset: 0x00000679
		public virtual int salePrice()
		{
			return -1;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x0000A42C File Offset: 0x0000862C
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

		// Token: 0x0600009C RID: 156 RVA: 0x0000A580 File Offset: 0x00008780
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

		// Token: 0x0600009D RID: 157 RVA: 0x00002545 File Offset: 0x00000745
		private void EndOfUse(Farmer who)
		{
			who.canMove = true;
			this.isHeld = false;
			who.jitterStrength = 0f;
			NatureInTheValleyEntry.TryCatch(who);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x0000249D File Offset: 0x0000069D
		public override bool canBeTrashed()
		{
			return true;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x0000249D File Offset: 0x0000069D
		public override bool canBeDropped()
		{
			return true;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x0000A620 File Offset: 0x00008820
		public NatInValeyGoldenNet()
		{
			base.ItemId = "NIVGoldNet";
			this.helper = NatureInTheValleyEntry.staticHelper;
			NatInValeyGoldenNet.texture = this.helper.ModContent.Load<Texture2D>("PNGs\\GoldenNet");
			base.Category = -99;
			this.Name = "NatValleyGoldNet";
			this.Stack = 1;
		}

		// Token: 0x04000059 RID: 89
		public static Texture2D texture;

		// Token: 0x0400005A RID: 90
		private IModHelper helper;

		// Token: 0x0400005B RID: 91
		public bool isHeld;

		// Token: 0x0400005C RID: 92
		private int save;

		// Token: 0x0400005D RID: 93
		public int variant;
	}
}
