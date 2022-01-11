# ReactivePropertySamples

ReactiveProperty Samples for WPF

.NET 6.0 + C# 9

Created in 2020/06

Updated in 2021/05



## Memo

##### `Select()` �� `Task` �����s����

�������ςȂ��Ȃ̂Œ��ӁB

```cs
_observable
    .ObserveOn(Scheduler.Default)
    .Select(x => Observable.FromAsync(x.CalcAsync))  // await�s�v
    .Concat()
    .ObserveOnUIDispatcher()
    .ToReadOnlyReactivePropertySlim();
```

�ȉ��̕����ǂ����H�iSwitch�͐V������������������ȑO�̔񓯊������̓L�����Z�����ĐV���������݂̂��㑱�ɗ����j

```cs
_observable
    .ObserveOn(Scheduler.Default)
    .Select(x => x.CalcAsync)  // await�s�v
    .Switch()
    .ObserveOnUIDispatcher()
    .ToReadOnlyReactivePropertySlim();
```

##### �R���N�V�����̕ύX

```cs
collection.ObserveAddChanged().ToUnit()
    .Merge(collection.ObserveRemoveChanged().ToUnit())
    .Merge(collection.ObserveResetChanged().ToUnit())
    .Subscribe(_ => Debug.WriteLine(collection.Count))
    .AddTo(_disposables);
```

## Reference Website

[runceel/ReactiveProperty](https://github.com/runceel/ReactiveProperty)

[��������Blog@hatena/ReactiveProperty](https://blog.okazuki.jp/archive/category/ReactiveProperty)

[MVVM�����A�N�e�B�u�v���O���~���O�ŉ��K��ReactiveProperty�I�[�o�[�r���[](https://blog.okazuki.jp/entry/2015/12/05/221154)

[ReactiveProperty documentation](https://okazuki.jp/ReactiveProperty/)

[ReactiveProperty : WPF/SL/WP7�̂��߂�Rx��MVVM���q���g�����C�u����](http://neue.cc/2011/10/07_346.html)

[WPF��ReactiveProperty���� �`�A�v���P�[�V�����̃X�e�[�^�X��G���[����IObservable �Œʒm����](https://pierre3.hatenablog.com/entry/2015/11/03/004119)

�ȏ�
