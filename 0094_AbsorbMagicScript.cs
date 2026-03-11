using System;
using System.Collections.Generic;
using Memoria.Data;

namespace Memoria.AlternateFantasy
{
	/// <summary>
	/// Absorb Magic
	/// </summary>
	[BattleScript(Id)]
	public sealed class AbsorbMagicScript : IBattleScript
	{
		public const Int32 Id = 0094;

		private readonly BattleCalculator _v;

		public AbsorbMagicScript(BattleCalculator v)
		{
			_v = v;
		}

		public void Perform()
		{
			if (!_v.Target.CanBeAttacked())
				return;

			UInt16 drainedAmount = (UInt16)Math.Max(1, _v.Target.Magic16 / 5);
			UInt16 targetNewMagic = (UInt16)Math.Max(0, _v.Target.Magic16 - drainedAmount);

			_v.Target.TryAlterSingleStatus(BattleStatusId.ChangeStat, true, _v.Caster, 0, 6, "Magic", targetNewMagic);
			if ((_v.Context.Flags & (BattleCalcFlags.Miss | BattleCalcFlags.Guard)) != 0)
				return;

			UInt16 casterNewMagic = (UInt16)Math.Min(999, _v.Caster.Magic16 + drainedAmount);
			btl_stat.AlterStatus(_v.Caster, BattleStatusId.ChangeStat, _v.Caster, false, 0, 6, "Magic", casterNewMagic);
		}
	}
}