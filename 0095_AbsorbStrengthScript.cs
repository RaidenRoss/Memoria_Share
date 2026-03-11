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
			
			_v.Target.TryAlterSingleStatus(BattleStatusId.ChangeStat, true, _v.Caster, 0, 6, "Strength", _v.Target.Strength / 2);
			if ((_v.Context.Flags & (BattleCalcFlags.Miss | BattleCalcFlags.Guard)) != 0)
				return;
			
			btl_stat.AlterStatus(_v.Caster, BattleStatusId.ChangeStat, _v.Caster, false, 0, 6, "Strength", Math.Min(UInt16.MaxValue, _v.Caster.Strength16 * 2));
		}
	}
}