using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Characters
{
    public class ProficiencyTests
    {
        [Test]
        public void ProficiencyTest1()
        {
            _ = new Proficiency(typeof(string), Proficiency.Rank.A);
            LogAssert.Expect(LogType.Error, "Created Proficiency for non-weapon: System.String");
        }

        [Test]
        public void ProficiencyTest2()
        {
            _ = new Proficiency(typeof(Fire), Proficiency.Rank.A);
            LogAssert.Expect(LogType.Error, "Created Proficiency for non-abstract: Fire");
        }

        [Test]
        public void AddExperience1()
        {
            Proficiency proficiency = new Proficiency(typeof(FireMagic), Proficiency.Rank.A);
            proficiency.AddExperience(101);
            LogAssert.Expect(LogType.Error, "Can't increase by more than 100: 101");
        }

        [Test]
        public void AddExperience2()
        {
            Proficiency proficiency = new Proficiency(typeof(FireMagic), Proficiency.Rank.A);
            proficiency.AddExperience(90);
            Assert.AreEqual(90, proficiency.Experience);

            proficiency.AddExperience(11);
            Assert.AreEqual(1, proficiency.Experience);
            Assert.AreEqual(Proficiency.Rank.S, proficiency.ProficiencyRank);
        }

        [Test]
        public void AddExperience3()
        {
            Proficiency proficiency = new Proficiency(typeof(FireMagic), Proficiency.Rank.S);
            proficiency.AddExperience(90);
            Assert.AreEqual(90, proficiency.Experience);
            proficiency.AddExperience(11);
            Assert.AreEqual(101, proficiency.Experience);
            Assert.AreEqual(Proficiency.Rank.S, proficiency.ProficiencyRank);
        }
    }
}
