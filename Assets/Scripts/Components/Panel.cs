using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Task = System.Threading.Tasks.Task;

namespace Components
{
    public sealed class Panel : MonoBehaviour
    {
        [SerializeField] private ListItem[] displayItems;
        [SerializeField] private GameObject loadingWidget;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Button previous;
        [SerializeField] private Button next;
        
        private IDataServer _dataServer;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private List<DataItem> _dataItems;
        private int _page = 0;

        private int _maxDataCount;

        private event Action PageChange;
        private event Action DataArrived;


        public CancellationToken CancellationToken => _cancellationTokenSource.Token;

        private async void Start()
        {
            previous.onClick.AddListener(() =>
            {
                _page--;
                PageChange?.Invoke();
            });
            next.onClick.AddListener(() =>
            {
                _page++;
                PageChange?.Invoke();
            });

            PageChange += UpdatePanel;
            DataArrived += UpdateButtons;
            
            loadingWidget.gameObject.SetActive(true);
            canvasGroup.alpha = 0f;
            
            FirstUpdate();
            
            _dataServer = new DataServerMock();
            await GetDataAsync();
        }

        private void OnApplicationQuit()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private async Task GetDataAsync()
        {
            int index = 0;
            _maxDataCount = await _dataServer.DataAvailable(CancellationToken);
            Debug.Log($"There is {_maxDataCount} {typeof(DataItem)} available.");
            _dataItems = new List<DataItem>(_maxDataCount);
            while (_dataItems.Count < _maxDataCount)
            {
                int requestDataCount = _maxDataCount < index + displayItems.Length ? _maxDataCount - index : displayItems.Length;
                _dataItems.AddRange(await _dataServer.RequestData(index, requestDataCount, CancellationToken));
                index += displayItems.Length;
                DataArrived?.Invoke();
            }
        }

        private async void FirstUpdate()
        {
            while (_dataItems == null || _dataItems.Count < displayItems.Length)
            {
                await Task.Yield();
            }
            UpdatePanel();
        }

        private void UpdatePanel()
        {
            UpdateButtons();
            PopulateDisplayItems();
            canvasGroup.alpha = 1f;
            loadingWidget.SetActive(false);
        }

        private void UpdateButtons()
        {
            previous.interactable = _page > 0;
            next.interactable = _dataItems.Count > (_page + 1) * displayItems.Length;
            
        }

        private void PopulateDisplayItems()
        {
            for (int i = 0; i < displayItems.Length; i++)
            {
                ListItem item = displayItems[i];
                int index = _page * displayItems.Length + i;
                if (index < _dataItems.Count)
                {
                    item.Set(index, _dataItems[index]);
                    continue;
                }
                
                item.Disable();
            }
        }

    }
}