using System;
using System.Collections.Generic;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace MonoDroid.TimesSquare
{
    public class MonthView : LinearLayout
    {
        private TextView _title;
        private CalendarGridView _grid;
        private IListener _listener;

        public MonthView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        public static MonthView Create(ViewGroup parent, LayoutInflater inflater, string weekdayNameFormat,
                                       IListener listener, DateTime today)
        {
            var view = (MonthView)inflater.Inflate(Resource.Layout.month, parent, false);

            var originalDay = today;

            var headerRow = (CalendarRowView)view._grid.GetChildAt(0);

            for (var c = (int)DayOfWeek.Sunday; c <=(int)DayOfWeek.Saturday; c++) {
                today = today.AddDays(c);
                var textView = (TextView)headerRow.GetChildAt(c);
                textView.Text = today.ToString(weekdayNameFormat);
            }
            today = originalDay;
            view._listener = listener;
            return view;
        }

        public void Init(MonthDescriptor month, List<List<MonthCellDescriptor>> cells)
        {
            Logr.D("Initializing MonthView for {0}", month);
            long start = DateTime.Now.Millisecond;
            _title.Text = month.Label;

            int numOfRows = cells.Count;
            for (int i = 0; i < 6; i++) {
                var weekRow = (CalendarRowView)_grid.GetChildAt(i + 1);
                weekRow.SetListener(_listener);
                if (i < numOfRows) {
                    weekRow.Visibility = ViewStates.Visible;
                    var week = cells[i];
                    for (int c = 0; c < week.Count; c++) {
                        var cell = week[c];
                        var cellView = (CheckedTextView)weekRow.GetChildAt(c);
                        cellView.Text = cell.Value.ToString();
                        cellView.Enabled = cell.IsCurrentMonth;
                        cellView.Checked = !cell.IsToday;
                        cellView.Selected = cell.IsSelected;
                        if (cell.IsSelectable) {
                            cellView.SetTextColor(Resources.GetColorStateList(Resource.Color.calendar_text_selector));
                        }
                        else {
                            cellView.SetTextColor(Resources.GetColor(Resource.Color.calendar_text_unselectable));
                        }
                        cellView.Tag = cell;
                    }
                }
                else {
                    weekRow.Visibility = ViewStates.Gone;
                }
            }
            Logr.D("MonthView.Init took {0} ms", DateTime.Now.Millisecond - start);
        }

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();
            _title = FindViewById<TextView>(Resource.Id.title);
            _grid = FindViewById<CalendarGridView>(Resource.Id.calendar_grid);
        }
    }
}