using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

        [SerializeField] private bool showMenu;
        [SerializeField] private GameObject headerBar;
        [SerializeField] private GameObject menuBar;
        [SerializeField] private GameObject titleText;

        private HashSet<string> movie_genre = new();

        public void Show()
        {
            gameObject.SetActive(true);
            menuBar.SetActive(showMenu);
            titleText.SetActive(true);
            headerBar.SetActive(true);
        }

        public void Hide() => gameObject.SetActive(false);

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

                    //List<Model_Genre> genres = now_playingData[i].genres;
                    //movie_genre = genres.Select(x => x.name).ToHashSet();
                }
            }
            loadData = true;
        }

        private void CreateGenreTag()
        {
            if (movie_genre.Count > 0)
            {

            }
        }
        private void OnCardClicked() => _controller.ChangeState<UI_MovieDetailsPage>();
    }
}
