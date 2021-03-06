﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Caliburn.Micro;
using Search2.Model.BitmapComparer;
using Search2.Model.Rectangle;
using Search2.Util;

namespace Search2.ViewModels
{
    public class FinderViewModel : PropertyChangedBase
    {
        private readonly Progress<int> _checkProgress = new Progress<int>();

        private readonly IBitmapComparer _bitmapComparer;

        private double _threshold = 0.98D;

        private int _progress;
        public int Progress
        {
            get => _progress;
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    NotifyOfPropertyChange(() => Progress);
                }
            }
        }

        private ObservableCollection<ElementViewModel> _elements = new ObservableCollection<ElementViewModel>()
        {
            new ElementViewModel(Colors.OrangeRed),
            new ElementViewModel(Colors.GreenYellow)
        };
        public ObservableCollection<ElementViewModel> Elements
        {
            get => _elements;
            private set
            {
                if (_elements != value)
                {
                    _elements = value;
                    NotifyOfPropertyChange(() => Elements);
                }
            }
        }

        #region Command
        public async void Find(object sender, EventArgs e)
        {
            if (Elements.All(x => x.IsExist))
            {
                var bitmap0 = Elements[0].GetBitmap() ?? throw new ArgumentNullException();
                var bitmap1 = Elements[1].GetBitmap() ?? throw new ArgumentNullException();

                var elements0 = await _bitmapComparer.CheckerAsync(
                    bitmap0, bitmap1,
                    _checkProgress, _threshold, false);

                Elements[0].Matrix = new ObservableCollection<RectangleModel>(elements0);

                //var elements1 = await _bitmapComparer.CheckerAsync(
                //    bitmap1, bitmap0,
                //    _checkProgress, _threshold, false);

                //Elements[1].Matrix = new ObservableCollection<RectangleModel>(elements1);
            }
        }

        public void Save(object sender, EventArgs e)
        {
            foreach (var elem in Elements)
            {
                WorkScreen.SaveFromScreen(new RectangleModel(elem.Area.LeftTop,
                    elem.Area.Width,
                    elem.Area.Height));
            }
        }

        public void Clear(object sender, EventArgs e)
        {
            Elements= new ObservableCollection<ElementViewModel>()
            {
                new ElementViewModel(Colors.OrangeRed),
                new ElementViewModel(Colors.GreenYellow)
            };
        }
        #endregion

        public FinderViewModel(IBitmapComparer bitmapComparer)
        {
            _bitmapComparer = bitmapComparer;

            _checkProgress.ProgressChanged += (s, param) =>
            {
                Progress = param;
            };
        }

        public FinderViewModel() : this(new TemplateBitmapComparer())
        {
            
        }

        public void SetThreshold(double threshold)
        {
            _threshold = threshold;
        }
    }
}