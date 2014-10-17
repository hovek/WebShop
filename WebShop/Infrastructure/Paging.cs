using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Infrastructure
{
    public sealed class Paging : IEnumerable<int>
    {
        private int _numberOfPages;
        private int _skipPages;
        private int _takePages;
        private int _currentPageIndex;
        private int _numberOfItems;
        private int _itemsPerPage;

        private Paging()
        {
        }

        private Paging(Paging pageing)
        {
            _numberOfItems = pageing._numberOfItems;
            _currentPageIndex = pageing._currentPageIndex;
            _numberOfPages = pageing._numberOfPages;
            _takePages = pageing._takePages;
            _skipPages = pageing._skipPages;
            _itemsPerPage = pageing._itemsPerPage;
        }

        /// <summary>
        /// Creates a pager for the given number of items.
        /// </summary>
        public static Paging Items(int numberOfItems)
        {
            return new Paging
            {
                _numberOfItems = numberOfItems,
                _currentPageIndex = 1,
                _numberOfPages = 1,
                _skipPages = 0,
                _takePages = 1,
                _itemsPerPage = numberOfItems
            };
        }

        /// <summary>
        /// Specifies the number of items per page.
        /// </summary>
        public Paging PerPage(int itemsPerPage)
        {
            int numberOfPages = (_numberOfItems + itemsPerPage - 1) / itemsPerPage;

            return new Paging(this)
            {
                _numberOfPages = numberOfPages,
                _skipPages = 0,
                _takePages = numberOfPages - _currentPageIndex + 1,
                _itemsPerPage = itemsPerPage
            };
        }

        /// <summary>
        /// Moves the pager to the given page index
        /// </summary>
        public Paging Move(int pageIndex)
        {
            return new Paging(this)
            {
                _currentPageIndex = pageIndex
            };
        }

        /// <summary>
        /// Segments the pager so that it will display a maximum number of pages.
        /// </summary>
        public Paging Segment(int maximum)
        {
            // broj stranica koje prikazuje
            int count = Math.Min(_numberOfPages, maximum);
            return new Paging(this)
            {
                _takePages = count,
                _skipPages = Math.Min(_skipPages, _numberOfPages - count),
            };
        }

        /// <summary>
        /// Centers the segment around the current page
        /// </summary>
        public Paging Center()
        {
            int radius = ((_takePages + 1) / 2);

            return new Paging(this)
            {
                _skipPages = Math.Min(Math.Max(_currentPageIndex - radius, 0), _numberOfPages - _takePages)
            };
        }

        public IEnumerator<int> GetEnumerator()
        {
            return Enumerable.Range(_skipPages + 1, _takePages).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool IsPaged { get { return _numberOfItems > _itemsPerPage; } }

        public int NumberOfPages { get { return _numberOfPages; } }

        public bool IsUnpaged { get { return _numberOfPages == 1; } }

        public int CurrentPageIndex { get { return _currentPageIndex; } }

        public int NextPageIndex { get { return _currentPageIndex + 1; } }

        public int LastPageIndex { get { return _numberOfPages; } }

        public int FirstPageIndex { get { return 1; } }

        public bool HasNextPage { get { return _currentPageIndex < _numberOfPages && _numberOfPages > 1; } }

        public bool HasPreviousPage { get { return _currentPageIndex > 1 && _numberOfPages > 1; } }

        public int PreviousPageIndex { get { return _currentPageIndex - 1; } }

        public bool IsSegmented { get { return _skipPages > 0 || _skipPages + 1 + _takePages < _numberOfPages; } }

        public bool IsEmpty { get { return _numberOfPages < 1; } }

        public bool IsFirstSegment { get { return _skipPages == 0; } }

        public bool IsLastSegment { get { return _skipPages + 1 + _takePages >= _numberOfPages; } }
    }
}