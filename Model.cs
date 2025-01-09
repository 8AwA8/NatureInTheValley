using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace NatureInTheValley
{
	// Token: 0x0200000F RID: 15
	public class Model
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x0000256F File Offset: 0x0000076F
		// (set) Token: 0x060000A6 RID: 166 RVA: 0x00002577 File Offset: 0x00000777
		public List<string> locations { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x00002580 File Offset: 0x00000780
		// (set) Token: 0x060000A8 RID: 168 RVA: 0x00002588 File Offset: 0x00000788
		public List<int> frames { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x060000AA RID: 170 RVA: 0x00002591 File Offset: 0x00000791
		// (set) Token: 0x060000AB RID: 171 RVA: 0x00002599 File Offset: 0x00000799
		public List<string> creatures { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x060000AC RID: 172 RVA: 0x000025A2 File Offset: 0x000007A2
		// (set) Token: 0x060000AD RID: 173 RVA: 0x000025AA File Offset: 0x000007AA
		public List<Vector2> positions { get; set; }
	}
}
