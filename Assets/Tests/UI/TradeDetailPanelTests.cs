using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.UI
{
    public class TradeDetailPanelTests : GameManagerTest
    {
        [UnityTest]
        public IEnumerator OnInformationTest()
        {
            GameManager.TradeDetailPanel.OnInformation();
            LogAssert.Expect(LogType.Log, "TradeDetailPanel.OnInformation is not implemented");
            yield return null;
        }
    }
}
