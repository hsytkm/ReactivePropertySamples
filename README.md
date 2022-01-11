# ReactivePropertySamples

ReactiveProperty Samples for WPF

.NET 6.0 + C# 9

Created in 2020/06

Updated in 2021/05



## Memo

##### `Select()` で `Task` を実行する

投げっぱなしなので注意。

```cs
_observable
    .ObserveOn(Scheduler.Default)
    .Select(x => Observable.FromAsync(x.CalcAsync))  // await不要
    .Concat()
    .ObserveOnUIDispatcher()
    .ToReadOnlyReactivePropertySlim();
```

以下の方が良さげ？（Switchは新しい処理が入ったら以前の非同期処理はキャンセルして新しい処理のみを後続に流す）

```cs
_observable
    .ObserveOn(Scheduler.Default)
    .Select(x => x.CalcAsync)  // await不要
    .Switch()
    .ObserveOnUIDispatcher()
    .ToReadOnlyReactivePropertySlim();
```

##### コレクションの変更

```cs
collection.ObserveAddChanged().ToUnit()
    .Merge(collection.ObserveRemoveChanged().ToUnit())
    .Merge(collection.ObserveResetChanged().ToUnit())
    .Subscribe(_ => Debug.WriteLine(collection.Count))
    .AddTo(_disposables);
```

## Reference Website

[runceel/ReactiveProperty](https://github.com/runceel/ReactiveProperty)

[かずきのBlog@hatena/ReactiveProperty](https://blog.okazuki.jp/archive/category/ReactiveProperty)

[MVVMをリアクティブプログラミングで快適にReactivePropertyオーバービュー](https://blog.okazuki.jp/entry/2015/12/05/221154)

[ReactiveProperty documentation](https://okazuki.jp/ReactiveProperty/)

[ReactiveProperty : WPF/SL/WP7のためのRxとMVVMを繋ぐ拡張ライブラリ](http://neue.cc/2011/10/07_346.html)

[WPFでReactiveProperty入門 ～アプリケーションのステータスやエラー情報をIObservable で通知する](https://pierre3.hatenablog.com/entry/2015/11/03/004119)

以上
