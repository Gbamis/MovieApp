using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using ModestTree;

namespace Savana.Movie
{
    public class UI_AuthenticatePage : MonoBehaviour, UIState
    {
        private UIController _controller;
        [SerializeField] private bool showMenu;
        [SerializeField] private GameObject menuBar;
        [SerializeField] private GameObject apiInputPanel;
        [SerializeField] private TMP_InputField apiInputKey;

        [SerializeField] private Button getStartedBtn;
        [SerializeField] private Button proceedBtn;
        [SerializeField] private GameObject loadingBar;
        [SerializeField] private RectTransform continueRect;


        [SerializeField] private Image backgroundImage;
        [SerializeField] List<Sprite> collages;

        public void AttachTo(UIController controller)
        {
            _controller = controller;
            _controller.RegisterState(this);
            showMenu = false;
            gameObject.SetActive(false);
        }

        public void Enter()
        {
            int rand = Random.Range(0, collages.Count);
            backgroundImage.sprite = collages[rand];

            menuBar.SetActive(showMenu);
            apiInputPanel.SetActive(false);
            gameObject.SetActive(true);
            loadingBar.SetActive(false);

            getStartedBtn.onClick.AddListener(GetStartedBtnClicked);
            proceedBtn.onClick.AddListener(ContinueBtnClicked);



            //AnimateContinueBtn().Forget();
        }
        public void Exit()
        {
            getStartedBtn.onClick.RemoveListener(GetStartedBtnClicked);
            proceedBtn.onClick.RemoveListener(ContinueBtnClicked);
            gameObject.SetActive(false);
        }

        public void Pause() { }
        public void Resume() { }

        private void GetStartedBtnClicked()
        {
            apiInputPanel.SetActive(true);
            proceedBtn.gameObject.SetActive(true);
        }

        private void ContinueBtnClicked()
        {
            string key = apiInputKey.text;
            key = key.Trim();

            if (key.IsEmpty())
            {
                apiInputKey.text = "Fill in an api key";
                return;
            }
            loadingBar.SetActive(true);
            proceedBtn.gameObject.SetActive(false);

            NetworkManager.Request_Get_NowPlaying(key, LoadHomePage, ErrorConnecting);
        }

        private void ErrorConnecting()
        {
            apiInputKey.text = "Error Connecting";
            proceedBtn.gameObject.SetActive(true);
            loadingBar.SetActive(false);
        }

        private void LoadHomePage() => _controller.ChangeState<UI_HomePage>();
        

    }

}