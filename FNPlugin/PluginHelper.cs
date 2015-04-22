﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace FNPlugin
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class PluginHelper : MonoBehaviour
    {
        public const double FIXED_SAT_ALTITUDE = 13599840256;
        public const int REF_BODY_KERBOL = 0;
        public const int REF_BODY_KERBIN = 1;
        public const int REF_BODY_MUN = 2;
        public const int REF_BODY_MINMUS = 3;
        public const int REF_BODY_MOHO = 4;
        public const int REF_BODY_EVE = 5;
        public const int REF_BODY_DUNA = 6;
        public const int REF_BODY_IKE = 7;
        public const int REF_BODY_JOOL = 8;
        public const int REF_BODY_LAYTHE = 9;
        public const int REF_BODY_VALL = 10;
        public const int REF_BODY_BOP = 11;
        public const int REF_BODY_TYLO = 12;
        public const int REF_BODY_GILLY = 13;
        public const int REF_BODY_POL = 14;
        public const int REF_BODY_DRES = 15;
        public const int REF_BODY_EELOO = 16;

        public static bool using_toolbar = false;
        public const int interstellar_major_version = 13;
        public const int interstellar_minor_version = 5;

        protected static bool plugin_init = false;
        protected static GameDatabase gdb;
        protected static bool resources_configured = false;

        #region Static Properties

        public static bool TechnologyIsInUse 
        { 
            get { return (HighLogic.CurrentGame.Mode == Game.Modes.CAREER || HighLogic.CurrentGame.Mode == Game.Modes.SCIENCE_SANDBOX); } 
        }

        public static ConfigNode PluginSettingsConfig 
        { 
            get { return GameDatabase.Instance.GetConfigNode("WarpPlugin/WarpPluginSettings/WarpPluginSettings"); } 
        }

        public static string PluginSaveFilePath
        {
            get { return KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/WarpPlugin.cfg"; }
        }

        public static string PluginSettingsFilePath
        {
            get {  return KSPUtil.ApplicationRootPath + "GameData/WarpPlugin/WarpPluginSettings.cfg"; }
        }

        public static Dictionary<string, string> PartTechUpgrades { get; private set; }

        public static Dictionary<string, string> OrsResourceMappings { get; private set; }

        private static double _maxAtmosphericAltitudeMult = 1;
        public static double MaxAtmosphericAltitudeMult { get { return _maxAtmosphericAltitudeMult; } }

        private static double _minAtmosphericAirDensity = 0;
        public static double MinAtmosphericAirDensity { get { return _minAtmosphericAirDensity; } }

		private static double _gravityConstant = GameConstants.STANDARD_GRAVITY; 
        public static double GravityConstant { get { return _gravityConstant; } }

		private static double _ispCoreTempMult = GameConstants.IspCoreTemperatureMultiplier;
        public static double IspCoreTempMult { get { return _ispCoreTempMult; } }

		private static double _lowCoreTempBaseThrust = 0;
        public static double LowCoreTempBaseThrust { get { return _lowCoreTempBaseThrust; } }

		private static double _highCoreTempThrustMult = GameConstants.HighCoreTempThrustMultiplier;
        public static double HighCoreTempThrustMult { get { return _highCoreTempThrustMult; } }

		private static double _thrustCoreTempThreshold = 0;
        public static double ThrustCoreTempThreshold { get { return _thrustCoreTempThreshold; } }

		private static double _globalThermalNozzlePowerMaxThrustMult = 1;
        public static double GlobalThermalNozzlePowerMaxThrustMult { get { return _globalThermalNozzlePowerMaxThrustMult; } }

		private static double _globalMagneticNozzlePowerMaxThrustMult = 1;
        public static double GlobalMagneticNozzlePowerMaxThrustMult { get { return _globalMagneticNozzlePowerMaxThrustMult; } }

		private static double _globalElectricEnginePowerMaxThrustMult = 1;
        public static double GlobalElectricEnginePowerMaxThrustMult { get { return _globalElectricEnginePowerMaxThrustMult; } }

		private static float _maxPowerDrawForExoticMatterMult = 1;
        public static float MaxPowerDrawForExoticMatterMult { get { return _maxPowerDrawForExoticMatterMult; } }

		private static double _lfoFuelThrustModifier = GameConstants.LfoFuelThrustModifier;
        public static double LfoFuelThrustModifier { get { return _lfoFuelThrustModifier; } }

        private static double _electricEngineIspMult = 1;
        public static double ElectricEngineIspMult { get { return _electricEngineIspMult; } }

        private static float _electricEnginePowerPropellantIspMultLimiter = 1;
        public static float ElectricEnginePowerPropellantIspMultLimiter { get { return _electricEnginePowerPropellantIspMultLimiter; } }

        private static float _electricEngineAtmosphericDensityThrustLimiter = 0;
        public static float ElectricEngineAtmosphericDensityThrustLimiter { get { return _electricEngineAtmosphericDensityThrustLimiter; } }

        //------------------------------------------------------------------------------------------

		private static double _basePowerConsumption = GameConstants.basePowerConsumption;
		public static double BasePowerConsumption { get { return PowerConsumptionMultiplier * _basePowerConsumption; } }

		private static double _baseAMFPowerConsumption = GameConstants.baseAMFPowerConsumption;
		public static double BaseAMFPowerConsumption { get { return PowerConsumptionMultiplier * _baseAMFPowerConsumption; } }

		private static double _baseCentriPowerConsumption = GameConstants.baseCentriPowerConsumption;
		public static double BaseCentriPowerConsumption { get { return PowerConsumptionMultiplier * _baseCentriPowerConsumption; } }

		private static double _baseELCPowerConsumption = GameConstants.baseELCPowerConsumption;
		public static double BaseELCPowerConsumption { get { return PowerConsumptionMultiplier * _baseELCPowerConsumption; } }

		private static double _baseAnthraquiononePowerConsumption = GameConstants.baseAnthraquiononePowerConsumption;
		public static double BaseAnthraquiononePowerConsumption { get { return PowerConsumptionMultiplier * _baseAnthraquiononePowerConsumption; } }

		private static double _basePechineyUgineKuhlmannPowerConsumption = GameConstants.basePechineyUgineKuhlmannPowerConsumption;
		public static double BasePechineyUgineKuhlmannPowerConsumption { get { return PowerConsumptionMultiplier * _basePechineyUgineKuhlmannPowerConsumption; } }

		private static double _baseHaberProcessPowerConsumption = GameConstants.baseHaberProcessPowerConsumption;
		public static double BaseHaberProcessPowerConsumption { get { return PowerConsumptionMultiplier * _baseHaberProcessPowerConsumption; } }

		private static double _baseUraniumAmmonolysisPowerConsumption = GameConstants.baseUraniumAmmonolysisPowerConsumption;
		public static double BaseUraniumAmmonolysisPowerConsumption { get { return PowerConsumptionMultiplier * _baseUraniumAmmonolysisPowerConsumption; } }

        //------------------------------------------------------------------------------------------------

		private static double _anthraquinoneEnergyPerTon = GameConstants.anthraquinoneEnergyPerTon;
		public static double AnthraquinoneEnergyPerTon { get { return PowerConsumptionMultiplier * _anthraquinoneEnergyPerTon; } }

		private static double _haberProcessEnergyPerTon = GameConstants.haberProcessEnergyPerTon;
		public static double HaberProcessEnergyPerTon { get { return PowerConsumptionMultiplier * _haberProcessEnergyPerTon; } }

		private static double _electrolysisEnergyPerTon = GameConstants.waterElectrolysisEnergyPerTon;
		public static double ElectrolysisEnergyPerTon { get { return PowerConsumptionMultiplier * _electrolysisEnergyPerTon; } }

		private static double _aluminiumElectrolysisEnergyPerTon = GameConstants.aluminiumElectrolysisEnergyPerTon;
		public static double AluminiumElectrolysisEnergyPerTon { get { return PowerConsumptionMultiplier * _aluminiumElectrolysisEnergyPerTon; } }

		private static double _pechineyUgineKuhlmannEnergyPerTon = GameConstants.pechineyUgineKuhlmannEnergyPerTon;
		public static double PechineyUgineKuhlmannEnergyPerTon { get { return PowerConsumptionMultiplier * _pechineyUgineKuhlmannEnergyPerTon; } }


        private static double _powerConsumptionMultiplier = 1;
        public static double PowerConsumptionMultiplier { get { return _powerConsumptionMultiplier; } }
        
        //----------------------------------------------------------------------------------------------
		private static float _ispNtrPropellantModifierBase = 0;
        public static float IspNtrPropellantModifierBase { get { return _ispNtrPropellantModifierBase; } }

        private static float _ispElectroPropellantModifierBase = 0;
        public static float IspElectroPropellantModifierBase { get { return _ispElectroPropellantModifierBase; } }

		private static float _maxThermalNozzleIsp = GameConstants.MaxThermalNozzleIsp;
        public static float MaxThermalNozzleIsp { get { return _maxThermalNozzleIsp; } }



		private static bool _isPanelHeatingClamped = false;
        public static bool IsSolarPanelHeatingClamped { get { return _isPanelHeatingClamped; }}

		private static bool _isThermalDissipationDisabled = false;
        public static bool IsThermalDissipationDisabled { get { return _isThermalDissipationDisabled; } }

		private static bool _isRecieverTempTweaked = false;
        public static bool IsRecieverCoreTempTweaked { get { return _isRecieverTempTweaked; } }

	    private static bool _limitedWarpTravel = false;
        public static bool LimitedWarpTravel { get { return _limitedWarpTravel; } }

        private static bool _radiationMechanicsDisabled = false;
        public static bool RadiationMechanicsDisabled { get { return _radiationMechanicsDisabled; } }

        private static bool _matchDemandWithSupply = false;
        public static bool MatchDemandWithSupply { get { return _matchDemandWithSupply; } }

        #endregion

        public static bool hasTech(string techid)
        {
            try
            {
                string persistentfile = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/persistent.sfs";
                ConfigNode config = ConfigNode.Load(persistentfile);
                ConfigNode gameconf = config.GetNode("GAME");
                ConfigNode[] scenarios = gameconf.GetNodes("SCENARIO");
                foreach (ConfigNode scenario in scenarios)
                {
                    if (scenario.GetValue("name") == "ResearchAndDevelopment")
                    {
                        ConfigNode[] techs = scenario.GetNodes("Tech");
                        foreach (ConfigNode technode in techs)
                        {
                            if (technode.GetValue("id") == techid)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool upgradeAvailable(string techid)
        {
            if (HighLogic.CurrentGame != null)
            {
                if (PluginHelper.TechnologyIsInUse)
                {
                    if (techid != null && PluginHelper.hasTech(techid))
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public static float getKerbalRadiationDose(int kerbalidx)
        {
            try
            {
                string persistentfile = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/persistent.sfs";
                ConfigNode config = ConfigNode.Load(persistentfile);
                ConfigNode gameconf = config.GetNode("GAME");
                ConfigNode crew_roster = gameconf.GetNode("ROSTER");
                ConfigNode[] crew = crew_roster.GetNodes("CREW");
                ConfigNode sought_kerbal = crew[kerbalidx];
                if (sought_kerbal.HasValue("totalDose"))
                {
                    float dose = float.Parse(sought_kerbal.GetValue("totalDose"));
                    return dose;
                }
                return 0.0f;
            }
            catch (Exception ex)
            {
                print(ex);
                return 0.0f;
            }
        }

        public static ConfigNode getKerbal(int kerbalidx)
        {
            try
            {
                string persistentfile = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/persistent.sfs";
                ConfigNode config = ConfigNode.Load(persistentfile);
                ConfigNode gameconf = config.GetNode("GAME");
                ConfigNode crew_roster = gameconf.GetNode("ROSTER");
                ConfigNode[] crew = crew_roster.GetNodes("CREW");
                ConfigNode sought_kerbal = crew[kerbalidx];
                return sought_kerbal;
            }
            catch (Exception ex)
            {
                print(ex);
                return null;
            }
        }

        public static void saveKerbalRadiationdose(int kerbalidx, float rad)
        {
            try
            {
                string persistentfile = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/persistent.sfs";
                ConfigNode config = ConfigNode.Load(persistentfile);
                ConfigNode gameconf = config.GetNode("GAME");
                ConfigNode crew_roster = gameconf.GetNode("ROSTER");
                ConfigNode[] crew = crew_roster.GetNodes("CREW");
                ConfigNode sought_kerbal = crew[kerbalidx];
                if (sought_kerbal.HasValue("totalDose"))
                {
                    sought_kerbal.SetValue("totalDose", rad.ToString("E"));
                }
                else
                {
                    sought_kerbal.AddValue("totalDose", rad.ToString("E"));
                }
                config.Save(persistentfile);
            }
            catch (Exception ex)
            {
                print(ex);
            }
        }

        public static ConfigNode getPluginSaveFile()
        {
            ConfigNode config = ConfigNode.Load(PluginHelper.PluginSaveFilePath);
            if (config == null)
            {
                config = new ConfigNode();
                config.AddValue("writtenat", DateTime.Now.ToString());
                config.Save(PluginHelper.PluginSaveFilePath);
            }
            return config;
        }

        public static ConfigNode getPluginSettingsFile()
        {
            ConfigNode config = ConfigNode.Load(PluginHelper.PluginSettingsFilePath);
            if (config == null)
            {
                config = new ConfigNode();
            }
            return config;
        }

        public static bool lineOfSightToSun(Vessel vess)
        {
            Vector3d a = PluginHelper.getVesselPos(vess);
            Vector3d b = FlightGlobals.Bodies[0].transform.position;
            foreach (CelestialBody referenceBody in FlightGlobals.Bodies)
            {
                if (referenceBody.flightGlobalsIndex == 0)
                { // the sun should not block line of sight to the sun
                    continue;
                }
                Vector3d refminusa = referenceBody.position - a;
                Vector3d bminusa = b - a;
                if (Vector3d.Dot(refminusa, bminusa) > 0)
                {
                    if (Vector3d.Dot(refminusa, bminusa.normalized) < bminusa.magnitude)
                    {
                        Vector3d tang = refminusa - Vector3d.Dot(refminusa, bminusa.normalized) * bminusa.normalized;
                        if (tang.magnitude < referenceBody.Radius)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public static Vector3d getVesselPos(Vessel v)
        {
            Vector3d v1p = (v.state == Vessel.State.ACTIVE) ? (Vector3d)v.transform.position : v.GetWorldPos3D();
            return v1p;
        }

        public static float getMaxAtmosphericAltitude(CelestialBody body)
        {
            if (!body.atmosphere) return 0;

            return (float)-body.atmosphereScaleHeight * 1000.0f * Mathf.Log(1e-6f);
        }

        public static float getScienceMultiplier(int refbody, bool landed)
        {
            float multiplier = 1;

            if (refbody == REF_BODY_DUNA || refbody == REF_BODY_EVE || refbody == REF_BODY_IKE || refbody == REF_BODY_GILLY)
                multiplier = 5f;
            else if (refbody == REF_BODY_MUN || refbody == REF_BODY_MINMUS)
                multiplier = 2.5f;
            else if (refbody == REF_BODY_JOOL || refbody == REF_BODY_TYLO || refbody == REF_BODY_POL || refbody == REF_BODY_BOP)
                multiplier = 10f;
            else if (refbody == REF_BODY_LAYTHE || refbody == REF_BODY_VALL)
                multiplier = 12f;
            else if (refbody == REF_BODY_EELOO || refbody == REF_BODY_MOHO)
                multiplier = 20f;
            else if (refbody == REF_BODY_DRES)
                multiplier = 7.5f;
            else if (refbody == REF_BODY_KERBIN)
                multiplier = 2f;
            else if (refbody == REF_BODY_KERBOL)
                multiplier = 15f;
            else
                multiplier = 20f; // must be somewhere special

            if (landed)
            {
                if (refbody == REF_BODY_TYLO)
                    multiplier *= 3f;
                if (refbody == REF_BODY_KERBIN)
                    multiplier *= 0.5f; // Starting on Kerbin is earier than getting a lab into orbit
                else if (refbody == REF_BODY_EVE)
                    multiplier *= 2.5f;
                else
                    multiplier *=  2f;
            }

            return multiplier;
        }

        public static float getImpactorScienceMultiplier(int refbody)
        {
            float multiplier = 1;

            if (refbody == REF_BODY_DUNA || refbody == REF_BODY_EVE || refbody == REF_BODY_IKE || refbody == REF_BODY_GILLY)
            {
                multiplier = 7f;
            }
            else if (refbody == REF_BODY_MUN || refbody == REF_BODY_MINMUS)
            {
                multiplier = 5f;
            }
            else if (refbody == REF_BODY_JOOL || refbody == REF_BODY_TYLO || refbody == REF_BODY_POL || refbody == REF_BODY_BOP)
            {
                multiplier = 9f;
            }
            else if (refbody == REF_BODY_LAYTHE || refbody == REF_BODY_VALL)
            {
                multiplier = 11f;
            }
            else if (refbody == REF_BODY_EELOO || refbody == REF_BODY_MOHO)
            {
                multiplier = 14f;
            }
            else if (refbody == REF_BODY_DRES)
            {
                multiplier = 8f;
            }
            else if (refbody == REF_BODY_KERBIN)
            {
                multiplier = 0.5f;
            }
            else
            {
                multiplier = 0f;
            }
            return multiplier;
        }

        public static string getFormattedPowerString(double power)
        {
            if (power > 1000)
            {
                if (power > 20000)
                {
                    return (power / 1000).ToString("0") + " GW";
                }
                else
                {
                    return (power / 1000).ToString("0.0") + " GW";
                }
            }
            else
            {
                if (power > 20)
                {
                    return power.ToString("0") + " MW";
                }
                else
                {
                    if (power > 1)
                    {
                        return power.ToString("0.0") + " MW";
                    }
                    else
                    {
                        return (power * 1000).ToString("0.0") + " KW";
                    }
                }
            }
        }

        public void Update()
        {
            this.enabled = true;
            AvailablePart intakePart = PartLoader.getPartInfoByName("CircularIntake");
            if (intakePart != null)
            {
                if (intakePart.partPrefab.FindModulesImplementing<AtmosphericIntake>().Count <= 0 && PartLoader.Instance.IsReady())
                {
                    plugin_init = false;
                }
            }

            if (!resources_configured)
            {
                // read WarpPluginSettings.cfg 
                ConfigNode plugin_settings = GameDatabase.Instance.GetConfigNode("WarpPlugin/WarpPluginSettings/WarpPluginSettings");
                if (plugin_settings != null)
                {
                    if (plugin_settings.HasValue("PartTechUpgrades"))
                    {
                        PartTechUpgrades = new Dictionary<string, string>();

                        string rawstring = plugin_settings.GetValue("PartTechUpgrades");
                        string[] splitValues = rawstring.Split(',').Select(sValue => sValue.Trim()).ToArray();

                        int pairs = splitValues.Length / 2;
                        int totalValues = splitValues.Length / 2 * 2;
                        for (int i = 0; i < totalValues; i += 2)
                            PartTechUpgrades.Add(splitValues[i], splitValues[i + 1]);

                        Debug.Log("[KSP Interstellar] Part Tech Upgrades set to: " + rawstring);
                    }
                    if (plugin_settings.HasValue("OrsResourceMappings"))
                    {
                        OrsResourceMappings = new Dictionary<string, string>();

                        string rawstring = plugin_settings.GetValue("OrsResourceMappings");
                        string[] splitValues = rawstring.Split(',').Select(sValue => sValue.Trim()).ToArray();

                        int pairs = splitValues.Length / 2;
                        int totalValues = pairs * 2;
                        for (int i = 0; i < totalValues; i += 2)
                            OrsResourceMappings.Add(splitValues[i], splitValues[i + 1]);
                    }
                    if (plugin_settings.HasValue("RadiationMechanicsDisabled"))
                    {
                        PluginHelper._radiationMechanicsDisabled = bool.Parse(plugin_settings.GetValue("RadiationMechanicsDisabled"));
                        Debug.Log("[KSP Interstellar] Radiation Mechanics Disabled set to: " + PluginHelper.RadiationMechanicsDisabled.ToString());
                    }
                    if (plugin_settings.HasValue("ThermalMechanicsDisabled"))
                    {
                        PluginHelper._isThermalDissipationDisabled = bool.Parse(plugin_settings.GetValue("ThermalMechanicsDisabled"));
                        Debug.Log("[KSP Interstellar] ThermalMechanics set to : " + (!PluginHelper.IsThermalDissipationDisabled).ToString());
                    }
                    if (plugin_settings.HasValue("SolarPanelClampedHeating"))
                    {
                        PluginHelper._isPanelHeatingClamped = bool.Parse(plugin_settings.GetValue("SolarPanelClampedHeating"));
                        Debug.Log("[KSP Interstellar] Solar panels clamped heating set to enabled: " + PluginHelper.IsSolarPanelHeatingClamped.ToString());
                    }
                    if (plugin_settings.HasValue("RecieverTempTweak"))
                    {
                        PluginHelper._isRecieverTempTweaked = bool.Parse(plugin_settings.GetValue("RecieverTempTweak"));
                        Debug.Log("[KSP Interstellar] Microwave reciever CoreTemp tweak is set to enabled: " + PluginHelper.IsRecieverCoreTempTweaked.ToString());
                    }
                    if (plugin_settings.HasValue("LimitedWarpTravel"))
                    {
                        PluginHelper._limitedWarpTravel = bool.Parse(plugin_settings.GetValue("LimitedWarpTravel"));
                        Debug.Log("[KSP Interstellar] Apply Limited Warp Travel: " + PluginHelper.LimitedWarpTravel.ToString());
                    }
                    if (plugin_settings.HasValue("MatchDemandWithSupply"))
                    {
                        PluginHelper._matchDemandWithSupply = bool.Parse(plugin_settings.GetValue("MatchDemandWithSupply"));
                        Debug.Log("[KSP Interstellar] Match Demand With Supply: " + PluginHelper.MatchDemandWithSupply.ToString());
                    }

                    if (plugin_settings.HasValue("MaxPowerDrawForExoticMatterMult"))
                    {
                        PluginHelper._maxPowerDrawForExoticMatterMult = float.Parse(plugin_settings.GetValue("MaxPowerDrawForExoticMatterMult"));
                        Debug.Log("[KSP Interstellar] Max Power Draw For Exotic Matter Multiplier set to: " + PluginHelper.MaxPowerDrawForExoticMatterMult.ToString("0.000000"));
                    }

                    if (plugin_settings.HasValue("GravityConstant"))
                    {
                        PluginHelper._gravityConstant = double.Parse(plugin_settings.GetValue("GravityConstant"));
                        Debug.Log("[KSP Interstellar] Gravity constant set to: " + PluginHelper.GravityConstant.ToString("0.000000"));
                    }
                    if (plugin_settings.HasValue("IspCoreTempMult"))
                    {
                        PluginHelper._ispCoreTempMult = double.Parse(plugin_settings.GetValue("IspCoreTempMult"));
                        Debug.Log("[KSP Interstellar] Isp core temperature multiplier set to: " + PluginHelper.IspCoreTempMult.ToString("0.000000"));
                    }
                    if (plugin_settings.HasValue("ElectricEngineIspMult"))
                    {
                        PluginHelper._electricEngineIspMult = double.Parse(plugin_settings.GetValue("ElectricEngineIspMult"));
                        Debug.Log("[KSP Interstellar] Electric EngineIsp Multiplier set to: " + PluginHelper.ElectricEngineIspMult.ToString("0.000000"));
                    }



                    if (plugin_settings.HasValue("GlobalThermalNozzlePowerMaxTrustMult"))
                    {
                        PluginHelper._globalThermalNozzlePowerMaxThrustMult = double.Parse(plugin_settings.GetValue("GlobalThermalNozzlePowerMaxTrustMult"));
                        Debug.Log("[KSP Interstellar] Maximum Global Thermal Power Maximum Thrust Multiplier set to: " + PluginHelper.GlobalThermalNozzlePowerMaxThrustMult.ToString("0.0"));
                    }
                    if (plugin_settings.HasValue("GlobalMagneticNozzlePowerMaxTrustMult"))
                    {
                        PluginHelper._globalMagneticNozzlePowerMaxThrustMult = double.Parse(plugin_settings.GetValue("GlobalMagneticNozzlePowerMaxTrustMult"));
                        Debug.Log("[KSP Interstellar] Maximum Global Magnetic Nozzle Power Maximum Thrust Multiplier set to: " + PluginHelper.GlobalMagneticNozzlePowerMaxThrustMult.ToString("0.0"));
                    }
                    if (plugin_settings.HasValue("GlobalElectricEnginePowerMaxTrustMult"))
                    {
                        PluginHelper._globalElectricEnginePowerMaxThrustMult = double.Parse(plugin_settings.GetValue("GlobalElectricEnginePowerMaxTrustMult"));
                        Debug.Log("[KSP Interstellar] Maximum Global Electric Engine Power Maximum Thrust Multiplier set to: " + PluginHelper.GlobalElectricEnginePowerMaxThrustMult.ToString("0.0"));
                    }
                    if (plugin_settings.HasValue("LfoFuelTrustModifier"))
                    {
                        PluginHelper._lfoFuelThrustModifier = double.Parse(plugin_settings.GetValue("LfoFuelTrustModifier"));
                        Debug.Log("[KSP Interstellar] Maximum Lfo Fuel Thrust Multiplier set to: " + PluginHelper.LfoFuelThrustModifier.ToString("0.0"));
                    }
                    if (plugin_settings.HasValue("MaxThermalNozzleIsp"))
                    {
                        PluginHelper._maxThermalNozzleIsp = float.Parse(plugin_settings.GetValue("MaxThermalNozzleIsp"));
                        Debug.Log("[KSP Interstellar] Maximum Thermal Nozzle Isp set to: " + PluginHelper.MaxThermalNozzleIsp.ToString("0.0"));
                    }

                    if (plugin_settings.HasValue("TrustCoreTempThreshold"))
                    {
                        PluginHelper._thrustCoreTempThreshold = double.Parse(plugin_settings.GetValue("TrustCoreTempThreshold"));
                        Debug.Log("[KSP Interstellar] Thrust core temperature threshold set to: " + PluginHelper.ThrustCoreTempThreshold.ToString("0.0"));
                    }
                    if (plugin_settings.HasValue("LowCoreTempBaseTrust"))
                    {
                        PluginHelper._lowCoreTempBaseThrust = double.Parse(plugin_settings.GetValue("LowCoreTempBaseTrust"));
                        Debug.Log("[KSP Interstellar] Low core temperature base thrust modifier set to: " + PluginHelper.LowCoreTempBaseThrust.ToString("0.0"));
                    }
                    if (plugin_settings.HasValue("HighCoreTempTrustMult"))
                    {
                        PluginHelper._highCoreTempThrustMult = double.Parse(plugin_settings.GetValue("HighCoreTempTrustMult"));
                        Debug.Log("[KSP Interstellar] High core temperature thrust divider set to: " + PluginHelper.HighCoreTempThrustMult.ToString("0.0"));
                    }
                    if (plugin_settings.HasValue("BasePowerConsumption"))
                    {
                        PluginHelper._basePowerConsumption = double.Parse(plugin_settings.GetValue("BasePowerConsumption"));
                        Debug.Log("[KSP Interstellar] Base Power Consumption set to: " + PluginHelper.BasePowerConsumption.ToString("0.0"));
                    }
                    if (plugin_settings.HasValue("PowerConsumptionMultiplier"))
                    {
                        PluginHelper._powerConsumptionMultiplier = double.Parse(plugin_settings.GetValue("PowerConsumptionMultiplier"));
                        Debug.Log("[KSP Interstellar] Base Power Consumption set to: " + PluginHelper.PowerConsumptionMultiplier.ToString("0.0"));
                    }
                    if (plugin_settings.HasValue("IspNtrPropellantModifierBase"))
                    {
                        PluginHelper._ispNtrPropellantModifierBase = float.Parse(plugin_settings.GetValue("IspNtrPropellantModifierBase"));
                        Debug.Log("[KSP Interstellar] Isp Ntr Propellant Modifier Base set to: " + PluginHelper.IspNtrPropellantModifierBase.ToString("0.0"));
                    }
                    if (plugin_settings.HasValue("IspElectroPropellantModifierBase"))
                    {
                        PluginHelper._ispElectroPropellantModifierBase = float.Parse(plugin_settings.GetValue("IspNtrPropellantModifierBase"));
                        Debug.Log("[KSP Interstellar] Isp Ntr Propellant Modifier Base set to: " + PluginHelper.IspElectroPropellantModifierBase.ToString("0.0"));
                    }
                    if (plugin_settings.HasValue("ElectricEnginePowerPropellantIspMultLimiter"))
                    {
                        PluginHelper._electricEnginePowerPropellantIspMultLimiter = float.Parse(plugin_settings.GetValue("ElectricEnginePowerPropellantIspMultLimiter"));
                        Debug.Log("[KSP Interstellar] Electric Engine Power Propellant IspMultiplier Limiter set to: " + PluginHelper.ElectricEnginePowerPropellantIspMultLimiter.ToString("0.0"));
                    }
                    if (plugin_settings.HasValue("ElectricEngineAtmosphericDensityTrustLimiter"))
                    {
                        PluginHelper._electricEngineAtmosphericDensityThrustLimiter = float.Parse(plugin_settings.GetValue("ElectricEngineAtmosphericDensityTrustLimiter"));
                        Debug.Log("[KSP Interstellar] Electric Engine Power Propellant IspMultiplier Limiter set to: " + PluginHelper.ElectricEngineAtmosphericDensityThrustLimiter.ToString("0.0"));
                    }

                    if (plugin_settings.HasValue("MaxAtmosphericAltitudeMult"))
                    {
                        PluginHelper._maxAtmosphericAltitudeMult = double.Parse(plugin_settings.GetValue("MaxAtmosphericAltitudeMult"));
                        Debug.Log("[KSP Interstellar] Maximum Atmospheric Altitude Multiplier set to: " + PluginHelper.MaxAtmosphericAltitudeMult.ToString("0.0"));
                    }
                    if (plugin_settings.HasValue("MinAtmosphericAirDensity"))
                    {
                        PluginHelper._minAtmosphericAirDensity = double.Parse(plugin_settings.GetValue("MinAtmosphericAirDensity"));
                        Debug.Log("[KSP Interstellar] Minimum Atmospheric Air Density set to: " + PluginHelper.MinAtmosphericAirDensity.ToString("0.0"));
                    }

                    resources_configured = true;
                }
                else
                {
                    showInstallationErrorMessage();
                }

            }

            if (plugin_init) return;

            gdb = GameDatabase.Instance;
            plugin_init = true;

            AvailablePart kerbalRadiationPart = PartLoader.getPartInfoByName("kerbalEVA");
            if (kerbalRadiationPart.partPrefab.Modules != null)
            {
                if (kerbalRadiationPart.partPrefab.FindModulesImplementing<FNModuleRadiation>().Count == 0)
                    kerbalRadiationPart.partPrefab.gameObject.AddComponent<FNModuleRadiation>();
            }
            else
                kerbalRadiationPart.partPrefab.gameObject.AddComponent<FNModuleRadiation>();

            List<AvailablePart> available_parts = PartLoader.LoadedPartsList;
            foreach (AvailablePart available_part in available_parts)
            {
                Part prefab_available_part = available_part.partPrefab;
                try
                {
                    if (prefab_available_part.Modules == null) continue;

                    if (prefab_available_part.FindModulesImplementing<ModuleResourceIntake>().Count > 0)
                    {
                        ModuleResourceIntake intake = prefab_available_part.Modules["ModuleResourceIntake"] as ModuleResourceIntake;
                        if (intake.resourceName == "IntakeAir")
                        {
                            var pm = prefab_available_part.gameObject.AddComponent<AtmosphericIntake>();
                            prefab_available_part.Modules.Add(pm);
                            pm.area = intake.area;
                            pm.aoaThreshold = intake.aoaThreshold;
                            pm.intakeTransformName = intake.intakeTransformName;
                            pm.maxIntakeSpeed = intake.maxIntakeSpeed;
                            pm.unitScalar = intake.unitScalar;
                            pm.useIntakeCompensation = intake.useIntakeCompensation;
                            pm.storesResource = intake.storesResource;

                            PartResource intake_air_resource = prefab_available_part.Resources["IntakeAir"];

                            if (intake_air_resource != null && !prefab_available_part.Resources.Contains(InterstellarResourcesConfiguration.Instance.IntakeAtmosphere))
                            {
                                ConfigNode node = new ConfigNode("RESOURCE");
                                node.AddValue("name", InterstellarResourcesConfiguration.Instance.IntakeAtmosphere);
                                node.AddValue("maxAmount", intake_air_resource.maxAmount);
                                node.AddValue("amount", intake_air_resource.amount);
                                prefab_available_part.AddResource(node);
                            }
                        }

                    }

                    if (prefab_available_part.FindModulesImplementing<ModuleDeployableSolarPanel>().Count > 0)
                    {
                        ModuleDeployableSolarPanel panel = prefab_available_part.Modules["ModuleDeployableSolarPanel"] as ModuleDeployableSolarPanel;
                        if (panel.chargeRate > 0)
                        {
                            Type type = AssemblyLoader.GetClassByName(typeof(PartModule), "FNSolarPanelWasteHeatModule");
                            if (type != null)
                            {
                                FNSolarPanelWasteHeatModule pm = prefab_available_part.gameObject.AddComponent(type) as FNSolarPanelWasteHeatModule;
                                prefab_available_part.Modules.Add(pm);
                            }
                        }


                        if (!prefab_available_part.Resources.Contains("WasteHeat") && panel.chargeRate > 0)
                        {
                            ConfigNode node = new ConfigNode("RESOURCE");
                            node.AddValue("name", "WasteHeat");
                            node.AddValue("maxAmount", panel.chargeRate * 100);
                            node.AddValue("amount", 0);
                            PartResource pr = prefab_available_part.AddResource(node);

                            if (available_part.resourceInfo != null && pr != null)
                            {
                                if (available_part.resourceInfo.Length == 0)
                                    available_part.resourceInfo = pr.resourceName + ":" + pr.amount + " / " + pr.maxAmount;
                                else
                                    available_part.resourceInfo = available_part.resourceInfo + "\n" + pr.resourceName + ":" + pr.amount + " / " + pr.maxAmount;
                            }
                        }
                    }

                    if (prefab_available_part.FindModulesImplementing<ElectricEngineControllerFX>().Count() > 0)
                    {
                        available_part.moduleInfo = prefab_available_part.FindModulesImplementing<ElectricEngineControllerFX>().First().GetInfo();
                        available_part.moduleInfos.RemoveAll(modi => modi.moduleName == "Engine");
                        AvailablePart.ModuleInfo mod_info = available_part.moduleInfos.Where(modi => modi.moduleName == "Electric Engine Controller").First();
                        mod_info.moduleName = "Electric Engine";
                    }

                    if (prefab_available_part.FindModulesImplementing<FNNozzleController>().Count() > 0)
                    {
                        available_part.moduleInfo = prefab_available_part.FindModulesImplementing<FNNozzleController>().First().GetInfo();
                        available_part.moduleInfos.RemoveAll(modi => modi.moduleName == "Engine");
                        AvailablePart.ModuleInfo mod_info = available_part.moduleInfos.Where(modi => modi.moduleName == "FNNozzle Controller").First();
                        mod_info.moduleName = "Thermal Nozzle";
                    }

                    if (prefab_available_part.CrewCapacity > 0 || prefab_available_part.FindModulesImplementing<ModuleCommand>().Count > 0)
                    {
                        Type type = AssemblyLoader.GetClassByName(typeof(PartModule), "FNModuleRadiation");
                        if (type != null)
                        {
                            FNModuleRadiation pm = prefab_available_part.gameObject.AddComponent(type) as FNModuleRadiation;
                            prefab_available_part.Modules.Add(pm);
                            pm.rad_hardness = (float)(prefab_available_part.mass / (Math.Max(prefab_available_part.CrewCapacity, 0.1)) * 7.5);
                            AvailablePart.ModuleInfo minfo = new AvailablePart.ModuleInfo();
                            minfo.moduleName = "Radiation Status";
                            minfo.info = pm.GetInfo();
                            available_part.moduleInfos.Add(minfo);
                        }
                        print("Adding ModuleRadiation to " + prefab_available_part.name);
                    }
                }
                catch (Exception ex)
                {
                    if (prefab_available_part != null)
                        print("[KSP Interstellar] Exception caught adding to: " + prefab_available_part.name + " part: " + ex.ToString());
                    else
                        print("[KSP Interstellar] Exception caught adding to unknown module");
                }
            }
        }

        protected static bool warning_displayed = false;

        public static void showInstallationErrorMessage()
        {
            if (!warning_displayed)
            {
                PopupDialog.SpawnPopupDialog("KSP Interstellar Installation Error", "KSP Interstellar is unable to detect files required for proper functioning.  Please make sure that this mod has been installed to [Base KSP directory]/GameData/WarpPlugin.", "OK", false, HighLogic.Skin);
                warning_displayed = true;
            }
        }

    }
}
