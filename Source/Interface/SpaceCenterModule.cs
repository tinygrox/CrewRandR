/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2015 Alexander Taylor
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using KSPPluginFramework;
using KSP.UI;
using KSP.UI.Screens;
using FingerboxLib;

namespace CrewRandR.Interface
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    class SpaceCenterModule : SceneModule
    {
        private bool astronautComplexSpawned;

        private void Start()
        {
            Logging.Info("SpaceCenterModule.Start");
            CrewRandRRoster.RestoreVacationingCrew();
        }

        protected override void Awake()
        {
            base.Awake();
            GameEvents.onGUILaunchScreenVesselSelected.Add(onVesselSelected);
            GameEvents.onGUIAstronautComplexSpawn.Add(onGUIAstronautComplexSpawn);
            GameEvents.onGUIAstronautComplexDespawn.Add(onGUIAstronautComplexDespawn);
            GameEvents.onGUILaunchScreenSpawn.Add(onGUILaunchScreenSpawn);
            GameEvents.onGUILaunchScreenDespawn.Add(onGUILaunchScreenDespawn);
            lastUpdated = MAX_UPDATE_THRESHOLD + Time.realtimeSinceStartup + 1;
            updateThreshold = 0;
        }

        // No need to update more than once every few seconds here
        private float lastUpdated = 0f;
        private float updateThreshold = 0f;
        private const float MAX_UPDATE_THRESHOLD = 5f; // UI update period in seconds

        protected override void LateUpdate()
        {
            if (astronautComplexSpawned)
            {
                if (lastUpdated + updateThreshold <= Time.realtimeSinceStartup)
                {
                    // This is so that it will update quickly when first entering, but then will be delayed to reduce lag
                    if (updateThreshold < MAX_UPDATE_THRESHOLD)
                        updateThreshold += 0.1f;

                    Logging.Debug("AC is spawned...");
                    lastUpdated = Time.realtimeSinceStartup;
                    AstronautComplex ac = GameObject.FindObjectOfType<AstronautComplex>();
                    if (ac)
                    {
                        foreach (var s in ac.ScrollListAvailable)
                        {
                            Logging.Info("");
                            IEnumerable<CrewListItem> crewItemContainers = GameObject.FindObjectsOfType<CrewListItem>().Where(x => x.GetCrewRef().rosterStatus == ProtoCrewMember.RosterStatus.Available);
                            foreach (CrewListItem crewContainer in crewItemContainers)
                            {
                                if (crewContainer.GetCrewRef().VacationExpiry() - Planetarium.GetUniversalTime() > 0)
                                {
                                    Logging.Debug("relabeling: " + crewContainer.GetName());
                                    string label = "Ready In: " + Utilities.GetFormattedTime(crewContainer.GetCrewRef().VacationExpiry() - Planetarium.GetUniversalTime());
                                    crewContainer.SetLabel(label);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void onGUIAstronautComplexDespawn()
        {
            astronautComplexSpawned = false;
        }

        private void onGUIAstronautComplexSpawn()
        {
            astronautComplexSpawned = true;
        }

        private void onGUILaunchScreenSpawn(GameEvents.VesselSpawnInfo info)
        {
            CrewRandRRoster.HideVacationingCrew();
        }

        private void onGUILaunchScreenDespawn()
        {
            CrewRandRRoster.RestoreVacationingCrew();
        }

        private void onVesselSelected(ShipTemplate shipTemplate)
        {
            CleanManifest();
            RemapFillButton();
        }
    }
}
