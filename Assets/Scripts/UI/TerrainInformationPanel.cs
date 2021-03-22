using UnityEngine;
using UnityEngine.UI;

namespace UI
{

    public class TerrainInformationPanel : MonoBehaviour
    {

        public Text TerrainNameText;
        public Text HitPercentageText;
        public Text DefenseText;
        public Text MovementCostText;

        public void Show(Terrain.Terrain terrain)
        {
            TerrainNameText.text = terrain.DisplayName;
            HitPercentageText.text = "Hit: " + terrain.HitPercentageBoost + "%";
            DefenseText.text = "Def: " + terrain.DefenseBoost + "%";
            MovementCostText.text = "Move: " + terrain.MovementCost;

            transform.position = new Vector2(terrain.transform.position.x, terrain.transform.position.y + 1);

            if (!transform.gameObject.activeSelf)
            {
                transform.gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            if (transform.gameObject.activeSelf)
            {
                transform.gameObject.SetActive(false);
            }
        }
    }
}