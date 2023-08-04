using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.UI
{
    public sealed class Panel : MonoBehaviour
    {
        [SerializeField] private ListItem[] displayItems;

        // loading widget is optional
        [SerializeField] [MaybeNull] private GameObject loadingWidget;

        // those fields should always be assigned in editor 
        [SerializeField] [NotNull] private CanvasGroup canvasGroup;
        [SerializeField] [NotNull] private Button previous;
        [SerializeField] [NotNull] private Button next;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private List<DataItem> _dataItems;

        private IDataServer _dataServer;

        private int _maxDataCount;
        private int _page;


        private CancellationToken CancellationToken => _cancellationTokenSource.Token;

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

            PageChange += UpdatePanelHandleAsync;
            PageChange += UpdateButtons;
            DataCountArrived += UpdateButtons;

            SetLoadingState(true);

            // ReSharper disable once MethodHasAsyncOverload
            UpdatePanel();

            _dataServer = new DataServerMock();
            await GetDataAsync();
        }

        private void OnApplicationQuit()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private event Action PageChange;
        private event Action DataCountArrived;

        private async Task GetDataAsync()
        {
            var index = 0;
            _maxDataCount = await _dataServer.DataAvailable(CancellationToken);
            DataCountArrived?.Invoke();
            Debug.Log($"There is {_maxDataCount} {typeof(DataItem)} available.");
            _dataItems = new List<DataItem>(_maxDataCount);
            while (_dataItems.Count < _maxDataCount)
            {
                var requestDataCount = _maxDataCount < index + displayItems.Length
                    ? _maxDataCount - index
                    : displayItems.Length;
                _dataItems.AddRange(await _dataServer.RequestData(index, requestDataCount, CancellationToken));
                index += displayItems.Length;
            }
        }

        private void UpdatePanel()
        {
            UpdatePanelAsync().ContinueWith(t => Debug.LogError(t.Exception), TaskContinuationOptions.NotOnFaulted);
        }

        private async void UpdatePanelHandleAsync()
        {
            await UpdatePanelAsync();
        }

        private async Task UpdatePanelAsync()
        {
            Debug.Log("UpdatePanelAsync");
            SetLoadingState(true);
            var maxIndex = Mathf.Min(_page * displayItems.Length, _maxDataCount);
            while (_dataItems == null || _dataItems.Count <= maxIndex) await Task.Yield();

            SetLoadingState(false);
            PopulateListItems();
        }

        private void UpdateButtons()
        {
            previous.interactable = _page > 0;
            next.interactable = _maxDataCount > (_page + 1) * displayItems.Length;
        }

        private void PopulateListItems()
        {
            for (var i = 0; i < displayItems.Length; i++)
            {
                var item = displayItems[i];
                var index = _page * displayItems.Length + i;
                if (index < _dataItems.Count)
                {
                    item.Set(index, _dataItems[index]);
                    continue;
                }

                item.Disable();
            }
        }

        private void SetLoadingState(bool loading)
        {
            if (loadingWidget != null)
                loadingWidget.SetActive(loading);
            canvasGroup.alpha = loading ? 0f : 1f;
            canvasGroup.interactable = !loading;
        }
    }
}