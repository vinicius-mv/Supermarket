﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.Helpers.Pagination
{
    public class PaginationParameters
    {
        private const int maxPageSize = 50;

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;

            set => _pageSize = value > maxPageSize ? maxPageSize : value;
        }
    }
}
