using System;
using StardewModdingAPI.Utilities;

namespace NatureInTheValley
{
	// Token: 0x02000010 RID: 16
	internal class NatInValleyConfig
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x060000AE RID: 174 RVA: 0x000025B3 File Offset: 0x000007B3
		// (set) Token: 0x060000AF RID: 175 RVA: 0x000025BB File Offset: 0x000007BB
		public float spawnRateMultiplier { get; set; } = 1f;

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x060000B0 RID: 176 RVA: 0x000025C4 File Offset: 0x000007C4
		// (set) Token: 0x060000B1 RID: 177 RVA: 0x000025CC File Offset: 0x000007CC
		public float creaturePriceMultiplier { get; set; } = 1f;

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x000025D5 File Offset: 0x000007D5
		// (set) Token: 0x060000B3 RID: 179 RVA: 0x000025DD File Offset: 0x000007DD
		public float maxcreaturelLimitMultiplier { get; set; } = 1f;

		// Token: 0x060000B4 RID: 180 RVA: 0x0000A680 File Offset: 0x00008880
		public NatInValleyConfig()
		{
			this.catchingDifficultyMultiplier = 1f;
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x000025E6 File Offset: 0x000007E6
		// (set) Token: 0x060000B6 RID: 182 RVA: 0x000025EE File Offset: 0x000007EE
		public bool addCreaturesToShippingCollection { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x060000B7 RID: 183 RVA: 0x000025F7 File Offset: 0x000007F7
		// (set) Token: 0x060000B8 RID: 184 RVA: 0x000025FF File Offset: 0x000007FF
		public float catchingDifficultyMultiplier { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x00002608 File Offset: 0x00000808
		// (set) Token: 0x060000BA RID: 186 RVA: 0x00002610 File Offset: 0x00000810
		public KeybindList KeyForEncy { get; set; } = KeybindList.Parse("I + LeftShift");

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x060000BB RID: 187 RVA: 0x00002619 File Offset: 0x00000819
		// (set) Token: 0x060000BC RID: 188 RVA: 0x00002621 File Offset: 0x00000821
		public bool useOnlyContentPacks { get; set; }
	}
}
