using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrewRandR
{
    class CrewRandRProxy
    {
        public static IEnumerable<ProtoCrewMember> AvailableCrew
        {
            get
            {
                return CrewRandRRoster.Instance.AvailableCrew;
            }
        }

        public static IEnumerable<ProtoCrewMember> UnavailableCrew
        {
            get
            {
                return CrewRandRRoster.Instance.UnavailableCrew;
            }
        }

        public static IEnumerable<ProtoCrewMember> LeastExperiencedCrew
        {
            get
            {
                return CrewRandRRoster.Instance.LeastExperiencedCrew;
            }
        }

        public static IEnumerable<ProtoCrewMember> MostExperiencedCrew
        {
            get
            {
                return CrewRandRRoster.Instance.MostExperiencedCrew;
            }
        }

        public static IEnumerable<ProtoCrewMember> GetCrewForPart(Part partPrefab, IEnumerable<ProtoCrewMember> exemptList, bool preferVeterans = false)
        {
            return CrewRandR.Instance.GetCrewForPart(partPrefab, exemptList, preferVeterans);
        }
    }
}
