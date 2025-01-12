using System;
using StardewModdingAPI.Utilities;

namespace NatureInTheValley
{
	// Token: 0x02000011 RID: 17
	internal class NatInValleyConfig
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x060000DC RID: 220 RVA: 0x000025FD File Offset: 0x000007FD
		// (set) Token: 0x060000DD RID: 221 RVA: 0x00002605 File Offset: 0x00000805
		public float spawnRateMultiplier { get; set; } = 1f;

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x060000DE RID: 222 RVA: 0x0000260E File Offset: 0x0000080E
		// (set) Token: 0x060000DF RID: 223 RVA: 0x00002616 File Offset: 0x00000816
		public float creaturePriceMultiplier { get; set; } = 1f;

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x0000261F File Offset: 0x0000081F
		// (set) Token: 0x060000E1 RID: 225 RVA: 0x00002627 File Offset: 0x00000827
		public float maxcreaturelLimitMultiplier { get; set; } = 1f;

		// Token: 0x060000E2 RID: 226 RVA: 0x0000B880 File Offset: 0x00009A80
		public NatInValleyConfig()
		{
			this.catchingDifficultyMultiplier = 1f;
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x060000E3 RID: 227 RVA: 0x00002630 File Offset: 0x00000830
		// (set) Token: 0x060000E4 RID: 228 RVA: 0x00002638 File Offset: 0x00000838
		public bool addCreaturesToShippingCollection { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x060000E5 RID: 229 RVA: 0x00002641 File Offset: 0x00000841
		// (set) Token: 0x060000E6 RID: 230 RVA: 0x00002649 File Offset: 0x00000849
		public float catchingDifficultyMultiplier { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x060000E7 RID: 231 RVA: 0x00002652 File Offset: 0x00000852
		// (set) Token: 0x060000E8 RID: 232 RVA: 0x0000265A File Offset: 0x0000085A
		public KeybindList KeyForEncy { get; set; } = KeybindList.Parse("I + LeftShift");

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x060000E9 RID: 233 RVA: 0x00002663 File Offset: 0x00000863
		// (set) Token: 0x060000EA RID: 234 RVA: 0x0000266B File Offset: 0x0000086B
		public bool useOnlyContentPacks { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x060000EB RID: 235 RVA: 0x00002674 File Offset: 0x00000874
		// (set) Token: 0x060000EC RID: 236 RVA: 0x0000267C File Offset: 0x0000087C
		public bool useTerrariumWallpapers { get; set; }
	}
}
