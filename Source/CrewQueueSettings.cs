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
using System.Collections;
using System.Reflection;



namespace CrewQueue
{
    [KSPScenario(ScenarioCreationOptions.AddToAllGames, new GameScenes[] { GameScenes.EDITOR, GameScenes.FLIGHT, GameScenes.SPACECENTER, GameScenes.TRACKSTATION })]
    class CrewQueueSettings : ScenarioModule
    {
        // Singleton boilerplate
        private static CrewQueueSettings _Instance;
        internal static CrewQueueSettings Instance
        {
            get
            {
                if (_Instance == null)
                {
                    throw new Exception("ERROR: Attempted to query CrewQueue.Data before it was loaded.");
                }

                return _Instance;
            }
        }

#if false
        [KSPField(isPersistant = true)]
        public bool HideSettingsIcon = false;
#endif

#if false
        [KSPField(isPersistant = true)]
        public bool AssignCrews = true;

        [KSPField(isPersistant = true)]
        public double VacationScalar = 0.1;

        [KSPField(isPersistant = true)]
        public int MinimumVacationDays = 7;

        [KSPField(isPersistant = true)]
        public int MaximumVacationDays = 28;
#endif
        public bool AssignCrews {  get { return HighLogic.CurrentGame.Parameters.CustomParams<CrewQCustomParams>().AssignCrews; } }
        public double VacationScalar { get { return HighLogic.CurrentGame.Parameters.CustomParams<CrewQCustomParams>().VacationScalar; } }
        public int MinimumVacationDays { get { return HighLogic.CurrentGame.Parameters.CustomParams<CrewQCustomParams>().MinimumVacationDays; } }
        public int MaximumVacationDays { get { return HighLogic.CurrentGame.Parameters.CustomParams<CrewQCustomParams>().MaximumVacationDays; } }
        public bool Enabled { get { return HighLogic.CurrentGame.Parameters.CustomParams<CrewQCustomParams>().enabled; } }

        public override void OnAwake()
        {
            _Instance = this;
        }

        void Destroy()
        {
            _Instance = null;
        }

        // ScenarioModule methods
        public override void OnLoad(ConfigNode rootNode)
        {
            CrewQueueRoster.Instance.Flush();
            if (rootNode.HasNode("CrewList"))
            {
                rootNode = rootNode.GetNode("CrewList");
                IEnumerable<ConfigNode> crewNodes = rootNode.GetNodes();

                foreach (ConfigNode crewNode in crewNodes)
                {
                    CrewQueueRoster.Instance.AddExtElement(new CrewQueueRoster.KerbalExtData(crewNode));
                }
            }
        }

        public override void OnSave(ConfigNode rootNode)
        {
            rootNode.RemoveNode("CrewList");
            ConfigNode crewNodes = new ConfigNode("CrewList");

            foreach (CrewQueueRoster.KerbalExtData crewNode in CrewQueueRoster.Instance.ExtDataSet)
            {
                bool rosterHidden = (crewNode.ProtoReference.rosterStatus == CrewQueue.ROSTERSTATUS_VACATION);

                if (rosterHidden)
                {
                    crewNode.ProtoReference.rosterStatus = ProtoCrewMember.RosterStatus.Available;
                }

                crewNodes.AddNode(crewNode.ConfigNode);

                if (rosterHidden)
                {
                    crewNode.ProtoReference.rosterStatus = CrewQueue.ROSTERSTATUS_VACATION;
                }
            }

            rootNode.AddNode(crewNodes);
        }
    }

    //   HighLogic.CurrentGame.Parameters.CustomParams<CrewQCustomParams>()

    public class CrewQCustomParams : GameParameters.CustomParameterNode
    {
        public override string Title { get { return ""; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Crew Queue"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return true; } }

#if false
        [GameParameters.CustomParameterUI("Mod Enabled?",
            toolTip = "Changing this requires restarting the game")]

#endif
        public bool enabled = true;


        [GameParameters.CustomParameterUI("Automatically select crew?")]
        public bool AssignCrews = true;

        // The following crazy code is due to a bug introduced in 1.2.2
        // See this link for details:  http://forum.kerbalspaceprogram.com/index.php?/topic/7542-the-official-unoffical-quothelp-a-fellow-plugin-developerquot-thread/&page=100#comment-2887044

        // [GameParameters.CustomFloatParameterUI("Base vacation rate", asPercentage = true)]
        //public float VacationScalar = .1f;       


        public float vacationScalar = 0.1F;
        [GameParameters.CustomFloatParameterUI("Base vacation rate (%)", displayFormat = "N0", minValue = 0, maxValue = 100, stepCount = 1, asPercentage = false,
            toolTip = "The vacation time will be calculated by multiplying the mission length by this")]
        public float VacationScalar
        {
            get { return vacationScalar * 100; }
            set { vacationScalar = value / 100.0f; }
        }

        [GameParameters.CustomIntParameterUI("Minimum vacation days", minValue = 1, maxValue = 100,
            toolTip = "Minimum time for vacation, overrides the calculated vacation")]
        public int MinimumVacationDays = 7;         


        [GameParameters.CustomIntParameterUI("Maximum vacation days", minValue = 1, maxValue = 100,
            toolTip = "Maximum time for vacation, overrides the calculated vacation")]
        public int MaximumVacationDays = 28;          


        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
        }

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            return true;
        }
        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            if (MinimumVacationDays > MaximumVacationDays)
                MaximumVacationDays = MinimumVacationDays;
            return true;
        }
        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }
    }

}
