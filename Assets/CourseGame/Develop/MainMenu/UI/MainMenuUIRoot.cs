using Assets.CourseGame.Develop.CommonUI;
using UnityEngine;

namespace Assets.CourseGame.Develop.MainMenu.UI
{
    public class MainMenuUIRoot : MonoBehaviour
    {
        [field: SerializeField] public IconsWithTextListView WalletView { get; private set; }
        [field: SerializeField] public ActionButton OpenLevelsMenuButton { get; private set; }

        [field: SerializeField] public Transform HUDLayer { get; private set; }
        [field: SerializeField] public Transform PopupsLayer { get; private set; }
        [field: SerializeField] public Transform VFXLayer { get; private set; }
    }
}