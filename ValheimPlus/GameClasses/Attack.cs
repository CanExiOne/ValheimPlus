﻿using HarmonyLib;
using UnityEngine;
using ValheimPlus.Configurations;

namespace ValheimPlus.GameClasses
{
    /// <summary>
    /// Alters stamina of weapons
    /// </summary>
    [HarmonyPatch(typeof(Attack), "GetStaminaUsage")]
    public static class Attack_GetStaminaUsage_Patch
    {
        private static void Postfix(ref Attack __instance, ref float __result)
        {
            if (Configuration.Current.StaminaUsage.IsEnabled)
            {
                if (__instance.m_character.IsPlayer())
                {
                    ItemDrop.ItemData item = __instance.m_character.GetCurrentWeapon();
                    Skills.SkillType skillType;
                    if (item == null)
                    {
                        skillType = Skills.SkillType.Unarmed;
                    }
                    else
                    {
                        skillType = item.m_shared.m_skillType;
                    }

                    switch (skillType)
                    {
                        case Skills.SkillType.Swords:
                            __result = Helper.applyModifierValue(__result, Configuration.Current.StaminaUsage.swords);
                            break;
                        case Skills.SkillType.Knives:
                            __result = Helper.applyModifierValue(__result, Configuration.Current.StaminaUsage.knives);
                            break;
                        case Skills.SkillType.Clubs:
                            __result = Helper.applyModifierValue(__result, Configuration.Current.StaminaUsage.clubs);
                            break;
                        case Skills.SkillType.Polearms:
                            __result = Helper.applyModifierValue(__result, Configuration.Current.StaminaUsage.polearms);
                            break;
                        case Skills.SkillType.Spears:
                            __result = Helper.applyModifierValue(__result, Configuration.Current.StaminaUsage.spears);
                            break;
                        case Skills.SkillType.Axes:
                            __result = Helper.applyModifierValue(__result, Configuration.Current.StaminaUsage.axes);
                            break;
                        case Skills.SkillType.Unarmed:
                            __result = Helper.applyModifierValue(__result, Configuration.Current.StaminaUsage.unarmed);
                            break;
                        case Skills.SkillType.Pickaxes:
                            __result = Helper.applyModifierValue(__result, Configuration.Current.StaminaUsage.pickaxes);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Alter projectile velocity and accuracy without affecting damage
    /// </summary>
    [HarmonyPatch(typeof(Attack), "ProjectileAttackTriggered")]
    public static class Attack_ProjectileAttackTriggered_Patch
    {
        private static void Prefix(ref Attack __instance)
        {
            if (Configuration.Current.ProjectileFired.IsEnabled)
            {
                if (__instance.m_character.IsPlayer())
                {
                    if (Configuration.Current.ProjectileFired.playerProjectileEnableScaling)
                    {
                        Player player = (Player)__instance.m_character;
                        Skills.Skill skill = player.m_skills.GetSkill(__instance.m_weapon.m_shared.m_skillType);
                        float maxLevelPercentage = skill.m_level * 0.01f;

                        __instance.m_projectileVelMin = Mathf.Lerp(Configuration.Current.ProjectileFired.playerProjectileVelMinCharge, Configuration.Current.ProjectileFired.playerProjectileVelScaledMin, maxLevelPercentage);
                        __instance.m_projectileVel = Mathf.Lerp(Configuration.Current.ProjectileFired.playerProjectileVelMaxCharge, Configuration.Current.ProjectileFired.playerProjectileVelScaledMax, maxLevelPercentage);

                        __instance.m_projectileAccuracyMin = Mathf.Lerp(Configuration.Current.ProjectileFired.playerProjectileVarMinCharge, Configuration.Current.ProjectileFired.playerProjectileVarScaledMin, maxLevelPercentage);
                        __instance.m_projectileAccuracy = Mathf.Lerp(Configuration.Current.ProjectileFired.playerProjectileVarMaxCharge, Configuration.Current.ProjectileFired.playerProjectileVarScaledMax, maxLevelPercentage);
                    }
                    else
                    {
                        __instance.m_projectileVelMin = Configuration.Current.ProjectileFired.playerProjectileVelMinCharge;
                        __instance.m_projectileVel = Configuration.Current.ProjectileFired.playerProjectileVelMaxCharge;

                        __instance.m_projectileAccuracyMin = Configuration.Current.ProjectileFired.playerProjectileVarMinCharge;
                        __instance.m_projectileAccuracy = Configuration.Current.ProjectileFired.playerProjectileVarMaxCharge;
                    }
                }
                else
                {
                    __instance.m_projectileVelMin = Configuration.Current.ProjectileFired.projectileVelMinCharge;
                    __instance.m_projectileVel = Configuration.Current.ProjectileFired.projectileVelMaxCharge;

                    __instance.m_projectileAccuracyMin = Configuration.Current.ProjectileFired.projectileVarMinCharge;
                    __instance.m_projectileAccuracy = Configuration.Current.ProjectileFired.projectileVarMaxCharge;
                }

                float maxValue = 1e+6f;

                __instance.m_projectileVelMin = Mathf.Clamp(__instance.m_projectileVelMin, 0f, maxValue);
                __instance.m_projectileVel = Mathf.Clamp(__instance.m_projectileVel, 0f, maxValue);

                __instance.m_projectileAccuracyMin = Mathf.Clamp(__instance.m_projectileAccuracyMin, 0f, maxValue);
                __instance.m_projectileAccuracy = Mathf.Clamp(__instance.m_projectileAccuracy, 0f, maxValue);
            }
        }
    }
}
