using System;
using System.Collections.Generic;

namespace NatureInTheValley
{
	// Token: 0x02000013 RID: 19
	public class RewardModel
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x060000F2 RID: 242 RVA: 0x00002685 File Offset: 0x00000885
		// (set) Token: 0x060000F3 RID: 243 RVA: 0x0000268D File Offset: 0x0000088D
		public string ItemId { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x00002696 File Offset: 0x00000896
		// (set) Token: 0x060000F5 RID: 245 RVA: 0x0000269E File Offset: 0x0000089E
		public int ItemCount { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x000026A7 File Offset: 0x000008A7
		// (set) Token: 0x060000F7 RID: 247 RVA: 0x000026AF File Offset: 0x000008AF
		public int TotalDonated { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x000026B8 File Offset: 0x000008B8
		// (set) Token: 0x060000F9 RID: 249 RVA: 0x000026C0 File Offset: 0x000008C0
		public List<string> creatureRequirements { get; set; }
	}
}
