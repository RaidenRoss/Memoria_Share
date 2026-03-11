using System;
using System.Collections.Generic;
using Memoria.Data;

namespace Memoria.AlternateFantasy
{
	/// <summary>
	/// Might
	/// </summary>
	[BattleScript(Id)]
	public sealed class MightScript : IBattleScript, IEstimateBattleScript
	{
		public const Int32 Id = 0043;

		private readonly BattleCalculator _v;

		public MightScript(BattleCalculator v)
		{
			_v = v;
		}

		public void Perform()
		{
			if (_v.Target.IsUnderAnyStatus(BattleStatus.Death))
			{
				_v.Context.Flags |= BattleCalcFlags.Miss;
				return;
			}
			_v.Target.TryAlterSingleStatus(BattleStatusId.ChangeStat, true, _v.Caster, 0, 6, "Strength", Math.Min(999, _v.Target.Strength16 + _v.Target.Strength16 / _v.Command.Power));
		}

		public Single RateTarget()
		{
			return Math.Min(999, _v.Target.Strength16 + _v.Target.Strength16 / _v.Command.Power);
		}
	}
}