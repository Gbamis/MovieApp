using UnityEngine;

namespace Savana.Movie
{
    public class UIManager : MonoBehaviour
    {
        private UIController controller;

        [SerializeField] private UI_AuthenticatePage authenticatePage;
        [SerializeField] private UI_HomePage homePage;
        [SerializeField] private UI_MovieDetailsPage movieDetailsPage;

        private void Awake() => controller = new();

        private void Start()
        {
            authenticatePage.AttachTo(controller);
            homePage.AttachTo(controller);
            movieDetailsPage.AttachTo(controller);

            controller.ChangeState<UI_AuthenticatePage>();
        }
    }

}