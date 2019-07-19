using SabberStoneCore.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SabberStoneCoreAi.Score
{
	// based on https://www.reddit.com/r/hearthstone/comments/7l1ob0/i_wrote_a_masters_thesis_on_effective_hearthstone/
	public class MyScore : SabberStoneCoreAi.Score.Score
	{
		public override int Rate()
		{
			if (OpHeroHp < 1)
				return Int32.MaxValue;

			if (HeroHp < 1)
				return Int32.MinValue;

			float resultHero = 0;
			float resultOpponent = 0;

			resultHero += 2 * (float)Math.Sqrt(HeroHp);
			resultHero += HandCnt > 3 ? 9 + 2 * (HandCnt - 3) : 3 * HandCnt;
			resultHero += (float)Math.Sqrt(DeckCnt); // - next draw damage
			resultHero += MinionTotAtk + MinionTotHealth;
			resultHero += OpBoardZone.Count < 1 ? 2 + Math.Min(Controller.NumTurnsInPlay, 10) : 0;

			resultOpponent += 2 * (float)Math.Sqrt(OpHeroHp);
			resultOpponent += OpHandCnt > 3 ? 9 + 2 * (OpHandCnt - 3) : 3 * OpHandCnt;
			resultOpponent += (float)Math.Sqrt(OpDeckCnt); // - next draw damage
			resultOpponent += OpMinionTotAtk + OpMinionTotHealth;
			resultOpponent += BoardZone.Count < 1 ? 2 + Math.Min(Controller.NumTurnsInPlay, 10) : 0;

			return (int)(resultHero - resultOpponent);
		}

		public override Func<List<IPlayable>, List<int>> MulliganRule()
		{
			return p => p.Where(t => t.Cost > 3).Select(t => t.Id).ToList();
		}
	}
}
