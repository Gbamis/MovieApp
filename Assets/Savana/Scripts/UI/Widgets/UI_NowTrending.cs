using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace Savana.Movie
{
    public class UI_NowTrending : MonoBehaviour
    {
        private UIController _controller;
        private bool loadData;
        private List<Response_MovieDetail> now_playingData;

        [SerializeField] private Transform listView;
        [SerializeField] private UI_MovieCard_Item movieCard_Item;
        [SerializeField] private UI_MovieDetailsPage movieDetailsPage;
        [SerializeField] private int maxViewAtOnce;
        [SerializeField] private ScrollRect scrollRect;

        [SerializeField] private bool showMenu;
        [SerializeField] private GameObject headerBar;
        [SerializeField] private GameObject menuBar;
        [SerializeField] private GameObject titleText;
        [SerializeField] private Image backdropImg;

        private HashSet<string> movie_genre = new();

        private Dictionary<int, UI_MovieCard_Item> cacheCards = new();
        private int lastScrollIndex;

        public void Show()
        {
            gameObject.SetActive(true);
            menuBar.SetActive(showMenu);
            titleText.SetActive(true);
            headerBar.SetActive(true);

            scrollRect.onValueChanged.AddListener(GetSVisibleItemWithinScroll);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            scrollRect.onValueChanged.RemoveListener(GetSVisibleItemWithinScroll);
        }

        public void PopulateListViewWithMovieItems(UIController con)
        {
            _controller = con;
            if (loadData) { return; }

            now_playingData = NetworkManager.response_NowPlaying;

            if (now_playingData.Count > 0)
            {
                for (int i = 0; i < Mathf.Min(maxViewAtOnce, now_playingData.Count); i++)
                {
                    UI_MovieCard_Item item = Instantiate(movieCard_Item, listView);
                    item.SetData(now_playingData[i], movieDetailsPage, OnCardClicked);

                    cacheCards.Add(i, item);

                    //List<Model_Genre> genres = now_playingData[i].genres;
                    //movie_genre = genres.Select(x => x.name).ToHashSet();
                }
            }
            loadData = true;
        }
        private void OnCardClicked() => _controller.ChangeState<UI_MovieDetailsPage>();

        private void GetSVisibleItemWithinScroll(Vector2 pos)
        {
            float scrollPos = 1 - scrollRect.horizontalNormalizedPosition; // 1 = Top, 0 = Bottom
            int totalItems = now_playingData.Count;

            int firstVisibleIndex = Mathf.FloorToInt((1 - scrollPos) * totalItems);
            firstVisibleIndex = Mathf.Clamp(firstVisibleIndex, 0, totalItems - 1);

            if (lastScrollIndex != firstVisibleIndex)
            {
                lastScrollIndex = firstVisibleIndex;
                DisplayBakdrop(firstVisibleIndex).Forget();
            }

        }

        private async UniTaskVoid DisplayBakdrop(int firstVisibleIndex)
        {
            await UniTask.WaitUntil(() => scrollRect.velocity == Vector2.zero);
            backdropImg.sprite = cacheCards[firstVisibleIndex].GetPoster();
        }

        private void CreateGenreTag()
        {
            if (movie_genre.Count > 0)
            {

            }
        }

    }
}
