﻿using System;
using System.Collections.Generic;
using System.Reflection;
using EightyOne.Areas;
using EightyOne.RedirectionFramework;
using EightyOne.ResourceManagers;
using EightyOne.Terrain;
using EightyOne.Zones;

namespace EightyOne
{
    public static class Detours
    {
        private static Dictionary<MethodInfo, RedirectCallsState> redirectsOnLoaded;
        private static Dictionary<MethodInfo, RedirectCallsState> redirectsOnCreated;
        public static bool IsEnabled;

        public static void SetUp()
        {
            Redirect(true);
        }

        public static void Deploy()
        {
            if (IsEnabled)
            {
                return;
            }
            IsEnabled = true;
            FakeWaterManager.Init();
            FakeDistrictManager.Init();
            FakeDistrictTool.Init();
            FakeImmaterialResourceManager.Init();
            FakeZoneManager.Init();
            FakeZoneTool.Init();
            FakeElectricityManager.Init();
            Redirect(false);
            FakeGameAreaManager.Init();
        }

        public static void Redirect(bool onCreated)
        {
            if (onCreated)
            {
                redirectsOnCreated = new Dictionary<MethodInfo, RedirectCallsState>();
            }
            else
            {
                redirectsOnLoaded = new Dictionary<MethodInfo, RedirectCallsState>();
            }
            var redirects = onCreated ? redirectsOnCreated : redirectsOnLoaded;
            var types = new[]
            {
                typeof(FakeAreasWrapper),
                typeof(FakeGameAreaInfoPanel),
                typeof(FakeGameAreaManager),
                typeof(FakeGameAreaManager.FakeData),
                typeof(FakeGameAreaManagerUI),
                typeof(FakeGameAreaTool),
                typeof(FakeNatualResourceManager),
                typeof(FakeNetManager),
                
                typeof(FakeDistrictManager),
                typeof(FakeDistrictTool),
                typeof(FakeElectricityManager),
                typeof(FakeImmaterialResourceManager),
                typeof(FakeWaterManager),
                
                typeof(FakeTerrainManager),
                
                typeof(FakeBuilding),
                typeof(FakeBuildingTool),
                typeof(FakeZoneBlock),
                typeof(FakeZoneManager),
                typeof(FakeZoneTool)
                
            };
            foreach (var type in types)
            {
                redirects.AddRange(RedirectionUtil.RedirectType(type, onCreated));
            }
        }

        private static void RevertRedirect(bool onCreated)
        {
            var redirects = onCreated ? redirectsOnCreated : redirectsOnLoaded;
            if(redirects == null){
                return;
            }
            foreach (var kvp in redirects)
            {
                RedirectionHelper.RevertRedirect(kvp.Key, kvp.Value);
            }
            redirects.Clear();
        }

        public static void Revert()
        {
            if (!IsEnabled)
            {
                return;
            }
            IsEnabled = false;
            RevertRedirect(false);
            FakeImmaterialResourceManager.OnDestroy();
            FakeDistrictManager.OnDestroy();
            FakeWaterManager.OnDestroy();
            FakeElectricityManager.OnDestroy();
            FakeGameAreaInfoPanel.OnDestroy();
        }

        public static void TearDown()
        {
            RevertRedirect(true);
        }

        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (source == null)
            {
                return;
            }
            foreach (var element in source)
                target.Add(element);
        }
    }
}