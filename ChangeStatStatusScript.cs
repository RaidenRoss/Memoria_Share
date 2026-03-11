using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Memoria.Data;
using Memoria.Assets;
using Object = System.Object;

namespace Memoria.AlternateFantasy
{
    [StatusScript(BattleStatusId.ChangeStat)]
    public class ChangeStatStatusScript : StatusScriptBase
    {
        public Int32 DiffLevel = 0;
        public Int32 DiffStrength = 0;
        public Int32 DiffMagic = 0;
        public Int32 DiffDexterity = 0;
        public Int32 DiffWill = 0;
        public Int32 DiffMaximumHp = 0;
        public Int32 DiffMaximumMp = 0;
        public Int32 DiffCriticalRateBonus = 0;
        public Int32 DiffCriticalRateResistance = 0;
        public Int32 DiffPhysicalDefence = 0;
        public Int32 DiffPhysicalEvade = 0;
        public Int32 DiffMagicDefence = 0;
        public Int32 DiffMagicEvade = 0;

        private static Dictionary<KeyValuePair<String, String>, String> Btl2dMessage = new Dictionary<KeyValuePair<String, String>, String>()
        {
            { new KeyValuePair<String, String>("Level", "US"), "Level" },
            { new KeyValuePair<String, String>("Level", "UK"), "Level" },
            { new KeyValuePair<String, String>("Level", "JP"), "レベル" },
            { new KeyValuePair<String, String>("Level", "GR"), "Niveau" },
            { new KeyValuePair<String, String>("Level", "FR"), "Niveau" },
            { new KeyValuePair<String, String>("Level", "IT"), "Livello" },
            { new KeyValuePair<String, String>("Level", "ES"), "Nivel" },
            { new KeyValuePair<String, String>("Strength", "US"), "Strength" },
            { new KeyValuePair<String, String>("Strength", "UK"), "Strength" },
            { new KeyValuePair<String, String>("Strength", "JP"), "ちから" },
            { new KeyValuePair<String, String>("Strength", "GR"), "Stärke" },
            { new KeyValuePair<String, String>("Strength", "FR"), "Force" },
            { new KeyValuePair<String, String>("Strength", "IT"), "Forza" },
            { new KeyValuePair<String, String>("Strength", "ES"), "Fuerza" },
            { new KeyValuePair<String, String>("Magic", "US"), "Magic" },
            { new KeyValuePair<String, String>("Magic", "UK"), "Magic" },
            { new KeyValuePair<String, String>("Magic", "JP"), "まりょく" },
            { new KeyValuePair<String, String>("Magic", "GR"), "Zauber" },
            { new KeyValuePair<String, String>("Magic", "FR"), "Magie" },
            { new KeyValuePair<String, String>("Magic", "IT"), "POT magico" },
            { new KeyValuePair<String, String>("Magic", "ES"), "Magia" },
            { new KeyValuePair<String, String>("Will", "US"), "Spirit" },
            { new KeyValuePair<String, String>("Will", "UK"), "Spirit" },
            { new KeyValuePair<String, String>("Will", "JP"), "きりょく" },
            { new KeyValuePair<String, String>("Will", "GR"), "Wille" },
            { new KeyValuePair<String, String>("Will", "FR"), "Psy" },
            { new KeyValuePair<String, String>("Will", "IT"), "POT spirito" },
            { new KeyValuePair<String, String>("Will", "ES"), "Espíritu" },
            { new KeyValuePair<String, String>("MaximumHp", "US"), "HP MAX" },
            { new KeyValuePair<String, String>("MaximumHp", "UK"), "HP MAX" },
            { new KeyValuePair<String, String>("MaximumHp", "JP"), "HP MAX" },
            { new KeyValuePair<String, String>("MaximumHp", "GR"), "HP MAX" },
            { new KeyValuePair<String, String>("MaximumHp", "FR"), "HP MAX" },
            { new KeyValuePair<String, String>("MaximumHp", "IT"), "HP MAX" },
            { new KeyValuePair<String, String>("MaximumHp", "ES"), "VIT" },
            { new KeyValuePair<String, String>("MaximumMp", "US"), "MP MAX" },
            { new KeyValuePair<String, String>("MaximumMp", "UK"), "MP MAX" },
            { new KeyValuePair<String, String>("MaximumMp", "JP"), "MP MAX" },
            { new KeyValuePair<String, String>("MaximumMp", "GR"), "MP MAX" },
            { new KeyValuePair<String, String>("MaximumMp", "FR"), "MP MAX" },
            { new KeyValuePair<String, String>("MaximumMp", "IT"), "MP MAX" },
            { new KeyValuePair<String, String>("MaximumMp", "ES"), "PM" },

			// === NUOVE VOCI PER MESSAGGIO UNICO HP+MP ===
			{ new KeyValuePair<String, String>("HpMpBoth", "US"), "HP & MP MAX" },
            { new KeyValuePair<String, String>("HpMpBoth", "UK"), "HP & MP MAX" },
            { new KeyValuePair<String, String>("HpMpBoth", "JP"), "HP & MP MAX" },
            { new KeyValuePair<String, String>("HpMpBoth", "GR"), "HP & MP MAX" },
            { new KeyValuePair<String, String>("HpMpBoth", "FR"), "HP & MP MAX" },
            { new KeyValuePair<String, String>("HpMpBoth", "IT"), "HP ed MP MAX" },
            { new KeyValuePair<String, String>("HpMpBoth", "ES"), "HP & MP MAX" },

            { new KeyValuePair<String, String>("CriticalRateBonus", "US"), "Critical" },
            { new KeyValuePair<String, String>("CriticalRateBonus", "UK"), "Critical" },
            { new KeyValuePair<String, String>("CriticalRateBonus", "JP"), "Critical" },
            { new KeyValuePair<String, String>("CriticalRateBonus", "GR"), "Kritisch" },
            { new KeyValuePair<String, String>("CriticalRateBonus", "FR"), "Critique" },
            { new KeyValuePair<String, String>("CriticalRateBonus", "IT"), "Colpo Critico" },
            { new KeyValuePair<String, String>("CriticalRateBonus", "ES"), "Letale" },
            { new KeyValuePair<String, String>("CriticalRateResistance", "US"), "Crit Resistance" },
            { new KeyValuePair<String, String>("CriticalRateResistance", "UK"), "Crit Resistance" },
            { new KeyValuePair<String, String>("CriticalRateResistance", "JP"), "Critical抵抗" },
            { new KeyValuePair<String, String>("CriticalRateResistance", "GR"), "Kritisch Resistenz" },
            { new KeyValuePair<String, String>("CriticalRateResistance", "FR"), "Critique Résist" },
            { new KeyValuePair<String, String>("CriticalRateResistance", "IT"), "Resistenza Colpo Critico" },
            { new KeyValuePair<String, String>("CriticalRateResistance", "ES"), "Letale Resistencia" },
            { new KeyValuePair<String, String>("Dexterity", "US"), "Speed" },
            { new KeyValuePair<String, String>("Dexterity", "UK"), "Speed" },
            { new KeyValuePair<String, String>("Dexterity", "JP"), "すばやさ" },
            { new KeyValuePair<String, String>("Dexterity", "GR"), "Gewandheit" },
            { new KeyValuePair<String, String>("Dexterity", "FR"), "Vitesse" },
            { new KeyValuePair<String, String>("Dexterity", "IT"), "Velocità" },
            { new KeyValuePair<String, String>("Dexterity", "ES"), "Rapidez" },
            { new KeyValuePair<String, String>("PhysicalDefence", "US"), "Defence" },
            { new KeyValuePair<String, String>("PhysicalDefence", "UK"), "Defence" },
            { new KeyValuePair<String, String>("PhysicalDefence", "JP"), "ぼうぎょりょく" },
            { new KeyValuePair<String, String>("PhysicalDefence", "GR"), "Abwehr" },
            { new KeyValuePair<String, String>("PhysicalDefence", "FR"), "Défense" },
            { new KeyValuePair<String, String>("PhysicalDefence", "IT"), "DIF fisica" },
            { new KeyValuePair<String, String>("PhysicalDefence", "ES"), "Defensa F" },
            { new KeyValuePair<String, String>("PhysicalEvade", "US"), "Evade" },
            { new KeyValuePair<String, String>("PhysicalEvade", "UK"), "Evade" },
            { new KeyValuePair<String, String>("PhysicalEvade", "JP"), "かいひりつ" },
            { new KeyValuePair<String, String>("PhysicalEvade", "GR"), "Reflex" },
            { new KeyValuePair<String, String>("PhysicalEvade", "FR"), "Esquive" },
            { new KeyValuePair<String, String>("PhysicalEvade", "IT"), "DST fisica" },
            { new KeyValuePair<String, String>("PhysicalEvade", "ES"), "Evasión F" },
            { new KeyValuePair<String, String>("MagicDefence", "US"), "Magic Def" },
            { new KeyValuePair<String, String>("MagicDefence", "UK"), "Magic Def" },
            { new KeyValuePair<String, String>("MagicDefence", "JP"), "まほうぼうぎょ" },
            { new KeyValuePair<String, String>("MagicDefence", "GR"), "Z-Abwehr" },
            { new KeyValuePair<String, String>("MagicDefence", "FR"), "Protection" },
            { new KeyValuePair<String, String>("MagicDefence", "IT"), "DIF magica" },
            { new KeyValuePair<String, String>("MagicDefence", "ES"), "Defensa M" },
            { new KeyValuePair<String, String>("MagicEvade", "US"), "Magic Eva" },
            { new KeyValuePair<String, String>("MagicEvade", "UK"), "Magic Eva" },
            { new KeyValuePair<String, String>("MagicEvade", "JP"), "まほうかいひ" },
            { new KeyValuePair<String, String>("MagicEvade", "GR"), "Z-Reflex" },
            { new KeyValuePair<String, String>("MagicEvade", "FR"), "Esqmg" },
            { new KeyValuePair<String, String>("MagicEvade", "IT"), "DST magica" },
            { new KeyValuePair<String, String>("MagicEvade", "ES"), "Evasión M" },
        };

        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            base.Apply(target, inflicter, parameters);
            Boolean success = false;
            Int32 startIndex = 0;
            Int32 delay = 0;
            Int32 delayAdd = 6;
            Boolean combineHpMpMsg = false;
            Boolean hpCombinedChanged = false;
            Boolean mpCombinedChanged = false;
            if (parameters.Length >= 2 && parameters[0] is Int32 && parameters[1] is Int32)
            {
                delay = (Int32)parameters[0];
                delayAdd = (Int32)parameters[1];
                startIndex = 2;
            }
            for (Int32 i = startIndex; i + 1 < parameters.Length; i += 2)
            {
                String kind = parameters[i] as String;
                if (kind == null)
                    continue;
                if (parameters[i + 1] is not Int32 && parameters[i + 1] is not UInt32 && parameters[i + 1] is not Int16 && parameters[i + 1] is not Byte)
                    continue;
                Int32 amount = Convert.ToInt32(parameters[i + 1]);
                Int32 chg = 0;
                Boolean skipMsg = false;
                switch (kind)
                {
                    case "CombineHpMpMsg":
                        combineHpMpMsg = amount != 0;
                        // non segnare come cambiamento con messaggio singolo
                        chg = 0;
                        break;
                    case "Level":
					{
						int upper = target.IsPlayer ? ff9level.LevelFromExp(target.Player.exp) : ff9level.LEVEL_COUNT;
						amount = Mathf.Clamp(amount, 1, upper);
						if (amount != target.Level16)
						{
							chg = amount - target.Level16;
							DiffLevel += chg;
							target.Level16 = (UInt16)amount;
						}
					}
					break;
                    case "Strength":
                        amount = Mathf.Clamp(amount, 0, UInt16.MaxValue);
                        if (amount != target.Strength16)
                        {
                            chg = amount - target.Strength16;
                            DiffStrength += chg;
                            target.Strength16 = (UInt16)amount;
                        }
                        break;
                    case "Magic":
                        amount = Mathf.Clamp(amount, 0, UInt16.MaxValue);
                        if (amount != target.Magic16)
                        {
                            chg = amount - target.Magic16;
                            DiffMagic += chg;
                            target.Magic16 = (UInt16)amount;
                        }
                        break;
                    case "Dexterity":
                        amount = Mathf.Clamp(amount, 0, UInt16.MaxValue);
                        if (amount != target.Dexterity16)
                        {
                            chg = amount - target.Dexterity16;
                            DiffDexterity += chg;
                            target.Dexterity16 = (UInt16)amount;
                        }
                        break;
                    case "Will":
                        amount = Mathf.Clamp(amount, 0, UInt16.MaxValue);
                        if (amount != target.Will16)
                        {
                            chg = amount - target.Will16;
                            DiffWill += chg;
                            target.Will16 = (UInt16)amount;
                        }
                        break;
                    case "MaximumHp":
                        amount = Math.Max(amount, 1);
                        if (amount != target.MaximumHp)
                        {
                            chg = (Int32)(amount - target.MaximumHp);
                            DiffMaximumHp += chg;
                            target.MaximumHp = (UInt32)amount;
                        }
                        if (combineHpMpMsg)
                        {
                            skipMsg = true;
                            hpCombinedChanged |= (chg != 0);
                        }
                        break;
                    case "MaximumMp":
                        amount = Math.Max(amount, 0);
                        if (amount != target.MaximumMp)
                        {
                            chg = (Int32)(amount - target.MaximumMp);
                            DiffMaximumMp += chg;
                            target.MaximumMp = (UInt32)amount;
                        }
                        if (combineHpMpMsg)
                        {
                            skipMsg = true;
                            mpCombinedChanged |= (chg != 0);
                        }
                        break;
                    case "CriticalRateBonus":
                        amount = Mathf.Clamp(amount, Int16.MinValue, Int16.MaxValue);
                        if (amount != target.CriticalRateBonus)
                        {
                            chg = amount - target.CriticalRateBonus;
                            DiffCriticalRateBonus += chg;
                            target.CriticalRateBonus = (Int16)amount;
                        }
                        break;
                    case "CriticalRateResistance":
                        amount = Mathf.Clamp(amount, Int16.MinValue, Int16.MaxValue);
                        if (amount != target.CriticalRateResistance)
                        {
                            chg = amount - target.CriticalRateResistance;
                            DiffCriticalRateResistance += chg;
                            target.CriticalRateResistance = (Int16)amount;
                        }
                        break;
                    case "PhysicalDefence":
                        amount = Mathf.Clamp(amount, 0, Int32.MaxValue);
                        if (amount != target.PhysicalDefence)
                        {
                            chg = amount - target.PhysicalDefence;
                            DiffPhysicalDefence += chg;
                            target.PhysicalDefence = amount;
                        }
                        break;
                    case "PhysicalEvade":
                        amount = Mathf.Clamp(amount, 0, Int32.MaxValue);
                        if (amount != target.PhysicalEvade)
                        {
                            chg = amount - target.PhysicalEvade;
                            DiffPhysicalEvade += chg;
                            target.PhysicalEvade = amount;
                        }
                        break;
                    case "MagicDefence":
                        amount = Mathf.Clamp(amount, 0, Int32.MaxValue);
                        if (amount != target.MagicDefence)
                        {
                            chg = amount - target.MagicDefence;
                            DiffMagicDefence += chg;
                            target.MagicDefence = amount;
                        }
                        break;
                    case "MagicEvade":
                        amount = Mathf.Clamp(amount, 0, Int32.MaxValue);
                        if (amount != target.MagicEvade)
                        {
                            chg = amount - target.MagicEvade;
                            DiffMagicEvade += chg;
                            target.MagicEvade = amount;
                        }
                        break;
                }
                if (chg != 0)
                {
                    if (!skipMsg)
                    {
                        KeyValuePair<String, String> kvp = new KeyValuePair<String, String>(kind, Localization.GetSymbol());
                        if (chg < 0)
                            btl2d.Btl2dReqSymbolMessage(target, CommonScript.BadMessageColor, "-" + Btl2dMessage[kvp], HUDMessage.MessageStyle.DAMAGE, (Byte)delay);
                        else
                            btl2d.Btl2dReqSymbolMessage(target, CommonScript.GoodMessageColor, "+" + Btl2dMessage[kvp], HUDMessage.MessageStyle.DAMAGE, (Byte)delay);
                        delay += delayAdd;
                        success = true;
                    }
                    else
                    {
                        // contiamo il cambiamento come avvenuto ma senza stampare qui
                        success = true;
                    }
                }
            }

            // -- Messaggio combinato unico per HP/MP --
            if (combineHpMpMsg && (hpCombinedChanged || mpCombinedChanged))
            {
                KeyValuePair<String, String> bothKvp = new KeyValuePair<String, String>("HpMpBoth", Localization.GetSymbol());
                btl2d.Btl2dReqSymbolMessage(target, CommonScript.GoodMessageColor, "+" + Btl2dMessage[bothKvp], HUDMessage.MessageStyle.DAMAGE, (Byte)delay);
                delay += delayAdd;
                success = true;
            }

            return success ? btl_stat.ALTER_SUCCESS : btl_stat.ALTER_INVALID;
        }

        public override Boolean Remove()
        {
            Target.Level16 = (UInt16)Mathf.Clamp(Target.Level16 - DiffLevel, 1, Target.IsPlayer ? ff9level.LevelFromExp(Target.Player.exp) : ff9level.LEVEL_COUNT);
            Target.Strength16 = (UInt16)Mathf.Clamp(Target.Strength - DiffStrength, 1, UInt16.MaxValue);
            Target.Magic16 = (UInt16)Mathf.Clamp(Target.Magic - DiffMagic, 1, UInt16.MaxValue);
            Target.Dexterity16 = (UInt16)Mathf.Clamp(Target.Dexterity - DiffDexterity, 1, UInt16.MaxValue);
            Target.Will16 = (UInt16)Mathf.Clamp(Target.Will - DiffWill, 1, UInt16.MaxValue);
            Target.MaximumHp = (UInt32)Math.Max(Target.MaximumHp - DiffMaximumHp, 0);
            Target.MaximumMp = (UInt32)Math.Max(Target.MaximumMp - DiffMaximumMp, 0);
            Target.CriticalRateBonus = (Int16)Mathf.Clamp(Target.CriticalRateBonus - DiffCriticalRateBonus, Int16.MinValue, Int16.MaxValue);
            Target.CriticalRateResistance = (Int16)Mathf.Clamp(Target.CriticalRateResistance - DiffCriticalRateResistance, Int16.MinValue, Int16.MaxValue);
            Target.PhysicalDefence = Math.Max(Target.PhysicalDefence - DiffPhysicalDefence, 0);
            Target.PhysicalEvade = Math.Max(Target.PhysicalEvade - DiffPhysicalEvade, 0);
            Target.MagicDefence = Math.Max(Target.MagicDefence - DiffMagicDefence, 0);
            Target.MagicEvade = Math.Max(Target.MagicEvade - DiffMagicEvade, 0);
            return true;
        }

        public Boolean RemovePartly(Boolean positivePart)
        {
            Boolean doneSomething = false;
            Int32 compareFactor = positivePart ? 1 : -1;
            if (compareFactor * DiffLevel > 0)
            {
                Target.Level16 = (UInt16)Mathf.Clamp(Target.Level16 - DiffLevel, 1, Target.IsPlayer ? ff9level.LevelFromExp(Target.Player.exp) : ff9level.LEVEL_COUNT);
                DiffLevel = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffStrength > 0)
            {
                Target.Strength16 = (UInt16)Mathf.Clamp(Target.Strength - DiffStrength, 1, UInt16.MaxValue);
                DiffStrength = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffMagic > 0)
            {
                Target.Magic16 = (UInt16)Mathf.Clamp(Target.Magic - DiffMagic, 1, UInt16.MaxValue);
                DiffMagic = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffDexterity > 0)
            {
                Target.Dexterity16 = (UInt16)Mathf.Clamp(Target.Dexterity - DiffDexterity, 1, UInt16.MaxValue);
                DiffDexterity = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffWill > 0)
            {
                Target.Will16 = (UInt16)Mathf.Clamp(Target.Will - DiffWill, 1, UInt16.MaxValue);
                DiffWill = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffMaximumHp > 0)
            {
                Target.MaximumHp = (UInt32)Math.Max(Target.MaximumHp - DiffMaximumHp, 1);
                DiffMaximumHp = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffMaximumMp > 0)
            {
                Target.MaximumMp = (UInt32)Math.Max(Target.MaximumMp - DiffMaximumMp, 0);
                DiffMaximumMp = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffCriticalRateBonus > 0)
            {
                Target.CriticalRateBonus = (Int16)Mathf.Clamp(Target.CriticalRateBonus - DiffCriticalRateBonus, Int16.MinValue, Int16.MaxValue);
                DiffCriticalRateBonus = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffCriticalRateResistance > 0)
            {
                Target.CriticalRateResistance = (Int16)Mathf.Clamp(Target.CriticalRateResistance - DiffCriticalRateResistance, Int16.MinValue, Int16.MaxValue);
                DiffCriticalRateResistance = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffPhysicalDefence > 0)
            {
                Target.PhysicalDefence = Math.Max(Target.PhysicalDefence - DiffPhysicalDefence, 0);
                DiffPhysicalDefence = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffPhysicalEvade > 0)
            {
                Target.PhysicalEvade = Math.Max(Target.PhysicalEvade - DiffPhysicalEvade, 0);
                DiffPhysicalEvade = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffMagicDefence > 0)
            {
                Target.MagicDefence = Math.Max(Target.MagicDefence - DiffMagicDefence, 0);
                DiffMagicDefence = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffMagicEvade > 0)
            {
                Target.MagicEvade = Math.Max(Target.MagicEvade - DiffMagicEvade, 0);
                DiffMagicEvade = 0;
                doneSomething = true;
            }
            return doneSomething;
        }
    }
}
