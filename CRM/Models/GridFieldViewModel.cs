﻿using CRM.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.Models
{
    public class GridFieldViewModel
    {
        public string Field { get; set; }
        public bool ShowOnGrid { get; set; }
        public int OrderDirection { get; set; }
        public int Order { get; set; }

        public GridFieldViewModel() { }
        public GridFieldViewModel(string field, bool showOnGrid, int orderDirection, int order)
        {
            this.Field = field;
            this.ShowOnGrid = showOnGrid;
            this.Order = order;
            this.OrderDirection = orderDirection;
        }
    }
}