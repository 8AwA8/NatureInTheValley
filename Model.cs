using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace NatureInTheValley
{
	// Token: 0x02000010 RID: 16
	public class Model
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x060000D3 RID: 211 RVA: 0x000025B9 File Offset: 0x000007B9
		// (set) Token: 0x060000D4 RID: 212 RVA: 0x000025C1 File Offset: 0x000007C1
		public List<string> locations { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x000025CA File Offset: 0x000007CA
		// (set) Token: 0x060000D6 RID: 214 RVA: 0x000025D2 File Offset: 0x000007D2
		public List<int> frames { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x000025DB File Offset: 0x000007DB
		// (set) Token: 0x060000D9 RID: 217 RVA: 0x000025E3 File Offset: 0x000007E3
		public List<string> creatures { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x060000DA RID: 218 RVA: 0x000025EC File Offset: 0x000007EC
		// (set) Token: 0x060000DB RID: 219 RVA: 0x000025F4 File Offset: 0x000007F4
		public List<Vector2> positions { get; set; }
	}
}
