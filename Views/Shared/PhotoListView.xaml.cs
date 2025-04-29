using System.Collections;
using System.Windows.Input;

namespace FlickrApp.Views.Shared;

public partial class PhotoListView : ContentView
{
    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(PhotoListView), null);

    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly BindableProperty ItemTappedCommandProperty =
        BindableProperty.Create(nameof(ItemTappedCommand), typeof(ICommand), typeof(PhotoListView), null);

    public ICommand ItemTappedCommand
    {
        get => (ICommand)GetValue(ItemTappedCommandProperty);
        set => SetValue(ItemTappedCommandProperty, value);
    }

    public static readonly BindableProperty LoadMoreItemsCommandProperty =
        BindableProperty.Create(nameof(LoadMoreItemsCommand), typeof(ICommand), typeof(PhotoListView), null);

    public ICommand LoadMoreItemsCommand
    {
        get => (ICommand)GetValue(LoadMoreItemsCommandProperty);
        set => SetValue(LoadMoreItemsCommandProperty, value);
    }

    public PhotoListView()
    {
        InitializeComponent();
    }
}