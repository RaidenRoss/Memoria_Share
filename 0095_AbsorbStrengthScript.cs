using System;
using System.Collections.Generic;
using Memoria.Data;

namespace Memoria.AlternateFantasy
{
	/// <summary>
	/// Absorb Strength
	/// </summary>
	[BattleScript(Id)]
	public sealed class AbsorbStrengthScript : IBattleScript
	{
		public const Int32 Id = 0095;

		private readonly BattleCalculator _v;

		public AbsorbStrengthScript(BattleCalculator v)
		{
			_v = v;
		}

		public void Perform()
		{
			if (!_v.Target.CanBeAttacked())
				return;

			UInt16 drainedAmount = (UInt16)Math.Max(1, _v.Target.Strength16 / 5);
			UInt16 targetNewStrength = (UInt16)Math.Max(0, _v.Target.Strength16 - drainedAmount);

			_v.Target.TryAlterSingleStatus(BattleStatusId.ChangeStat, true, _v.Caster, 0, 6, "Strength", targetNewStrength);
			if ((_v.Context.Flags & (BattleCalcFlags.Miss | BattleCalcFlags.Guard)) != 0)
				return;

			UInt16 casterNewStrength = (UInt16)Math.Min(999, _v.Caster.Strength16 + drainedAmount);
			btl_stat.AlterStatus(_v.Caster, BattleStatusId.ChangeStat, _v.Caster, false, 0, 6, "Strength", casterNewStrength);
		}
	}
}