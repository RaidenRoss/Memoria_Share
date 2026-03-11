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
			
			_v.Target.TryAlterSingleStatus(BattleStatusId.ChangeStat, true, _v.Caster, 0, 6, "Magic", _v.Target.Magic / 2);
			if ((_v.Context.Flags & (BattleCalcFlags.Miss | BattleCalcFlags.Guard)) != 0)
				return;
			
			btl_stat.AlterStatus(_v.Caster, BattleStatusId.ChangeStat, _v.Caster, false, 0, 6, "Magic", Math.Min(UInt16.MaxValue, _v.Caster.Magic16 * 2));
		}
	}
}