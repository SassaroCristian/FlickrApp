using System.Collections;
using System.Diagnostics;
using System.Windows.Input;

namespace FlickrApp.Views.Shared;

public partial class PhotoListView : ContentView
{
    private const double ScrollThreshold = 700;

    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(PhotoListView));

    public static readonly BindableProperty ItemTappedCommandProperty =
        BindableProperty.Create(nameof(ItemTappedCommand), typeof(ICommand), typeof(PhotoListView));

    public static readonly BindableProperty LoadMoreItemsCommandProperty =
        BindableProperty.Create(nameof(LoadMoreItemsCommand), typeof(ICommand), typeof(PhotoListView));

    private bool _isLoadingMoreItems;

    public PhotoListView()
    {
        InitializeComponent();
    }

    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public ICommand ItemTappedCommand
    {
        get => (ICommand)GetValue(ItemTappedCommandProperty);
        set => SetValue(ItemTappedCommandProperty, value);
    }

    public ICommand LoadMoreItemsCommand
    {
        get => (ICommand)GetValue(LoadMoreItemsCommandProperty);
        set => SetValue(LoadMoreItemsCommandProperty, value);
    }

    private async void OnMainScrollViewScrolled(object? sender, ScrolledEventArgs e)
    {
        if (sender is not ScrollView scrollView)
            return;


        var isNearBottom = e.ScrollY >= scrollView.ContentSize.Height - scrollView.Height - ScrollThreshold;

        if (isNearBottom && !_isLoadingMoreItems)
        {
            Debug.WriteLine("PhotoListView: Rilevato scorrimento vicino al fondo (soglia anticipata).");
            if (LoadMoreItemsCommand != null && LoadMoreItemsCommand.CanExecute(null))
            {
                _isLoadingMoreItems = true;
                Debug.WriteLine("PhotoListView: Esecuzione di LoadMoreItemsCommand.");
                LoadMoreItemsCommand.Execute(null);
                await Task.Delay(300);
                _isLoadingMoreItems = false;
            }
            else
            {
                Debug.WriteLine(
                    "PhotoListView: LoadMoreItemsCommand è nullo o non può essere eseguito (forse IsBusy o AreMoreItemsAvailable è false).");
            }
        }
    }
}